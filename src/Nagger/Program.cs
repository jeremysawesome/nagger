namespace Nagger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Data.JIRA;
    using Data.Local;
    using Data.Meazure;
    using Interfaces;
    using Models;
    using Quartz;
    using Quartz.Impl;
    using Services;
    using Services.JIRA;
    using Services.Meazure;

    internal class Program
    {
        static bool _running;
        // note: Elysium can be used for WPF theming - seems like pretty easily
        //http://bizvise.com/2012/09/27/how-to-create-metro-style-window-on-wpf-using-elysium/


        // jira documentation for api:
        //https://docs.atlassian.com/jira/REST/latest/#id165531

        // jira api browser: https://bunjil.jira-dev.com/plugins/servlet/restbrowser#/resource/api-2-issue-issueidorkey-worklog

        //get all projects: https://www.example.com/rest/api/latest/project
        // - search for issues https://www.example.com/rest/api/latest/search
        // get all ProjectName issues
        // https://www.example.com/rest/api/latest/search?jql=project%3D%22ProjectName%22

        static IContainer Container { get; set; }

        static void RegisterInitialComponents(ContainerBuilder builder)
        {
            builder.RegisterType<CommandService>().As<ICommandService>();
            builder.RegisterType<ConsoleInputService>().As<IInputService>();
            builder.RegisterType<ConsoleOutputService>().As<IOutputService>();

            builder.RegisterType<SettingsRepository>().As<ISettingsRepository>();
            builder.RegisterType<LocalProjectRepository>().As<ILocalProjectRepository>();
            builder.RegisterType<LocalTaskRepository>().As<ILocalTaskRepository>();
            builder.RegisterType<LocalTimeRepository>().As<ILocalTimeRepository>();

            builder.RegisterType<SettingsService>().As<ISettingsService>();
        }

        static void RegisterConditionalComponents(IContainer container)
        {
            var updater = new ContainerBuilder();
            var primaryRepository = GetPrimaryRemoteRepository();

            if (primaryRepository == SupportedRemoteRepository.Meazure)
            {
                updater.RegisterType<MeazureProjectRepository>().As<IRemoteProjectRepository>();
                updater.RegisterType<MeazureTaskRepository>().As<IRemoteTaskRepository>();
                updater.RegisterType<MeazureTimeRepository>().As<IRemoteTimeRepository>();
                updater.RegisterType<MeazureRunner>().As<IRemoteRunner>();
            }
            else if (primaryRepository == SupportedRemoteRepository.Jira)
            {
                updater.RegisterType<JiraProjectRepository>().As<IRemoteProjectRepository>();
                updater.RegisterType<JiraTaskRepository>().As<IRemoteTaskRepository>();
                updater.RegisterType<JiraTimeRepository>().As<IRemoteTimeRepository>();
                updater.RegisterType<BaseJiraRepository>();
                updater.RegisterType<JiraRunner>().As<IRemoteRunner>();
            }

            updater.Update(container);
        }

        static void RegisterFinalComponents(IContainer container)
        {
            var updater = new ContainerBuilder();

            updater.RegisterType<ProjectService>().As<IProjectService>();
            updater.RegisterType<TaskService>().As<ITaskService>();
            updater.RegisterType<TimeService>().As<ITimeService>();

            updater.RegisterType<NaggerRunner>().As<IRunnerService>();

            updater.Update(container);
        }

        static void SetupIocContainer()
        {
            var builder = new ContainerBuilder();
            RegisterInitialComponents(builder);
            Container = builder.Build();
        }

        static void FinalizeIocContainer()
        {
            RegisterConditionalComponents(Container);
            RegisterFinalComponents(Container);
        }

        static void Schedule()
        {
            int naggingInterval;
            using (var scope = Container.BeginLifetimeScope())
            {
                var settingsService = scope.Resolve<ISettingsService>();
                naggingInterval = settingsService.GetSetting<int>("NaggingInterval");
            }

            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            var job = JobBuilder.Create<JobRunner>()
                .WithIdentity("naggerJob")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("naggerJob")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(naggingInterval)
                    .RepeatForever())
                .Build();

            var nightlyJob = JobBuilder.Create<NightlyJob>().WithIdentity("nightlyJob").Build();

            var nightlyTrigger = TriggerBuilder.Create()
                .WithIdentity("nightlyJob")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(23, 55))
                .Build();

            scheduler.ScheduleJob(job, trigger);
            scheduler.ScheduleJob(nightlyJob, nightlyTrigger);
        }

        static void Run()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                if (_running) return;

                _running = true;

                var runner = scope.Resolve<IRunnerService>();
                runner.Run();

                _running = false;
            }
        }

        static void NightlyRun()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var timeService = scope.Resolve<ITimeService>();
                timeService.DailyTimeOperations(true);
            }
        }

        static void MonitorEvents()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var settingsService = scope.Resolve<ISettingsService>();

                var syncOnLock = settingsService.GetSetting<bool>("SyncOnLock");
                if (!syncOnLock) return;

                var timeService = scope.Resolve<ITimeService>();
                var outputService = scope.Resolve<IOutputService>();
                var monitorService = new EventMonitoringService(outputService,
                    x => { timeService.DailyTimeOperations(true); });
                monitorService.Monitor();
            }
        }

        static bool ExecuteCommands(string[] args)
        {
            if (!args.Any()) return false;
            using (var scope = Container.BeginLifetimeScope())
            {
                var commandService = scope.Resolve<ICommandService>();
                commandService.ExecuteCommands(args);
            }

            return true;
        }

        static IEnumerable<SupportedRemoteRepository> SupportedRemoteRepositories()
        {
            return Enum.GetValues(typeof (SupportedRemoteRepository)).Cast<SupportedRemoteRepository>();
        }

        static SupportedRemoteRepository GetPrimaryRemoteRepository()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var settingsService = scope.Resolve<ISettingsService>();
                return (SupportedRemoteRepository)Enum.Parse(typeof(SupportedRemoteRepository), settingsService.GetSetting<string>("PrimaryRemoteRepository"));
            }
        }

        static void Initialize()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var settingsService = scope.Resolve<ISettingsService>();

                if (settingsService.GetSetting<bool>("Initialized")) return;

                var inputService = scope.Resolve<IInputService>();

                var repository = inputService.AskForSelection("What will your primary remote repository be?",
                    SupportedRemoteRepositories().ToList());

                settingsService.SaveSetting("PrimaryRemoteRepository", repository.ToString());
                settingsService.SaveSetting("Initialized", true.ToString());
            }
        }

        static void Main(string[] args)
        {
            SetupIocContainer();
            Initialize();
            FinalizeIocContainer();
            if (ExecuteCommands(args)) return;

            Schedule();
            MonitorEvents();
        }

        class JobRunner : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                Run();
            }
        }

        class NightlyJob : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                NightlyRun();
            }
        }
    }
}
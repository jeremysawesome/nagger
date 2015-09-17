namespace Nagger
{
    using Autofac;
    using Data;
    using Data.JIRA;
    using Interfaces;
    using Quartz;
    using Quartz.Impl;
    using Services;

    internal class Program
    {
        // note: Elysium can be used for WPF theming - seems like pretty easily
        //http://bizvise.com/2012/09/27/how-to-create-metro-style-window-on-wpf-using-elysium/


        // jira documentation for api:
        //https://docs.atlassian.com/jira/REST/latest/#id165531

        // jira api browser: https://bunjil.jira-dev.com/plugins/servlet/restbrowser#/resource/api-2-issue-issueidorkey-worklog

        //get all projects: https://www.example.com/rest/api/latest/project
        // - search for issues https://www.example.com/rest/api/latest/search
        // get all ProjectName issues
        // https://www.example.com/rest/api/latest/search?jql=project%3D%22ProjectName%22

        // schedule a task to run
        // use hangfire or Quartz.net to schedule tasks: http://hangfire.io/

        static IContainer Container { get; set; }

        static int _runMiss;
        static bool _running;

        static void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<JiraRemoteProjectRepository>().As<IRemoteProjectRepository>();
            builder.RegisterType<JiraRemoteTaskRepository>().As<IRemoteTaskRepository>();
            builder.RegisterType<JiraRemoteTimeRepository>().As<IRemoteTimeRepository>();

            builder.RegisterType<LocalProjectRepository>().As<ILocalProjectRepository>();
            builder.RegisterType<LocalTaskRepository>().As<ILocalTaskRepository>();
            builder.RegisterType<LocalTimeRepository>().As<ILocalTimeRepository>();

            builder.RegisterType<SettingsRepository>().As<ISettingsRepository>();

            builder.RegisterType<ProjectService>().As<IProjectService>();
            builder.RegisterType<SettingsService>().As<ISettingsService>();
            builder.RegisterType<TaskService>().As<ITaskService>();
            builder.RegisterType<TimeService>().As<ITimeService>();
            builder.RegisterType<ConsoleInputService>().As<IInputService>();
            builder.RegisterType<ConsoleOutputService>().As<IOutputService>();

            builder.RegisterType<NaggerRunner>().As<IRunnerService>();

            builder.RegisterType<BaseJiraRepository>();
        }

        static void SetupIocContainer()
        {
            var builder = new ContainerBuilder();
            RegisterComponents(builder);
            Container = builder.Build();
        }

        static void Schedule()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            var job = JobBuilder.Create<JobRunner>()
                .WithIdentity("naggerJob")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("naggerJob")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(15)
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        static void Run()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                if (_running)
                {
                    _runMiss++;
                    return;
                }
                _running = true;

                var runner = scope.Resolve<IRunnerService>();
                runner.Run(_runMiss);

                _runMiss = 0;
                _running = false;
            }
        }

        static void Main(string[] args)
        {
            // task scheduler example:
            //https://taskscheduler.codeplex.com/documentation

            // example url
            //https://www.example.com/rest/api/latest/issue/cat-262

            SetupIocContainer();
            Schedule();
        }

        class JobRunner : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                Run();
            }
        }
    }
}
namespace Nagger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using Data;
    using Data.JIRA;
    using Interfaces;
    using Models;
    using Quartz;
    using Quartz.Impl;
    using Services;
    using Autofac;

    internal static class ConsoleUtil
    {
        const int Hide = 0;
        const int Show = 5;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void HideWindow()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, Hide);
        }

        public static void ShowWindow()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, Show);
        }
    }

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

        static bool running = false;

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
            builder.RegisterType<InputService>().As<IInputService>();

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

        static Task AskForTask(IProjectService projectService, IInputService inputService, ITaskService taskService)
        {
            Console.WriteLine("Ok. We're going to list some projects. Let's figure out what you are working on.");
            var projects = projectService.GetProjects().ToList();
            Console.WriteLine(OutputProjects(projects));
            var projectId = inputService.AskForInput("Which project Id are you working on?");
            Console.WriteLine("Getting tasks for that project. This might take a while.");
            var tasks = taskService.GetTasksByProjectId(projectId);
            Console.WriteLine("Ok. We've got the tasks. If you know the key of the task you are working on you can insert that key. Or we can show all the tasks and you can pick.");
            var idIsKnown = inputService.AskForBoolean("Do you know the key of the task you are working on? (example: CAT-102)");
            if (idIsKnown)
            {
                var taskId = inputService.AskForInput("What is the task key?");
                return taskService.GetTaskByName(taskId);
            }
            else
            {
                Console.WriteLine("This feature is not implemented yet. :(");
                return null;
            }
        }

        static void Run()
        {
            using (var scope = Container.BeginLifetimeScope())
            {       
                var projectService = scope.Resolve<IProjectService>();
                var taskService = scope.Resolve<ITaskService>();
                var inputService = scope.Resolve<IInputService>();
                var timeService = scope.Resolve<ITimeService>();

                if (projectService == null || taskService == null) return;

                ConsoleUtil.ShowWindow();
                var currentTask = taskService.GetLastTask();
                if (currentTask != null)
                {
                    var stillWorking = inputService.AskForBoolean("Are you still working on " + currentTask.Name+"?");
                    if (!stillWorking) currentTask = null;
                }
                if (currentTask == null) currentTask = AskForTask(projectService, inputService, taskService);
                ConsoleUtil.HideWindow();

                if (currentTask == null) return;
                timeService.RecordTime(currentTask);
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

        #region OutputProjects

        static string OutputProjects(ICollection<Project> projects)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Outputting {0} Projects...", projects.Count).AppendLine();
            foreach (var project in projects)
            {
                sb.AppendFormat("ID: {0} || Name: {1} || Key: {2}", project.Id, project.Name, project.Key);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        #endregion

        class JobRunner : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                if (running) return;
                running = true;
                Run();
                running = false;
            }
        }

        #region OutputTasks

        static string OutputTasks(IEnumerable<Task> tasks)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Outputting Tasks...");

            foreach (var task in tasks)
            {
                sb.Append(TaskString(task));
                if (task.HasTasks) sb.Append(OutputTasks(task.Tasks));
            }

            return sb.ToString();
        }

        static string TaskString(Task task)
        {
            var beginningSpace = string.Empty;

            var checkingTask = task;
            while (checkingTask.HasParent)
            {
                beginningSpace += "  ";
                checkingTask = checkingTask.Parent;
            }

            return String.Format("{0}Name: {1} || id: {2} || HasTasks: {3} {4}", beginningSpace, task.Name, task.Id,
                task.HasTasks, Environment.NewLine);
        }

        #endregion
    }
}
namespace Nagger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Autofac;
    using Data;
    using Data.JIRA;
    using Interfaces;
    using Models;
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

        static IContainer Container { get; set; }

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

        static void Setup()
        {
            var builder = new ContainerBuilder();
            RegisterComponents(builder);
            Container = builder.Build();
        }

        static void Main(string[] args)
        {
            // task scheduler example:
            //https://taskscheduler.codeplex.com/documentation

            // example url
            //https://www.example.com/rest/api/latest/issue/cat-262

            Setup();

            using (var scope = Container.BeginLifetimeScope())
            {
                var projectService = scope.Resolve<IProjectService>();
                var taskService = scope.Resolve<ITaskService>();
                var inputService = scope.Resolve<IInputService>();

                if (projectService == null || taskService == null) return;

                var projects = projectService.GetProjects().ToList();

                Console.WriteLine(OutputProjects(projects));

                var projectId = inputService.AskForInput("Which project Id should we retrieve tasks for?");

                Console.WriteLine("Getting Tasks for specific project");
                var tasks = taskService.GetTasksByProjectId(projectId);
                Console.WriteLine(OutputTasks(tasks));

                Console.WriteLine("done");

                Console.Read();
            }
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
namespace Nagger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Authentication;
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

        static void PopulateTestData()
        {
            PopulateTestData(DateTime.Now);
            PopulateTestData(DateTime.Today.AddDays(4));
        }

        static void PopulateTestData(DateTime timeRecorded)
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var timeService = scope.Resolve<ITimeService>();
                var taskService = scope.Resolve<ITaskService>();
                const int numEntries = 10;
                for (var i = 0; i < numEntries; i++)
                {
                    var entry = timeService.GetTestTimeEntry();
                    entry.TimeRecorded = timeRecorded;
                    entry.TimeRecorded = entry.TimeRecorded.AddHours(i); //add some time
                    if (i%3 == 0)
                    {
                        var task = taskService.GetTestTask();
                        task.Id = "test";
                        task.Name = "test";
                        entry.Task = task;
                    }
                    timeService.RecordTime(entry);
                }
            }
        }

        static void Main(string[] args)
        {
            // task scheduler example:
            //https://taskscheduler.codeplex.com/documentation

            // example url
            //https://www.example.com/rest/api/latest/issue/cat-262

            // first step call the Jira API and get a list of current tasks for this sprint back
            /*Setup();
            
            Console.WriteLine("Populating Test Data...");
            PopulateTestData();
            Console.WriteLine("Done");

            Console.WriteLine("Getting Last Task");
            var task = _taskService.GetLastTask();
            Console.WriteLine("Id: {0}", task.Id);
            Console.WriteLine("Name: {0}", task.Name);

            Console.WriteLine("Getting Last TimeEntry");
            var timeEntry = _timeService.GetLastTimeEntry();
            Console.WriteLine("Id: {0}", timeEntry.Id);
            Console.WriteLine("timeRecorded: {0}", timeEntry.TimeRecorded);*/

            /* var newTask = new Task() {
                Id = "12394",
                Name = "Finalize fixes from Sprint 5 review (PR #180)"
            };

            var newEntry = new TimeEntry() 
            {
                Task = newTask,
                TimeRecorded = new DateTime(2014,11,14),
                MinutesSpent = 450,
                Synced = false
            };

            _testRemote = new JiraRemoteTimeRepository();
            _testRemote.RecordTime(newEntry);*/

            Setup();

            using (var scope = Container.BeginLifetimeScope())
            {
                var settingsService = scope.Resolve<ISettingsService>();
                IProjectService projectService = null;
                ITaskService taskService = null;

                var noCreds = true;
                while (noCreds)
                {
                    try
                    {
                        projectService = scope.Resolve<IProjectService>();
                        taskService = scope.Resolve<ITaskService>();
                        noCreds = false;
                    }
                    catch (InvalidCredentialException ex)
                    {
                        /**
                     * Obviously credentials will need to be handled better in the actual interface. But for the "testing" console
                     * app we are just going to do things the easy way.
                    **/
                        Console.WriteLine("The following error ocurred: {0}", ex.Message);
                        Console.WriteLine("Please provide your credentials.");
                        var user = "";
                        while (string.IsNullOrWhiteSpace(user))
                        {
                            Console.WriteLine("Username:");
                            user = Console.ReadLine();
                        }
                        var pass = "";
                        while (string.IsNullOrWhiteSpace(pass))
                        {
                            Console.WriteLine("Password:");
                            pass = Console.ReadLine();
                        }

                        settingsService.SaveSetting("JiraUsername", user);
                        settingsService.SaveSetting("JiraPassword", pass);
                        noCreds = false;
                    }
                }

                if (projectService == null || taskService == null) return;

                var projects = projectService.GetProjects().ToList();
                var tasks = taskService.GetTasks();

                Console.WriteLine(OutputProjects(projects));
                Console.WriteLine(OutputTasks(tasks));

                Console.WriteLine("Getting Tasks for specific project");
                var project = projects.FirstOrDefault(x => x.Key == "CAT");
                Console.WriteLine(OutputTasks(taskService.GetTasksByProject(project)));

                Console.WriteLine("done");

                Console.Read();
            }
        }

        #region OutputProjects

        static string OutputProjects(IEnumerable<Project> projects)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Outputting Projects...");
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
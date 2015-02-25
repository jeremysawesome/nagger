namespace Nagger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Authentication;
    using System.Text;
    using Data;
    using Data.JIRA;
    using Interfaces;
    using Models;
    using Services;

    internal class Program
    {
        // jira documentation for api:
        //https://docs.atlassian.com/jira/REST/latest/#id165531

        // jira api browser: https://bunjil.jira-dev.com/plugins/servlet/restbrowser#/resource/api-2-issue-issueidorkey-worklog

        //get all projects: https://www.example.com/rest/api/latest/project
        // - search for issues https://www.example.com/rest/api/latest/search
        // get all ProjectName issues
        // https://www.example.com/rest/api/latest/search?jql=project%3D%22ProjectName%22

        static ITaskService _taskService;
        static ITimeService _timeService;
        static IProjectService _projectService;

        static IRemoteTimeRepository _testRemote;
        static ISettingsService _settingsService;

        static void Setup()
        {
            // for DI with unity refer to http://geekswithblogs.net/danielggarcia/archive/2014/01/23/introduction-to-dependency-injection-with-unity.aspx

            var localTaskRepo = new LocalTaskRepository();
            var localTimeRepo = new LocalTimeRepository(localTaskRepo);
            var localProjectRepo = new LocalProjectRepository();

            //var fakeRemoteTaskRepo = new FakeRemoteTaskRepository();
            var settingsRepository = new SettingsRepository();
            _settingsService = new SettingsService(settingsRepository);
            var baseJiraRepository = new BaseJiraRepository(_settingsService);
            var jiraRemoteTaskRepo = new JiraRemoteTaskRepository(baseJiraRepository);

            var remoteTimeRepo = new JiraRemoteTimeRepository(_settingsService);
            var remoteProjectRepository = new JiraRemoteProjectRepository(_settingsService);

            _taskService = new TaskService(localTaskRepo, jiraRemoteTaskRepo);
            _timeService = new TimeService(localTimeRepo, localTaskRepo, remoteTimeRepo);
            _projectService = new ProjectService(localProjectRepo, remoteProjectRepository);
        }

        static void PopulateTestData()
        {
            PopulateTestData(DateTime.Now);
            PopulateTestData(DateTime.Today.AddDays(4));
        }

        static void PopulateTestData(DateTime timeRecorded)
        {
            const int numEntries = 10;
            for (var i = 0; i < numEntries; i++)
            {
                var entry = _timeService.GetTestTimeEntry();
                entry.TimeRecorded = timeRecorded;
                entry.TimeRecorded = entry.TimeRecorded.AddHours(i); //add some time
                if (i%3 == 0)
                {
                    var task = _taskService.GetTestTask();
                    task.Id = "test";
                    task.Name = "test";
                    entry.Task = task;
                }
                _timeService.RecordTime(entry);
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
            try
            {
                Setup();
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

                _settingsService.SaveSetting("JiraUsername", user);
                _settingsService.SaveSetting("JiraPassword", pass);

                // try it again
                Setup();
            }

            var projects = _projectService.GetProjects().ToList();
            var tasks = _taskService.GetTasks();

            Console.WriteLine(OutputProjects(projects));
            Console.WriteLine(OutputTasks(tasks));

            Console.WriteLine("Getting Tasks for specific project");
            var project = projects.FirstOrDefault(x => x.Key == "CAT");
            Console.WriteLine(OutputTasks(_taskService.GetTasksByProject(project)));

            Console.WriteLine("done");

            Console.Read();
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
    }
}

namespace Nagger.Services.JIRA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Extensions;
    using Interfaces;
    using Models;

    public class JiraRunner : IRemoteRunner
    {
        readonly IInputService _inputService;
        readonly IOutputService _outputService;
        readonly IProjectService _projectService;
        readonly ITaskService _taskService;
        readonly ITimeService _timeService;

        public JiraRunner(IOutputService outputService, ITaskService taskService, ITimeService timeService,
            IProjectService projectService, IInputService inputService)
        {
            _outputService = outputService;
            _taskService = taskService;
            _timeService = timeService;
            _projectService = projectService;
            _inputService = inputService;
        }

        public Task AskForTask()
        {
            var mostRecentTasks = _taskService.GetTasksByTaskIds(_timeService.GetRecentlyRecordedTaskIds(5));
            _outputService.ShowInformation("Recent Tasks:");
            _outputService.OutputList(
                mostRecentTasks.Select(x => $"{x.Name,10} {x.Description.Truncate(50),10}"));

            var task = AskForSpecificTask();
            if (task != null) return task;

            _outputService.ShowInformation(
                "Ok. Let's figure out what you are working on. We're going to list some projects.");
            var projects = _projectService.GetProjects().ToList();
            _outputService.ShowInformation(OutputProjects(projects));
            var projectKey = _inputService.AskForInput("Which project Key are you working on?");
            _outputService.LoadingMessage("Getting tasks for that project. This might take a while.");
            var project = _projectService.GetProjectByKey(projectKey);
            var tasks = _taskService.GetTasksByProject(project);
            _outputService.ShowInformation("Ok. We've got the tasks. Outputting the tasks for that project.");
            _outputService.ShowInformation(OutputTasks(tasks));

            return AskForSpecificTask();
        }

        static string OutputProjects(ICollection<Project> projects)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Outputting {0} Projects...", projects.Count).AppendLine();
            foreach (var project in projects)
            {
                sb.AppendFormat("{0,10} || {1}", project.Key, project.Name);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        Task AskForSpecificTask()
        {
            var idIsKnown =
                _inputService.AskForBoolean("Do you know the key of the task you are working on? (example: CAT-102)");
            if (!idIsKnown) return null;
            var taskId = _inputService.AskForInput("What is the task key?");
            return _taskService.GetTaskByName(taskId);
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

            return $"{beginningSpace}{task.Name} - {task.Description.Truncate(50)}{Environment.NewLine}";
        }

        static string OutputTasks(IEnumerable<Task> tasks)
        {
            var sb = new StringBuilder();

            foreach (var task in tasks)
            {
                sb.Append(TaskString(task));
                if (task.HasTasks) sb.Append(OutputTasks(task.Tasks));
            }

            return sb.ToString();
        }
    }
}
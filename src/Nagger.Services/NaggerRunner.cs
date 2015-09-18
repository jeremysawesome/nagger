namespace Nagger.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Interfaces;
    using Models;

    public class NaggerRunner : IRunnerService
    {
        readonly IInputService _inputService;
        readonly IOutputService _outputService;
        readonly IProjectService _projectService;
        readonly ITaskService _taskService;
        readonly ITimeService _timeService;

        public NaggerRunner(IProjectService projectService, ITaskService taskService, ITimeService timeService,
            IInputService inputService, IOutputService outputService)
        {
            _projectService = projectService;
            _taskService = taskService;
            _timeService = timeService;
            _inputService = inputService;
            _outputService = outputService;
        }

        public void Run()
        {
            _timeService.DailyTimeSync();

            var askTime = DateTime.Now;

            // todo: somewhere in here we need to ask if the user is working (what if they are at home?) 
            // or do we care about this? If they aren't working they can just close nagger and restart it in the morning.


            _outputService.ShowInterface();
            var currentTask = _taskService.GetLastTask();
            if (currentTask != null)
            {
                var stillWorking = _inputService.AskForBoolean("Are you still working on " + currentTask.Name + "?");
                if (!stillWorking) currentTask = null;
            }
            if (currentTask == null) currentTask = AskForTask();

            if (currentTask == null)
            {
                _outputService.HideInterface();
                return;
            }

            // keep track of if we missed a check in with a variable set in the execute method (maybe a miss count)
            // check the variable here, if it's true then we missed a check in
            var runMiss = _timeService.IntervalsSinceLastRecord();

            // there will usually be 1 interval between the last time this ran and this time (it only makes sense)
            if (runMiss <= 1) _timeService.RecordTime(currentTask, askTime);
            else
            {
                AskAboutBreak(currentTask, askTime, runMiss);
            }
            _outputService.HideInterface();
        }

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

            return String.Format("{0}Name: {1} || Description: {2} || HasTasks: {3} {4}", beginningSpace, task.Name,
                task.Description,
                task.HasTasks, Environment.NewLine);
        }

        Task AskForSpecificTask()
        {
            var idIsKnown =
                _inputService.AskForBoolean("Do you know the key of the task you are working on? (example: CAT-102)");
            if (!idIsKnown) return null;
            var taskId = _inputService.AskForInput("What is the task key?");
            return _taskService.GetTaskByName(taskId);
        }

        Task AskForTask()
        {
            var task = AskForSpecificTask();
            if (task != null) return task;

            _outputService.ShowInformation(
                "Ok. Let's figure out what you are working on. We're going to list some projects.");
            var projects = _projectService.GetProjects().ToList();
            _outputService.ShowInformation(OutputProjects(projects));
            var projectId = _inputService.AskForInput("Which project Id are you working on?");
            _outputService.LoadingMessage("Getting tasks for that project. This might take a while.");
            var tasks = _taskService.GetTasksByProjectId(projectId);
            _outputService.ShowInformation("Ok. We've got the tasks. Outputting the tasks for that project.");
            _outputService.ShowInformation(OutputTasks(tasks));

            return AskForSpecificTask();
        }

        void AskAboutBreak(Task currentTask, DateTime askTime, int missedInterval)
        {
            if (
                !_inputService.AskForBoolean("Looks like we missed " + missedInterval +
                                             " check in(s). Were you on break?"))
            {
                _timeService.RecordTime(currentTask, askTime);
            }
            else
            {
                // insert an internal time marker for ask time
                _timeService.RecordMarker(askTime);

                if (_inputService.AskForBoolean("Have you worked on anything since you've been back?"))
                {
                    var intervalsMissed = _timeService.GetIntervalMinutes(missedInterval).ToList();

                    var minutesWorked =
                        _inputService.AskForSelection(
                            "Which of these options represents about how long you have been working?", intervalsMissed);

                    _timeService.RecordTime(currentTask, missedInterval, minutesWorked, askTime);
                }
                else
                {
                    // if no: insert a time entry for the current task for right now
                    _timeService.RecordTime(currentTask, DateTime.Now);
                }
            }
        }
    }
}
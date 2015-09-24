namespace Nagger.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ExtensionMethods;
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

            _outputService.ShowInterface();
            _outputService.OutputSound();
            var currentTask = _taskService.GetLastTask();
            if (currentTask != null)
            {
                var stillWorking = _inputService.AskForBoolean("Are you still working on " + currentTask.Name + "?");
                if (!stillWorking) currentTask = null;
            }

            if (currentTask == null)
            {
                if (!_inputService.AskForBoolean("Are you working?"))
                {
                    _timeService.RecordMarker(askTime);
                }
                else
                {
                    currentTask = AskForTask();   
                }
            }

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
                sb.AppendFormat("{0,10} || {1}", project.Key, project.Name);
                sb.AppendLine();
            }

            return sb.ToString();
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

        static string TaskString(Task task)
        {
            var beginningSpace = string.Empty;

            var checkingTask = task;
            while (checkingTask.HasParent)
            {
                beginningSpace += "  ";
                checkingTask = checkingTask.Parent;
            }

            return String.Format("{0}{1} - {2}{3}", beginningSpace, task.Name,
                task.Description.Truncate(50),
                Environment.NewLine);
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
            var mostRecentTasks = _taskService.GetTasksByTaskIds(_timeService.GetRecentlyRecordedTaskIds(5));
            _outputService.ShowInformation("Recent Tasks:");
            _outputService.OutputList(mostRecentTasks.Select(x=> string.Format("{0,10} {1,10}", x.Name,x.Description.Truncate(50))));

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
                var lastTime = _timeService.GetLastTimeEntry().TimeRecorded;
                var minutes = _timeService.GetIntervalMinutes(1).First();
                lastTime = lastTime.AddMinutes(minutes);

                // insert an internal time marker for ask time
                _timeService.RecordMarker(lastTime);

                if (_inputService.AskForBoolean("Have you worked on anything since you've been back?"))
                {
                    var intervalsMissed = _timeService.GetIntervalMinutes(missedInterval).ToList();

                    var minutesWorked =
                        _inputService.AskForSelection(
                            "Which of these options represents about how long you have been working?", intervalsMissed);

                    // insert an entry for when they started working
                    _timeService.RecordTime(currentTask, missedInterval, minutesWorked, lastTime);
                }
                
                // also insert an entry for the current time (since they are working and are no longer on break)
                _timeService.RecordTime(currentTask, DateTime.Now);
            }
        }
    }
}
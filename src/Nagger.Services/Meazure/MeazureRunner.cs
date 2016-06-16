namespace Nagger.Services.Meazure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models;

    public class MeazureRunner: IRemoteRunner
    {
        readonly ITaskService _taskService;
        readonly ITimeService _timeService;
        readonly IInputService _inputService;
        readonly IProjectService _projectService;

        public MeazureRunner(ITaskService taskService, ITimeService timeService, IInputService inputService, IProjectService projectService)
        {
            _taskService = taskService;
            _timeService = timeService;
            _inputService = inputService;
            _projectService = projectService;
        }

        public Task AskForTask()
        {
            var mostRecentTask = _taskService.GetLastTask();

            var currentProject = GetProject(mostRecentTask);

            var recentTaskNames = _taskService.GetTasksByTaskIds(_timeService.GetRecentlyRecordedTaskIds(5)).Select(x=>x.Name);
            var taskName = _inputService.AskForSelectionOrInput("Choose from a recent task. (If none of these match what you are doing then just leave blank.)", recentTaskNames.ToList());

            var currentTask = (string.IsNullOrWhiteSpace(taskName)) ? null: _taskService.GetTaskByName(taskName);

            if (currentTask == null)
            {
                var tasks = _taskService.GetGeneralTasks();
                currentTask = _inputService.AskForSelection("Ok. Above is a list of tasks. Which one were you working on?",
                    tasks.ToList());
            }

            currentTask.Project = currentProject;

            return currentTask;
        }

        public Task AskForAssociatedTask(Task currentTask)
        {
            if (!_inputService.AskForBoolean("Associate this entry with an additional task?")) return null;

            Task associatedTask;
            var recentlyAssociatedTasks = _taskService.GetTasksByTaskIds(_timeService.GetRecentlyAssociatedTaskIds(5, currentTask)).ToDictionary(key=> key.Name);
            var associatedTaskName = _inputService.AskForSelectionOrInput("Choose from a recent task or insert a new task name",
                    recentlyAssociatedTasks.Keys.ToList());

            if (recentlyAssociatedTasks.TryGetValue(associatedTaskName, out associatedTask)) return associatedTask;

            return _taskService.GetAssociatedTaskByName(associatedTaskName, currentTask.Project);
        }

        //TODO: Pull this function and the one in Program.cs out into a shared location
        static IEnumerable<SupportedRemoteRepository> SupportedRemoteRepositories()
        {
            return Enum.GetValues(typeof(SupportedRemoteRepository)).Cast<SupportedRemoteRepository>();
        }

        Project GetProject(Task currentTask)
        {
            var project = AskAboutProject(currentTask);

            if (!project.AssociatedRemoteRepository.HasValue && _inputService.AskForBoolean("Would you like to record time for this project in an additional repository?"))
            {
                var selectedRepository = _inputService.AskForSelection("What will your additional remote repository be?",
                    SupportedRemoteRepositories().ToList());
                _projectService.AssociateProjectWithRepository(project, selectedRepository);
            }

            return project;
        }

        Project AskAboutProject(Task currentTask)
        {
            if (currentTask?.Project != null && _inputService.AskForBoolean($"Are you still working on {currentTask.Project.Name}?"))
            {
                return currentTask.Project;
            }

            var recentProjects = _projectService.GetProjectsByIds(_timeService.GetRecentlyRecordedProjectIds()).ToDictionary(key=>key.Name);

            if (recentProjects.Any())
            {
                var selectedProjectName = _inputService.AskForSelectionOrInput("Select a recent project. (Or insert a new project name)",
                    recentProjects.Keys.ToList());

                Project recentProject;
                if (recentProjects.TryGetValue(selectedProjectName, out recentProject)) return recentProject;
            }

            if (_inputService.AskForBoolean("Do you know the name of the project you are working on?"))
            {
                var projectName = _inputService.AskForInput("What is the name?");
                var project = _projectService.GetProjectByName(projectName);
                if (project != null) return project;
            }

            var projects = _projectService.GetProjects();

            return _inputService.AskForSelection("Ok. Above is a list of projects. Which one were you working on?",
                projects.ToList());
        }
    }
}

namespace Nagger.Services.Fake
{
    using System.Linq;
    using Interfaces;
    using Models;

    public class FakeRunner : IRemoteRunner
    {
        readonly ITaskService _taskService;
        readonly IOutputService _outputService;

        public FakeRunner(ITaskService taskService, IOutputService outputService)
        {
            _taskService = taskService;
            _outputService = outputService;
        }

        public Task AskForTask()
        {
            // fake runner just use the first general task always
            var task = _taskService.GetGeneralTasks().First();
            _outputService.ShowInformation($"You are working on {task.Name}.");
            return task;
        }

        public Task AskForAssociatedTask(Task currentTask)
        {
            return null;
        }
    }
}
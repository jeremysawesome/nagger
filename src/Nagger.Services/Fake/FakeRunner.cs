namespace Nagger.Services.Fake
{
    using Interfaces;
    using Models;

    public class FakeRunner : IRemoteRunner
    {
        public Task AskForTask()
        {
            return null;
        }

        public Task AskForAssociatedTask(Task currentTask)
        {
            return null;
        }
    }
}
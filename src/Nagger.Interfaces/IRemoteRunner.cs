namespace Nagger.Interfaces
{
    using Models;

    public interface IRemoteRunner
    {
        Task AskForTask();

        Task AskForAssociatedTask(Task currentTask);
    }
}

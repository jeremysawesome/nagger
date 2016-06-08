namespace Nagger.Interfaces
{
    using Models;

    public interface IRemoteRunner
    {
        Task AskForTask();
    }
}

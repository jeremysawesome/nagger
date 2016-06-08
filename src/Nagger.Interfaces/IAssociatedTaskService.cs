namespace Nagger.Interfaces
{
    using Models;

    public interface IAssociatedTaskService
    {
        IRemoteTaskRepository GetAssociatedRemoteTaskRepository(Project project);
    }
}

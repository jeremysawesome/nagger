namespace Nagger.Interfaces
{
    using Models;

    public interface IAssociatedRemoteRepositoryService
    {
        IRemoteTaskRepository GetAssociatedRemoteTaskRepository(Project project);
    }
}

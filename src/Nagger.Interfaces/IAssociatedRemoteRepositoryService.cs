namespace Nagger.Interfaces
{
    using Models;

    public interface IAssociatedRemoteRepositoryService
    {
        IRemoteTaskRepository GetAssociatedRemoteTaskRepository(Project project);
        IRemoteTimeRepository GetAssociatedRemoteTimeRepository(Project project);
        void InitializeAssociatedRepositories(Project project);
    }
}

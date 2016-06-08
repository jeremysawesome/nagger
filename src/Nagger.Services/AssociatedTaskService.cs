namespace Nagger.Services
{
    using Autofac.Features.Indexed;
    using Interfaces;
    using Models;

    public class AssociatedTaskService : IAssociatedTaskService
    {
        readonly IIndex<SupportedRemoteRepository, IRemoteTaskRepository> _remoteTaskRepositories;

        public AssociatedTaskService(IIndex<SupportedRemoteRepository, IRemoteTaskRepository> remoteTaskRepositories)
        {
            _remoteTaskRepositories = remoteTaskRepositories;
        }

        public IRemoteTaskRepository GetAssociatedRemoteTaskRepository(Project project)
        {
            return project.AssociatedRemoteRepository == null ? null : _remoteTaskRepositories[project.AssociatedRemoteRepository.Value];
        }
    }
}
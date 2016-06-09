namespace Nagger.Services
{
    using Autofac.Features.Indexed;
    using Interfaces;
    using Models;

    public class AssociatedRemoteRepositoryService : IAssociatedRemoteRepositoryService
    {
        readonly IIndex<SupportedRemoteRepository, IRemoteTaskRepository> _remoteTaskRepositories;
        readonly IIndex<SupportedRemoteRepository, IRemoteTimeRepository> _remoteTimeRepositories;

        public AssociatedRemoteRepositoryService(IIndex<SupportedRemoteRepository, IRemoteTaskRepository> remoteTaskRepositories, IIndex<SupportedRemoteRepository, IRemoteTimeRepository> remoteTimeRepositories)
        {
            _remoteTaskRepositories = remoteTaskRepositories;
            _remoteTimeRepositories = remoteTimeRepositories;
        }

        public IRemoteTaskRepository GetAssociatedRemoteTaskRepository(Project project)
        {
            if (project.AssociatedRemoteRepository == null) return null;

            var remoteRepository = _remoteTaskRepositories[project.AssociatedRemoteRepository.Value];
            remoteRepository.InitializeForProject(project);
            return remoteRepository;
        }

        public IRemoteTimeRepository GetAssociatedRemoteTimeRepository(Project project)
        {
            if (project.AssociatedRemoteRepository == null) return null;

            var remoteRepository = _remoteTimeRepositories[project.AssociatedRemoteRepository.Value];
            remoteRepository.InitializeForProject(project);
            return remoteRepository;
        }

        public void InitializeAssociatedRepositories(Project project)
        {
            GetAssociatedRemoteTaskRepository(project);
            GetAssociatedRemoteTimeRepository(project);
        }
    }
}
namespace Nagger.Services
{
    using System.Collections.Generic;
    using Interfaces;
    using Models;

    public class ProjectService : IProjectService
    {
        readonly ILocalProjectRepository _localProjectRepository;
        readonly IRemoteProjectRepository _remoteProjectRepository;
        readonly IAssociatedRemoteRepositoryService _associatedRemoteRepositoryService;

        public ProjectService(ILocalProjectRepository localProjectRepository,
            IRemoteProjectRepository remoteProjectRepository, IAssociatedRemoteRepositoryService associatedRemoteRepositoryService)
        {
            _localProjectRepository = localProjectRepository;
            _remoteProjectRepository = remoteProjectRepository;
            _associatedRemoteRepositoryService = associatedRemoteRepositoryService;
        }

        public IEnumerable<Project> GetProjects()
        {
            SyncProjectsWithRemote();
            return _localProjectRepository.GetProjects();
        }

        public IEnumerable<Project> GetProjectsByIds(IEnumerable<string> ids)
        {
            return _localProjectRepository.GetProjectsByIds(ids);
        }

        public Project GetProjectById(string id)
        {
            return _localProjectRepository.GetProjectById(id);
        }

        public Project GetProjectByKey(string key)
        {
            return _localProjectRepository.GetProjectByKey(key);
        }

        public Project GetProjectByName(string name)
        {
            return _localProjectRepository.GetProjectByName(name);
        }

        public void AssociateProjectWithRepository(Project project, SupportedRemoteRepository repository)
        {
            project.AssociatedRemoteRepository = repository;
            _associatedRemoteRepositoryService.InitializeAssociatedRepositories(project);
            _localProjectRepository.StoreProject(project);
        }

        void SyncProjectsWithRemote()
        {
            /**
             * Todo: look into a way to improve the way projects are retrieved. Ideally we would only pull the ones we don't have
            **/
            var remoteProjects = _remoteProjectRepository.GetProjects();

            foreach (var remoteProject in remoteProjects)
            {
                _localProjectRepository.StoreProject(remoteProject);
            }
        }
    }
}
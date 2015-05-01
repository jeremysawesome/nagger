using System.Collections.Generic;
using Nagger.Interfaces;
using Nagger.Models;

namespace Nagger.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ILocalProjectRepository _localProjectRepository;
        private readonly IRemoteProjectRepository _remoteProjectRepository;

        public ProjectService(ILocalProjectRepository localProjectRepository,
            IRemoteProjectRepository remoteProjectRepository)
        {
            _localProjectRepository = localProjectRepository;
            _remoteProjectRepository = remoteProjectRepository;
        }

        public IEnumerable<Project> GetProjects()
        {
            SyncProjectsWithRemote();
            return _localProjectRepository.GetProjects();
        }

        private void SyncProjectsWithRemote()
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

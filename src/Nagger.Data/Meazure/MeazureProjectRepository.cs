namespace Nagger.Data.Meazure
{
    using System.Collections.Generic;
    using System.Linq;
    using API;
    using Interfaces;
    using Models;
    using RestSharp;

    public class MeazureProjectRepository :IRemoteProjectRepository
    {
        readonly MeazureApi _api;

        public MeazureProjectRepository(ISettingsService settingsService, IInputService inputService)
        {
            var baseRepository = new BaseMeazureRepository(settingsService, inputService);
            _api = new MeazureApi(baseRepository.User, baseRepository.ApiBaseUrl, "/Project");
        }

        public IEnumerable<Project> GetProjects()
        {
            var request = new RestRequest {Resource = "List"};

            var apiResult = _api.Execute<List<DTO.Project>>(request);

            return apiResult?.Select(project => new Project
            {
                Id = project.Id.ToString(),
                Name = project.Name,
                Key = project.FromIntegrationId
            });
        }
    }
}

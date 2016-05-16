namespace Nagger.Data.JIRA
{
    using System.Collections.Generic;
    using System.Linq;
    using API;
    using Interfaces;
    using Models;
    using RestSharp;

    public class JiraRemoteProjectRepository : BaseJiraRepository, IRemoteProjectRepository
    {
        readonly JiraApi _api;

        public JiraRemoteProjectRepository(ISettingsService settingsService, IInputService inputService)
            : base(settingsService, inputService)
        {
            // the project and time repositories use different API's but the same user and password
            // question: how do I best go about getting the correct urls?
            // do I just assume that the last portion of the url will always be the same?
            // we'll go down that route for now
            _api = new JiraApi(JiraUser, ApiBaseUrl);
        }

        public IEnumerable<Project> GetProjects()
        {
            var request = new RestRequest {Resource = "project"};

            var apiResult = _api.Execute<List<DTO.Project>>(request);

            return apiResult?.Select(x => new Project
            {
                Id = x.id,
                Name = x.name,
                Key = x.key
            });
        }
    }
}

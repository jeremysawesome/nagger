namespace Nagger.Data.JIRA
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Authentication;
    using API;
    using Interfaces;
    using Models;
    using RestSharp;

    public class JiraRemoteProjectRepository : BaseJiraRepository, IRemoteProjectRepository
    {
        readonly JiraApi _api;

        public JiraRemoteProjectRepository(ISettingsService settingsService)
            : base(settingsService)
        {
            if (!UserExists) throw new InvalidCredentialException("There is no JIRA user specified");
            _api = new JiraApi(JiraUser);
        }

        public IEnumerable<Project> GetProjects()
        {
            var request = new RestRequest {Resource = "project"};

            var apiResult = _api.Execute<List<DTO.Project>>(request);

            if (apiResult == null) return null;

            return apiResult.Select(x => new Project
            {
                Id = x.id,
                Name = x.name,
                Key = x.key
            });
        }
    }
}

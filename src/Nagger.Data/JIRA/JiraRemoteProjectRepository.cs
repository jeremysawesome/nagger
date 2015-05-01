using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using Nagger.Data.JIRA.API;
using Nagger.Interfaces;
using Nagger.Models;
using RestSharp;

namespace Nagger.Data.JIRA
{
    public class JiraRemoteProjectRepository : BaseJiraRepository, IRemoteProjectRepository
    {
        private readonly JiraApi _api;

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
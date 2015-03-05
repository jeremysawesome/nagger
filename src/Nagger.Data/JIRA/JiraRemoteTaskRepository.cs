namespace Nagger.Data.JIRA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using API;
    using DTO;
    using Interfaces;
    using Models;
    using RestSharp;
    using Project = Models.Project;

    public class JiraRemoteTaskRepository : IRemoteTaskRepository
    {
        readonly JiraApi _api;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        readonly BaseJiraRepository _baseJiraRepository;

        public JiraRemoteTaskRepository(BaseJiraRepository baseJiraRepository)
        {
            _baseJiraRepository = baseJiraRepository;
            _api = new JiraApi(_baseJiraRepository.JiraUser);
        }


        /**
		 * Todo: Note: The JIRA api has a `maxResults` and a `total` property. If there are a sufficient number of issues then we end up with
		 * A response with only `maxResults` number of results. It could be that there are more results, so we are going to have to 
		 * handle this.
         * 
         * 
         * Each task has an ID. This is stored in the local db. We can use this id to grab tasks that are greater than a certain id (the last id in the 
         * database). This should allow us to grab what we need (and only load the newest tasks).
		**/

        // quick note: It appears that "id" in JQL is equal to the Issue Key (ex: CAT-123) and not the actual Id that JIRA assigns to an issue

        public IEnumerable<Task> GetTasks(string lastTaskId = null)
        {
            //todo: add support for "all results" instead of the limit, maybe a "while more" loop?
            //https://www.example.com/rest/api/latest/search?jql=project%3D%22ProjectName%22&fields=summary

            var request = new RestRequest
            {
                Resource = "search",
                Parameters =
                {
                    new Parameter {Name = "fields", Type = ParameterType.QueryString, Value = "summary,parent"}
                }
            };

            if (lastTaskId != null)
            {
                request.Parameters.Add(new Parameter
                {
                    Name = "jql",
                    Type = ParameterType.QueryString,
                    Value = "id > " + lastTaskId
                });
            }

            var apiResult = _api.Execute<TaskResult>(request);

            if (apiResult == null || apiResult.issues == null) return null;

            return apiResult.issues.Select(x => new Task
            {
                Id = x.id,
                Name = x.key,
                Description = (x.fields == null) ? "" : x.fields.summary,
                Project = (x.fields == null || x.fields.project == null)
                    ? null
                    : new Project
                    {
                        Id = x.fields.project.id,
                        Key = x.fields.project.key,
                        Name = x.fields.project.name
                    },
                Parent = (x.fields == null || x.fields.parent == null) ? null : new Task {Id = x.fields.parent.id}
            });
        }

        public IEnumerable<Task> GetTasks()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Task> GetTasks(Project project)
        {
            // https://www.example.com/rest/api/latest/search?jql=project%3D%22ProjectName%22%20order%20by%20id&fields=summary
            var request = new RestRequest
            {
                Resource = "search",
                Parameters =
                {
                    new Parameter
                    {
                        Name = "jql",
                        Type = ParameterType.QueryString,
                        Value = string.Format("project=\"{0}\" order by id", project.Name)
                    },
                    new Parameter {Name = "fields", Type = ParameterType.QueryString, Value = "summary"}
                }
            };

            var apiResult = _api.Execute<TaskResult>(request);

            if (apiResult == null || apiResult.issues == null) return null;

            return apiResult.issues.Select(x => new Task
            {
                Id = x.id,
                Name = x.key,
                Description = (x.fields == null) ? "" : x.fields.summary,
                Project = project,
                Parent = (x.fields == null || x.fields.parent == null) ? null : new Task {Id = x.fields.parent.id}
            });
        }
    }
}

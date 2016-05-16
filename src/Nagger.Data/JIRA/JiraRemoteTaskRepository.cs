namespace Nagger.Data.JIRA
{
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
            _api = new JiraApi(_baseJiraRepository.JiraUser, _baseJiraRepository.ApiBaseUrl);
        }


        /**
		 * Todo: Note: The JIRA api has a `maxResults` and a `total` property. If there are a sufficient number of issues then we end up with
		 * A response with only `maxResults` number of results. It could be that there are more results, so we are going to have to 
		 * handle this.
         * 
         * 
         * Each task has an ID. This is stored in the local db. We can use this projectId to grab tasks that are greater than a certain projectId (the last projectId in the 
         * database). This should allow us to grab what we need (and only load the newest tasks).
		**/

        // quick note: It appears that "projectId" in JQL is equal to the Issue Key (ex: CAT-123) and not the actual Id that JIRA assigns to an issue
        // - or.. to be more specific. the "projectId" can be either IssueKey or IssueId.

        // more thoughts: It is possible for stories/tasks to be renamed, or even deleted. How are we going to handle this?

        // potentially order by date created over the actual projectId. This seems to work: https://www.example.com/rest/api/latest/search?jql=order%20by%20created%20asc&fields=summary,parent


        // todo: it's possible for issues to be deleted from JIRA. If this is the case we won't be able to track time against them.
        // also - it doesn't seem like JIRA tracks deleted issues. If they are gone then they are gone. So... we are going to need
        // to somehow check and remove issues from our DB (or mark them as removed)
        // maybe this can be part of the SyncWithRemote functionality? not sure how to handle it right now. Whatever it is needs to be
        // as fast as possible. We don't want to ping JIRA unnecessarily.
        // currrently this problem makes it so we cannot track tasks accurately (because we aren't able to get the new issues)

        public IEnumerable<Task> GetTasks(string lastTaskId = null)
        {
            //todo: add support for "all results" instead of the limit, maybe a "while more" loop?
            //https://www.example.com/rest/api/latest/search?jql=project%3D%22ProjectName%22&fields=summary,parent

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

            return apiResult?.issues?.Select(x => new Task
            {
                Id = x.id,
                Name = x.key,
                Description = (x.fields == null) ? "" : x.fields.summary,
                Project = (x.fields?.project == null)
                    ? null
                    : new Project
                    {
                        Id = x.fields.project.id,
                        Key = x.fields.project.key,
                        Name = x.fields.project.name
                    },
                Parent = (x.fields?.parent == null) ? null : new Task {Id = x.fields.parent.id}
            });
        }

        public Task GetTaskByName(string name)
        {
            var request = new RestRequest
            {
                Resource = "search",
                Parameters =
                {
                    new Parameter {Name = "fields", Type = ParameterType.QueryString, Value = "summary,parent,project"},
                    new Parameter {Name = "jql", Type = ParameterType.QueryString, Value = "id = " + name}
                }
            };

            var apiResult = _api.Execute<TaskResult>(request);

            if (apiResult?.issues == null || !apiResult.issues.Any()) return null;

            var issue = apiResult.issues[0];
            return new Task
            {
                Id = issue.id,
                Name = issue.key,
                Description = (issue.fields == null) ? "" : issue.fields.summary,
                Project = (issue.fields?.project == null)
                    ? null
                    : new Project
                    {
                        Id = issue.fields.project.id,
                        Key = issue.fields.project.key,
                        Name = issue.fields.project.name
                    },
                Parent = (issue.fields?.parent == null) ? null : new Task { Id = issue.fields.parent.id }
            };
        }

        public IEnumerable<Task> GetTasks()
        {
            // todo: Implement this
            return new List<Task>();
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
                        Value = $"project=\"{project.Name}\" order by id"
                    },
                    new Parameter {Name = "fields", Type = ParameterType.QueryString, Value = "summary"}
                }
            };

            var apiResult = _api.Execute<TaskResult>(request);

            return apiResult?.issues?.Select(x => new Task
            {
                Id = x.id,
                Name = x.key,
                Description = (x.fields == null) ? "" : x.fields.summary,
                Project = project,
                Parent = (x.fields?.parent == null) ? null : new Task {Id = x.fields.parent.id}
            });
        }

        Task BuildTaskFromIssue(Issue issue)
        {
            return new Task
            {
                Id = issue.id,
                Name = issue.key,
                Description = (issue.fields == null) ? "" : issue.fields.summary,
                Project = (issue.fields?.project == null)
                    ? null
                    : new Project
                    {
                        Id = issue.fields.project.id,
                        Key = issue.fields.project.key,
                        Name = issue.fields.project.name
                    },
                Parent =
                    (issue.fields?.parent == null)
                        ? null
                        : new Task {Id = issue.fields.parent.id}
            };
        }

        public IEnumerable<Task> GetTasksByProjectId(string projectId, string lastTaskId = null)
        {
            lastTaskId = lastTaskId ?? "";
            while (true)
            {
                var lastTaskCondition = (string.IsNullOrEmpty(lastTaskId)) ? "" : "AND id > " + lastTaskId;
                var request = new RestRequest
                {
                    Resource = "search",
                    Parameters =
                    {
                        new Parameter
                        {
                            Name = "jql",
                            Type = ParameterType.QueryString,
                            Value = $"project=\"{projectId}\" {lastTaskCondition} order by id"
                        },
                        new Parameter {Name = "fields", Type = ParameterType.QueryString, Value = "summary,project"}
                    }
                };

                var apiResult = _api.Execute<TaskResult>(request);
                if (apiResult?.issues == null) yield break;

                foreach (var issue in apiResult.issues)
                {
                    var task = BuildTaskFromIssue(issue);
                    if (task.Project == null) task.Project = new Project {Id = projectId};
                    yield return task;
                }

                // because we limit the query, the total value will change. It will decrease every single time we limit the query
                // so we need to continue until the cound of issue is greater than or equal to the total
                if (apiResult.startAt + apiResult.issues.Count < apiResult.total)
                {
                    lastTaskId = apiResult.issues.Last().id;
                    continue;
                }
                yield break;
            }
        }
    }
}
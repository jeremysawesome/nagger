namespace Nagger.Data.JIRA
{
    using System;
    using API;
    using DTO;
    using Extensions;
    using Interfaces;
    using Models;
    using RestSharp;
    using Project = Models.Project;

    public class JiraTimeRepository : IRemoteTimeRepository
    {
        readonly JiraApi _api;
      

        public JiraTimeRepository(BaseJiraRepository baseJiraRepository)
        {
            _api = new JiraApi(baseJiraRepository.JiraUser, baseJiraRepository.ApiBaseUrl);
        }

        public bool RecordTime(TimeEntry timeEntry)
        {
            if (!timeEntry.HasTask) return false;
            return RecordTime(timeEntry, timeEntry.Task);
        }

        public bool RecordAssociatedTime(TimeEntry timeEntry)
        {
            if (!timeEntry.HasAssociatedTask) return false;
            try
            {
                return RecordTime(timeEntry, timeEntry.AssociatedTask);
            }
            catch (ApplicationException)
            {
                return false;
            }
        }

        public void InitializeForProject(Project project)
        {
            _baseJiraRepository.KeyModifier = project.Id;
            _api = new JiraApi(_baseJiraRepository.JiraUser, _baseJiraRepository.ApiBaseUrl);
        }

        // needs to post to: /rest/api/2/issue/{issueIdOrKey}/worklog
        bool RecordTime(TimeEntry timeEntry, Task task)
        {
            // jira requires a special format - like ISO 8601 but not quite
            const string jiraTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffK";

            // jira doesn't like the colon in the ISO 8601 string. so we strip it out.
            var timeStarted = timeEntry.TimeRecorded.ToString(jiraTimeFormat).ReplaceLastOccurrence(":", "");

            var worklog = new Worklog
            {
                comment = timeEntry.Comment ?? "",
                started = timeStarted,
                timeSpent = timeEntry.MinutesSpent +"m"
            };

            var post = new RestRequest()
            {
                Resource = "issue/"+task.Id+"/worklog?adjustEstimate=leave",
                Method = Method.POST,
                RequestFormat = DataFormat.Json
            };

            post.AddBody(worklog);

            var result = _api.Execute<Worklog>(post);
            return result != null;
        }
    }
}

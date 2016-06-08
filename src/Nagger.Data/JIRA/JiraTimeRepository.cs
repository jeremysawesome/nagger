namespace Nagger.Data.JIRA
{
    using API;
    using DTO;
    using Extensions;
    using Interfaces;
    using Models;
    using RestSharp;

    public class JiraTimeRepository : IRemoteTimeRepository
    {
        readonly JiraApi _api;
      

        public JiraTimeRepository(BaseJiraRepository baseJiraRepository)
        {
            _api = new JiraApi(baseJiraRepository.JiraUser, baseJiraRepository.ApiBaseUrl);
        }

        // needs to post to: /rest/api/2/issue/{issueIdOrKey}/worklog
        public bool RecordTime(TimeEntry timeEntry)
        {
            // Jira requires a task
            if (!timeEntry.HasTask) return false;
            
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
                Resource = "issue/"+timeEntry.Task.Id+"/worklog?adjustEstimate=leave",
                Method = Method.POST,
                RequestFormat = DataFormat.Json
            };

            post.AddBody(worklog);

            var result = _api.Execute<Worklog>(post);
            return result != null;
        }
    }
}

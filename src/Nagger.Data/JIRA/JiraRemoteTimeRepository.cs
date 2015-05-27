namespace Nagger.Data.JIRA
{
    using System;
    using Interfaces;
    using Models;

    public class JiraRemoteTimeRepository : BaseJiraRepository, IRemoteTimeRepository
    {
        const string apiUrl = "https://www.example.com/rest/api/latest";
        // insert a worklog in Jira
        /*        {
            "self": "http://www.example.com/jira/rest/api/2/issue/10010/worklog/10000",
            "author": {
                "self": "http://www.example.com/jira/rest/api/2/user?username=fred",
                "name": "fred",
                "displayName": "Fred F. User",
                "active": false
            },
            "updateAuthor": {
                "self": "http://www.example.com/jira/rest/api/2/user?username=fred",
                "name": "fred",
                "displayName": "Fred F. User",
                "active": false
            },
            "comment": "I did some work here.",
            "visibility": {
                "type": "group",
                "value": "jira-developers"
            },
            "started": "2014-10-28T06:52:53.720+0000",
            "timeSpent": "3h 20m",
            "timeSpentSeconds": 12000,
            "id": "100028"
        }*/

        public JiraRemoteTimeRepository(ISettingsService settingsService, IInputService inputService) : base(settingsService,inputService)
        {
        }

        public bool RecordTime(TimeEntry timeEntry)
        {
            // currently looking at: http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
            // Jira requires a task
            if (!timeEntry.HasTask) return false;


            // https://www.example.com/rest/api/latest/search?jql=project%3D%22ProjectName%22
            // needs to post to: /rest/api/2/issue/{issueIdOrKey}/worklog
            // todo: implement this instead of just returning false
            return false;
        }
    }
}

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
        JiraApi _api;
        readonly BaseJiraRepository _baseJiraRepository;
        readonly ISettingsService _settingsService;
        readonly IInputService _inputService;
        const string AdjustEstimateKey = "AdjustJiraEstimate";
        JiraApi Api => _api ?? (_api = new JiraApi(_baseJiraRepository.JiraUser, _baseJiraRepository.ApiBaseUrl));

        public JiraTimeRepository(BaseJiraRepository baseJiraRepository, ISettingsService settingsService, IInputService inputService)
        {
            _baseJiraRepository = baseJiraRepository;
            _settingsService = settingsService;
            _inputService = inputService;
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
            AdjustJiraEstimate();
        }

        bool AdjustJiraEstimate(Project project = null)
        {
            var settingKey = _baseJiraRepository.GetSettingKey(AdjustEstimateKey);
            if (!_settingsService.GetSetting<string>(settingKey).IsNullOrWhitespace()) return _settingsService.GetSetting<bool>(settingKey);

            var projectString = (project != null) ? " for {0} ".FormatWith(project.Name) : "";
            var adjustJiraEstimate = _inputService.AskForBoolean("Would you like JIRA to automatically adjust estimates {0}when you log your time?".FormatWith(projectString));
            _settingsService.SaveSetting(settingKey, adjustJiraEstimate.ToString());
            return adjustJiraEstimate;
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

            var adjustEstimate = !AdjustJiraEstimate() ? "?adjustEstimate=leave": "";

            var post = new RestRequest()
            {
                Resource = "issue/{0}/worklog{1}".FormatWith(task.Id, adjustEstimate),
                Method = Method.POST,
                RequestFormat = DataFormat.Json
            };

            post.AddBody(worklog);

            var result = Api.Execute<Worklog>(post);
            return result != null;
        }
    }
}

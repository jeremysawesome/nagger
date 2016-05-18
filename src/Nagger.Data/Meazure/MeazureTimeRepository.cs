namespace Nagger.Data.Meazure
{
    using System.Collections.Generic;
    using API;
    using DTO;
    using Interfaces;
    using Models;
    using RestSharp;

    public class MeazureTimeRepository : IRemoteTimeRepository
    {
        readonly MeazureApi _api;

        public MeazureTimeRepository(ISettingsService settingsService, IInputService inputService)
        {
            var baseRepository = new BaseMeazureRepository(settingsService, inputService);
            _api = new MeazureApi(baseRepository.User, baseRepository.ApiBaseUrl);
        }

        public bool RecordTime(TimeEntry timeEntry)
        {
            // meazure requires either Notes, a project, or a task
            if (!timeEntry.HasProject && !timeEntry.HasTask && !timeEntry.HasComment) return false;

            var timeEntryModel = new TimeEntryModel
            {
                Date = timeEntry.TimeRecorded.ToString("O"),
                Notes = timeEntry.Comment,
                TimeString = timeEntry.MinutesSpent + "m",
                DurationSeconds = timeEntry.MinutesSpent*60,
                ProjectId = timeEntry.Project?.Id,
                TaskId = timeEntry.Task?.Id,
                WorkItems = new List<string>(), //TODO: add functionality for tracking WorkItems
            };

            var post = new RestRequest
            {
                Resource = "Time/Save",
                Method = Method.POST,
                RequestFormat = DataFormat.Json
            };

            post.AddBody(timeEntryModel);

            var result = _api.Execute<TimeEntryModel>(post);
            return result != null;
        }
    }
}

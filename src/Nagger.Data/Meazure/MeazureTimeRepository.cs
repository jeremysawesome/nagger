namespace Nagger.Data.Meazure
{
    using System.Collections.Generic;
    using DTO;
    using Interfaces;
    using Models;

    public class MeazureTimeRepository : IRemoteTimeRepository
    {
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



            throw new System.NotImplementedException();
        }
    }
}

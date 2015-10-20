namespace Nagger.Data.Meazure
{
    using System;
    using System.Collections.Generic;
    using DTO;
    using Interfaces;
    using Models;

    public class MeazureRemoteTimeRepository : IRemoteTimeRepository
    {
        public bool RecordTime(TimeEntry timeEntry)
        {
            /**
             note. Add in an implementation for Meazure.
             you should be able to post a JSON object to /Time/Save  which will save a single time entry
             example JSON:
             {
                "Date":"2015-10-16T00:00:00.000Z",
                "Notes":"Example Notes(comments)",
                "ProjectId":"426", // project
                "TaskId":"3", // this is more like a "task type"
                "WorkItems":[], // task
                "TimeString":"120m",
                "DurationSeconds":7200
            }
             a workitem in Meazure is akin to a issue key in JIRA. 
            **/

            // meazure requires either Notes, a project, or a task
            if (!timeEntry.HasProject && !timeEntry.HasTask && !timeEntry.HasComment) return false;

            var timeEntryModel = new TimeEntryModel
            {
                Date = timeEntry.TimeRecorded.ToString("O"),
                Notes = timeEntry.Comment,
                TimeString = timeEntry.MinutesSpent + "m",
                DurationSeconds = timeEntry.MinutesSpent * 60,
                ProjectId = timeEntry.Project?.Id,
                TaskId = timeEntry.Task?.Id // note the task is different in meazure, a task is more of a "task type" in meazure
            };
            throw new NotImplementedException();
        }
    }
}

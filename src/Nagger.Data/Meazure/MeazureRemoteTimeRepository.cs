namespace Nagger.Data.Meazure
{
    using System;
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
            throw new NotImplementedException();
        }
    }
}

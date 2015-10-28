﻿namespace Nagger.Data.Meazure
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

            /** note: It would be nice to be able to pull projects/tasks from JIRA and insert them into Meazure.
             * however, Meazure has it's own concept of tasks and projects. 
             * Example: The Meazure project might be "Work with this Client" but the client has it's own projects and tasks.
             *    in that case, I guess that the Project would be the Meazure project and the client's projects and tasks would be tasks and sub tasks?
             *      would it then be wise to, when tracking time, track the task and subtask in the WorkItems list collection?
            **/
            var timeEntryModel = new TimeEntryModel
            {
                Date = timeEntry.TimeRecorded.ToString("O"),
                Notes = timeEntry.Comment,
                TimeString = timeEntry.MinutesSpent + "m",
                DurationSeconds = timeEntry.MinutesSpent*60,
                ProjectId = timeEntry.Project?.Id,
                TaskId = timeEntry.Task?.Id,// note the task is different in meazure, a task is more of a "task type" in meazure
                WorkItems = new List<string> { timeEntry.Task?.Id }

            };


            /*
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
            */

            throw new NotImplementedException();
        }
    }
}
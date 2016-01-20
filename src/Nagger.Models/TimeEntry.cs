namespace Nagger.Models
{
    using System;

    public class TimeEntry
    {
        public TimeEntry()
        {
        }

        public TimeEntry(Task task, string comment = null)
        {
            TimeRecorded = DateTime.Now;
            Task = task;
            Comment = comment;
            Project = task.Project;
        }

        public TimeEntry(Task task, DateTime time, string comment = null)
        {
            Task = task;
            TimeRecorded = time;
            Comment = comment;
            Project = task.Project;
        }

        public TimeEntry(Project project, int minutesSpent, string comment = null)
        {
            TimeRecorded = DateTime.Now;
            Project = project;
            MinutesSpent = minutesSpent;
            Comment = comment;
        }

        public TimeEntry(Task task, int minutesSpent, string comment = null)
        {
            TimeRecorded = DateTime.Now;
            Task = task;
            Comment = comment;
            MinutesSpent = minutesSpent;
            Project = task.Project;
        }

        public int Id { get; set; }
        public DateTime TimeRecorded { get; set; }
        public string Comment { get; set; }
        public Task Task { get; set; }
        public int MinutesSpent { get; set; }
        public Project Project { get; set; }
        public bool Synced { get; set; }
        public bool Internal { get; set; }

        public bool HasTask
        {
            get { return Task != null; }
        }

        public bool HasComment => !string.IsNullOrWhiteSpace(Comment);

        // note: no need to add a "user" we are going to work under the assumption that there is only one user
    }
}

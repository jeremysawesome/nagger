namespace Nagger.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Extensions;
    using Interfaces;
    using Models;

    public class TimeService : ITimeService
    {
        readonly ILocalTimeRepository _localTimeRepository;
        readonly IRemoteTimeRepository _remoteTimeRepository;
        readonly ISettingsService _settingsService;
        readonly IAssociatedRemoteRepositoryService _associatedRemoteRepositoryService;

        public TimeService(ILocalTimeRepository localTimeRepository, IRemoteTimeRepository remoteTimeRepository,
            ISettingsService settingsService, IAssociatedRemoteRepositoryService associatedRemoteRepositoryService)
        {
            _localTimeRepository = localTimeRepository;
            _remoteTimeRepository = remoteTimeRepository;
            _settingsService = settingsService;
            _associatedRemoteRepositoryService = associatedRemoteRepositoryService;
        }

        int NaggingInterval
        {
            get { return _settingsService.GetSetting<int>("NaggingInterval"); }
        }

        public void RecordTime(Task task)
        {
            var timeEntry = new TimeEntry(task);
            _localTimeRepository.RecordTime(timeEntry);
        }

        public void RecordTime(Task task, DateTime time, string comment, Task associatedTask)
        {
            var timeEntry = new TimeEntry(task, time, comment, associatedTask);
            _localTimeRepository.RecordTime(timeEntry);
        }

        public void RecordTime(Task task, int intervalCount, int minutesWorked, DateTime offset, string comment, Task associatedTask)
        {
            var totalMinutes = intervalCount*NaggingInterval;
            var minutesOfBreak = totalMinutes - minutesWorked;
            var recordTime = offset.AddMinutes(minutesOfBreak);
            RecordTime(task, recordTime, comment, associatedTask);
        }

        public void RecordMarker(DateTime time)
        {
            var timeEntry = new TimeEntry
            {
                Internal = true,
                TimeRecorded = time
            };
            RecordTime(timeEntry);
        }

        public void RecordTime(TimeEntry timeEntry)
        {
            _localTimeRepository.RecordTime(timeEntry);
        }

        public TimeEntry GetLastTimeEntry(bool getInternal = false)
        {
            var entry = _localTimeRepository.GetLastTimeEntry(getInternal);
            if (entry == null) return null;
            if (entry.HasProject && entry.HasTask) entry.Task.Project = entry.Project;
            return entry;
        }

        public IEnumerable<int> GetIntervalMinutes(int intervalCount)
        {
            for (var i = 1; i <= intervalCount; i++)
            {
                yield return i*NaggingInterval;
            }
        }

        public int IntervalsSinceTime(DateTime startTime)
        {
            var difference = DateTime.Now - startTime;
            if (!(difference.TotalMinutes > NaggingInterval)) return 0;
            var span = ApplyFloor(difference);
            return (int) span.TotalMinutes/NaggingInterval;
        }

        public int IntervalsSinceLastRecord(bool justToday = true)
        {
            var lastRecord = GetLastTimeEntry(true);
            if (lastRecord == null) return 0;
            if (justToday && lastRecord.TimeRecorded < DateTime.Today) return 0;
            return IntervalsSinceTime(lastRecord.TimeRecorded);
        }

        public IEnumerable<string> GetRecentlyRecordedTaskIds(int limit)
        {
            return _localTimeRepository.GetRecentlyRecordedTaskIds(limit);
        }

        public IEnumerable<string> GetRecentlyAssociatedTaskIds(int limit, Task task)
        {
            return task?.Project?.Id == null ? new List<string>() : _localTimeRepository.GetRecentlyAssociatedTaskIds(limit, task.Project.Id);
        }

        public IEnumerable<string> GetRecentlyRecordedCommentsForTask(int limit, Task task)
        {
            return task?.Id == null
                ? new List<string>()
                : _localTimeRepository.GetRecentlyRecordedCommentsForTaskId(limit, task.Id);
        }

        public IEnumerable<string> GetRecentlyRecordedProjectIds(int limit = 5)
        {
            return _localTimeRepository.GetRecentlyRecordedProjectIds(limit);
        }

        public string GetTimeReport()
        {
            SquashTime();

            var workThisWeek = GetSpanOfWorkSince(DayOfWeek.Sunday);
            var workToday = GetSpanOfWorkSince(DateTime.Today.DayOfWeek);

            var hoursThisWeek = Math.Truncate(workThisWeek.TotalHours);
            var minutesThisWeek = (workThisWeek.TotalHours - hoursThisWeek) * 60;

            var hoursToday = Math.Truncate(workToday.TotalHours);
            var minutesToday = (workToday.TotalHours - hoursToday) * 60;

            var builder = new StringBuilder();
            builder.AppendLine("---This Week---");
            builder.AppendFormat("{0} hours and {1} minutes", hoursThisWeek, minutesThisWeek);
            builder.AppendLine();
            builder.AppendLine("---Today---");
            builder.AppendFormat("{0} hours and {1} minutes", hoursToday, minutesToday);

            return builder.ToString();
        }

        public void DailyTimeOperations(bool force = false)
        {
            var lastSquash = _settingsService.GetSetting<DateTime>("LastSquashDate");
            if (force || lastSquash < DateTime.Today)
            {
                // Squash the time.
                SquashTime();
                _settingsService.SaveSetting("LastSquashDate", DateTime.Now.ToString());
            }

            var lastSync = _settingsService.GetSetting<DateTime>("LastSyncedDate");
            if (!force && lastSync.Date >= DateTime.Today) return;
            SyncWithRemote();
            _settingsService.SaveSetting("LastSyncedDate", DateTime.Now.ToString());
        }

        List<int> SyncUnsyncedAssociatedEntries()
        {
            var unsyncedIdString = _settingsService.GetSetting<string>("UnsyncedEntries");
            if(unsyncedIdString.IsNullOrWhitespace()) return new List<int>();

            var previouslyUnsyncedProjectEntries = unsyncedIdString.Split(',').Select(int.Parse).ToList();
            var unsyncedEntries = _localTimeRepository.GetTimeEntries(previouslyUnsyncedProjectEntries);
            var unsyncedAssociatedEntries = new List<int>();

            foreach (var entry in unsyncedEntries)
            {
                if(!LogWithAssociatedRepository(entry)) unsyncedAssociatedEntries.Add(entry.Id);
            }
            return unsyncedAssociatedEntries;
        }

        public void SyncWithRemote()
        {
            // only sync if this feature is enabled
            var remoteSyncEnabled = _settingsService.GetSetting<bool>("RemoteSyncEnabled");
            if (!remoteSyncEnabled) return;

            // get the unsynced entries
            var unsyncedEntries = _localTimeRepository.GetUnsyncedEntries();

            var unsyncedAssociatedEntries = SyncUnsyncedAssociatedEntries();

            // loop through each and record in the remote repo
            foreach (var entry in unsyncedEntries)
            {
                if (!_remoteTimeRepository.RecordTime(entry)) continue;
                if (entry.HasAssociatedTask)
                {
                     if(!LogWithAssociatedRepository(entry)) unsyncedAssociatedEntries.Add(entry.Id);
                }

                entry.Synced = true;
                _localTimeRepository.UpdateSyncedOnTimeEntry(entry);
            }

            _settingsService.SaveSetting("UnsyncedEntries", string.Join(",", unsyncedAssociatedEntries));
        }

        bool LogWithAssociatedRepository(TimeEntry entry)
        {
            var projectTimeRepository = _associatedRemoteRepositoryService.GetAssociatedRemoteTimeRepository(entry.Project);
            if (projectTimeRepository == null) return false;
            return projectTimeRepository.RecordAssociatedTime(entry);
        }

        public void SquashTime()
        {
            // get all time entries that have not been synced
            var unsyncedEntries = _localTimeRepository.GetUnsyncedEntries(true).OrderBy(x => x.TimeRecorded);
            if (!unsyncedEntries.Any()) return;

            var currentDate = DateTime.MinValue;
            TimeEntry firstEntryForTask = null;
            var squashedEntries = new List<TimeEntry>();
            var entriesToRemove = new List<TimeEntry>();
            foreach (var entry in unsyncedEntries)
            {
                if (currentDate == DateTime.MinValue) currentDate = entry.TimeRecorded.Date;

                if (currentDate != entry.TimeRecorded.Date)
                {
                    currentDate = entry.TimeRecorded.Date;
                    firstEntryForTask = null;
                }

                if (firstEntryForTask == null) firstEntryForTask = entry;

                // this makes sure that time spent between the two entries is accounted for
                firstEntryForTask = UpdateEntryWithTimeDifference(firstEntryForTask, entry);
                if (firstEntryForTask != entry && EntriesAreForSameWork(firstEntryForTask, entry))
                {
                    entriesToRemove.Add(entry);
                }
                else
                {
                    /** 
                     * question: what about lunches in the middle of the day? if there are no entries in the DB then how can you track lunches and breaks?
                     * do we really care about tracking lunches and breaks though... we are tracking time worked on tasks (not time on breaks).
                     * NOTE: We care about lunches and breaks because the difference between two time entries is used to
                     *  calculate the amount of time spent working on a task.
                     *  If the difference between two time entries includes lunch time, then we have a problem.
                    **/
                    squashedEntries.Add(firstEntryForTask);
                    firstEntryForTask = entry;
                }
            }

            // it's possible to skip the last entry of the day.this makes sure we don't do that
            if (firstEntryForTask != null && firstEntryForTask.TimeRecorded.Date == currentDate &&
                firstEntryForTask != entriesToRemove.LastOrDefault() &&
                firstEntryForTask != squashedEntries.LastOrDefault())
            {
                squashedEntries.Add(firstEntryForTask);
            }

            // update any timeEntries with '0' Minutes with the minimum
            foreach (var squashedEntry in squashedEntries)
            {
                if (squashedEntry.MinutesSpent == 0) squashedEntry.MinutesSpent = NaggingInterval;
            }

            // note: the current implementation does not update the last entry in a list of entries - that's because there is nothing to difference it against.
            // we might want to rectify that... or we just assume that they will continue to enter time
            // but what a task that is the last in the day and the only difference is to difference with the task a day later?
            // that sounds like something that needs to be fixed...

            // other thoughts: say we enter the MinutesSpent whenever we enter a task.... the minutes spent would be the difference between the current
            // recorded time and the most recently recorded time.

            _localTimeRepository.UpdateMinutesSpentOnTimeEntries(squashedEntries);
            _localTimeRepository.RemoveTimeEntries(entriesToRemove);

            // insert a marker for the very last entry inserted. This helps to avoid issues in asking about breaks next time Nagger runs.
            RecordMarker(unsyncedEntries.Last().TimeRecorded);
        }

        TimeSpan GetSpanOfWorkSince(DayOfWeek dayOfWeek)
        {
            var weekStart = DateTime.Now.StartOfWeek(dayOfWeek);
            return GetSpanOfWorkSince(weekStart);
        }

        TimeSpan GetSpanOfWorkSince(DateTime time)
        {
            var entries = _localTimeRepository.GetTimeEntriesSince(time);
            return TimeSpan.FromMinutes(entries.Sum(entry => entry.MinutesSpent));
        }

        static bool EntriesAreForSameWork(TimeEntry firstEntry, TimeEntry secondEntry)
        {
            // entries are considered for the same work if they have the same task and if they have the same comment
            // entries without tasks are considered different even if the comment is the same (for now)
            if (!(firstEntry.HasTask && secondEntry.HasTask)) return false;
            if (firstEntry.Comment != secondEntry.Comment) return false;
            return firstEntry.Task.Id == secondEntry.Task.Id;
        }

        TimeSpan ApplyCeiling(TimeSpan span)
        {
            var intervalSpan = TimeSpan.FromMinutes(NaggingInterval);
            return span.Ceiling(intervalSpan);
        }

        TimeSpan ApplyFloor(TimeSpan span)
        {
            var intervalSpan = TimeSpan.FromMinutes(NaggingInterval);
            return span.Floor(intervalSpan);
        }

        TimeEntry UpdateEntryWithTimeDifference(TimeEntry first, TimeEntry second)
        {
            // just return the entry if they are the same
            if (first == second) return first;

            // quick notes here - the problem is that sometimes the difference between the two entries
            // is 14 minutes and sometimes it's 16 minutes.
            // so do we want to do a ceiling or not? Or perhaps there is a better way to fix this... maybe by using the time
            // that nagger asked the question as the entry time... instead of the time that the question was answered?
            // this was updated in the Run function. The time nagger asks is used as the time entry time. (This seems to be working well)


            // get the difference between the two entries and update the first
            var timeDifference = second.TimeRecorded - first.TimeRecorded;
            timeDifference = ApplyCeiling(timeDifference);
            first.MinutesSpent = (int) timeDifference.TotalMinutes;
            return first;
        }
    }
}
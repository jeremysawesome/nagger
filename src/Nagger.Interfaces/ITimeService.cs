namespace Nagger.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Models;

    public interface ITimeService
    {
        void RecordTime(Task task);
        void RecordTime(TimeEntry timeEntry);
        void RecordTime(Task task, DateTime time, string comment);
        void RecordTime(Task task, int intervalCount, int minutesWorked, DateTime offset, string comment);
        void RecordMarker(DateTime time);
        void DailyTimeOperations(bool force = false);
        void SquashTime(); // this will probably end up being internal to the time service
        void SyncWithRemote();
        TimeEntry GetLastTimeEntry(bool getInternal = false);
        IEnumerable<int> GetIntervalMinutes(int intervalCount);
        int IntervalsSinceTime(DateTime startTime);
        int IntervalsSinceLastRecord(bool justToday = true);
        IEnumerable<string> GetRecentlyRecordedTaskIds(int limit);
        IEnumerable<string> GetRecentlyRecordedCommentsForTask(int limit, Task task);

        string GetTimeReport();
    }
}
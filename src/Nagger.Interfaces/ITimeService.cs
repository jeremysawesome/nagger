namespace Nagger.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Models;

    public interface ITimeService
    {
        void RecordTime(Task task);
        void RecordTime(TimeEntry timeEntry);
        void RecordTime(Task task, DateTime time);
        void RecordTime(Task task, int intervalCount, int minutesWorked, DateTime offset);
        void RecordMarker(DateTime time);
        void DailyTimeSync();
        void SquashTime(); // this will probably end up being internal to the time service
        void SyncWithRemote();
        TimeEntry GetLastTimeEntry();
        IEnumerable<int> GetIntervalMinutes(int intervalCount);
        int IntervalsSinceTime(DateTime startTime);
    }
}
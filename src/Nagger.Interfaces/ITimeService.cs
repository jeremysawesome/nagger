namespace Nagger.Interfaces
{
    using System;
    using Models;

    public interface ITimeService
    {
        void RecordTime(Task task);
        void RecordTime(TimeEntry timeEntry);
        void RecordTime(Task task, DateTime time);
        void DailyTimeSync();
        void SquashTime(); // this will probably end up being internal to the time service
        void SyncWithRemote();
        TimeEntry GetLastTimeEntry();
    }
}
namespace Nagger.Interfaces
{
    using Models;

    public interface ITimeService
    {
        void RecordTime(TimeEntry timeEntry);
        void SquashTime(); // this will probably end up being internal to the time service
        void SyncWithRemote();

        TimeEntry GetLastTimeEntry();
        // todo: remove
        TimeEntry GetTestTimeEntry();
    }
}

namespace Nagger.Interfaces
{
    using Models;

    public interface IRemoteTimeRepository
    {
        bool RecordTime(TimeEntry timeEntry);
    }
}

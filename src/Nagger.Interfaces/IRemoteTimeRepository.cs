using Nagger.Models;

namespace Nagger.Interfaces
{
    public interface IRemoteTimeRepository
    {
        bool RecordTime(TimeEntry timeEntry);
    }
}

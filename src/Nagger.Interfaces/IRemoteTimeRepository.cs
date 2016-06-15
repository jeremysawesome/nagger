namespace Nagger.Interfaces
{
    using Models;

    public interface IRemoteTimeRepository
    {
        bool RecordTime(TimeEntry timeEntry);
        bool RecordAssociatedTime(TimeEntry timeEntry);
        void InitializeForProject(Project project);
    }
}

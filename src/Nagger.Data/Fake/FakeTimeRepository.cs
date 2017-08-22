namespace Nagger.Data.Fake
{
    using Interfaces;
    using Models;

    public class FakeTimeRepository : IRemoteTimeRepository
    {
        public bool RecordTime(TimeEntry timeEntry)
        {
            return true;
        }

        public bool RecordAssociatedTime(TimeEntry timeEntry)
        {
            return true;
        }

        public void InitializeForProject(Project project)
        {
        }
    }
}
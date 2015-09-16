namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface ILocalTimeRepository
    {
        void RecordTime(TimeEntry timeEntry);
        void UpdateMinutesSpentOnTimeEntry(TimeEntry entry);
        void UpdateMinutesSpentOnTimeEntries(IEnumerable<TimeEntry> entries);
        void UpdateSyncedOnTimeEntry(TimeEntry entry);
        void RemoveTimeEntries(IEnumerable<TimeEntry> entries);

        TimeEntry GetLastTimeEntry();
        IEnumerable<TimeEntry> GetUnsyncedEntries(bool getInternal = false);
    }
}

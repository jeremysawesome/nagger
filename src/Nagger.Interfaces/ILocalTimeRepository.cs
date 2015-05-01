using System.Collections.Generic;
using Nagger.Models;

namespace Nagger.Interfaces
{
    public interface ILocalTimeRepository
    {
        void RecordTime(TimeEntry timeEntry);
        void UpdateMinutesSpentOnTimeEntry(TimeEntry entry);
        void UpdateMinutesSpentOnTimeEntries(IEnumerable<TimeEntry> entries);
        void UpdateSyncedOnTimeEntry(TimeEntry entry);
        void RemoveTimeEntries(IEnumerable<TimeEntry> entries);
        int GetNaggingInterval();

        TimeEntry GetLastTimeEntry();
        IEnumerable<TimeEntry> GetUnsyncedEntries();
    }
}

namespace Nagger.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models;

    public class LocalTimeRepository : LocalBaseRepository, ILocalTimeRepository
    {
        readonly ILocalTaskRepository _localTaskRepository;

        public LocalTimeRepository(ILocalTaskRepository localTaskRepository)
        {
            _localTaskRepository = localTaskRepository;
        }

        public void RecordTime(TimeEntry timeEntry)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO TimeEntries (TimeRecorded, Comment, MinutesSpent, TaskId, ProjectId)
                                VALUES (@TimeRecorded, @Comment, @MinutesSpent, @TaskId, @ProjectId)";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@TimeRecorded", timeEntry.TimeRecorded);
                cmd.Parameters.AddWithValue("@Comment", timeEntry.Comment);
                cmd.Parameters.AddWithValue("@MinutesSpent", timeEntry.MinutesSpent); // not sure if necessary

                cmd.Parameters.AddWithValue("@TaskId", (timeEntry.Task == null) ? "" : timeEntry.Task.Id);
                cmd.Parameters.AddWithValue("@ProjectId", (timeEntry.Project == null) ? "" : timeEntry.Project.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public TimeEntry GetLastTimeEntry()
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM TimeEntries ORDER BY Id DESC LIMIT 1";
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    var timeEntry = new TimeEntry
                    {
                        TimeRecorded = DateTime.SpecifyKind(reader.GetDateTime(reader.GetOrdinal("TimeRecorded")), DateTimeKind.Local),
                        Comment = reader.Get<string>("Comment"),
                        Id = reader.Get<int>("Id"),
                        Task = _localTaskRepository.GetTaskById(reader.Get<string>("TaskId")),
                        Synced = reader.Get<bool>("Synced")
                        //Project = _projectRepository.GetProjectById(reader.GetString(reader.GetOrdinal("ProjectId")))
                    };

                    return timeEntry;
                }
            }
        }

        //todo: refactor the building of timeentries into an execute method (potentially)
        public IEnumerable<TimeEntry> GetUnsyncedEntries()
        {
            var entries = new List<TimeEntry>();
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM TimeEntries WHERE Synced=0";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var timeEntry = new TimeEntry
                        {
                            TimeRecorded = DateTime.SpecifyKind(reader.GetDateTime(reader.GetOrdinal("TimeRecorded")), DateTimeKind.Local),
                            Comment = reader.Get<string>("Comment"),
                            Id = reader.Get<int>("Id"),
                            Task = _localTaskRepository.GetTaskById(reader.Get<string>("TaskId")),
                            Synced = reader.Get<bool>("Synced")
                            //Project = _projectRepository.GetProjectById(reader.GetString(reader.GetOrdinal("ProjectId")))
                        };

                        entries.Add(timeEntry);
                    }
                }
            }
            return entries;
        }

        public void RemoveTimeEntries(IEnumerable<TimeEntry> entries)
        {
            if (!entries.Any()) return;
            var idsToRemove = entries.Select(x => x.Id);

            //todo: there has got to be a better way of doing this... (building ids and looping through to insert the params)
            const string sql = "DELETE FROM TimeEntries WHERE Id IN ({0})";

            var index = 0;
            var idParameterPlaceholders = new List<string>();
            var idParameters = new List<int>();
            foreach (var id in idsToRemove)
            {
                idParameterPlaceholders.Add("@id" + index);
                idParameters.Add(id);
                index++;
            }

            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = string.Format(sql, string.Join(",", idParameterPlaceholders));

                cmd.Prepare();

                for (var i = 0; i < idParameters.Count; i++)
                {
                    cmd.Parameters.AddWithValue("@id" + i, idParameters[i]);
                }

                cmd.ExecuteNonQuery();
            }
        }

        // maybe just create an update entry method? maybe not...
        public void UpdateSyncedOnTimeEntry(TimeEntry entry)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"UPDATE TimeEntries
                                    SET Synced = @Synced
                                    WHERE Id = @id";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Synced", entry.Synced);
                cmd.Parameters.AddWithValue("@id", entry.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateMinutesSpentOnTimeEntry(TimeEntry entry)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"UPDATE TimeEntries
                                    SET MinutesSpent = @MinutesSpent
                                    WHERE Id = @id";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@MinutesSpent", entry.MinutesSpent);
                cmd.Parameters.AddWithValue("@id", entry.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateMinutesSpentOnTimeEntries(IEnumerable<TimeEntry> entries)
        {
            foreach (var timeEntry in entries)
            {
                UpdateMinutesSpentOnTimeEntry(timeEntry);
            }
        }

        public int GetNaggingInterval()
        {
            // todo: hook this up to the db or something to allow users to set this
            return 15;
        }
    }
}

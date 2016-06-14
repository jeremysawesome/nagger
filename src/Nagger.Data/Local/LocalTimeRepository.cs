namespace Nagger.Data.Local
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models;

    public class LocalTimeRepository : LocalBaseRepository, ILocalTimeRepository
    {
        readonly ILocalProjectRepository _localProjectRepository;
        readonly ILocalTaskRepository _localTaskRepository;

        public LocalTimeRepository(ILocalTaskRepository localTaskRepository,
            ILocalProjectRepository localProjectRepository)
        {
            _localTaskRepository = localTaskRepository;
            _localProjectRepository = localProjectRepository;
        }

        public void RecordTime(TimeEntry timeEntry)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText =
                    @"INSERT INTO TimeEntries (TimeRecorded, Comment, MinutesSpent, TaskId, ProjectId, Internal)
                                VALUES (@TimeRecorded, @Comment, @MinutesSpent, @TaskId, @ProjectId, @Internal)";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@TimeRecorded", timeEntry.TimeRecorded);
                cmd.Parameters.AddWithValue("@Comment", timeEntry.Comment);
                cmd.Parameters.AddWithValue("@MinutesSpent", timeEntry.MinutesSpent); // not sure if necessary
                cmd.Parameters.AddWithValue("@Internal", timeEntry.Internal);

                cmd.Parameters.AddWithValue("@TaskId", (timeEntry.Task == null) ? "" : timeEntry.Task.Id);
                cmd.Parameters.AddWithValue("@ProjectId", (timeEntry.Project == null) ? "" : timeEntry.Project.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public TimeEntry GetLastTimeEntry(bool getInternal = false)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                var internalWhere = (getInternal) ? "" : "WHERE Internal = 0";

                cmd.CommandText = "SELECT * FROM TimeEntries " + internalWhere + " ORDER BY Id DESC LIMIT 1";

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    var timeEntry = new TimeEntry
                    {
                        TimeRecorded =
                            DateTime.SpecifyKind(reader.GetDateTime(reader.GetOrdinal("TimeRecorded")),
                                DateTimeKind.Local),
                        Comment = reader.Get<string>("Comment"),
                        Id = reader.Get<int>("Id"),
                        Task = _localTaskRepository.GetTaskById(reader.Get<string>("TaskId")),
                        Synced = reader.Get<bool>("Synced"),
                        MinutesSpent = reader.Get<int>("MinutesSpent"),
                        Internal = reader.Get<bool>("Internal"),
                        Project = _localProjectRepository.GetProjectById(reader.Get<string>("ProjectId"))
                    };

                    return timeEntry;
                }
            }
        }

        //todo: refactor the building of timeentries into an execute method (potentially)
        public IEnumerable<TimeEntry> GetUnsyncedEntries(bool getInternal = false)
        {
            var entries = new List<TimeEntry>();
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                var cmdText = "SELECT * FROM TimeEntries WHERE Synced=0";
                if (!getInternal) cmdText += " AND Internal = 0";
                cmd.CommandText = cmdText;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var timeEntry = new TimeEntry
                        {
                            TimeRecorded =
                                DateTime.SpecifyKind(reader.GetDateTime(reader.GetOrdinal("TimeRecorded")),
                                    DateTimeKind.Local),
                            Comment = reader.Get<string>("Comment"),
                            Id = reader.Get<int>("Id"),
                            Task = _localTaskRepository.GetTaskById(reader.Get<string>("TaskId")),
                            Synced = reader.Get<bool>("Synced"),
                            MinutesSpent = reader.Get<int>("MinutesSpent"),
                            Internal = reader.Get<bool>("Internal"),
                            Project = _localProjectRepository.GetProjectById(reader.Get<string>("ProjectId"))
                        };

                        entries.Add(timeEntry);
                    }
                }
            }
            return entries;
        }

        public IEnumerable<TimeEntry> GetTimeEntriesSince(DateTime time, bool getInternal = false)
        {
            var entries = new List<TimeEntry>();
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                var cmdText = "SELECT * FROM TimeEntries WHERE TimeRecorded >= @time";
                if (!getInternal) cmdText += " AND Internal = 0";
                cmd.CommandText = cmdText;

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@time", time);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var timeEntry = new TimeEntry
                        {
                            TimeRecorded =
                                DateTime.SpecifyKind(reader.GetDateTime(reader.GetOrdinal("TimeRecorded")),
                                    DateTimeKind.Local),
                            Comment = reader.Get<string>("Comment"),
                            Id = reader.Get<int>("Id"),
                            Task = _localTaskRepository.GetTaskById(reader.Get<string>("TaskId")),
                            Synced = reader.Get<bool>("Synced"),
                            MinutesSpent = reader.Get<int>("MinutesSpent"),
                            Internal = reader.Get<bool>("Internal"),
                            Project = _localProjectRepository.GetProjectById(reader.Get<string>("ProjectId"))
                        };

                        entries.Add(timeEntry);
                    }
                }
            }
            return entries;
        }

        public IEnumerable<string> GetRecentlyRecordedTaskIds(int limit)
        {
            var ids = new List<string>();

            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                // for some reason, if we don't select the TimeRecorded out as well, we end up with unexpected results
                // there is something strange going on with the way the order works. The order has to be run twice 
                // it could be that the LIMIT applies before the ORDER. So the order only works on a set of limited rows?
                cmd.CommandText = @"SELECT DISTINCT TaskId FROM(
                                        SELECT TaskId, TimeRecorded 
                                        FROM TimeEntries 
                                        WHERE Internal = 0 
                                        ORDER BY TimeRecorded DESC
                                    ) AS temp 
                                    ORDER BY TimeRecorded DESC
                                    LIMIT @limit";
                cmd.Parameters.AddWithValue("@limit", limit);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ids.Add(reader.Get<string>("TaskId"));
                    }
                }
            }
            return ids;
        }

        public IEnumerable<string> GetRecentlyRecordedCommentsForTaskId(int limit, string taskId)
        {
            var comments = new List<string>();
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"SELECT Comment
                                    FROM TimeEntries 
                                    WHERE Internal = 0 AND TaskId = @taskId AND trim(Comment) != ''
                                    GROUP BY Comment
                                    ORDER BY TimeRecorded DESC
                                    LIMIT @limit";
                cmd.Parameters.AddWithValue("@taskId", taskId);
                cmd.Parameters.AddWithValue("@limit", limit);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comments.Add(reader.Get<string>("Comment"));
                    }
                }
            }
            return comments;
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
    }
}

namespace Nagger.Data
{
    using System.Collections.Generic;
    using Interfaces;
    using Models;

    public class LocalProjectRepository : LocalBaseRepository, ILocalProjectRepository
    {
        public Project GetProjectById(string id)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Projects
                                    WHERE Id = @id";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    var project = new Project
                    {
                        Id = reader.Get<string>("Id"),
                        Name = reader.Get<string>("Name"),
                        Key = reader.Get<string>("Key")
                    };

                    return project;
                }
            }
        }

        public Project GetProjectByKey(string key)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Projects
                                    WHERE Key = @key";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@key", key);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    var project = new Project
                    {
                        Id = reader.Get<string>("Id"),
                        Name = reader.Get<string>("Name"),
                        Key = reader.Get<string>("Key")
                    };

                    return project;
                }
            }
        }

        public IEnumerable<Project> GetProjects()
        {
            var projects = new List<Project>();

            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Projects";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var project = new Project
                        {
                            Id = reader.Get<string>("Id"),
                            Name = reader.Get<string>("Name"),
                            Key = reader.Get<string>("Key")
                        };

                        projects.Add(project);
                    }
                }
            }

            return projects;
        }

        public void StoreProject(Project project)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"INSERT OR IGNORE INTO Projects (Id, Name, Key)
                                VALUES (@Id, @Name, @Key)";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Id", project.Id);
                cmd.Parameters.AddWithValue("@Name", project.Name);
                cmd.Parameters.AddWithValue("@Key", project.Key);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
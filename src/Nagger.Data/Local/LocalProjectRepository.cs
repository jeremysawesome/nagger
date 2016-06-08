﻿namespace Nagger.Data.Local
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
                                    WHERE Projects.Id = @id";

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

                    project.AssociatedRemoteRepository = GetAssociatedRemoteRepositories(project.Id).FirstOrDefault();

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
                                    WHERE Key = @key COLLATE NOCASE";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@key", key);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    var project = new Project
                    {
                        Id = reader.Get<string>("Id"),
                        Name = reader.Get<string>("Name"),
                        Key = reader.Get<string>("Key"),
                    };

                    project.AssociatedRemoteRepository = GetAssociatedRemoteRepositories(project.Id).FirstOrDefault();

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
                            Key = reader.Get<string>("Key"),
                        };

                        project.AssociatedRemoteRepository = GetAssociatedRemoteRepositories(project.Id).FirstOrDefault();

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
                cmd.CommandText = @"INSERT OR IGNORE INTO Projects (Id, Name, Key) VALUES (@Id, @Name, @Key)";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Id", project.Id);
                cmd.Parameters.AddWithValue("@Name", project.Name);
                cmd.Parameters.AddWithValue("@Key", project.Key);

                cmd.ExecuteNonQuery();
            }

            StoreAssociatedRepository(project.Id, project.AssociatedRemoteRepository.ToString());
            
        }

        void StoreAssociatedRepository(string projectId, string remoteRepository)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"INSERT OR IGNORE INTO AssociatedRemoteRepositories (ProjectId, RemoteRepository) VALUES (@ProjectId, @RemoteRepository)";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                cmd.Parameters.AddWithValue("@RemoteRepository", remoteRepository);

                cmd.ExecuteNonQuery();
            }
        }

        public Project GetProjectByName(string name)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Projects
                                    WHERE Name = @name COLLATE NOCASE";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@name", name);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    var project = new Project
                    {
                        Id = reader.Get<string>("Id"),
                        Name = reader.Get<string>("Name"),
                        Key = reader.Get<string>("Key")
                    };
                    project.AssociatedRemoteRepository = GetAssociatedRemoteRepositories(project.Id).FirstOrDefault();

                    return project;
                }
            }
        }

        IEnumerable<SupportedRemoteRepository> GetAssociatedRemoteRepositories(string projectId)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"SELECT RemoteRepository FROM AssociatedRemoteRepositories
                                    WHERE ProjectId = @ProjectId";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@ProjectId", projectId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SupportedRemoteRepository remoteRepository;
                        if (Enum.TryParse(reader.Get<string>("RemoteRepository"), out remoteRepository))
                            yield return remoteRepository;
                    }
                }
            }
        }
    }
}
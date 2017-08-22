namespace Nagger.Data.Fake
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models;

    public class FakeTaskRepository : IRemoteTaskRepository
    {
        public Task GetTaskByName(string name)
        {
            return null;
        }

        public IEnumerable<Task> GetTasks()
        {
            yield break;
        }

        public IEnumerable<Task> GetTasks(Project project)
        {
            yield break;
        }

        public IEnumerable<Task> GetTasksByProjectId(string projectId, string lastTaskId = "")
        {
            yield break;
        }

        public void InitializeForProject(Project project)
        {
            
        }

        static Task GetTask(string id, string name, Task parent = null)
        {
            return new Task
            {
                Id = id,
                Name = name,
                Parent = parent
            };
        }
    }
}

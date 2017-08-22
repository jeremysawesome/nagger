namespace Nagger.Data.Fake
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models;

    public class FakeTaskRepository : IRemoteTaskRepository
    {
        static readonly Task Development = new Task { Id = "1", Name = "Development", Description = "Development" };

        public Task GetTaskByName(string name)
        {
            return name == Development.Name ? Development : null;
        }

        public IEnumerable<Task> GetTasks()
        {
            yield return Development;
        }

        public IEnumerable<Task> GetTasks(Project project)
        {
            return GetTasks();
        }

        public IEnumerable<Task> GetTasksByProjectId(string projectId, string lastTaskId = "")
        {
            return GetTasks();
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

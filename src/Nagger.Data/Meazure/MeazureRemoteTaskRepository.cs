namespace Nagger.Data.Meazure
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Models;

    public class MeazureRemoteTaskRepository : IRemoteTaskRepository
    {
        public Task GetTaskByName(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Task> GetTasks()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Task> GetTasks(Project project)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Task> GetTasksByProjectId(string projectId, string lastTaskId = null)
        {
            throw new NotImplementedException();
        }
    }
}

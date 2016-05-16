namespace Nagger.Data.Meazure
{
    using System.Collections.Generic;
    using Interfaces;
    using Models;

    public class MeazureTaskRepository : IRemoteTaskRepository
    {
        public Task GetTaskByName(string name)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Task> GetTasks()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Task> GetTasks(Project project)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Task> GetTasksByProjectId(string projectId, string lastTaskId = null)
        {
            throw new System.NotImplementedException();
        }
    }
}

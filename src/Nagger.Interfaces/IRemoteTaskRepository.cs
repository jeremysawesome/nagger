namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface IRemoteTaskRepository
    {
        Task GetTaskByName(string name);
        IEnumerable<Task> GetTasks();
        IEnumerable<Task> GetTasks(Project project);
        IEnumerable<Task> GetTasksByProjectId(string projectId, string lastTaskId = null);

        void InitializeForProject(Project project);
    }
}

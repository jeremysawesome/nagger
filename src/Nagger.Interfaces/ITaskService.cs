using System.Collections.Generic;
using Nagger.Models;

namespace Nagger.Interfaces
{
    public interface ITaskService
    {
        Task GetLastTask();
        void StoreTask(Task task);
        void SyncTasksWithRemote();
        IEnumerable<Task> GetTasks();
        IEnumerable<Task> GetTasksByProject(Project project);
        IEnumerable<Task> GetTasksByProjectId(string projectId);

        // todo: remove
        Task GetTestTask();
    }
}

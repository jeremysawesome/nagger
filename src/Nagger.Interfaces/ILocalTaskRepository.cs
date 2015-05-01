using System.Collections.Generic;
using Nagger.Models;

namespace Nagger.Interfaces
{
    public interface ILocalTaskRepository
    {
        Task GetLastTask();
        Task GetTaskById(string id);
        void StoreTask(Task task);
        IEnumerable<Task> GetTasks(string projectId = null);
        IEnumerable<Task> GetTasksByProject(Project project);

        // todo: remove
        Task GetTestTask();
    }
}

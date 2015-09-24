namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface ILocalTaskRepository
    {
        Task GetLastTask();
        Task GetLastSyncedTask(string projectId = null);
        Task GetTaskByName(string name);
        Task GetTaskById(string id);
        void StoreTask(Task task);
        IEnumerable<Task> GetTasks(string projectId = null);
        IEnumerable<Task> GetTasksByTaskIds(IEnumerable<string> taskIds);
        IEnumerable<Task> GetTasksByProject(Project project);

        // todo: remove
        Task GetTestTask();
    }
}

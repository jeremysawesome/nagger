namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

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

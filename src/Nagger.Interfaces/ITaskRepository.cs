namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface ITaskRepository
    {
        IEnumerable<Task> GetTasksByProject(Project project);
        IEnumerable<Task> GetTasksByTask(Task task);
        Task GetTaskById(string id);
    }
}

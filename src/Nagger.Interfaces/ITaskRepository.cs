using System.Collections.Generic;
using Nagger.Models;

namespace Nagger.Interfaces
{
    public interface ITaskRepository
    {
        IEnumerable<Task> GetTasksByProject(Project project);
        IEnumerable<Task> GetTasksByTask(Task task);
        Task GetTaskById(string id);
    }
}

using System.Collections.Generic;
using Nagger.Models;

namespace Nagger.Interfaces
{
    public interface IRemoteTaskRepository
    {
        IEnumerable<Task> GetTasks();
        IEnumerable<Task> GetTasks(Project project);
    }
}

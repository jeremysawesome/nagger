using System.Collections.Generic;
using Nagger.Models;

namespace Nagger.Interfaces
{
    public interface ILocalProjectRepository
    {
        Project GetProjectById(string id);
        IEnumerable<Project> GetProjects();
        void StoreProject(Project project);
    }
}

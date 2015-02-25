namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface ILocalProjectRepository
    {
        Project GetProjectById(string id);
        IEnumerable<Project> GetProjects();
        void StoreProject(Project project);
    }
}

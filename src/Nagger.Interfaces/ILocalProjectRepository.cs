namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface ILocalProjectRepository
    {
        Project GetProjectById(string id);
        Project GetProjectByKey(string key);
        IEnumerable<Project> GetProjects();
        void StoreProject(Project project);
        Project GetProjectByName(string name);
    }
}

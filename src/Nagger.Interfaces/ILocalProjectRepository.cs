namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface ILocalProjectRepository
    {
        Project GetProjectById(string id);
        Project GetProjectByKey(string key);
        IEnumerable<Project> GetProjects();
        IEnumerable<Project> GetProjectsByIds(IEnumerable<string> ids);
        void StoreProject(Project project);
        Project GetProjectByName(string name);
    }
}

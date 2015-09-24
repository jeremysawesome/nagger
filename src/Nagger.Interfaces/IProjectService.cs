namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface IProjectService
    {
        IEnumerable<Project> GetProjects();

        Project GetProjectById(string id);
        Project GetProjectByKey(string key);
    }
}

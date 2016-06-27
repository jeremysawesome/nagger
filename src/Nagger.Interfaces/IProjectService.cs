namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface IProjectService
    {
        IEnumerable<Project> GetProjects();
        IEnumerable<Project> GetProjectsByIds(IEnumerable<string> ids);
        Project GetProjectById(string id);
        Project GetProjectByKey(string key);
        Project GetProjectByName(string name);

        void AssociateProjectWithRepository(Project project, SupportedRemoteRepository repository);
    }
}

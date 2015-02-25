namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface IRemoteProjectRepository
    {
        IEnumerable<Project> GetProjects();
        //Project GetProjectById(string getString);
    }
}

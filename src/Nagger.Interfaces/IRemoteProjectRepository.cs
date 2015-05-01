using System.Collections.Generic;
using Nagger.Models;

namespace Nagger.Interfaces
{
    public interface IRemoteProjectRepository
    {
        IEnumerable<Project> GetProjects();
        //Project GetProjectById(string getString);
    }
}

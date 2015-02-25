namespace Nagger.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface IProjectService
    {
        IEnumerable<Project> GetProjects();
    }
}

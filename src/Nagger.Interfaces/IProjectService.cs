using System.Collections.Generic;
using Nagger.Models;

namespace Nagger.Interfaces
{
    public interface IProjectService
    {
	    IEnumerable<Project> GetProjects();
    }
}

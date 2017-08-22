namespace Nagger.Data.Fake
{
    using System.Collections.Generic;
    using Interfaces;
    using Models;

    public class FakeProjectRepository : IRemoteProjectRepository
    {
        public IEnumerable<Project> GetProjects()
        {
            yield break;
        }
    }
}
namespace Nagger.Data.Fake
{
    using System.Collections.Generic;
    using Interfaces;
    using Models;

    public class FakeProjectRepository : IRemoteProjectRepository
    {
        static readonly Project FakeProject = new Project {Id = "1", Key = "Fake", Name = "Fake"};

        public IEnumerable<Project> GetProjects()
        {
            yield return FakeProject;
        }
    }
}
namespace Nagger.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;

    public class Task
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Task Parent { get; set; }
        public IEnumerable<Task> Tasks { get; set; }

        public Project Project { get; set; }

        public bool HasTasks => Tasks != null && Tasks.Any();

        public bool HasParent => Parent != null;

        public override string ToString()
        {
            return (!string.IsNullOrWhiteSpace(Description) && Name != Description) ? $"{Name} ({Description.Truncate(50)})" : Name;
        }
    }
}
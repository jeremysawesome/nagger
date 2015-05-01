using System.Collections.Generic;
using System.Linq;

namespace Nagger.Models
{
    public class Task
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Task Parent { get; set; }
        public IEnumerable<Task> Tasks { get; set; }

        public Project Project { get; set; }

        public bool HasTasks { get { return Tasks != null && Tasks.Any(); } }
        public bool HasParent { get { return Parent != null; } }
    }
}

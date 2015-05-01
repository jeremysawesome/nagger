namespace Nagger.Models
{
    //question: do we even need the concept of a project?
    // answer: Jira has the concept of projects - so we should probably keep a project around
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; } 
        /*public IEnumerable<Task> Tasks { get; set; }

        public bool HasTasks { get { return Tasks.Any(); } }*/
    }
}

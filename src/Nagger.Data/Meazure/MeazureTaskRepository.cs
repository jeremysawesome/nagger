namespace Nagger.Data.Meazure
{
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models;

    public static class KnownTasks
    {
        public static Task Design => BuildTask("4", "Design");
        public static Task Development => BuildTask("3", "Development");
        public static Task TestingAndBugFixing => BuildTask("337", "Testing / Bug Fixing");
        public static Task ProjectManagement => BuildTask("336", "Project Management");
        public static Task SupportAdmin => BuildTask("345", "Support / Admin");
        public static Task SalesMarketing => BuildTask("344", "Sales & Marketing");
        public static Task Vacation => BuildTask("346", "Vacation");
        public static Task CompanyHoliday => BuildTask("905", "Company Holiday");

        private static Task BuildTask(string id, string name)
        {
            return new Task
            {
                Id = id,
                Name = name,
                Description = name,
            };
        }
    }

    public class MeazureTaskRepository : IRemoteTaskRepository
    {
        public Task GetTaskByName(string name)
        {
            return GetTasks().First(task => task.Name == name);
        }

        public IEnumerable<Task> GetTasks()
        {
            yield return KnownTasks.Design;
            yield return KnownTasks.Development;
            yield return KnownTasks.TestingAndBugFixing;
            yield return KnownTasks.ProjectManagement;
            yield return KnownTasks.SupportAdmin;
            yield return KnownTasks.SalesMarketing;
            yield return KnownTasks.Vacation;
            yield return KnownTasks.CompanyHoliday;
        }

        public IEnumerable<Task> GetTasks(Project project)
        {
            return GetTasks();
        }

        public IEnumerable<Task> GetTasksByProjectId(string projectId, string lastTaskId = null)
        {
            return GetTasks();
        }

        public void InitializeForProject(Project project)
        {
        }
    }
}

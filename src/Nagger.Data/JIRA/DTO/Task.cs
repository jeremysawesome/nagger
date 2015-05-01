using System.Collections.Generic;

namespace Nagger.Data.JIRA.DTO
{
    public class IssueFields
    {
        public string summary { get; set; }
        public Issue parent { get; set; }
        public Project project { get; set; }
    }

    public class Issue
    {
        public string expand { get; set; }
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public IssueFields fields { get; set; }
    }

    public class TaskResult
    {
        public string expand { get; set; }
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public List<Issue> issues { get; set; }
    }
}
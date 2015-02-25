namespace Nagger.Data.JIRA.DTO
{
    using System.Collections.Generic;

    public class Project
    {
        public string expand { get; set; }
        public string self { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public Dictionary<string, string> avatarUrls { get; set; }
    }
}

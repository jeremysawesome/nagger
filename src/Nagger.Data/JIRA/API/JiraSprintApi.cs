using Nagger.Data.JIRA.API;
using Nagger.Models;

namespace Nagger.Data.JIRA
{
    public class JiraSprintApi : JiraBaseApi
    {
        private const string ApiUrl = "https://www.example.com/rest/greenhopper/latest";

        public JiraSprintApi(User user)
            : base(user, ApiUrl)
        {
            // maybe refactor this sucker down the road?
        }
    }
}
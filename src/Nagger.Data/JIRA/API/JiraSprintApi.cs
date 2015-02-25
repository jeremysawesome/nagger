namespace Nagger.Data.JIRA
{
    using API;
    using Models;

    public class JiraSprintApi : JiraBaseApi
    {
        const string ApiUrl = "https://www.example.com/rest/greenhopper/latest";

        public JiraSprintApi(User user)
            : base(user, ApiUrl)
        {
            // maybe refactor this sucker down the road?
        }
    }
}

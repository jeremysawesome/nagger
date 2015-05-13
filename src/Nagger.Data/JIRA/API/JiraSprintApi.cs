namespace Nagger.Data.JIRA
{
    using API;
    using Models;

    public class JiraSprintApi : JiraBaseApi
    {
        //todo: move the url out to a setting so this can be used by whomever
        const string ApiUrlPath = "/rest/greenhopper/latest";

        public JiraSprintApi(User user, string apiBaseUrl)
            : base(user, apiBaseUrl, ApiUrlPath)
        {
            // maybe refactor this sucker down the road?
        }
    }
}

namespace Nagger.Data.JIRA.API
{
    using Data.API;
    using Models;

    /**
     * Based on the example here: https://github.com/restsharp/RestSharp/wiki/Recommended-Usage
    **/

    public class JiraApi : BaseApi
    {
        // todo: move the ApiUrl out into a setting so this can be used by whomever
        const string ApiUrlPath = "/rest/api/latest";

        public JiraApi(User user, string apiBaseUrl)
            : base(user, apiBaseUrl, ApiUrlPath)
        {
            // maybe refactor this sucker down the road?
        }
    }
}

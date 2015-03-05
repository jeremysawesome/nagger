namespace Nagger.Data.JIRA.API
{
    using Models;

    /**
     * Based on the example here: https://github.com/restsharp/RestSharp/wiki/Recommended-Usage
    **/

    public class JiraApi : JiraBaseApi
    {
        // todo: move the ApiUrl out into a setting so this can be used by whomever
        const string ApiUrl = "https://www.example.com/rest/api/latest";

        public JiraApi(User user)
            : base(user, ApiUrl)
        {
            // maybe refactor this sucker down the road?
        }
    }
}

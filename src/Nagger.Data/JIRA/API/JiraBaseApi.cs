namespace Nagger.Data.JIRA.API
{
    using System;
    using System.Net;
    using System.Security.Authentication;
    using Models;
    using RestSharp;

    public class JiraBaseApi
    {
        readonly string _apiUrl;
        readonly User _user;

        public JiraBaseApi(User user, string apiBaseUrl, string apiPath)
        {
            _user = user;
            _apiUrl = apiBaseUrl.TrimEnd('/') + apiPath;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = GetClient();
            var response = client.Execute<T>(request);

            // we will deal with this later. When InvalidCredentials are provided then we need to clear out the saved creds
            if(response.StatusCode == HttpStatusCode.Unauthorized) throw new InvalidCredentialException("User provided is not authorized to access this JIRA API: "+_apiUrl);

            if (response.ErrorException == null) return response.Data;

            throw new ApplicationException("Error retrieving data from Jira", response.ErrorException);
        }

        RestClient GetClient()
        {
            return new RestClient
            {
                BaseUrl = new Uri(_apiUrl),
                Authenticator = new HttpBasicAuthenticator(_user.Username, _user.Password)
            };
        }
    }
}

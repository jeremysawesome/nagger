namespace Nagger.Data.JIRA.API
{
    using System;
    using Models;
    using RestSharp;

    public class JiraBaseApi
    {
        readonly string _apiUrl;
        readonly User _user;

        public JiraBaseApi(User user, string apiUrl)
        {
            _user = user;
            _apiUrl = apiUrl;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = GetClient();
            var response = client.Execute<T>(request);

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

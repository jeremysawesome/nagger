using System;
using Nagger.Models;
using RestSharp;

namespace Nagger.Data.JIRA.API
{
    public class JiraBaseApi
    {
        private readonly string _apiUrl;
        private readonly User _user;

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

        private RestClient GetClient()
        {
            return new RestClient
            {
                BaseUrl = new Uri(_apiUrl),
                Authenticator = new HttpBasicAuthenticator(_user.Username, _user.Password)
            };
        }
    }
}
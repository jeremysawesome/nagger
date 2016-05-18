namespace Nagger.Data.Meazure.API
{
    using System;
    using System.Net;
    using Data.API;
    using Models;
    using RestSharp;

    public class MeazureApi : BaseApi
    {
        const string LoginPath = "/Auth/Login";

        public MeazureApi(User user, string apiBaseUrl) : base(user, apiBaseUrl, "")
        {
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            return Execute<T>(request, GetClientWithLogin());
        }

        IRestClient GetClientWithLogin()
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(ApiUrl),
                CookieContainer = new CookieContainer()
            };

            var loginRequest = new RestRequest(LoginPath, Method.POST);
            loginRequest.AddParameter("Email", User.Username);
            loginRequest.AddParameter("Password", User.Password);

            client.Execute(loginRequest);

            return client;
        }
    }
}
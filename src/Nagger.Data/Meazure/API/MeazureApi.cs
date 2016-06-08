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

            //Meazure requires an actual login
            var loginRequest = new RestRequest
            {
                Resource = LoginPath,
                Method = Method.POST,
                RequestFormat = DataFormat.Json
            };

            loginRequest.AddBody(new DTO.LoginModel
            {
                Email = User.Username,
                Password = User.Password
            });

            client.Execute(loginRequest);

            return client;
        }
    }
}
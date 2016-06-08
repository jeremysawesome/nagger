namespace Nagger.Data.API
{
    using System;
    using System.Net;
    using System.Security.Authentication;
    using Models;
    using RestSharp;

    public class BaseApi
    {
        protected readonly string ApiUrl;
        protected readonly User User;

        public BaseApi(User user, string apiBaseUrl, string apiPath)
        {
            User = user;
            ApiUrl = apiBaseUrl.TrimEnd('/') + apiPath;
        }

        public T Execute<T>(RestRequest request, IRestClient client = null) where T : new()
        {
            if(client == null) client = GetClient();
            var response = client.Execute<T>(request);

            // we will deal with this later. When InvalidCredentials are provided then we need to clear out the saved creds
            if(response.StatusCode == HttpStatusCode.Unauthorized) throw new InvalidCredentialException("User provided is not authorized to access this API: "+ApiUrl);

            if (response.ErrorException == null) return response.Data;

            throw new ApplicationException("Error retrieving data from API", response.ErrorException);
        }

        RestClient GetClient()
        {
            return new RestClient
            {
                BaseUrl = new Uri(ApiUrl),
                Authenticator = new HttpBasicAuthenticator(User.Username, User.Password)
            };
        }
    }
}

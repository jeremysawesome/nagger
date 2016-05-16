namespace Nagger.Data.JIRA
{
    using Interfaces;
    using Models;

    public class BaseJiraRepository
    {
        const string ApiName = "JIRA";
        const string UsernameKey = "JiraUsername";
        const string PasswordKey = "JiraPassword";
        const string ApiBaseUrlKey = "JiraApi";
        readonly BaseRepository _baseRepository;

        User _user;
        string _apiBaseUrl;

        public BaseJiraRepository(ISettingsService settingsService, IInputService inputService)
        {
            _baseRepository = new BaseRepository(settingsService, inputService);
        }

        public string ApiBaseUrl
        {
            get
            {
                _apiBaseUrl = _apiBaseUrl ?? _baseRepository.GetApiBaseUrl(ApiName, ApiBaseUrlKey);
                return _apiBaseUrl;
            }
        }

        public User JiraUser
        {
            get
            {
                _user = _user ?? _baseRepository.GetUser(ApiName, UsernameKey, PasswordKey);
                return _user;
            }
        }

        public bool UserExists => JiraUser != null;
    }
}

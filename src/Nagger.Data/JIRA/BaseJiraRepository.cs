namespace Nagger.Data.JIRA
{
    using Extensions;
    using Interfaces;
    using Models;

    public class BaseJiraRepository
    {
        const string ApiName = "JIRA";
        const string UsernameKey = "JiraUsername";
        const string PasswordKey = "JiraPassword";
        const string ApiBaseUrlKey = "JiraApi";
        const string ModifiedFormat = "{0}_{1}";
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
                var apiKey = GetSettingKey(ApiBaseUrlKey);
                _apiBaseUrl = _apiBaseUrl ?? _baseRepository.GetApiBaseUrl(ApiName, apiKey);
                return _apiBaseUrl;
            }
        }

        public User JiraUser
        {
            get
            {
                var uKey = GetSettingKey(UsernameKey);
                var pKey = GetSettingKey(PasswordKey);
                _user = _user ?? _baseRepository.GetUser(ApiName, uKey, pKey);
                return _user;
            }
        }

        public string GetSettingKey(string key)
        {
            return !KeyModifier.IsNullOrWhitespace() ? ModifiedFormat.FormatWith(KeyModifier, key) : key;
        }

        public string KeyModifier { get; set; }

        public bool UserExists => JiraUser != null;
    }
}

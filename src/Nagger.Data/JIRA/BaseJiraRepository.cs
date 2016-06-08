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
                _apiBaseUrl = _apiBaseUrl ?? _baseRepository.GetApiBaseUrl(ApiName, ApiBaseUrlKey);
                return _apiBaseUrl;
            }
        }

        public User JiraUser
        {
            get
            {
                var uKey = UsernameKey;
                var pKey = PasswordKey;
                if (KeyModifier.IsNullOrWhitespace())
                {
                    uKey = ModifiedFormat.FormatWith(KeyModifier, UsernameKey);
                    pKey = ModifiedFormat.FormatWith(KeyModifier, PasswordKey);
                }

                _user = _user ?? _baseRepository.GetUser(ApiName, uKey, pKey);
                return _user;
            }
        }

        public string KeyModifier { get; set; }

        public bool UserExists => JiraUser != null;
    }
}

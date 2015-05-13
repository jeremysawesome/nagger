namespace Nagger.Data.JIRA
{
    using Interfaces;
    using Models;

    public class BaseJiraRepository
    {
        const string UsernameKey = "JiraUsername";
        const string PasswordKey = "JiraPassword";
        const string ApiBaseUrlKey = "JiraApi";
        readonly ISettingsService _settingsService;
        readonly IInputService _inputService;

        User _user;
        string _apiBaseUrl;

        public BaseJiraRepository(ISettingsService settingsService, IInputService inputService)
        {
            _settingsService = settingsService;
            _inputService = inputService;
        }

        public string ApiBaseUrl
        {
            get
            {
                _apiBaseUrl = _apiBaseUrl ?? GetApiBaseUrl();
                return _apiBaseUrl;
            }
        }

        public User JiraUser
        {
            get
            {
                _user = _user ?? GetUser();
                return _user;
            }
        }

        public bool UserExists
        {
            get { return JiraUser != null; }
        }

        User GetUser()
        {
            var username = _settingsService.GetSetting<string>(UsernameKey);
            if (string.IsNullOrEmpty(username))
            {
                username = _inputService.AskForInput("Please provide your username for JIRA");
                _settingsService.SaveSetting(UsernameKey, username);
            }

            var password = _settingsService.GetSetting<string>(PasswordKey);
            if (string.IsNullOrEmpty(password))
            {
                password = _inputService.AskForPassword("Please provide your password for JIRA");
                _settingsService.SaveSetting(PasswordKey, password);
            }

            return new User
            {
                Username = username,
                Password = password
            };
        }

        string GetApiBaseUrl()
        {
            var baseUrl = _settingsService.GetSetting<string>(ApiBaseUrlKey);
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = _inputService.AskForInput("Please provide your JIRA base url");
                _settingsService.SaveSetting(ApiBaseUrlKey, baseUrl);
            }
            return baseUrl;
        }

        public void SaveUser(User user)
        {
            if (user == null) return;
            _settingsService.SaveSetting(UsernameKey, user.Username);
            _settingsService.SaveSetting(PasswordKey, user.Password);
        }
    }
}

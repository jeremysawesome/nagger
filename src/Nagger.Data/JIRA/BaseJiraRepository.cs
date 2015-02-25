namespace Nagger.Data.JIRA
{
    using System.Security.Authentication;
    using Interfaces;
    using Models;

    public class BaseJiraRepository
    {
        const string UsernameKey = "JiraUsername";
        const string PasswordKey = "JiraPassword";
        readonly ISettingsService _settingsService;
        User _user;

        public BaseJiraRepository(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            if (!UserExists) throw new InvalidCredentialException("There is no JIRA user specified");
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
            if (string.IsNullOrEmpty(username)) return null;

            var password = _settingsService.GetSetting<string>(PasswordKey);

            return new User
            {
                Username = username,
                Password = password
            };
        }

        // also need to be able to save a user
        public void SaveUser(User user)
        {
            if (user == null) return;
            _settingsService.SaveSetting(UsernameKey, user.Username);
            _settingsService.SaveSetting(PasswordKey, user.Password);
        }
    }
}

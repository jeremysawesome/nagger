using System.Security.Authentication;
using Nagger.Interfaces;
using Nagger.Models;

namespace Nagger.Data.JIRA
{
    public class BaseJiraRepository
    {
        private const string UsernameKey = "JiraUsername";
        private const string PasswordKey = "JiraPassword";
        private readonly ISettingsService _settingsService;
        private User _user;

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

        private User GetUser()
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
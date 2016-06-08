namespace Nagger.Data
{
    using Interfaces;
    using Models;

    public class BaseRepository
    {
        readonly ISettingsService _settingsService;
        readonly IInputService _inputService;

        public BaseRepository(ISettingsService settingsService, IInputService inputService)
        {
            _settingsService = settingsService;
            _inputService = inputService;
        }
        public User GetUser(string apiName, string usernameKey, string passwordKey)
        {
            var username = _settingsService.GetSetting<string>(usernameKey);
            if (string.IsNullOrEmpty(username))
            {
                username = _inputService.AskForInput($"Please provide your username for {apiName}");
                _settingsService.SaveSetting(usernameKey, username);
            }

            var password = _settingsService.GetSetting<string>(passwordKey);
            if (string.IsNullOrEmpty(password))
            {
                password = _inputService.AskForPassword($"Please provide your password for {apiName}");
                _settingsService.SaveSetting(passwordKey, password);
            }

            return new User
            {
                Username = username,
                Password = password
            };
        }

        public string GetApiBaseUrl(string apiName, string apiBaseUrlKey)
        {
            var baseUrl = _settingsService.GetSetting<string>(apiBaseUrlKey);
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = _inputService.AskForInput($"Please provide your {apiName} base url");
                _settingsService.SaveSetting(apiBaseUrlKey, baseUrl);
            }
            return baseUrl;
        }
    }
}

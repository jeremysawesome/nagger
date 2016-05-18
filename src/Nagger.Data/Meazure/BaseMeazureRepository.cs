namespace Nagger.Data.Meazure
{
    using Interfaces;
    using Models;

    public class BaseMeazureRepository
    {
        const string ApiName = "Meazure";
        const string UsernameKey = "MeazureUsername";
        const string PasswordKey = "MeazurePassword";
        const string ApiBaseUrlKey = "MeazureApi";
        readonly BaseRepository _baseRepository;

        User _user;
        string _apiBaseUrl;

        public BaseMeazureRepository(ISettingsService settingsService, IInputService inputService)
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

        public User User
        {
            get
            {
                _user = _user ?? _baseRepository.GetUser(ApiName, UsernameKey, PasswordKey);
                return _user;
            }
        }

        public bool UserExists => User != null;
    }
}

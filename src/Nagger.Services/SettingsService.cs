namespace Nagger.Services
{
    using Interfaces;

    public class SettingsService : ISettingsService
    {
        readonly ISettingsRepository _settingsRepository;

        public SettingsService(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public T GetSetting<T>(string name)
        {
            return _settingsRepository.GetSetting<T>(name);
        }

        public void SaveSetting(string name, string value)
        {
            _settingsRepository.SaveSetting(name, value);
        }
    }
}

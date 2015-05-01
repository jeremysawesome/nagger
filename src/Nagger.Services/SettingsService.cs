using Nagger.Interfaces;

namespace Nagger.Services
{
    public class SettingsService :ISettingsService
    {
        private readonly ISettingsRepository _settingsRepository;

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

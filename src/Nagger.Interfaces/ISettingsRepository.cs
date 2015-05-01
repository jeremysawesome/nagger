namespace Nagger.Interfaces
{
    public interface ISettingsRepository
    {
        T GetSetting<T>(string name);
        void SaveSetting(string name, string value);
    }
}

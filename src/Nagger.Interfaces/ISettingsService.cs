namespace Nagger.Interfaces
{
    public interface ISettingsService
    {
        T GetSetting<T>(string name);
        void SaveSetting(string name, string value);
    }
}

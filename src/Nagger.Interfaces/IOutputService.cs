namespace Nagger.Interfaces
{
    public interface IOutputService
    {
        void ShowInformation(string information);
        void LoadingMessage(string message);
        void HideInterface();
        void ShowInterface();
    }
}

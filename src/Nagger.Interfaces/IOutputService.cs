namespace Nagger.Interfaces
{
    using System.Collections.Generic;

    public interface IOutputService
    {
        void ShowInformation(string information);
        void LoadingMessage(string message);
        void OutputList(IEnumerable<object> outputObjects);
        void OutputSound();
        void HideInterface();
        void ShowInterface();
    }
}

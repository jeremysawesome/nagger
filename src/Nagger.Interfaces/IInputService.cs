namespace Nagger.Interfaces
{
    using System.Collections.Generic;

    public interface IInputService
    {
        string AskForInput(string question);
        bool AskForBoolean(string question);
        string AskForPassword(string question);
        string AskForSelectionOrInput(string question, IList<string> options);

        T AskForSelection<T>(string question, IList<T> options);
    }
}

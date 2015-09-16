namespace Nagger.Interfaces
{
    using System.Collections.Generic;

    public interface IInputService
    {
        string AskForInput(string question);
        bool AskForBoolean(string question);
        string AskForPassword(string question);

        T AskForSelection<T>(string question, IList<T> options);
    }
}

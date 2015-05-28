namespace Nagger.Interfaces
{
    public interface IInputService
    {
        string AskForInput(string question);
        bool AskForBoolean(string question);
        string AskForPassword(string question);
    }
}

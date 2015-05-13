namespace Nagger.Interfaces
{
    public interface IInputService
    {
        string AskForInput(string question);
        string AskForPassword(string question);
    }
}

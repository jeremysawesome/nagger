namespace Nagger.Services
{
    using System;
    using System.Text;
    using Interfaces;

    public class InputService : IInputService
    {
        public string AskForInput(string question)
        {
            Console.WriteLine(question);
            var answer = Console.ReadLine();

            return answer;
        }

        // see: https://gist.github.com/huobazi/1039424
        public string AskForPassword(string question)
        {
            Console.WriteLine(question);
            var sb = new StringBuilder();
            while (true)
            {
                var cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (cki.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        Console.Write("\b\0\b");
                        sb.Length--;
                    }

                    continue;
                }

                Console.Write('*');
                sb.Append(cki.KeyChar);
            }

            return sb.ToString();
        }
    }
}
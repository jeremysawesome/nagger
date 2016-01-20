namespace Nagger.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Interfaces;

    public class ConsoleInputService : IInputService
    {
        public string AskForInput(string question)
        {
            Console.WriteLine(question);
            var answer = Console.ReadLine();
            Console.WriteLine();

            return answer;
        }

        public bool AskForBoolean(string question)
        {
            Console.WriteLine(question);
            Console.Write("(Y)es/(N)o: ");
            var answer = Console.ReadLine() ?? "";
            Console.WriteLine();
            return (answer.ToLower().StartsWith("y"));
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

        public string AskForSelectionOrInput(string question, IList<string> options)
        {
            OutputOptions(options);
            Console.WriteLine();
            var answer = AskForInput(question);
            string result;
            return TryGetSelection(answer, options, out result) ? result : answer;
        }

        static void OutputOptions<T>(IList<T> options)
        {
            Console.WriteLine("----Options----");
            for (var i = 0; i < options.Count; i++)
            {
                Console.WriteLine("{0}) {1}", i, options[i]);
            }
        }

        static bool TryGetSelection<T>(string answer, IList<T> options, out T result)
        {
            int selection;
            if (int.TryParse(answer, out selection) && selection < options.Count && selection >= 0)
            {
                result = options[selection];
                return true;
            }
            result = default(T);
            return false;
        }

        public T AskForSelection<T>(string question, IList<T> options)
        {
            while (true)
            {
                OutputOptions(options);
                var answer = AskForInput(question);
                T result;
                if (TryGetSelection(answer, options, out result))
                {
                    return result;
                }
                Console.WriteLine("That was an invalid selection. Please try again.");
            }
        }
    }
}
namespace Nagger.Services
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Interfaces;

    public class ConsoleOutputService : IOutputService
    {
        internal static class ConsoleUtil
        {
            const int Hide = 0;
            const int Show = 5;

            [DllImport("kernel32.dll")]
            static extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            public static void HideWindow()
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, Hide);
            }

            public static void ShowWindow()
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, Show);
            }
        }

        public void ShowInformation(string information)
        {
            Console.WriteLine(information);
        }

        public void LoadingMessage(string message)
        {
            ShowInformation(message);
        }

        public void OutputList(IEnumerable<object> outputObjects)
        {
            ShowInformation("");
            foreach (var value in outputObjects)
            {
                ShowInformation(value.ToString());
            }
            ShowInformation("");
        }

        public void HideInterface()
        {
            ConsoleUtil.HideWindow();
        }

        public void ShowInterface()
        {
            ConsoleUtil.ShowWindow();
        }
    }
}

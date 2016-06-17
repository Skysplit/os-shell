using System;
using System.IO;

namespace ShellApplication.Commands
{
    class PauseInput : CommandInterface
    {
        public string GetName()
        {
            return "pause";
        }

        public int Execute(Loop ctx, TextWriter stdout, TextReader stdin, TextWriter stderr, string[] args)
        {
            stdout.WriteLine("Press enter to unpause");

            while (true)
            {
                // Wait until enter is hit
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    break;
                }
            }

            return 0;
        }
    }
}

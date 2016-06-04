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

        public int Execute(Loop ctx, TextWriter stdout, TextReader stdin, string[] args)
        {
            stdout.WriteLine("Press enter to unpause");

            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    break;
                }
            }

            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShellApplication.Commands
{
    class PauseInput : CommandInterface
    {
        public string GetName()
        {
            return "pause";
        }

        public string Execute(Loop ctx, string[] args)
        {
            Console.WriteLine("Press enter to unpause");

            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    break;
                }
            }

            return "";
        }
    }
}

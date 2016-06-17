using System;
using System.IO;

namespace ShellApplication.Commands
{
    class ClearConsole : CommandInterface
    {
        public string GetName()
        {
            return "clr";
        }

        public int Execute(Loop ctx, TextWriter stdout, TextReader stdin, TextWriter stderr, string[] args)
        {
            Console.Clear();
            return 0;
        }
    }
}

using System;
using System.IO;

namespace ShellApplication.Commands
{
    class QuitShell : CommandInterface
    {
        public string GetName()
        {
            return "quit";
        }

        public int Execute(Loop ctx, TextWriter stdout, TextReader stdin, string[] args)
        {
            Environment.Exit(1);

            return 0;
        }
    }
}

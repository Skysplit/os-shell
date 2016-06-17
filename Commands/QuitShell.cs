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

        public int Execute(Loop ctx, TextWriter stdout, TextReader stdin, TextWriter stderr, string[] args)
        {
            // Terminate current process
            Environment.Exit(1);

            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellApplication.Commands
{
    class QuitShell : CommandInterface
    {
        public string GetName()
        {
            return "quit";
        }

        public string Execute(Loop ctx, string[] args)
        {
            System.Environment.Exit(1);

            return "Bye bye!";
        }
    }
}

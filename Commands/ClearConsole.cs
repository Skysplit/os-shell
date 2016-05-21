using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellApplication.Commands
{
    class ClearConsole : CommandInterface
    {
        public string GetName()
        {
            return "clr";
        }

        public string Execute(Loop ctx, string[] args)
        {
            Console.Clear();
            return "";
        }
    }
}

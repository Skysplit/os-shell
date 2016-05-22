using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShellApplication.Commands
{
    class ComandHelper : CommandInterface
    {
        public string GetName()
        {
            return "help";
        }

        public string Execute(Loop ctx, string[] args)
        {
            string HelperFileRoot = Path.Combine(Path.GetDirectoryName(Environment.GetEnvironmentVariable("shell")), "docs");
            string HelperFile = "help";

            if (args.Length != 0)
            {
                HelperFile = args[0];
            }

            string HelperFilePath = Path.Combine(HelperFileRoot, HelperFile);

            if (File.Exists(HelperFilePath))
            {
                return string.Join("\n", File.ReadAllLines(HelperFilePath));
            }

            return string.Format("Could not find help file for command \"{0}\"", HelperFile);
        }
    }
}

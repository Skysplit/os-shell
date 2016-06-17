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

        public int Execute(Loop ctx, TextWriter stdout, TextReader stdin, TextWriter stderr, string[] args)
        {
            string HelperFileRoot = Path.GetDirectoryName(Environment.GetEnvironmentVariable("shell"));
            string HelperFile = "help";

            // If there is an argument, try to find help file for this argument command
            if (args.Length != 0)
            {
                HelperFile = args[0];
            }

            string HelperFilePath = Path.Combine(HelperFileRoot, "docs", HelperFile);

            // Check if helper file exists and print it to stdout
            if (File.Exists(HelperFilePath))
            {
                stdout.WriteLine(string.Join("\n", File.ReadAllLines(HelperFilePath)));
                return 0;
            }

            stderr.WriteLine(string.Format("Could not find help file for command \"{0}\"", HelperFile));
            return 1;
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;

namespace ShellApplication.Commands
{
    class ListDirectory : CommandInterface
    {
        public string GetName()
        {
            return "dir";
        }

        public int Execute(Loop ctx, TextWriter stdout, TextReader stdin, TextWriter stderr, string[] args)
        {
            // Make current directory default directory we are listing
            string PrintDirectory = Directory.GetCurrentDirectory();

            if (args.Length != 0)
            {
                PrintDirectory = args[0];
            }

            // Check if given directory exists
            if (!Directory.Exists(PrintDirectory))
            {
                stdout.WriteLine(string.Format("Directory \"{0}\" does not exists", PrintDirectory));
                return 1;
            }

            List<string> FilesList = new List<string>();

            // Convert array of system entries into array of strings and join them with newlines
            stdout.WriteLine(string.Join("\r\n", Array.ConvertAll<string, string>(Directory.GetFileSystemEntries(PrintDirectory), s => Path.GetFileName(s))));

            return 0;
        }
    }
}

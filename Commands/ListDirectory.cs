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

        public int Execute(Loop ctx, TextWriter stdout, TextReader stdin, string[] args)
        {
            string CurrentDirectory = Directory.GetCurrentDirectory();

            if (args.Length != 0)
            {
                CurrentDirectory = args[0];
            }

            if (!Directory.Exists(CurrentDirectory))
            {
                stdout.WriteLine(string.Format("Directory \"{0}\" does not exists", CurrentDirectory));
                return 1;
            }

            List<string> FilesList = new List<string>();

            stdout.WriteLine(string.Join("\r\n", Array.ConvertAll<string, string>(Directory.GetFileSystemEntries(CurrentDirectory), s => Path.GetFileName(s))));

            return 0;
        }
    }
}

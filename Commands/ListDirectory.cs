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

        public string Execute(Loop ctx, string[] args)
        {
            string CurrentDirectory = Directory.GetCurrentDirectory();

            if (args.Length != 0)
            {
                CurrentDirectory = args[0];
            }

            if (!Directory.Exists(CurrentDirectory))
            {
                return string.Format("Directory \"{0}\" does not exists", CurrentDirectory);
            }

            List<string> FilesList = new List<string>();

            return string.Join("\r\n", Array.ConvertAll<string, string>(Directory.GetFileSystemEntries(CurrentDirectory), s => Path.GetFileName(s)));
        }
    }
}

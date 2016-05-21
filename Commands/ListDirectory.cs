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

            List<string> FilesList = new List<string>(Directory.GetFileSystemEntries(CurrentDirectory));

            return string.Join("\r\n", FilesList.ConvertAll(s => Path.GetFileName(s)).ToArray());
        }
    }
}

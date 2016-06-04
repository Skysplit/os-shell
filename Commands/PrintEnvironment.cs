using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace ShellApplication.Commands
{
    class PrintEnvironment : CommandInterface
    {
        public string GetName()
        {
            return "environ";
        }

        public int Execute(Loop ctx, TextWriter stdout, TextReader stdin, string[] args)
        {
            List<string> EnvList = new List<string>();

            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                EnvList.Add(string.Format("{0}={1}", entry.Key, entry.Value));
            }

            stdout.WriteLine(string.Join("\r\n", EnvList.ToArray()));
            return 0;
        }
    }
}

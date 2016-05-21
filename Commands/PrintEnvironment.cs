using System;
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

        public string Execute(Loop ctx, string[] args)
        {
            List<string> EnvList = new List<string>();

            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                EnvList.Add(string.Format("{0}={1}", entry.Key, entry.Value));
            }

            return string.Join("\r\n", EnvList.ToArray());
        }
    }
}

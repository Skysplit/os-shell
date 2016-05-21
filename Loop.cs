using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace ShellApplication
{
    public class Loop
    {
        private CommandsList BuiltinCommands { get; set; }

        public Loop()
        {
            this.InitBuiltinCommands();
            this.InitEnvVariables();
        }

        public void InitLoop()
        {
            //Process ExternalProcess = new Process();
            //ExternalProcess.StartInfo.FileName = "docker";
            //ExternalProcess.StartInfo.RedirectStandardOutput = true;
            //ExternalProcess.StartInfo.UseShellExecute = false;
            //ExternalProcess.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
            //ExternalProcess.Start();
            //ExternalProcess.BeginOutputReadLine();

            while (true)
            {
                this.WriteHeading();
                Console.WriteLine(this.ExecuteCommand(this.ReadCommandLine()));
            }
        }

        private string GetUserNameFromSystem()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        public void WriteHeading()
        {
            Console.Write("User");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(string.Format(" {0} ", this.GetUserNameFromSystem()));
            Console.ResetColor();

            Console.Write(" in ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(string.Format(" {0} ", Directory.GetCurrentDirectory()));
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(string.Format("[{0:HH:mm:ss}]", DateTime.Now));
            Console.ResetColor();

            Console.Write("\r\n> ");
        }


        private void InitBuiltinCommands()
        {
            this.BuiltinCommands = new CommandsList(this);
        }

        private void InitEnvVariables()
        {
            Environment.SetEnvironmentVariable("shell", System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        private string ReadCommandLine()
        {
            return Console.ReadLine();
        }

        private string ExecuteCommand(string command)
        {
            List<string> Arguments = new List<string>(this.ParseCommand(command.Trim()));

            if (Arguments[0] == "")
            {
                return "";
            }

            string CmdName = Arguments[0];
            Arguments.RemoveAt(0);
            
            if (this.BuiltinCommands.List.ContainsKey(CmdName))
            {
                return this.BuiltinCommands.List[CmdName].Execute(this, Arguments.ToArray());
            }

            return string.Format("Command \"{0}\" not found", CmdName);
        }

        private string[] ParseCommand(string command)
        {
            return command.Split(new char[] { ' ' });
        }
    }
}

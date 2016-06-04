using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

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
            while (true)
            {
                this.WriteHeading();
                this.ExecuteCommand(this.ReadCommandLine());
            }
        }

        private void InitBuiltinCommands()
        {
            this.BuiltinCommands = new CommandsList(this);
        }

        private void InitEnvVariables()
        {
            Environment.SetEnvironmentVariable("shell", System.Reflection.Assembly.GetEntryAssembly().Location);
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

            foreach (Type tinterface in typeof(StreamReader).GetInterfaces())
            {
                Console.WriteLine(tinterface);
            }
        }

        private string ReadCommandLine()
        {
            return Console.ReadLine();
        }

        private int ExecuteCommand(string command)
        {
            List<string> Arguments = new List<string>(this.ParseCommand(command.Trim()));

            if (Arguments[0] == "")
            {
                return 0;
            }

            string CmdName = Arguments[0];
            Arguments.RemoveAt(0);

            if (Arguments.Count > 0 && Arguments[Arguments.Count - 1] == "&")
            {
                Arguments.RemoveAt(Arguments.Count - 1);

                this.ExecuteBackground(CmdName, TextWriter.Null, TextReader.Null, TextWriter.Null, Arguments.ToArray());
                return 0;
            }


            TextWriter stdout = Console.Out;
            TextReader stdin = Console.In;
            TextWriter stderr = Console.Error;

            foreach (string streamChar in new string[] { "<", ">" })
            {
                if (Arguments.Count >= 2) {
                    int index = Arguments.LastIndexOf(streamChar);

                    if (index == -1 || index == Arguments.Count - 1)
                    {
                        continue;
                    }

                    string filePath = Arguments[index + 1];

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine(string.Format("File {0} not exists", filePath));
                        return 1;
                    }

                    if (streamChar == "<")
                    {
                        
                    }
                }
                else
                {
                    break;
                }
            }

            try
            {
                return this.ExecuteForeground(CmdName, stdout, stdin, stderr, Arguments.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }

        private string[] ParseCommand(string command)
        {
            return command.Split(new char[] { ' ' });
        }

        private void ExecuteBackground(string cmd, TextWriter stdout, TextReader stdin, TextWriter stderr, string[] args)
        {
            Thread thread = new Thread(() => this.ExecuteForeground(cmd, stdout, stdin, stderr, args));
            thread.Start();
        }

        private int ExecuteForeground(string cmd, TextWriter stdout, TextReader stdin, TextWriter stderr, string[] args)
        {
            if (this.BuiltinCommands.List.ContainsKey(cmd))
            {
                return this.BuiltinCommands.List[cmd].Execute(this, stdout, stdin, args);
            }
            else
            {
                Process ExternalProcess = new Process();

                ExternalProcess.StartInfo.FileName = cmd;
                ExternalProcess.StartInfo.Arguments = string.Join(" ", args);

                ExternalProcess.StartInfo.UseShellExecute = false;

                ExternalProcess.StartInfo.RedirectStandardInput = true;
                ExternalProcess.StartInfo.RedirectStandardInput = true;
                ExternalProcess.StartInfo.RedirectStandardError = true;

                ExternalProcess.Start();
                ExternalProcess.WaitForExit();

                return ExternalProcess.ExitCode;
            }
        }
    }
}

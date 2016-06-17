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
            // Setup shell loop
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
        }

        private string ReadCommandLine()
        {
            return Console.ReadLine();
        }

        public int ExecuteCommand(string command)
        {
            // Parse arguments
            List<string> Arguments = new List<string>(this.ParseCommand(command.Trim()));

            // Check if first argument actually exists
            if (Arguments[0] == "")
            {
                return 0;
            }

            string CmdName = Arguments[0];
            Arguments.RemoveAt(0);

            // Check if last argument is an ampersand (&)
            if (Arguments.Count > 0 && Arguments[Arguments.Count - 1] == "&")
            {
                // Do not treat ampersand as argument
                Arguments.RemoveAt(Arguments.Count - 1);

                // Execute process in background using null stdin, stdout and stderr
                this.ExecuteBackground(CmdName, TextWriter.Null, TextReader.Null, TextWriter.Null, Arguments.ToArray());
                return 0;
            }

            // Setup default stdin, stdout and stderr
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
                        stdin = new StreamReader(filePath);
                    }
                    else
                    {
                        stdout = new StreamWriter(filePath);
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
            // Execute foreground process in background using non-blocking thread
            Thread thread = new Thread(() => this.ExecuteForeground(cmd, stdout, stdin, stderr, args));
            thread.Start();
        }

        private int ExecuteForeground(string cmd, TextWriter stdout, TextReader stdin, TextWriter stderr, string[] args)
        {
            // Check if command is on list of built-in commands
            if (this.BuiltinCommands.List.ContainsKey(cmd))
            {
                // Execute built-in command
                return this.BuiltinCommands.List[cmd].Execute(this, stdout, stdin, stderr, args);
            }
            else
            {
                // Create new external process
                Process ExternalProcess = new Process();

                // Setup executed command
                ExternalProcess.StartInfo.FileName = cmd;

                // Setup arguments
                ExternalProcess.StartInfo.Arguments = string.Join(" ", args);

                // Setup parent environment variable for this process
                ExternalProcess.StartInfo.EnvironmentVariables["parent"] = Environment.GetEnvironmentVariable("shell");
                ExternalProcess.StartInfo.UseShellExecute = false;

                // Start process
                ExternalProcess.Start();

                string line;

                // Write to process stdin from defined stdin
                while ((line = stdin.ReadLine()) != null) {
                    ExternalProcess.StandardInput.WriteLine(line);
                }

                // Write to defined stdout from process stdout
                stdout.Write(ExternalProcess.StandardOutput.ReadToEnd());

                // Wait for process to exit
                ExternalProcess.WaitForExit();

                // Return process' exit code
                return ExternalProcess.ExitCode;
            }
        }
    }
}

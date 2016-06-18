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

            // First argument is command name
            string CmdName = Arguments[0];

            // Remove command name from arguments array
            Arguments.RemoveAt(0);

            // Flag for executing process in background
            bool ExecuteInBackground = false;

            // Set default stdin, stdout  and stderr
            TextWriter stdout = Console.Out;
            TextReader stdin = Console.In;
            TextWriter stderr = Console.Error;

            // Check if last argument is an ampersand (&)
            if (Arguments.Count > 0 && Arguments[Arguments.Count - 1] == "&")
            {
                // Do not treat ampersand as argument
                Arguments.RemoveAt(Arguments.Count - 1);

                // Set process to be executed in background
                ExecuteInBackground = true;

                // Set default background process stdin and stderr
                stdout = TextWriter.Null;
                stdin = TextReader.Null;
                stderr = TextWriter.Null;
            }

            // Check for input/output redirection characters
            foreach (string streamChar in new string[] { "<", ">" })
            {
                // Check if there are at least two arguments
                if (Arguments.Count >= 2) {
                    int index = Arguments.LastIndexOf(streamChar);

                    // Check if arrow character is one index before the last one
                    // Continue loop if it is not
                    if (index == -1 || index == Arguments.Count - 1)
                    {
                        continue;
                    }

                    // Last argument is file path
                    string filePath = Arguments[index + 1];

                    // Check if file exits
                    if (!File.Exists(filePath))
                    {
                        // If file does not exists and we are setting it as stdout, create new file
                        // Otherwise return an error
                        if (streamChar == ">")
                        {
                            File.Create(filePath).Close();
                        }
                        else
                        {
                            Console.Error.WriteLine(string.Format("File {0} not exists", filePath));
                            return 1;
                        }
                    }

                    // Finally, set proper streams for stdin/stdout
                    if (streamChar == "<")
                    {
                        stdin = new StreamReader(filePath);
                    }
                    else
                    {
                        stdout = new StreamWriter(filePath);
                    }

                    // Remove file and arrow character from arguments array
                    Arguments.RemoveAt(index + 1);
                    Arguments.RemoveAt(index);
                }
                else
                {
                    break;
                }
            }

            try
            {
                // Check if process is set to be executed in background
                if (ExecuteInBackground)
                {
                    this.ExecuteBackground(CmdName, stdout, stdin, stderr, Arguments.ToArray());
                    return 0;
                }

                // Get exit code
                return this.ExecuteForeground(CmdName, stdout, stdin, stderr, Arguments.ToArray());
            }
            catch (Exception e)
            {
                // Write error message to stderr
                stderr.WriteLine(e.Message);

                // Close streams
                this.CloseStreams(stdout, stdin, stderr);

                // Return error code
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
            int ExitCode;

            // Check if command is on list of built-in commands
            if (this.BuiltinCommands.List.ContainsKey(cmd))
            {
                // Execute built-in command
                ExitCode = this.BuiltinCommands.List[cmd].Execute(this, stdout, stdin, stderr, args);
            }
            else
            {
                bool isConsoleInput = stdin.Equals(Console.In);

                // Create new external process
                Process ExternalProcess = new Process();

                // Setup executed command
                ExternalProcess.StartInfo.FileName = cmd;

                // Setup arguments
                ExternalProcess.StartInfo.Arguments = string.Join(" ", args);

                // Setup parent environment variable for this process
                ExternalProcess.StartInfo.EnvironmentVariables["parent"] = Environment.GetEnvironmentVariable("shell");
                ExternalProcess.StartInfo.UseShellExecute = false;

                // Always redirect standard output and standard error
                ExternalProcess.StartInfo.RedirectStandardOutput = true;
                ExternalProcess.StartInfo.RedirectStandardError = true;

                // Check if stdin is set as console input
                if (!isConsoleInput)
                {
                    ExternalProcess.StartInfo.RedirectStandardInput = true;
                }

                // Start process
                ExternalProcess.Start();

                string line;

                // Write to process stdin from defined stdin if it is not console stdin
                while (!isConsoleInput && (line = stdin.ReadLine()) != null) {
                    ExternalProcess.StandardInput.WriteLine(line);
                }

                // Write to defined stdout from process stdout
                stdout.Write(ExternalProcess.StandardOutput.ReadToEnd());

                // Write to defined stderr from process stderr
                stderr.Write(ExternalProcess.StandardError.ReadToEnd());

                // Wait for process to exit
                ExternalProcess.WaitForExit();

                // Return process' exit code
                ExitCode = ExternalProcess.ExitCode;
            }

            // Close opened streams
            this.CloseStreams(stdout, stdin, stderr);

            // Return exit code
            return ExitCode;
        }

        // Close given stdout, stdin and stder streams
        private void CloseStreams(TextWriter stdout, TextReader stdin, TextWriter stderr)
        {
            stdin.Close();
            stdout.Close();
            stderr.Close();
        }
    }
}

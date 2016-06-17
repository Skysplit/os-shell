using System.IO;

namespace ShellApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create now shell loop object
            Loop loop = new Loop();

            // Check if shell has been run with  any arguments
            if (args.Length > 0)
            {
                string FilePath = args[0];

                // Check if argument is file that exists
                if (File.Exists(FilePath))
                {
                    StreamReader file = new StreamReader(FilePath);
                    string line;

                    // Read lines from file and execute them as commands
                    while ((line = file.ReadLine()) != null)
                    {
                        loop.ExecuteCommand(line);
                    }
                }
            }

            // Initialize shell loop
            loop.InitLoop();
        }
    }
}

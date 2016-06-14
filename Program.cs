using System.IO;

namespace ShellApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Loop loop = new Loop();

            if (args.Length > 0)
            {
                string FilePath = args[0];

                if (File.Exists(FilePath))
                {
                    StreamReader file = new StreamReader(FilePath);
                    string line;

                    while ((line = file.ReadLine()) != null)
                    {
                        loop.ExecuteCommand(line);
                    }
                }
            }

            loop.InitLoop();
        }
    }
}

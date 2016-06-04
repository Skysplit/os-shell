using System.IO;

namespace ShellApplication.Commands
{
    class ChangeDirectory : CommandInterface
    {
        public string GetName()
        {
            return "cd";
        }

        public int Execute(Loop ctx, TextWriter stdout, TextReader stdin, string[] args)
        {
            if (args.Length == 0)
            {
                stdout.WriteLine(Directory.GetCurrentDirectory());
                return 0;
            }

            if (!Directory.Exists(args[0]))
            {
                stdout.WriteLine(string.Format("Directory {0} does not exists", args[0]));
                return 1;
            }

            Directory.SetCurrentDirectory(args[0]);

            stdout.WriteLine(string.Format("Directory changed to {0}", args[0]));
            return 0;
        }
    }
}

using System.IO;

namespace ShellApplication.Commands
{
    class ChangeDirectory : CommandInterface
    {
        public string GetName()
        {
            return "cd";
        }

        public string Execute(Loop ctx, string[] args)
        {
            if (args.Length == 0)
            {
                return Directory.GetCurrentDirectory();
            }

            if (!Directory.Exists(args[0]))
            {
                return string.Format("Directory {0} does not exists", args[0]);
            }

            Directory.SetCurrentDirectory(args[0]);

            return string.Format("Directory changed to {0}", args[0]);
        }
    }
}

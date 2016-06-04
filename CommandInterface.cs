using System.IO;

namespace ShellApplication
{
    interface CommandInterface
    {
        string GetName();
        int Execute(Loop ctx, TextWriter stdout, TextReader stdin, string[] args);
    }
}

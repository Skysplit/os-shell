namespace ShellApplication
{
    interface CommandInterface
    {
        string GetName();
        string Execute(Loop ctx, string[] args);
    }
}

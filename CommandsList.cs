using System.Collections.Generic;
using ShellApplication.Commands;

namespace ShellApplication
{
    class CommandsList
    {
        private Loop ctx;
        public IDictionary<string, CommandInterface> List { get; set; }

        public CommandsList(Loop loop)
        {
            this.ctx = loop;
            this.List = new Dictionary<string, CommandInterface>();

            // Register builtin commands
            this.RegisterBuiltinFunction(new ChangeDirectory());
            this.RegisterBuiltinFunction(new ListDirectory());
            this.RegisterBuiltinFunction(new PrintEnvironment());
            this.RegisterBuiltinFunction(new QuitShell());
            this.RegisterBuiltinFunction(new ClearConsole());
        }

        private void RegisterBuiltinFunction(CommandInterface BuiltinFunction)
        {
            this.List.Add(BuiltinFunction.GetName(), BuiltinFunction);
        }

    }
}

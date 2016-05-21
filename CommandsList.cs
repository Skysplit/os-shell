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
            this.RegisterBuiltinFunction(new ChangeDirectory()); // cd
            this.RegisterBuiltinFunction(new ListDirectory()); // dir
            this.RegisterBuiltinFunction(new PrintEnvironment()); // environ
            this.RegisterBuiltinFunction(new QuitShell()); // quit
            this.RegisterBuiltinFunction(new ClearConsole()); // clr
            this.RegisterBuiltinFunction(new PauseInput()); // pause
        }

        private void RegisterBuiltinFunction(CommandInterface BuiltinFunction)
        {
            this.List.Add(BuiltinFunction.GetName(), BuiltinFunction);
        }

    }
}

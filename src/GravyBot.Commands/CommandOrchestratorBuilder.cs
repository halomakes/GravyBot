using System.Collections.Generic;
using System.Reflection;

namespace GravyBot.Commands
{
    public class CommandOrchestratorBuilder
    {
        private readonly Dictionary<string, CommandBinding> bindings = new Dictionary<string, CommandBinding>();

        public void RegisterProcessor<TProcessor>() where TProcessor : CommandProcessor
        {
            foreach (var method in typeof(TProcessor).GetMethods())
                foreach (var command in method.GetCommands())
                    RegisterCommand(command, method);
        }

        private void RegisterCommand(CommandAttribute command, MethodInfo method)
        {
            if (bindings.ContainsKey(command.Command))
            {
                var existingCommand = bindings[command.Command];
                throw new DuplicateCommandException(command.Command, existingCommand.Method, method);
            }

            bindings[command.Command] = new CommandBinding(command, method);
        }

    }
}

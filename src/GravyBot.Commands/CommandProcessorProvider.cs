using GravyIrc.Messages;
using System;

namespace GravyBot.Commands
{
    public class CommandProcessorProvider : ICommandProcessorProvider
    {
        private readonly IServiceProvider serviceProvider;

        public CommandProcessorProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public CommandProcessor GetProcessor(CommandBinding binding, PrivateMessage message)
        {
            var processor = serviceProvider.GetService(binding.Method.ReflectedType) as CommandProcessor;
            processor.IncomingMessage = message;
            processor.TriggeringCommandName = binding.Command.CommandName;
            return processor;
        }
    }
}

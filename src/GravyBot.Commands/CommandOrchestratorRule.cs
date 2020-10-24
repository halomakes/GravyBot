using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace GravyBot.Commands
{
    public class CommandOrchestratorRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration configuration;
        private readonly IServiceProvider serviceProvider;

        public CommandOrchestratorRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider)
        {
            configuration = options.Value;
            this.serviceProvider = serviceProvider;
        }

        public bool Matches(PrivateMessage incomingMessage) => incomingMessage.Message.StartsWith(configuration.CommandPrefix);

        public IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            throw new NotImplementedException();
        }
    }
}

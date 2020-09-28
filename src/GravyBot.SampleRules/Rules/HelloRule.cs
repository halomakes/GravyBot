using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace GravyBot.DefaultRules.Rules
{
    public class HelloRule : IMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;

        public HelloRule(IOptions<IrcBotConfiguration> options)
        {
            config = options.Value;
        }

        public IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            if (incomingMessage.Message == $"{config.CommandPrefix}hello")
            {
                yield return new PrivateMessage(incomingMessage.IsChannelMessage ? incomingMessage.To : incomingMessage.From, $"Hello, {incomingMessage.From}!");
            }
        }
    }
}

using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace GravyBot.DefaultRules.Rules
{
    public class HelloRule : MessageRuleBase<PrivateMessage>, IMessageRule<PrivateMessage>
    {
        private readonly ChatBeetConfiguration config;

        public HelloRule(IOptions<ChatBeetConfiguration> options)
        {
            config = options.Value;
        }

        public override async IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            if (incomingMessage.Message == $"{config.CommandPrefix}hello")
            {
                yield return new PrivateMessage(incomingMessage.IsChannelMessage ? incomingMessage.To : incomingMessage.From, $"Hello, {incomingMessage.From}!");
            }
        }
    }
}

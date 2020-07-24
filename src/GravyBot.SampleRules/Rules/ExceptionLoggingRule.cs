using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace GravyBot.DefaultRules.Rules
{
    public class ExceptionLoggingRule : MessageRuleBase<Exception>, IMessageRule<Exception>
    {
        private readonly ChatBeetConfiguration config;

        public ExceptionLoggingRule(IOptions<ChatBeetConfiguration> options)
        {
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(Exception exception)
        {
            yield return new OutboundIrcMessage
            {
                Content = $"{IrcValues.RED}Encountered exception from {IrcValues.ORANGE}{IrcValues.BOLD}{exception.Source}",
                OutputType = IrcMessageType.Announcement,
                Target = config.LogChannel
            };

            yield return new OutboundIrcMessage
            {
                Content = $"Base exception: {exception.Message}",
                Target = config.LogChannel
            };

            var depth = 0;
            var currentException = exception;

            while (depth < 4 && currentException.InnerException != null)
            {
                currentException = currentException.InnerException;
                depth++;

                yield return new OutboundIrcMessage
                {
                    Content = $"Inner exception: {currentException.Message}",
                    Target = config.LogChannel
                };
            }
        }
    }
}

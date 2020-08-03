using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace GravyBot.DefaultRules.Rules
{
    public class ExceptionLoggingRule : MessageRuleBase<Exception>, IMessageRule<Exception>
    {
        private readonly IrcBotConfiguration config;

        public ExceptionLoggingRule(IOptions<IrcBotConfiguration> options)
        {
            config = options.Value;
        }

        public override IEnumerable<IClientMessage> Respond(Exception exception)
        {
            yield return new NoticeMessage(config.LogChannel, $"{IrcValues.RED}Encountered exception from {IrcValues.ORANGE}{IrcValues.BOLD}{exception.Source}");

            yield return new PrivateMessage(config.LogChannel, $"Base exception: {exception.Message}");

            var depth = 0;
            var currentException = exception;

            while (depth < 4 && currentException.InnerException != null)
            {
                currentException = currentException.InnerException;
                depth++;

                yield return new PrivateMessage(config.LogChannel, $"Inner exception: {currentException.Message}");
            }
        }
    }
}

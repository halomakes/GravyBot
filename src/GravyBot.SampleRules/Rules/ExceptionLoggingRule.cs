using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace GravyBot.DefaultRules.Rules
{
    public class ExceptionLoggingRule : IMessageRule<Exception>
    {
        private readonly IrcBotConfiguration config;

        public ExceptionLoggingRule(IOptions<IrcBotConfiguration> options)
        {
            config = options.Value;
        }

        public IEnumerable<IClientMessage> Respond(Exception exception)
        {
            yield return new PrivateMessage(config.LogChannel, $"{IrcValues.RED}Encountered {IrcValues.ITALIC}{exception.GetType().Name}{IrcValues.RESET} from {IrcValues.ORANGE}{IrcValues.BOLD}{exception.Source}");

            yield return new PrivateMessage(config.LogChannel, $"Base {exception.GetType().Name}: {exception.Message}");

            var depth = 0;
            var currentException = exception;

            while (depth < 4 && currentException.InnerException != null)
            {
                currentException = currentException.InnerException;
                depth++;

                yield return new PrivateMessage(config.LogChannel, $"Inner {currentException.GetType().Name}: {currentException.Message}");
            }
        }
    }
}

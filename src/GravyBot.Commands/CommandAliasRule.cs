using GravyIrc.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GravyBot.Commands
{
    /// <summary>
    /// Enables alternative regular expression matches to trigger a command
    /// </summary>
    /// <typeparam name="TProcessor">Type of the command processor to alias</typeparam>
    public abstract class CommandAliasRule<TProcessor> : IAsyncMessageRule<PrivateMessage> where TProcessor : CommandProcessor
    {
        private readonly IServiceProvider ServiceProvider;
        protected IrcBotConfiguration Configuration;

        /// <summary>
        /// Regular Expression pattern to use for matching
        /// </summary>
        protected Regex Pattern;

        /// <summary>
        /// Command name to pass to processor if needed
        /// </summary>
        protected string SimulatedCommandName;

        public CommandAliasRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider)
        {
            Configuration = options.Value;
            ServiceProvider = serviceProvider;
        }

        public virtual bool Matches(PrivateMessage incomingMessage) => Pattern.IsMatch(incomingMessage.Message);

        public virtual IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var processor = ServiceProvider.GetService<TProcessor>();
            processor.IncomingMessage = incomingMessage;
            processor.TriggeringCommandName = SimulatedCommandName;
            return OnMatch(Pattern.Match(incomingMessage.Message), processor);
        }

        /// <summary>
        /// Action to take when a match is found
        /// </summary>
        /// <param name="match">Information about regular expression match</param>
        /// <param name="commandProcessor">Instance of processor</param>
        protected abstract IAsyncEnumerable<IClientMessage> OnMatch(Match match, TProcessor commandProcessor);
    }
}

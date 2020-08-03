using GravyIrc.Messages;
using System.Collections.Generic;
using System.Linq;

namespace GravyBot
{
    /// <summary>
    /// Base provided for rules to inherit from
    /// </summary>
    /// <typeparam name="TMessage">Type of message to handle events for</typeparam>
    public abstract class AsyncMessageRuleBase<TMessage> : IAsyncMessageRule<TMessage>, IAsyncMessageRule
    {
        public abstract bool MatchesFilter(TMessage incomingMessage);

        public bool MatchesFilter(object incomingMessage) =>
            incomingMessage is TMessage message
            ? MatchesFilter(message)
            : false;

        /// <summary>
        /// Respond to an incoming message
        /// </summary>
        /// <param name="incomingMessage">Incoming message</param>
        /// <returns>Messages to send</returns>
        public abstract IAsyncEnumerable<IClientMessage> RespondAsync(TMessage incomingMessage);

        /// <summary>
        /// Respond to an incoming message
        /// </summary>
        /// <remarks>Used to handle non-generic implementation</remarks>
        /// <param name="incomingMessage">Incoming message</param>
        public IAsyncEnumerable<IClientMessage> RespondAsync(object incomingMessage) =>
            incomingMessage is TMessage message
            ? RespondAsync(message)
            : EmptyResult();

        protected IAsyncEnumerable<IClientMessage> EmptyResult() => AsyncEnumerable.Empty<IClientMessage>();
    }

    /// <summary>
    /// Base provided for rules to inherit from
    /// </summary>
    /// <typeparam name="TMessage">Type of message to handle events for</typeparam>
    public abstract class MessageRuleBase<TMessage> : IMessageRule<TMessage>, IMessageRule
    {
        /// <summary>
        /// Respond to an incoming message
        /// </summary>
        /// <param name="incomingMessage">Incoming message</param>
        /// <returns>Messages to send</returns>
        public abstract IEnumerable<IClientMessage> Respond(TMessage incomingMessage);

        /// <summary>
        /// Respond to an incoming message
        /// </summary>
        /// <remarks>Used to handle non-generic implementation</remarks>
        /// <param name="incomingMessage">Incoming message</param>
        public IEnumerable<IClientMessage> Respond(object incomingMessage) =>
            incomingMessage is TMessage message
            ? Respond(message)
            : EmptyResult();

        protected IEnumerable<IClientMessage> EmptyResult() => Enumerable.Empty<IClientMessage>();
    }
}

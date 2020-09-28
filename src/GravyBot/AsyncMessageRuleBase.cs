using GravyIrc.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GravyBot
{
    /// <summary>
    /// Base provided for rules to inherit from
    /// </summary>
    /// <typeparam name="TMessage">Type of message to handle events for</typeparam>
    public abstract class AsyncMessageRuleBase<TMessage> : IAsyncMessageRule<TMessage>
    {
        public abstract bool Matches(TMessage incomingMessage);

        [Obsolete("Response methods for raw objects are no longer required.")]
        public bool Matches(object incomingMessage) => incomingMessage is TMessage message && Matches(message);

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
        [Obsolete("Response methods for raw objects are no longer required.")]
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
    public abstract class MessageRuleBase<TMessage> : IMessageRule<TMessage>
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
        [Obsolete("Response methods for raw objects are no longer required.")]
        public IEnumerable<IClientMessage> Respond(object incomingMessage) =>
            incomingMessage is TMessage message
            ? Respond(message)
            : EmptyResult();

        [Obsolete("Response methods for raw objects are no longer required.")]
        protected IEnumerable<IClientMessage> EmptyResult() => Enumerable.Empty<IClientMessage>();
    }
}

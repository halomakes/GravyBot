using GravyIrc.Messages;
using System.Collections.Generic;

namespace GravyBot
{
    /// <summary>
    /// A rule that can be applied to incoming messages
    /// </summary>
    public interface IMessageRule
    {
        /// <summary>
        /// Process an inbound message
        /// </summary>
        /// <param name="incomingMessage">Inbound message</param>
        IAsyncEnumerable<IClientMessage> Respond(object incomingMessage);
    }

    /// <summary>
    /// A rule that can be applied to incoming messages
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageRule<TMessage> : IMessageRule
    {
        /// <summary>
        /// Process an inbound message
        /// </summary>
        /// <param name="incomingMessage">Inbound message</param>
        IAsyncEnumerable<IClientMessage> Respond(TMessage incomingMessage);
    }
}

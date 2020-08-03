using GravyIrc.Messages;
using System.Collections.Generic;

namespace GravyBot
{
    /// <summary>
    /// A rule that can be applied to incoming messages
    /// </summary>
    public interface IAsyncMessageRule
    {
        /// <summary>
        /// Process an inbound message
        /// </summary>
        /// <param name="incomingMessage">Inbound message</param>
        IAsyncEnumerable<IClientMessage> RespondAsync(object incomingMessage);

        /// <summary>
        /// Indicates if a task should be created to fully process the rule
        /// </summary>
        /// <param name="incomingMessage">Inbound message</param>
        bool Matches(object incomingMessage);
    }

    /// <summary>
    /// A rule that can be applied to incoming messages
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IAsyncMessageRule<TMessage> : IAsyncMessageRule
    {
        /// <summary>
        /// Process an inbound message
        /// </summary>
        /// <param name="incomingMessage">Inbound message</param>
        IAsyncEnumerable<IClientMessage> RespondAsync(TMessage incomingMessage);

        /// <summary>
        /// Indicates if a task should be created to fully process the rule
        /// </summary>
        /// <param name="incomingMessage">Inbound message</param>
        bool Matches(TMessage incomingMessage);
    }
}

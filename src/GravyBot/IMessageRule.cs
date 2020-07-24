using System.Collections.Generic;

namespace GravyBot
{
    public interface IMessageRule
    {
        IAsyncEnumerable<OutboundIrcMessage> Respond(object incomingMessage);
    }

    public interface IMessageRule<TMessage> : IMessageRule
    {
        IAsyncEnumerable<OutboundIrcMessage> Respond(TMessage incomingMessage);
    }
}

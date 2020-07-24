using GravyIrc.Messages;
using System.Collections.Generic;

namespace GravyBot
{
    public interface IMessageRule
    {
        IAsyncEnumerable<IClientMessage> Respond(object incomingMessage);
    }

    public interface IMessageRule<TMessage> : IMessageRule
    {
        IAsyncEnumerable<IClientMessage> Respond(TMessage incomingMessage);
    }
}

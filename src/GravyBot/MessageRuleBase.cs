using GravyIrc.Messages;
using System.Collections.Generic;
using System.Linq;

namespace GravyBot
{
    public abstract class MessageRuleBase<TMessage> : IMessageRule<TMessage>, IMessageRule
    {
        public abstract IAsyncEnumerable<IClientMessage> Respond(TMessage incomingMessage);

        public IAsyncEnumerable<IClientMessage> Respond(object incomingMessage) =>
            incomingMessage is TMessage message
            ? Respond(message)
            : EmptyResult();

        protected IAsyncEnumerable<IClientMessage> EmptyResult() => AsyncEnumerable.Empty<IClientMessage>();
    }
}

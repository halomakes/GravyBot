using System;

namespace GravyBot
{
    public interface IInboundMessage
    {
        string Sender { get; }

        string Content { get; }

        DateTime DateRecieved { get; }
    }
}

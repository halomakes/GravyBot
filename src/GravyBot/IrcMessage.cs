using System;

namespace GravyBot
{
    public class IrcMessage : IInboundMessage
    {
        public string Sender { get; set; }

        public string Channel { get; set; }

        public string Content { get; set; }

        public DateTime DateRecieved { get; set; }
    }
}

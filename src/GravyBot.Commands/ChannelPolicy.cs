using GravyIrc.Messages;
using System.Collections.Generic;
using System.Linq;

namespace GravyBot.Commands
{
    public class ChannelPolicy
    {
        public IEnumerable<string> Channels { get; set; }
        public PolicyMode Mode { get; set; }

        public bool Blocks(PrivateMessage message) => Mode == PolicyMode.Blacklist && Channels.Contains(message.To) || Mode == PolicyMode.Whitelist && !Channels.Contains(message.To);

        public enum PolicyMode
        {
            Blacklist,
            Whitelist
        }
    }
}

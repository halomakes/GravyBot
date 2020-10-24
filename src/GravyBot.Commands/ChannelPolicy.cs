using GravyIrc.Messages;
using System.Collections.Generic;
using System.Linq;

namespace GravyBot.Commands
{
    /// <summary>
    /// A policy controlling how rules are applied in channels
    /// </summary>
    public class ChannelPolicy
    {
        /// <summary>
        /// Channels to apply to
        /// </summary>
        public IEnumerable<string> Channels { get; set; }

        /// <summary>
        /// Action to apply to channels
        /// </summary>
        public PolicyMode Mode { get; set; }

        /// <summary>
        /// Check if a command request is blocked by this policy
        /// </summary>
        /// <param name="message">Message to check</param>
        public bool Blocks(PrivateMessage message) => Mode == PolicyMode.Blacklist && Channels.Contains(message.To) || Mode == PolicyMode.Whitelist && !Channels.Contains(message.To);

        /// <summary>
        /// Mode to apply on a policy
        /// </summary>
        public enum PolicyMode
        {
            /// <summary>
            /// Block command in specified channels
            /// </summary>
            Blacklist,
            /// <summary>
            /// Only allow command in specified channels
            /// </summary>
            /// <remarks>Direct messages still allowed</remarks>
            Whitelist
        }
    }
}

using GravyIrc.Messages;
using System;

namespace GravyBot.Commands
{
    /// <summary>
    /// Base class for command processors
    /// </summary>
    public abstract class CommandProcessor
    {
        /// <summary>
        /// Raw incoming IRC message
        /// </summary>
        public PrivateMessage IncomingMessage { get; set; }

        /// <summary>
        /// Name of command that triggered method
        /// </summary>
        /// <remarks>Useful for methods with multiple command bindings</remarks>
        public string TriggeringCommandName { get; set; }

        /// <summary>
        /// Prevents an invokation from bumping ratelimit history
        /// </summary>
        public Action BypassRateLimit { get; set; }
    }
}

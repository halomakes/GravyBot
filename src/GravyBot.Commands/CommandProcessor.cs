using GravyIrc.Messages;

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
    }
}

using GravyIrc.Messages;

namespace GravyBot.Commands
{
    public abstract class CommandProcessor
    {
        public PrivateMessage IncomingMessage { get; set; }

        public string TriggeringCommandName { get; set; }
    }
}

using GravyIrc.Messages;

namespace GravyBot.Commands
{
    public interface ICommandProcessorProvider
    {
        CommandProcessor GetProcessor(CommandBinding binding, PrivateMessage message);
    }
}

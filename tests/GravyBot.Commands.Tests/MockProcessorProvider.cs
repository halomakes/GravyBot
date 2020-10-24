using GravyIrc.Messages;

namespace GravyBot.Commands.Tests
{
    public class MockProcessorProvider<TProcessor> : ICommandProcessorProvider where TProcessor : CommandProcessor, new()
    {
        public CommandProcessor GetProcessor(CommandBinding binding, PrivateMessage message) => new TProcessor();
    }
}

using GravyIrc.Messages;

namespace GravyBot.Commands
{
    /// <summary>
    /// Provides access to commands with DI context
    /// </summary>
    public interface ICommandProcessorProvider
    {
        /// <summary>
        /// Get a processor based on a binding
        /// </summary>
        /// <param name="binding">Binding to get processor for</param>
        /// <param name="message">Triggering message</param>
        /// <returns>Bound processor</returns>
        CommandProcessor GetProcessor(CommandBinding binding, PrivateMessage message);
    }
}

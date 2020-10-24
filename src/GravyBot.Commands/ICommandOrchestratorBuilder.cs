using System.Collections.Generic;

namespace GravyBot.Commands
{
    public interface ICommandOrchestratorBuilder
    {
        IReadOnlyDictionary<string, CommandBinding> Bindings { get; }

        void RegisterProcessor<TProcessor>() where TProcessor : CommandProcessor;
    }
}
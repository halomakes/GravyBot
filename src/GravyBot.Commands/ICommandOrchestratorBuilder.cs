using System.Collections.Generic;

namespace GravyBot.Commands
{
    public interface ICommandOrchestratorBuilder
    {
        IReadOnlyDictionary<string, CommandBinding> Bindings { get; }
        IReadOnlyDictionary<string, ChannelPolicy> Policies { get; }

        void RegisterProcessor<TProcessor>() where TProcessor : CommandProcessor;
        void AddChannelPolicy(string policyName, IEnumerable<string> channels, ChannelPolicy.PolicyMode mode);
        void AddChannelPolicy(string policyName, ChannelPolicy policy);

    }
}
using System.Collections.Generic;
using System.Reflection;

namespace GravyBot.Commands
{
    /// <summary>
    /// Configures a command orchestrator
    /// </summary>
    public interface ICommandOrchestratorBuilder
    {
        /// <summary>
        /// Active command bindings
        /// </summary>
        IReadOnlyDictionary<string, CommandBinding> Bindings { get; }

        /// <summary>
        /// Active channel policies
        /// </summary>
        IReadOnlyDictionary<string, ChannelPolicy> Policies { get; }

        /// <summary>
        /// Register all processors in an assembly
        /// </summary>
        /// <param name="assembly">Assembly to register processors from</param>
        void RegisterProcessors(Assembly assembly);

        /// <summary>
        /// Register a processor
        /// </summary>
        /// <typeparam name="TProcessor">Type of processor to register</typeparam>
        void RegisterProcessor<TProcessor>() where TProcessor : CommandProcessor;

        /// <summary>
        /// Create a channel policy
        /// </summary>
        /// <param name="policyName">Name of policy</param>
        /// <param name="channels">Channels to apply action to</param>
        /// <param name="mode">Behavior for selected channels</param>
        void AddChannelPolicy(string policyName, IEnumerable<string> channels, ChannelPolicy.PolicyMode mode);

        /// <summary>
        /// Create a channel policy
        /// </summary>
        /// <param name="policyName">Name of policy</param>
        /// <param name="policy">Policy to add</param>
        void AddChannelPolicy(string policyName, ChannelPolicy policy);

    }
}
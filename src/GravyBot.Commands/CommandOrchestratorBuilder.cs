using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;

namespace GravyBot.Commands
{
    public class CommandOrchestratorBuilder : ICommandOrchestratorBuilder
    {
        private readonly IServiceCollection serviceCollection;

        public CommandOrchestratorBuilder(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        private readonly Dictionary<string, CommandBinding> bindings = new Dictionary<string, CommandBinding>();
        private readonly Dictionary<string, ChannelPolicy> policies = new Dictionary<string, ChannelPolicy>();
        public IReadOnlyDictionary<string, CommandBinding> Bindings => bindings;
        public IReadOnlyDictionary<string, ChannelPolicy> Policies => policies;

        public virtual void RegisterProcessor<TProcessor>() where TProcessor : CommandProcessor
        {
            foreach (var method in typeof(TProcessor).GetMethods())
                foreach (var command in method.GetCommands())
                    RegisterCommand(command, method);

            serviceCollection.AddScoped<TProcessor>();
        }

        protected void RegisterCommand(CommandAttribute command, MethodInfo method)
        {
            if (bindings.ContainsKey(command.CommandName))
            {
                var existingCommand = bindings[command.CommandName];
                throw new DuplicateCommandException(command.CommandName, existingCommand.Method, method);
            }

            bindings[command.CommandName] = new CommandBinding(command, method);
        }

        public void AddChannelPolicy(string policyName, IEnumerable<string> channels, ChannelPolicy.PolicyMode mode) => AddChannelPolicy(policyName, new ChannelPolicy
        {
            Channels = channels,
            Mode = mode
        });

        public void AddChannelPolicy(string policyName, ChannelPolicy policy)
        {
            policies[policyName] = policy;
        }
    }
}

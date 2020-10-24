using GravyIrc.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GravyBot.Commands
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Add command orchestration core to a rule pipeline
        /// </summary>
        /// <param name="pipeline">Rule pipeline</param>
        public static BotRulePipeline AddCommandOrchestrator(this BotRulePipeline pipeline)
        {
            pipeline.RegisterAsyncRule<CommandOrchestratorRule, PrivateMessage>();
            return pipeline;
        }

        /// <summary>
        /// Register services for a command orchestrator
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configureOrchestrator">Configuration action to apply to orchestrator</param>
        /// <returns></returns>
        public static IServiceCollection AddCommandOrchestrator(this IServiceCollection services, Action<CommandOrchestratorBuilder> configureOrchestrator)
        {
            var builder = new CommandOrchestratorBuilder(services);
            configureOrchestrator(builder);
            services.AddSingleton<ICommandOrchestratorBuilder>(provider => builder);
            services.AddTransient<ICommandProcessorProvider, CommandProcessorProvider>();

            return services;
        }
    }
}

using GravyIrc.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GravyBot.Commands
{
    public static class StartupExtensions
    {
        public static BotRulePipeline AddCommandOrchestrator(this BotRulePipeline pipeline)
        {
            pipeline.RegisterAsyncRule<CommandOrchestratorRule, PrivateMessage>();
            return pipeline;
        }
        public static IServiceCollection AddCommandOrchestrator(this IServiceCollection services, Action<CommandOrchestratorBuilder> configureOrchestrator)
        {
            var builder = new CommandOrchestratorBuilder();
            configureOrchestrator(builder);
            services.AddSingleton(provider => builder);

            return services;
        }
    }
}

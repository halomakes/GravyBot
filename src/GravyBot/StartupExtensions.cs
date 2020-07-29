using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace GravyBot
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Add an IRC bot to the dependency injection container
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configSection">Configuration section to bind to</param>
        /// <param name="configurePipeline">Configuration action for rule pipeline</param>
        public static IServiceCollection AddIrcBot(this IServiceCollection services, IConfigurationSection configSection, Action<BotRulePipeline> configurePipeline = null)
        {
            services.Configure<IrcBotConfiguration>(c => configSection.Bind(c));
            ConfigureIrcBot(services, configurePipeline);
            return services;
        }

        /// <summary>
        /// Add an IRC bot to the dependency injection container
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configure">Action to configure bot settings</param>
        /// <param name="configurePipeline">Configuration action for rule pipeline</param>
        public static IServiceCollection AddIrcBot(this IServiceCollection services, Action<IrcBotConfiguration> configure, Action<BotRulePipeline> configurePipeline = null)
        {
            if (configure == null)
            {
                configure = (options) => { };
            }

            services.Configure(configure);
            ConfigureIrcBot(services, configurePipeline);
            return services;
        }

        private static void ConfigureIrcBot(IServiceCollection services, Action<BotRulePipeline> configurePipeline)
        {
            services.AddSingleton<MessageQueueService>();

            var pipeline = new BotRulePipeline(services);
            configurePipeline?.Invoke(pipeline);

            services.AddHostedService(p => new IrcBotService(
                p.GetRequiredService<MessageQueueService>(),
                p.GetRequiredService<IOptions<IrcBotConfiguration>>(),
                pipeline
            ));
        }
    }
}

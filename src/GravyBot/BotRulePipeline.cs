using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace GravyBot
{
    /// <summary>
    /// Pipeline used to configure rules to be applied to incoming messages
    /// </summary>
    public class BotRulePipeline
    {
        private readonly IServiceCollection services;

        /// <summary>
        /// Types that are being subscribed to
        /// </summary>
        public IEnumerable<Type> SubscribedTypes => subscribedTypes;

        /// <summary>
        /// Enable automatic reconnection after a connection drop
        /// </summary>
        public bool IsAutoReconnectEnabled { get; set; } = true;

        private readonly List<Type> subscribedTypes = new List<Type>();

        /// <summary>
        /// Create a new pipeline, providing service collection
        /// </summary>
        /// <param name="services">Service collection to register rules into</param>
        public BotRulePipeline(IServiceCollection services)
        {
            this.services = services;
        }

        /// <summary>
        /// Adds a rule for dependency injection and configures incoming message events to trigger it
        /// </summary>
        /// <typeparam name="TRule"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        public void RegisterRule<TRule, TMessage>() where TRule : class, IMessageRule<TMessage>, IMessageRule
        {
            services.AddTransient<IMessageRule, TRule>();
            subscribedTypes.Add(typeof(TMessage));
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace GravyBot
    public class BotRulePipeline
    {
        private readonly IServiceCollection services;
        public List<Type> SubscribedTypes = new List<Type>();

        public BotRulePipeline(IServiceCollection services)
        {
            this.services = services;
        }

        public void RegisterRule<TRule, TMessage>() where TRule : class, IMessageRule<TMessage>, IMessageRule
        {
            services.AddTransient<IMessageRule, TRule>();
            SubscribedTypes.Add(typeof(TMessage));
        }
    }
}

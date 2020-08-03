using GravyBot.DefaultRules.Rules;
using GravyIrc.Messages;
using System;

namespace GravyBot.DefaultRules
{
    public static class StartupExtensions
    {
        public static BotRulePipeline AddSampleRules(this BotRulePipeline pipeline)
        {
            pipeline.RegisterAsyncRule<HelloRule, PrivateMessage>();
            pipeline.RegisterAsyncRule<ExceptionLoggingRule, Exception>();

            return pipeline;
        }
    }
}

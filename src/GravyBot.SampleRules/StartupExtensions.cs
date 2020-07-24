using GravyBot.DefaultRules.Rules;
using GravyIrc.Messages;

namespace GravyBot.DefaultRules
{
    public static class StartupExtensions
    {
        public static BotRulePipeline AddDefaultRules(this BotRulePipeline pipeline)
        {
            pipeline.RegisterRule<HelloRule, PrivateMessage>();
            pipeline.RegisterRule<ExceptionLoggingRule, ExceptionMessage>();

            return pipeline;
        }
    }
}

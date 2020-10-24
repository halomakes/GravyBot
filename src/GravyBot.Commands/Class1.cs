using GravyIrc.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GravyBot.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RateLimitAttribute : Attribute
    {
        public RateLimitAttribute(double cooldownPeriod, TimeUnit cooldownUnit)
        {
            Func<double, TimeSpan> timeSpanInitializer = cooldownUnit switch
            {
                TimeUnit.Millisecond => TimeSpan.FromMilliseconds,
                TimeUnit.Second => TimeSpan.FromSeconds,
                TimeUnit.Minute => TimeSpan.FromMinutes,
                TimeUnit.Hour => TimeSpan.FromHours,
                TimeUnit.Day => TimeSpan.FromDays,
                _ => TimeSpan.FromSeconds
            };
            CooldownPeriod = timeSpanInitializer(cooldownPeriod);
        }

        public TimeSpan CooldownPeriod { get; private set; }
    }

    public enum TimeUnit
    {
        Millisecond,
        Second,
        Minute,
        Hour,
        Day
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {
        private static readonly Regex ParameterRgx = new Regex(@"\{([^\{\}]+)\}+", RegexOptions.Compiled); // params indicated with {paramName}

        public CommandAttribute(string commandFormat)
        {
            CommandFormat = commandFormat;
            ParseCommandFormat(commandFormat);
        }

        public string CommandFormat { get; private set; }

        public string Command { get; private set; }

        public IEnumerable<string> ParameterNames { get; private set; }

        public Regex MatchingPattern { get; private set; }

        private void ParseCommandFormat(string commandFormat)
        {
            var matches = ParameterRgx.Matches(commandFormat);
            ParameterNames = matches.Select(m => m.Groups[1].Value);

            var duplicateKey = ParameterNames.GroupBy(n => n.ToLower()).FirstOrDefault(g => g.Count() > 1);
            if (duplicateKey != default)
                throw new ArgumentException($"Multiple parameters specified with name ${duplicateKey.Key}");

            var paramReplacement = @"(.+)";
            var split = ParameterRgx.Split(commandFormat);
            var commandRgxSegments = split.Select(section => ParameterNames.Contains(section) ? paramReplacement : Regex.Escape(section));
            MatchingPattern = new Regex(string.Join(string.Empty, split));

            Command = commandFormat.Split(' ').First().Trim().ToLower();
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DirectOnlyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ChannelOnlyAttribute : Attribute { }

    public abstract class CommandProcessor
    {

    }

    public class CommandOrchestratorRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration configuration;
        private readonly IServiceProvider serviceProvider;

        public CommandOrchestratorRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider)
        {
            configuration = options.Value;
            this.serviceProvider = serviceProvider;
        }

        public bool Matches(PrivateMessage incomingMessage) => incomingMessage.Message.StartsWith(configuration.CommandPrefix);

        public IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            throw new NotImplementedException();
        }
    }
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
    public class CommandOrchestratorBuilder
    {
        private readonly Dictionary<string, CommandBinding> bindings = new Dictionary<string, CommandBinding>();

        public void RegisterProcessor<TProcessor>() where TProcessor : CommandProcessor
        {
            foreach (var method in typeof(TProcessor).GetMethods())
                foreach (var command in method.GetCommands())
                    RegisterCommand(command, method);
        }

        private void RegisterCommand(CommandAttribute command, MethodInfo method)
        {

        }

        private struct CommandBinding
        {
            public Type ProcessorType;
            public MethodInfo CommandMethod;
            public bool IsChannelOnly;
            public bool IsDirectOnly;
            public TimeSpan? RateLimitPeriod;
            public bool IsRateLimited => RateLimitPeriod.HasValue;
            public bool IsAsync;
        }
    }
    internal static class Extensions
    {
        public static IEnumerable<CommandAttribute> GetCommands(this MethodInfo method) => method.GetCustomAttributes<CommandAttribute>();
        public static bool IsCommand(this MethodInfo method) => method.GetCommands().Any();
    }
}

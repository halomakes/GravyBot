using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GravyBot.Commands
{
    public class CommandOrchestratorRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration configuration;
        private readonly IServiceProvider serviceProvider;
        private readonly CommandOrchestratorBuilder builder;
        private readonly Dictionary<UserInvocation, DateTime> InvocationHistory = new Dictionary<UserInvocation, DateTime>();

        public CommandOrchestratorRule(IOptions<IrcBotConfiguration> options, CommandOrchestratorBuilder builder, IServiceProvider serviceProvider)
        {
            configuration = options.Value;
            this.builder = builder;
            this.serviceProvider = serviceProvider;
        }

        public bool Matches(PrivateMessage incomingMessage)
        {
            var binding = GetBinding(incomingMessage.Message);
            return binding.HasValue && binding.Value.Value.Command.MatchingPattern.IsMatch(incomingMessage.Message);
        }

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var binding = GetBinding(incomingMessage.Message).Value.Value;

            bool hasMainResponseBlocked = false;

            if (binding.IsRateLimited)
            {
                var key = new UserInvocation(incomingMessage.From, incomingMessage.To, binding.Command.CommandName);
                var now = DateTime.Now;
                if (incomingMessage.IsChannelMessage && InvocationHistory.TryGetValue(key, out var previousInvocationTime))
                {
                    if (now - previousInvocationTime < binding.RateLimitPeriod)
                    {
                        hasMainResponseBlocked = true;
                        var remainingTime = binding.RateLimitPeriod.Value - (now - previousInvocationTime);
                        yield return new NoticeMessage(incomingMessage.From, $"You must wait {remainingTime} before using the {binding.Command.CommandName} command again.");
                    }
                }

                InvocationHistory[key] = now;
            }

            if (binding.IsChannelOnly && !incomingMessage.IsChannelMessage)
            {
                hasMainResponseBlocked = true;
                yield return new NoticeMessage(incomingMessage.From, $"The {binding.Command.CommandName} command can only be used in channels.");
            }

            if (binding.IsDirectOnly && incomingMessage.IsChannelMessage)
            {
                hasMainResponseBlocked = true;
                yield return new NoticeMessage(incomingMessage.From, $"The {binding.Command.CommandName} command can only be used in direct messages.");
            }

            if (!hasMainResponseBlocked)
            {
                if (binding.IsAsync)
                {
                    if (binding.ProducesMultipleResponses)
                        foreach (var r in Invoke<IEnumerable<IClientMessage>>(binding, incomingMessage))
                            yield return r;
                    else
                        yield return Invoke<IClientMessage>(binding, incomingMessage);
                }
                else
                {
                    if (binding.ProducesMultipleResponses)
                        await foreach (var r in Invoke<IAsyncEnumerable<IClientMessage>>(binding, incomingMessage))
                            yield return r;
                    else
                        yield return await Invoke<Task<IClientMessage>>(binding, incomingMessage);
                }
            }
        }

        private TResult Invoke<TResult>(CommandBinding binding, PrivateMessage message) =>
            (TResult)binding.Method.Invoke(GetProcessor(binding, message), GetArguments(binding, message.Message));

        private object[] GetArguments(CommandBinding binding, string message)
        {
            var match = binding.Command.MatchingPattern.Match(message);
            return binding.Method.GetParameters().Select(GetArgument).ToArray();

            object GetArgument(ParameterInfo paramInfo)
            {
                var indexInCommand = Array.IndexOf(binding.Command.ParameterNames, paramInfo.Name);
                if (indexInCommand > -1)
                {
                    var matchingGroup = match.Groups[indexInCommand];
                    if (matchingGroup.Success)
                    {
                        var converter = TypeDescriptor.GetConverter(paramInfo.ParameterType);
                        if (converter.CanConvertFrom(typeof(string)))
                        {
                            try
                            {
                                return converter.ConvertFromString(matchingGroup.Value);
                            }
                            catch (NotSupportedException)
                            {
                                return null;
                            }
                        }
                    }
                }
                return null;
            }
        }

        private CommandProcessor GetProcessor(CommandBinding binding, PrivateMessage message)
        {
            var processor = serviceProvider.GetService(binding.Method.ReflectedType) as CommandProcessor;
            processor.IncomingMessage = message;
            processor.TriggeringCommandName = binding.Command.CommandName;
            return processor;
        }

        private KeyValuePair<string, CommandBinding>? GetBinding(string message)
        {
            if (message.StartsWith(configuration.CommandPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var commandSegment = message.Remove(0, configuration.CommandPrefix.Length);
                return builder.Bindings.FirstOrDefault(p => commandSegment.StartsWith(p.Key, StringComparison.OrdinalIgnoreCase));
            }
            return null;
        }

        private struct UserInvocation
        {
            public UserInvocation(string nick, string channel, string command)
            {
                Nick = nick;
                Channel = channel;
                Command = command;
            }

            public string Nick;
            public string Channel;
            public string Command;
        }
    }
}

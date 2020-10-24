using GravyIrc.Messages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace GravyBot.Commands
{
    public struct CommandBinding
    {
        public MethodInfo Method;
        public bool IsChannelOnly;
        public bool IsDirectOnly;
        public TimeSpan? RateLimitPeriod;
        public bool IsRateLimited => RateLimitPeriod.HasValue;
        public bool IsAsync;
        public bool ProducesMultipleResponses;
        public CommandAttribute Command;

        public CommandBinding(CommandAttribute command, MethodInfo method)
        {
            if (!(typeof(Task<IClientMessage>).IsAssignableFrom(method.ReturnType) || typeof(IClientMessage).IsAssignableFrom(method.ReturnType) || typeof(IAsyncEnumerable<IClientMessage>).IsAssignableFrom(method.ReturnType) || typeof(IEnumerable<IClientMessage>).IsAssignableFrom(method.ReturnType)))
                throw new ArgumentException($"Method {method.ReflectedType.Name}.{method.Name} must return an IClientMessage, an IEnumerable of IClientMessage, or their async counterparts.");

            Method = method;
            IsChannelOnly = method.HasAttribute<ChannelOnlyAttribute>();
            IsDirectOnly = method.HasAttribute<DirectOnlyAttribute>();
            RateLimitPeriod = method.GetAttribute<RateLimitAttribute>()?.CooldownPeriod;
            IsAsync = typeof(Task<IClientMessage>).IsAssignableFrom(method.ReturnType) || typeof(IAsyncEnumerable<IClientMessage>).IsAssignableFrom(method.ReturnType);
            ProducesMultipleResponses = typeof(IEnumerable<IClientMessage>).IsAssignableFrom(method.ReturnType) || typeof(IAsyncEnumerable<IClientMessage>).IsAssignableFrom(method.ReturnType);
            Command = command;

            if (IsChannelOnly && IsDirectOnly)
                throw new ArgumentException($"A command cannot be limited to both only channels and only direct messages.");
        }
    }
}

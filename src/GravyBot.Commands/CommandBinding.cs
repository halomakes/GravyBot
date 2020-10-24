using GravyIrc.Messages;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace GravyBot.Commands
{
    /// <summary>
    /// Stores information about a command's binding
    /// </summary>
    public struct CommandBinding
    {
        /// <summary>
        /// Bound method
        /// </summary>
        public MethodInfo Method;

        /// <summary>
        /// Indicates if command can only be used in channels
        /// </summary>
        public bool IsChannelOnly;

        /// <summary>
        /// Indicates if command can only be used in direct messages
        /// </summary>
        public bool IsDirectOnly;

        /// <summary>
        /// Period to use for rate-limiting
        /// </summary>
        public TimeSpan? RateLimitPeriod;

        /// <summary>
        /// Indicates if rate-limiting is applied
        /// </summary>
        public bool IsRateLimited => RateLimitPeriod.HasValue;

        /// <summary>
        /// Indicates if method is ansynchronous
        /// </summary>
        public bool IsAsync;

        /// <summary>
        /// Indicates if method produces multiple messages
        /// </summary>
        public bool ProducesMultipleResponses;

        /// <summary>
        /// Name of applied channel policy
        /// </summary>
        public string PolicyName;

        /// <summary>
        /// Indicates if a channel policy is applied
        /// </summary>
        public bool HasPolicy => !string.IsNullOrEmpty(PolicyName);

        /// <summary>
        /// Original information about command
        /// </summary>
        public CommandAttribute Command;

        /// <summary>
        /// Create a command binding
        /// </summary>
        /// <param name="command">Command attribute of method</param>
        /// <param name="method">Bound method info</param>
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
            PolicyName = method.GetAttribute<ChannelPolicyAttribute>()?.PolicyName;

            if (IsChannelOnly && IsDirectOnly)
                throw new ArgumentException($"A command cannot be limited to both only channels and only direct messages.");
        }
    }
}

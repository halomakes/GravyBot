using System;

namespace GravyBot.Commands
{
    /// <summary>
    /// Indicates that a command can only be performed in channels
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ChannelOnlyAttribute : Attribute { }
}

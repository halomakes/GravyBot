using System;

namespace GravyBot.Commands
{
    /// <summary>
    /// Indicates that a command can only be used in direct messages
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DirectOnlyAttribute : Attribute { }
}

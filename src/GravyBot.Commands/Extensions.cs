using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GravyBot.Commands
{
    internal static class Extensions
    {
        public static IEnumerable<CommandAttribute> GetCommands(this MethodInfo method) => method.GetCustomAttributes<CommandAttribute>();
        public static bool IsCommand(this MethodInfo method) => method.GetCommands().Any();
        public static bool HasAttribute<TAttribute>(this MethodInfo method) where TAttribute : Attribute => method.GetCustomAttributes<TAttribute>().Any();
        public static TAttribute GetAttribute<TAttribute>(this MethodInfo method) where TAttribute : Attribute => method.GetCustomAttributes<TAttribute>().FirstOrDefault();
    }
}

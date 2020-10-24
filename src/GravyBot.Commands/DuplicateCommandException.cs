using System;
using System.Reflection;

namespace GravyBot.Commands
{
    public class DuplicateCommandException : Exception
    {
        public DuplicateCommandException(string commandName, MethodInfo attempted, MethodInfo conflicting)
            : base($"Could not register {attempted.ReflectedType.Name}.{attempted.Name} to {commandName} because it is already registered to {conflicting.ReflectedType.Name}.{conflicting.Name}")
        {
            AttemptedCommand = attempted;
            ConflictingCommand = conflicting;
        }

        public MethodInfo AttemptedCommand { get; private set; }
        public MethodInfo ConflictingCommand { get; private set; }
    }
}

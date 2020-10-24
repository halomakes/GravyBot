using System;

namespace GravyBot.Commands
{
    public class InvalidPolicyException : Exception
    {
        public InvalidPolicyException(string policyName) : base($"Policy {policyName} does not exist.") { }
    }
}

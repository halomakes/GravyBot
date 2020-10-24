using System;

namespace GravyBot.Commands
{
    /// <summary>
    /// Specifies a policy to apply to a command
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ChannelPolicyAttribute : Attribute
    {
        /// <summary>
        /// Apply a policy to a command
        /// </summary>
        /// <param name="policyName">Name of the policy</param>
        public ChannelPolicyAttribute(string policyName)
        {
            PolicyName = policyName;
        }

        /// <summary>
        /// Name of the policy
        /// </summary>
        /// <remarks>Must be registered to CommandOrchestratorBuilder</remarks>
        public string PolicyName { get; private set; }
    }
}

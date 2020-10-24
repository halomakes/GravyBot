using System;

namespace GravyBot.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ChannelPolicyAttribute : Attribute
    {
        public ChannelPolicyAttribute(string policyName)
        {
            PolicyName = policyName;
        }

        public string PolicyName { get; private set; }
    }
}

using System;

namespace GravyBot.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RateLimitAttribute : Attribute
    {
        public RateLimitAttribute(double cooldownPeriod, TimeUnit cooldownUnit)
        {
            Func<double, TimeSpan> timeSpanInitializer = cooldownUnit switch
            {
                TimeUnit.Millisecond => TimeSpan.FromMilliseconds,
                TimeUnit.Second => TimeSpan.FromSeconds,
                TimeUnit.Minute => TimeSpan.FromMinutes,
                TimeUnit.Hour => TimeSpan.FromHours,
                TimeUnit.Day => TimeSpan.FromDays,
                _ => TimeSpan.FromSeconds
            };
            CooldownPeriod = timeSpanInitializer(cooldownPeriod);
        }

        public TimeSpan CooldownPeriod { get; private set; }
    }
}

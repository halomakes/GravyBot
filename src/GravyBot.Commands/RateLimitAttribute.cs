using System;

namespace GravyBot.Commands
{
    /// <summary>
    /// Indicates that a command should be rate-limited
    /// </summary>
    /// <remarks>Cooldown is applied per-channel</remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RateLimitAttribute : Attribute
    {
        /// <summary>
        /// Indicate that a command should be rate-limited
        /// </summary>
        /// <param name="cooldownPeriod">Cooldown interval to require between invocations</param>
        /// <param name="cooldownUnit">Unit of time to use for period</param>
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

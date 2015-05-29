namespace Nagger.Services.ExtensionMethods
{
    using System;

    public static class TimeSpanExtensions
    {
        public static TimeSpan Ceiling(this TimeSpan original, TimeSpan ceiling)
        {
            if (original == ceiling || ceiling.Ticks == 0) return original;
            var ticks = (original.Ticks + ceiling.Ticks - 1)/ceiling.Ticks;
            return new TimeSpan(ticks * ceiling.Ticks);
        }
    }
}
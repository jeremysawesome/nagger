﻿namespace Nagger.Extensions
{
    using System;

    public static class TimeSpanExtensions
    {
        public static TimeSpan Ceiling(this TimeSpan original, TimeSpan ceiling)
        {
            if (original == ceiling || ceiling.Ticks == 0) return original;
            var ticks = (original.Ticks + ceiling.Ticks - 1)/ceiling.Ticks;
            return new TimeSpan(ticks*ceiling.Ticks);
        }

        public static TimeSpan Floor(this TimeSpan original, TimeSpan floor)
        {
            if (original == floor || floor.Ticks == 0) return original;
            var ticks = (original.Ticks/floor.Ticks);
            return new TimeSpan(ticks*floor.Ticks);
        }
    }
}
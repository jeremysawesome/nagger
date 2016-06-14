namespace Nagger.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        // see http://stackoverflow.com/a/38064/296889
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1*diff).Date;
        }
    }
}
namespace Nagger.Extensions
{
    using System;

    public static class StringExtensions
    {
        //see: http://stackoverflow.com/a/14826068/296889
        public static string ReplaceLastOccurrence(this string source, string find, string replace)
        {
            var place = source.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
                return string.Empty;

            var result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        public static string Truncate(this string source, int length, string ellipsis = "...")
        {
            if (source.Length > length)
            {
                source = source.Substring(0, length);
                source += ellipsis;
            }
            return source;
        }
    }
}
namespace ARMTokenApp.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    static class StringExtensions
    {
        public static string Indent(this string multiLineString, int numberOfSpaces)
        {
            return multiLineString
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => "".PadLeft(numberOfSpaces) + line)
                .ConcatStrings(Environment.NewLine);
        }

        public static string ConcatStrings(this IEnumerable<string> strings, string separator = "")
        {
            return strings.Any() ? strings.Aggregate((l, r) => l + separator + r) : string.Empty;
        }

        public static bool IsGuid(this string source)
        {
            Guid subscriptionIdGuid;
            return !string.IsNullOrEmpty(source) && Guid.TryParse(source, out subscriptionIdGuid);
        }
    }
}

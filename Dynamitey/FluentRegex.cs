using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dynamitey
{
    /// <summary>
    /// Extension Methods for fluent Regex
    /// </summary>
    public static class FluentRegex
    {
  
        public static IEnumerable<dynamic> FluentFilter(this IEnumerable<string> list, Regex regex)
        {
            return list.Select(it => regex.Match(it)).Where(it => it.Success).Select(it => new DynamicObjects.RegexMatch(it, regex)).Cast<dynamic>();
        }

        public static IEnumerable<dynamic> Matches(string inputString, Regex regex)
        {
            var tMatches = regex.Matches(inputString);

            return tMatches.Cast<Match>().Where(it => it.Success).Select(it => new DynamicObjects.RegexMatch(it, regex)).Cast<dynamic>();
        }

        public static dynamic Match(string inputString, Regex regex)
        {
            var tMatch = regex.Match(inputString);
            return tMatch.Success ? new DynamicObjects.RegexMatch(tMatch, regex) : null;
        }

        public static dynamic FluentMatch(this Regex regex, string inputString)
        {
            var tMatch = regex.Match(inputString);
            return tMatch.Success ? new DynamicObjects.RegexMatch(tMatch, regex) : null;
        }

        public static IEnumerable<dynamic> FluentMatches(this Regex regex, string inputString)
        {
            return Matches(inputString, regex);
        }

    }
}

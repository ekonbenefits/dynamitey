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

        /// <summary>
        /// Fluents the filter.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="regex">The regex.</param>
        /// <returns></returns>
        public static IEnumerable<dynamic> FluentFilter(this IEnumerable<string> list, Regex regex)
        {
            return list.Select(it => regex.Match(it)).Where(it => it.Success).Select(it => new DynamicObjects.RegexMatch(it, regex)).Cast<dynamic>();
        }

        /// <summary>
        /// Matcheses the specified input string.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="regex">The regex.</param>
        /// <returns></returns>
        public static IEnumerable<dynamic> Matches(string inputString, Regex regex)
        {
            var tMatches = regex.Matches(inputString);

            return tMatches.Cast<Match>().Where(it => it.Success).Select(it => new DynamicObjects.RegexMatch(it, regex)).Cast<dynamic>();
        }

        /// <summary>
        /// Matches the specified input string.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="regex">The regex.</param>
        /// <returns></returns>
        public static dynamic Match(string inputString, Regex regex)
        {
            var tMatch = regex.Match(inputString);
            return tMatch.Success ? new DynamicObjects.RegexMatch(tMatch, regex) : null;
        }

        /// <summary>
        /// Fluents the match.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="inputString">The input string.</param>
        /// <returns></returns>
        public static dynamic FluentMatch(this Regex regex, string inputString)
        {
            var tMatch = regex.Match(inputString);
            return tMatch.Success ? new DynamicObjects.RegexMatch(tMatch, regex) : null;
        }

        /// <summary>
        /// Fluents the matches.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="inputString">The input string.</param>
        /// <returns></returns>
        public static IEnumerable<dynamic> FluentMatches(this Regex regex, string inputString)
        {
            return Matches(inputString, regex);
        }

    }
}

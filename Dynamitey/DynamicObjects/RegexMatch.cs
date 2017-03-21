using System;
using System.Dynamic;
using System.Linq;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;

namespace Dynamitey.DynamicObjects
{

    /// <summary>
    /// A Regex Match Interface
    /// </summary>
    public interface IRegexMatch
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        string Value { get;}
    }



    /// <summary>
    /// A Dynamic Regex Match
    /// </summary>
    public class RegexMatch : BaseObject, IRegexMatch
    {
       
        private readonly Match _match;
       
        private readonly Regex _regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatch" /> class.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <param name="regex">The regex.</param>
        public RegexMatch(Match match, Regex regex = null)
        {
            _match = match;
            _regex = regex;
        }


        /// <summary>
        /// Gets the dynamic member names.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (_regex == null)
                return Enumerable.Empty<string>();
            return _regex.GetGroupNames();
        }

        /// <summary>
        /// Tries the get member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
       public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var tGroup = _match.Groups[binder.Name];
            Type outType;
            if (!TryTypeForName(binder.Name, out outType))
                outType = typeof (string);

            if (!tGroup.Success)
            {
                result = null;
                if (outType.GetTypeInfo().IsValueType)
                    result = Dynamic.InvokeConstructor(outType);
                return true;
            }

            result = Dynamic.CoerceConvert(tGroup.Value, outType);
            return true;
        }

       /// <summary>
       /// Gets the <see cref="System.String" /> with the specified value.
       /// </summary>
       /// <value>
       /// The <see cref="System.String" />.
       /// </value>
       /// <param name="value">The value.</param>
       /// <returns></returns>
        public string this[int value]
        {
            get
            {
                var tGroup = _match.Groups[value];

                if (!tGroup.Success)
                {
                    return null;
                }
                return tGroup.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified value.
        /// </summary>
        /// <value>
        /// The <see cref="System.String" />.
        /// </value>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string this[string value]
        {
            get
            {
                var tGroup = _match.Groups[value];

                if (!tGroup.Success)
                {
                    return null;
                }
                return tGroup.Value;
            }
        }

        string IRegexMatch.Value
        {
            get { return _match.Value; }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _match.ToString();
        }
    }
}

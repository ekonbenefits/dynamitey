using System;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Dynamitey.DynamicObjects
{

    public interface IRegexMatch
    {
        string Value { get;}
    }

  
    public class RegexMatch : BaseObject, IRegexMatch
    {
        private readonly Match _match;
        private readonly Regex _regex;

        public RegexMatch(Match match, Regex regex = null)
        {
            _match = match;
            _regex = regex;
        }


        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (_regex == null)
                return Enumerable.Empty<string>();
            return _regex.GetGroupNames();
        }

      /*  public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var tGroup = _match.Groups[binder.Name];
            Type outType;
            if (!TryTypeForName(binder.Name, out outType))
                outType = typeof (string);

            if (!tGroup.Success)
            {
                result = null;
                if (outType.IsValueType)
                    result = Impromptu.InvokeConstructor(outType);
                return true;
            }

            result = Impromptu.CoerceConvert(tGroup.Value, outType);
            return true;
        }*/

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

        public override string ToString()
        {
            return _match.ToString();
        }
    }
}

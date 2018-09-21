using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace iText.Svg.Utils
{
    public class SvgRegexUtils
    {
        /// <summary>
        /// Use the passed pattern to check if there's at least one match in the passed string
        /// </summary>
        /// <param name="pattern">regex pattern that describes the match</param>
        /// <param name="stringToExamine">string to search matches in</param>
        /// <returns>True if there's at least one match in the string, false otherwise</returns>
        public static Boolean ContainsAtLeastOneMatch(Regex pattern, String stringToExamine) {
            Match match = pattern.Match(stringToExamine);
            return match.Success;
        }
    }
}

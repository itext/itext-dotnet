/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2025 Apryse Group NV
    Authors: Apryse Software.

    This program is offered under a commercial and under the AGPL license.
    For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

    AGPL licensing:
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace iText.Commons.Utils {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class StringUtil {
        public static String ReplaceAll(String srcString, String regex, String replacement) {
            return Regex.Replace(srcString, regex, replacement);
        }

        public static Regex RegexCompile(String s) {
            return RegexCompile(s, RegexOptions.None);
        }

        public static Regex RegexCompile(String s, RegexOptions options) {
            Regex regex = new Regex(s, options);
            //This is needed so the method throw an exception in case of invalid regular expression.
            regex.IsMatch("");
            return regex;
        }

        [Obsolete]
        public static Match Match(Regex r, String s) {
            return r.Match(s);
        }
        
        [Obsolete]
        public static String Group(Match match, int index) {
            return match.Groups[index].Success ? match.Groups[index].Value : null;
        }

        [Obsolete]
        public static String Group(Match match) {
            return Group(match, 0);
        }

        public static String Normalize(String s, NormalizationForm form) {
            return s.Normalize(form);
        }

        public static String[] Split(String srcStr, String splitSequence) {
            if (splitSequence.Length == 1)
                return srcStr.TrimEnd().Split(splitSequence.ToCharArray());

            Regex regex = new Regex(splitSequence);
            return Split(regex, srcStr);
        }

        public static String[] Split(Regex regex, String srcStr) {
            Match match = regex.Match(srcStr);
            String[] result;
            if (!match.Success)
            {
                result = new String[1];
                result[0] = srcStr;
                return result;
            }
            List<String> al = new List<String>();
            int prevat = 0;

            for (; ; )
            {
                if (match.Index != 0)
                    al.Add(srcStr.Substring(prevat, match.Index - prevat));
                prevat = match.Index + match.Length;
                match = match.NextMatch();
                if (!match.Success)
                    break;
            }
            if (prevat != srcStr.Length)
                al.Add(srcStr.Substring(prevat, srcStr.Length - prevat));
            return al.ToArray();
        }
    }
}

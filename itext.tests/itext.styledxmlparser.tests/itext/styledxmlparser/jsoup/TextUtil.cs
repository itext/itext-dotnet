/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Text.RegularExpressions;

namespace iText.StyledXmlParser.Jsoup {
    /// <summary>Text utils to ease testing</summary>
    public class TextUtil {
        internal static Regex stripper = iText.Commons.Utils.StringUtil.RegexCompile("\\r?\\n\\s*");

        public static String StripNewlines(String text)
        {
            return stripper.Replace(text, "");
        }
    }
}

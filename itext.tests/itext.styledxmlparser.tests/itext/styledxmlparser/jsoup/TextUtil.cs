/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;

namespace iText.StyledXmlParser.Jsoup {
    /// <summary>Text utils to ease testing</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class TextUtil {
        public static String StripNewlines(String text) {
            text = iText.IO.Util.StringUtil.ReplaceAll(text, "\\n\\s*", "");
            return text;
        }
    }
}

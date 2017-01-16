using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iText.Layout.Font {
    internal sealed class FontFamilySplitter {
        private static readonly Regex FONT_FAMILY_PATTERN = iText.IO.Util.StringUtil.RegexCompile("^ *(\\w+) *$");

        private static readonly Regex FONT_FAMILY_PATTERN_QUOTED = iText.IO.Util.StringUtil.RegexCompile("^ *(('[\\w ]+')|(\"[\\w ]+\")) *$"
            );

        private static readonly Regex FONT_FAMILY_PATTERN_QUOTED_SELECT = iText.IO.Util.StringUtil.RegexCompile("\\w+( +\\w+)*"
            );

        public static IList<String> SplitFontFamily(String fontFamily) {
            String[] names = iText.IO.Util.StringUtil.Split(fontFamily, ",");
            IList<String> result = new List<String>(names.Length);
            foreach (String name in names) {
                if (iText.IO.Util.StringUtil.Match(FONT_FAMILY_PATTERN, name).Success) {
                    result.Add(name.Trim());
                }
                else {
                    if (iText.IO.Util.StringUtil.Match(FONT_FAMILY_PATTERN_QUOTED, name).Success) {
                        Match selectMatcher = iText.IO.Util.StringUtil.Match(FONT_FAMILY_PATTERN_QUOTED_SELECT, name);
                        if (selectMatcher.Success) {
                            result.Add(iText.IO.Util.StringUtil.Group(selectMatcher));
                        }
                    }
                }
            }
            return result;
        }
    }
}

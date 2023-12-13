/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using iText.IO.Util;

namespace iText.Layout.Font {
    /// <summary>Split CSS 'font-family' string into list of font-families or generic-families</summary>
    [System.ObsoleteAttribute(@"will be moved to styled-xml-parser module in iText 7.2.")]
    public sealed class FontFamilySplitter {
        private static readonly Regex FONT_FAMILY_PATTERN = iText.IO.Util.StringUtil.RegexCompile("^ *([\\w-]+) *$"
            );

        private static readonly Regex FONT_FAMILY_PATTERN_QUOTED = iText.IO.Util.StringUtil.RegexCompile("^ *(('[\\w -]+')|(\"[\\w -]+\")) *$"
            );

        private static readonly Regex FONT_FAMILY_PATTERN_QUOTED_SELECT = iText.IO.Util.StringUtil.RegexCompile("[\\w-]+( +[\\w-]+)*"
            );

        public static IList<String> SplitFontFamily(String fontFamilies) {
            if (fontFamilies == null) {
                return null;
            }
            String[] names = iText.IO.Util.StringUtil.Split(fontFamilies, ",");
            IList<String> result = new List<String>(names.Length);
            foreach (String name in names) {
                // TODO DEVSIX-2534 improve pattern matching according to CSS specification. E.g. unquoted font-families with spaces.
                if (iText.IO.Util.Matcher.Match(FONT_FAMILY_PATTERN, name).Matches()) {
                    result.Add(name.Trim());
                }
                else {
                    if (iText.IO.Util.Matcher.Match(FONT_FAMILY_PATTERN_QUOTED, name).Matches()) {
                        Matcher selectMatcher = iText.IO.Util.Matcher.Match(FONT_FAMILY_PATTERN_QUOTED_SELECT, name);
                        if (selectMatcher.Find()) {
                            result.Add(selectMatcher.Group());
                        }
                    }
                }
            }
            return result;
        }

        public static String RemoveQuotes(String fontFamily) {
            Matcher selectMatcher = iText.IO.Util.Matcher.Match(FONT_FAMILY_PATTERN_QUOTED_SELECT, fontFamily);
            if (selectMatcher.Find()) {
                return selectMatcher.Group();
            }
            return null;
        }
    }
}

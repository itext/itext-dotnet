/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Utils;

namespace iText.StyledXmlParser.Util {
    /// <summary>Split CSS 'font-family' string into list of font-families or generic-families</summary>
    public sealed class FontFamilySplitterUtil {
        private static readonly Regex FONT_FAMILY_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile("^([\\w-]+)$"
            );

        private static readonly Regex FONT_FAMILY_PATTERN_QUOTED = iText.Commons.Utils.StringUtil.RegexCompile("^(('[\\w -]+')|(\"[\\w -]+\"))$"
            );

        private static readonly Regex FONT_FAMILY_PATTERN_QUOTED_SELECT = iText.Commons.Utils.StringUtil.RegexCompile
            ("([\\w -]+)");

        public static IList<String> SplitFontFamily(String fontFamilies) {
            if (fontFamilies == null) {
                return null;
            }
            String[] names = iText.Commons.Utils.StringUtil.Split(fontFamilies, ",");
            IList<String> result = new List<String>(names.Length);
            foreach (String name in names) {
                String trimmedName = name.Trim();
                // TODO DEVSIX-2534 improve pattern matching according to CSS specification. E.g. unquoted font-families with spaces.
                if (iText.Commons.Utils.Matcher.Match(FONT_FAMILY_PATTERN, trimmedName).Matches()) {
                    result.Add(trimmedName);
                }
                else {
                    if (iText.Commons.Utils.Matcher.Match(FONT_FAMILY_PATTERN_QUOTED, trimmedName).Matches()) {
                        Matcher selectMatcher = iText.Commons.Utils.Matcher.Match(FONT_FAMILY_PATTERN_QUOTED_SELECT, trimmedName);
                        if (selectMatcher.Find()) {
                            result.Add(selectMatcher.Group());
                        }
                    }
                }
            }
            return result;
        }

        public static String RemoveQuotes(String fontFamily) {
            Matcher selectMatcher = iText.Commons.Utils.Matcher.Match(FONT_FAMILY_PATTERN_QUOTED_SELECT, fontFamily);
            if (selectMatcher.Find()) {
                return selectMatcher.Group();
            }
            return null;
        }
    }
}

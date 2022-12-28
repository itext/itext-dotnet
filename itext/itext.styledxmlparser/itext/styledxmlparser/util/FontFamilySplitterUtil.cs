/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using iText.Commons.Utils;

namespace iText.StyledXmlParser.Util {
    /// <summary>Split CSS 'font-family' string into list of font-families or generic-families</summary>
    public sealed class FontFamilySplitterUtil {
        private static readonly Regex FONT_FAMILY_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile("^ *([\\w-]+) *$"
            );

        private static readonly Regex FONT_FAMILY_PATTERN_QUOTED = iText.Commons.Utils.StringUtil.RegexCompile("^ *(('[\\w -]+')|(\"[\\w -]+\")) *$"
            );

        private static readonly Regex FONT_FAMILY_PATTERN_QUOTED_SELECT = iText.Commons.Utils.StringUtil.RegexCompile
            ("[\\w-]+( +[\\w-]+)*");

        public static IList<String> SplitFontFamily(String fontFamilies) {
            if (fontFamilies == null) {
                return null;
            }
            String[] names = iText.Commons.Utils.StringUtil.Split(fontFamilies, ",");
            IList<String> result = new List<String>(names.Length);
            foreach (String name in names) {
                // TODO DEVSIX-2534 improve pattern matching according to CSS specification. E.g. unquoted font-families with spaces.
                if (iText.Commons.Utils.Matcher.Match(FONT_FAMILY_PATTERN, name).Matches()) {
                    result.Add(name.Trim());
                }
                else {
                    if (iText.Commons.Utils.Matcher.Match(FONT_FAMILY_PATTERN_QUOTED, name).Matches()) {
                        Matcher selectMatcher = iText.Commons.Utils.Matcher.Match(FONT_FAMILY_PATTERN_QUOTED_SELECT, name);
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

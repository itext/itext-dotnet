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
using System.Text;
using iText.Test;

namespace iText.StyledXmlParser.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class FontFamilySplitterTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void FontFamilySplitter() {
            String fontFamilies = "'Puritan'\n" + "Puritan\n" + "'Pur itan'\n" + "Pur itan\n" + "'Pur it an'\n" + "Pur it an\n"
                 + "   \"Puritan\"\n" + "Puritan\n" + "  \"Pur itan\"\n" + "Pur itan\n" + "\"Pur it an\"\n" + "Pur it an\n"
                 + "FreeSans\n" + "FreeSans\n" + "'Puritan', FreeSans\n" + "Puritan; FreeSans\n" + "'Pur itan' , FreeSans\n"
                 + "Pur itan; FreeSans\n" + "   'Pur it an'  ,  FreeSans   \n" + "Pur it an; FreeSans\n" + "\"Puritan\", FreeSans\n"
                 + "Puritan; FreeSans\n" + "\"Pur itan\", FreeSans\n" + "Pur itan; FreeSans\n" + "\"Pur it an\", FreeSans\n"
                 + "Pur it an; FreeSans\n" + "\"Puritan\"\n" + "Puritan\n" + "'Free Sans',\n" + "Free Sans\n" + "'Free-Sans',\n"
                 + "Free-Sans\n" + "  'Free-Sans' , Puritan\n" + "Free-Sans; Puritan\n" + "  \"Free-Sans\" , Puritan\n"
                 + "Free-Sans; Puritan\n" + "  Free-Sans , Puritan\n" + "Free-Sans; Puritan\n" + "  Free-Sans\n" + "Free-Sans\n"
                 + "\"Puritan\", Free Sans\n" + "Puritan\n" + "\"Puritan 2.0\"\n" + "-\n" + "'Puritan' FreeSans\n" + "-\n"
                 + "Pur itan\n" + "-\n" + "Pur it an\"\n" + "-\n" + "\"Free Sans\n" + "-\n" + "Pur it an'\n" + "-\n" +
                 "'Free Sans\n" + "-";
            String[] splitFontFamilies = iText.Commons.Utils.StringUtil.Split(fontFamilies, "\n");
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < splitFontFamilies.Length; i += 2) {
                IList<String> fontFamily = FontFamilySplitterUtil.SplitFontFamily(splitFontFamilies[i]);
                result.Length = 0;
                foreach (String ff in fontFamily) {
                    result.Append(ff).Append("; ");
                }
                NUnit.Framework.Assert.AreEqual(splitFontFamilies[i + 1], result.Length > 2 ? result.JSubstring(0, result.
                    Length - 2) : "-");
            }
        }
    }
}

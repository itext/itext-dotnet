/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout.Element;

namespace iText.Layout.Testutil {
    public sealed class TestResourceUtil {
        private TestResourceUtil() {
        }

        /// <summary>Returns a Byron stanza string.</summary>
        public static String GetByronStanza() {
            return "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
        }

        /// <summary>Returns a narrow Byron stanza string.</summary>
        public static String GetByronStanzaNarrow() {
            return "When a man hath no freedom to fight for at home, " + "Let him combat for that of his neighbours; "
                 + "Let him think of the glories of Greece and of Rome, " + "And get knocked on the head for his labours. "
                 + "\n" + "To do good to Mankind is the chivalrous plan, " + "And is always as nobly requited; " + "Then battle for Freedom wherever you can, "
                 + "And, if not shot or hanged, you'll get knighted.";
        }

        /// <summary>Returns a tall div element.</summary>
        /// <param name="paragraphCount">count of paragraphs</param>
        /// <returns>resulting div element</returns>
        public static Div GetTallDiv(int paragraphCount) {
            Div div = new Div().SetBackgroundColor(new DeviceRgb(78, 151, 205));
            for (int i = 0; i < paragraphCount; i++) {
                div.Add(new Paragraph("BLOCK " + i + "\n" + iText.Layout.Testutil.TestResourceUtil.GetByronStanza()));
            }
            return div;
        }

        /// <summary>Repeats a string N times and returns result</summary>
        /// <param name="s">string</param>
        /// <param name="n">number of repeats</param>
        /// <returns>resulting string</returns>
        public static String RepeatString(String s, int n) {
            StringBuilder sb = new StringBuilder(s.Length * n);
            for (int i = 0; i < n; i++) {
                sb.Append(s);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Calculates the available content rectangle after subtracting the document margins
        /// and the given offsets on each side.
        /// </summary>
        /// <param name="docHeight">the full page height</param>
        /// <param name="docWidth">the full page width</param>
        /// <param name="docMargin">the margin for the doc</param>
        /// <param name="top">the offset to subtract from the top</param>
        /// <param name="bottom">the offset to subtract from the bottom</param>
        /// <param name="left">the offset to subtract from the left</param>
        /// <param name="right">the offset to subtract from the right</param>
        /// <returns>the remaining Rectangle available for content layout</returns>
        public static Rectangle GetAvailableRect(float docHeight, float docWidth, float docMargin, float top, float
             bottom, float left, float right) {
            float x = docMargin + left;
            float y = docMargin + bottom;
            float w = docWidth - 2 * docMargin - left - right;
            float h = docHeight - 2 * docMargin - top - bottom;
            return new Rectangle(x, y, Math.Max(w, 1f), Math.Max(h, 1f));
        }
    }
}

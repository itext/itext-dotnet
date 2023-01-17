/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("UnitTest")]
    public class GlyphLineTest : ExtendedITextTest {
        private static IList<Glyph> ConstructGlyphListFromString(String text, TrueTypeFont font) {
            IList<Glyph> glyphList = new List<Glyph>();
            char[] chars = text.ToCharArray();
            foreach (char letter in chars) {
                glyphList.Add(font.GetGlyph(letter));
            }
            return glyphList;
        }

        [NUnit.Framework.Test]
        public virtual void TestEquals() {
            Glyph glyph = new Glyph(200, 200, 200);
            GlyphLine.ActualText actualText = new GlyphLine.ActualText("-");
            GlyphLine one = new GlyphLine(new List<Glyph>(JavaUtil.ArraysAsList(glyph)), new List<GlyphLine.ActualText
                >(JavaUtil.ArraysAsList(actualText)), 0, 1);
            GlyphLine two = new GlyphLine(new List<Glyph>(JavaUtil.ArraysAsList(glyph)), new List<GlyphLine.ActualText
                >(JavaUtil.ArraysAsList(actualText)), 0, 1);
            one.Add(glyph);
            two.Add(glyph);
            one.end++;
            two.end++;
            NUnit.Framework.Assert.IsTrue(one.Equals(two));
        }

        [NUnit.Framework.Test]
        public virtual void TestOtherLinesAddition() {
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSans.ttf", FileMode.Open, FileAccess.Read
                ));
            TrueTypeFont font = new TrueTypeFont(ttf);
            GlyphLine containerLine = new GlyphLine(ConstructGlyphListFromString("Viva France!", font));
            GlyphLine childLine1 = new GlyphLine(ConstructGlyphListFromString(" Liberte", font));
            containerLine.Add(childLine1);
            NUnit.Framework.Assert.AreEqual(containerLine.end, 12);
            containerLine.end = 20;
            GlyphLine childLine2 = new GlyphLine(ConstructGlyphListFromString(" Egalite", font));
            containerLine.Add(childLine2);
            NUnit.Framework.Assert.AreEqual(containerLine.end, 20);
            containerLine.start = 10;
            GlyphLine childLine3 = new GlyphLine(ConstructGlyphListFromString(" Fraternite", font));
            containerLine.Add(childLine3);
            NUnit.Framework.Assert.AreEqual(containerLine.start, 10);
            containerLine.start = 0;
            containerLine.Add(ConstructGlyphListFromString("!", font)[0]);
            containerLine.end = 40;
            NUnit.Framework.Assert.AreEqual(containerLine.glyphs.Count, 40);
        }

        [NUnit.Framework.Test]
        public virtual void TestAdditionWithActualText() {
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSans.ttf", FileMode.Open, FileAccess.Read
                ));
            TrueTypeFont font = new TrueTypeFont(ttf);
            IList<Glyph> glyphs = ConstructGlyphListFromString("Viva France!", font);
            GlyphLine containerLine = new GlyphLine(glyphs);
            NUnit.Framework.Assert.IsNull(containerLine.actualText);
            containerLine.SetActualText(0, 1, "TEST");
            NUnit.Framework.Assert.IsNotNull(containerLine.actualText);
            NUnit.Framework.Assert.AreEqual(12, containerLine.actualText.Count);
            NUnit.Framework.Assert.AreEqual("TEST", containerLine.actualText[0].value);
            containerLine.Add(new GlyphLine(glyphs));
            NUnit.Framework.Assert.AreEqual(24, containerLine.actualText.Count);
            for (int i = 13; i < 24; i++) {
                NUnit.Framework.Assert.IsNull(containerLine.actualText[i]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestOtherLinesWithActualTextAddition() {
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSans.ttf", FileMode.Open, FileAccess.Read
                ));
            TrueTypeFont font = new TrueTypeFont(ttf);
            GlyphLine containerLine = new GlyphLine(ConstructGlyphListFromString("France", font));
            GlyphLine childLine = new GlyphLine(ConstructGlyphListFromString("---Liberte", font));
            childLine.SetActualText(3, 10, "Viva");
            containerLine.Add(childLine);
            containerLine.end = 16;
            for (int i = 0; i < 9; i++) {
                NUnit.Framework.Assert.IsNull(containerLine.actualText[i]);
            }
            for (int i = 9; i < 16; i++) {
                NUnit.Framework.Assert.AreEqual("Viva", containerLine.actualText[i].value);
            }
            NUnit.Framework.Assert.AreEqual("France---Viva", containerLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestOtherLinesWithActualTextAddition02() {
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSans.ttf", FileMode.Open, FileAccess.Read
                ));
            TrueTypeFont font = new TrueTypeFont(ttf);
            GlyphLine containerLine = new GlyphLine(ConstructGlyphListFromString("France", font));
            containerLine.SetActualText(1, 5, "id");
            GlyphLine childLine = new GlyphLine(ConstructGlyphListFromString("---Liberte", font));
            childLine.SetActualText(3, 10, "Viva");
            containerLine.Add(childLine);
            containerLine.end = 16;
            NUnit.Framework.Assert.IsNull(containerLine.actualText[0]);
            for (int i = 1; i < 5; i++) {
                NUnit.Framework.Assert.AreEqual("id", containerLine.actualText[i].value);
            }
            for (int i = 5; i < 9; i++) {
                NUnit.Framework.Assert.IsNull(containerLine.actualText[i]);
            }
            for (int i = 9; i < 16; i++) {
                NUnit.Framework.Assert.AreEqual("Viva", containerLine.actualText[i].value);
            }
            NUnit.Framework.Assert.AreEqual("Fide---Viva", containerLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestContentReplacingWithNullActualText() {
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSans.ttf", FileMode.Open, FileAccess.Read
                ));
            TrueTypeFont font = new TrueTypeFont(ttf);
            GlyphLine lineToBeReplaced = new GlyphLine(ConstructGlyphListFromString("Byelorussia", font));
            lineToBeReplaced.SetActualText(1, 2, "e");
            GlyphLine lineToBeCopied = new GlyphLine(ConstructGlyphListFromString("Belarus", font));
            lineToBeReplaced.ReplaceContent(lineToBeCopied);
            // Test that no exception has been thrown. Also check the content.
            NUnit.Framework.Assert.AreEqual("Belarus", lineToBeReplaced.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestActualTextForSubstitutedGlyphProcessingInSubstituteOneToMany01() {
            String expectedActualTextForFirstGlyph = "0";
            String expectedActualTextForSecondGlyph = "A";
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSans.ttf", FileMode.Open, FileAccess.Read
                ));
            TrueTypeFont font = new TrueTypeFont(ttf);
            // no actual text for the second glyph is set - it should be created during substitution
            GlyphLine line = new GlyphLine(ConstructGlyphListFromString("AA", font));
            line.SetActualText(0, 1, expectedActualTextForFirstGlyph);
            line.idx = 1;
            line.SubstituteOneToMany(font.GetGsubTable(), new int[] { 39, 40 });
            NUnit.Framework.Assert.IsNotNull(line.actualText);
            NUnit.Framework.Assert.AreEqual(3, line.actualText.Count);
            NUnit.Framework.Assert.AreSame(line.actualText[1], line.actualText[2]);
            NUnit.Framework.Assert.AreEqual(expectedActualTextForSecondGlyph, line.actualText[1].value);
            // check that it hasn't been corrupted
            NUnit.Framework.Assert.AreEqual(expectedActualTextForFirstGlyph, line.actualText[0].value);
        }

        [NUnit.Framework.Test]
        public virtual void TestActualTextForSubstitutedGlyphProcessingInSubstituteOneToMany02() {
            String expectedActualTextForFirstGlyph = "A";
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSans.ttf", FileMode.Open, FileAccess.Read
                ));
            TrueTypeFont font = new TrueTypeFont(ttf);
            GlyphLine line = new GlyphLine(ConstructGlyphListFromString("A", font));
            line.SetActualText(0, 1, expectedActualTextForFirstGlyph);
            line.SubstituteOneToMany(font.GetGsubTable(), new int[] { 39, 40 });
            NUnit.Framework.Assert.IsNotNull(line.actualText);
            NUnit.Framework.Assert.AreEqual(2, line.actualText.Count);
            NUnit.Framework.Assert.AreSame(line.actualText[0], line.actualText[1]);
            NUnit.Framework.Assert.AreEqual(expectedActualTextForFirstGlyph, line.actualText[0].value);
        }

        [NUnit.Framework.Test]
        public virtual void TestActualTextForSubstitutedGlyphProcessingInSubstituteOneToMany03() {
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSans.ttf", FileMode.Open, FileAccess.Read
                ));
            TrueTypeFont font = new TrueTypeFont(ttf);
            // no actual text is set
            GlyphLine line = new GlyphLine(ConstructGlyphListFromString("A", font));
            line.SubstituteOneToMany(font.GetGsubTable(), new int[] { 39, 40 });
            NUnit.Framework.Assert.IsNull(line.actualText);
        }
    }
}

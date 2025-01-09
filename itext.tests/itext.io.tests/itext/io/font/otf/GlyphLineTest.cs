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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("UnitTest")]
    public class GlyphLineTest : ExtendedITextTest {
        public static readonly String FREESANS_FONT_PATH = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSans.ttf";

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
            one.SetEnd(one.GetEnd() + 1);
            two.SetEnd(two.GetEnd() + 1);
            NUnit.Framework.Assert.IsTrue(one.Equals(two));
        }

        [NUnit.Framework.Test]
        public virtual void TestOtherLinesAddition() {
            TrueTypeFont font = InitializeFont();
            GlyphLine containerLine = new GlyphLine(ConstructGlyphListFromString("Viva France!", font));
            GlyphLine childLine1 = new GlyphLine(ConstructGlyphListFromString(" Liberte", font));
            containerLine.Add(childLine1);
            NUnit.Framework.Assert.AreEqual(12, containerLine.GetEnd());
            containerLine.SetEnd(20);
            GlyphLine childLine2 = new GlyphLine(ConstructGlyphListFromString(" Egalite", font));
            containerLine.Add(childLine2);
            NUnit.Framework.Assert.AreEqual(20, containerLine.GetEnd());
            containerLine.SetStart(10);
            GlyphLine childLine3 = new GlyphLine(ConstructGlyphListFromString(" Fraternite", font));
            containerLine.Add(childLine3);
            NUnit.Framework.Assert.AreEqual(10, containerLine.GetStart());
            containerLine.SetStart(0);
            containerLine.Add(ConstructGlyphListFromString("!", font)[0]);
            containerLine.SetEnd(40);
            NUnit.Framework.Assert.AreEqual(40, containerLine.glyphs.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestAdditionWithActualText() {
            TrueTypeFont font = InitializeFont();
            IList<Glyph> glyphs = ConstructGlyphListFromString("Viva France!", font);
            GlyphLine containerLine = new GlyphLine(glyphs);
            NUnit.Framework.Assert.IsNull(containerLine.actualText);
            containerLine.SetActualText(0, 1, "TEST");
            NUnit.Framework.Assert.IsNotNull(containerLine.actualText);
            NUnit.Framework.Assert.AreEqual(12, containerLine.actualText.Count);
            NUnit.Framework.Assert.AreEqual("TEST", containerLine.actualText[0].GetValue());
            containerLine.Add(new GlyphLine(glyphs));
            NUnit.Framework.Assert.AreEqual(24, containerLine.actualText.Count);
            for (int i = 13; i < 24; i++) {
                NUnit.Framework.Assert.IsNull(containerLine.actualText[i]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestOtherLinesWithActualTextAddition() {
            TrueTypeFont font = InitializeFont();
            GlyphLine containerLine = new GlyphLine(ConstructGlyphListFromString("France", font));
            GlyphLine childLine = new GlyphLine(ConstructGlyphListFromString("---Liberte", font));
            childLine.SetActualText(3, 10, "Viva");
            containerLine.Add(childLine);
            containerLine.SetEnd(16);
            for (int i = 0; i < 9; i++) {
                NUnit.Framework.Assert.IsNull(containerLine.actualText[i]);
            }
            for (int i = 9; i < 16; i++) {
                NUnit.Framework.Assert.AreEqual("Viva", containerLine.actualText[i].GetValue());
            }
            NUnit.Framework.Assert.AreEqual("France---Viva", containerLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestOtherLinesWithActualTextAddition02() {
            TrueTypeFont font = InitializeFont();
            GlyphLine containerLine = new GlyphLine(ConstructGlyphListFromString("France", font));
            containerLine.SetActualText(1, 5, "id");
            GlyphLine childLine = new GlyphLine(ConstructGlyphListFromString("---Liberte", font));
            childLine.SetActualText(3, 10, "Viva");
            containerLine.Add(childLine);
            containerLine.SetEnd(16);
            NUnit.Framework.Assert.IsNull(containerLine.actualText[0]);
            for (int i = 1; i < 5; i++) {
                NUnit.Framework.Assert.AreEqual("id", containerLine.actualText[i].GetValue());
            }
            for (int i = 5; i < 9; i++) {
                NUnit.Framework.Assert.IsNull(containerLine.actualText[i]);
            }
            for (int i = 9; i < 16; i++) {
                NUnit.Framework.Assert.AreEqual("Viva", containerLine.actualText[i].GetValue());
            }
            NUnit.Framework.Assert.AreEqual("Fide---Viva", containerLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestContentReplacingWithNullActualText() {
            TrueTypeFont font = InitializeFont();
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
            TrueTypeFont font = InitializeFont();
            // no actual text for the second glyph is set - it should be created during substitution
            GlyphLine line = new GlyphLine(ConstructGlyphListFromString("AA", font));
            line.SetActualText(0, 1, expectedActualTextForFirstGlyph);
            line.SetIdx(1);
            line.SubstituteOneToMany(font.GetGsubTable(), new int[] { 39, 40 });
            NUnit.Framework.Assert.IsNotNull(line.actualText);
            NUnit.Framework.Assert.AreEqual(3, line.actualText.Count);
            NUnit.Framework.Assert.AreSame(line.actualText[1], line.actualText[2]);
            NUnit.Framework.Assert.AreEqual(expectedActualTextForSecondGlyph, line.actualText[1].GetValue());
            // check that it hasn't been corrupted
            NUnit.Framework.Assert.AreEqual(expectedActualTextForFirstGlyph, line.actualText[0].GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void TestActualTextForSubstitutedGlyphProcessingInSubstituteOneToMany02() {
            String expectedActualTextForFirstGlyph = "A";
            TrueTypeFont font = InitializeFont();
            GlyphLine line = new GlyphLine(ConstructGlyphListFromString("A", font));
            line.SetActualText(0, 1, expectedActualTextForFirstGlyph);
            line.SubstituteOneToMany(font.GetGsubTable(), new int[] { 39, 40 });
            NUnit.Framework.Assert.IsNotNull(line.actualText);
            NUnit.Framework.Assert.AreEqual(2, line.actualText.Count);
            NUnit.Framework.Assert.AreSame(line.actualText[0], line.actualText[1]);
            NUnit.Framework.Assert.AreEqual(expectedActualTextForFirstGlyph, line.actualText[0].GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void TestActualTextForSubstitutedGlyphProcessingInSubstituteOneToMany03() {
            TrueTypeFont font = InitializeFont();
            // no actual text is set
            GlyphLine line = new GlyphLine(ConstructGlyphListFromString("A", font));
            line.SubstituteOneToMany(font.GetGsubTable(), new int[] { 39, 40 });
            NUnit.Framework.Assert.IsNull(line.actualText);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultConstructorTest() {
            GlyphLine glyphLine = new GlyphLine();
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetStart());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetEnd());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetIdx());
        }

        [NUnit.Framework.Test]
        public virtual void OtherConstructorTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine otherLine = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            GlyphLine glyphLine = new GlyphLine(otherLine);
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetStart());
            NUnit.Framework.Assert.AreEqual(16, glyphLine.GetEnd());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetIdx());
            NUnit.Framework.Assert.AreEqual("A test otherLine", glyphLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void StartEndConstructorTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine otherLine = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            GlyphLine glyphLine = new GlyphLine(otherLine, 2, 16);
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetStart());
            NUnit.Framework.Assert.AreEqual(14, glyphLine.GetEnd());
            NUnit.Framework.Assert.AreEqual(-2, glyphLine.GetIdx());
            NUnit.Framework.Assert.AreEqual("test otherLine", glyphLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void StartEndAndActualTextTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine glyphLine = new GlyphLine(ConstructGlyphListFromString("XXX otherLine", font));
            glyphLine.SetActualText(0, 3, "txt");
            GlyphLine other = new GlyphLine(glyphLine, 0, 13);
            NUnit.Framework.Assert.AreEqual("txt otherLine", other.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void CopyGlyphLineTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine glyphLine = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            GlyphLine copyLine = glyphLine.Copy(2, 6);
            NUnit.Framework.Assert.AreEqual(0, copyLine.GetStart());
            NUnit.Framework.Assert.AreEqual(4, copyLine.GetEnd());
            NUnit.Framework.Assert.AreEqual(0, copyLine.GetIdx());
            NUnit.Framework.Assert.AreEqual("test", copyLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void CopyWithActualTextGlyphLineTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine glyphLine = new GlyphLine(ConstructGlyphListFromString("XXX otherLine", font));
            glyphLine.SetActualText(0, 3, "txt");
            GlyphLine copyLine = glyphLine.Copy(0, 3);
            NUnit.Framework.Assert.AreEqual(0, copyLine.GetStart());
            NUnit.Framework.Assert.AreEqual(3, copyLine.GetEnd());
            NUnit.Framework.Assert.AreEqual(0, copyLine.GetIdx());
            NUnit.Framework.Assert.AreEqual("txt", copyLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void AddIndexedGlyphLineTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine glyphLine = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            Glyph glyph = new Glyph(200, 200, 200);
            glyphLine.Add(0, glyph);
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetStart());
            NUnit.Framework.Assert.AreEqual(16, glyphLine.GetEnd());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetIdx());
            NUnit.Framework.Assert.AreEqual("ÈA test otherLin", glyphLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void AddIndexedGlyphLineActualTextTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine glyphLine = new GlyphLine(ConstructGlyphListFromString("XXX otherLine", font));
            glyphLine.SetActualText(0, 3, "txt");
            Glyph glyph = new Glyph(200, 200, 200);
            glyphLine.Add(0, glyph);
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetStart());
            NUnit.Framework.Assert.AreEqual(13, glyphLine.GetEnd());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetIdx());
            NUnit.Framework.Assert.AreEqual("Ètxt otherLin", glyphLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceGlyphInLineTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine glyphLine = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            Glyph glyph = new Glyph(200, 200, 200);
            glyphLine.Set(0, glyph);
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetStart());
            NUnit.Framework.Assert.AreEqual(16, glyphLine.GetEnd());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetIdx());
            NUnit.Framework.Assert.AreEqual("È test otherLine", glyphLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceGlyphLineNoActualTextTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine glyphLine = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            GlyphLine replaceLine = new GlyphLine(ConstructGlyphListFromString("different text", font));
            replaceLine.SetActualText(0, 14, "different text");
            glyphLine.ReplaceContent(replaceLine);
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetStart());
            NUnit.Framework.Assert.AreEqual(14, glyphLine.GetEnd());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetIdx());
            NUnit.Framework.Assert.AreEqual("different text", glyphLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceGlyphLineWithActualTextTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine glyphLine = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            glyphLine.SetActualText(0, 14, "A test otherLine");
            GlyphLine replaceLine = new GlyphLine(ConstructGlyphListFromString("different text", font));
            replaceLine.SetActualText(0, 14, "different text");
            glyphLine.ReplaceContent(replaceLine);
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetStart());
            NUnit.Framework.Assert.AreEqual(14, glyphLine.GetEnd());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.GetIdx());
            NUnit.Framework.Assert.AreEqual("different text", glyphLine.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void NullEqualsTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine glyphLine = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            bool equals = glyphLine.Equals(null);
            NUnit.Framework.Assert.IsFalse(equals);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsItselfTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine glyphLine = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            bool equals = glyphLine.Equals(glyphLine);
            NUnit.Framework.Assert.IsTrue(equals);
        }

        [NUnit.Framework.Test]
        public virtual void EqualGlyphLinesTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine first = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            first.SetActualText(0, 14, "A test otherLine");
            GlyphLine second = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            second.SetActualText(0, 14, "A test otherLine");
            bool equals = first.Equals(second);
            NUnit.Framework.Assert.IsTrue(equals);
        }

        [NUnit.Framework.Test]
        public virtual void DiffStartEndEqualsTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine first = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            GlyphLine second = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            second.SetEnd(3);
            second.SetStart(1);
            bool equals = first.Equals(second);
            NUnit.Framework.Assert.IsFalse(equals);
        }

        [NUnit.Framework.Test]
        public virtual void DiffActualTextEqualsTest() {
            TrueTypeFont font = InitializeFont();
            GlyphLine first = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            first.SetActualText(0, 3, "txt");
            GlyphLine second = new GlyphLine(ConstructGlyphListFromString("A test otherLine", font));
            bool equals = first.Equals(second);
            NUnit.Framework.Assert.IsFalse(equals);
        }

        private TrueTypeFont InitializeFont() {
            byte[] ttf = StreamUtil.InputStreamToArray(FileUtil.GetInputStreamForFile(FREESANS_FONT_PATH));
            return new TrueTypeFont(ttf);
        }
    }
}

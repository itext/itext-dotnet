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
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class FontEncodingTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NotSetDifferenceToMinus1IndexTest() {
            FontEncoding encoding = FontEncoding.CreateEmptyFontEncoding();
            String[] initialDifferences = (String[])encoding.differences.Clone();
            encoding.SetDifference(-1, "a");
            NUnit.Framework.Assert.AreEqual(initialDifferences, encoding.differences);
        }

        [NUnit.Framework.Test]
        public virtual void NotSetDifferenceTo256IndexTest() {
            FontEncoding encoding = FontEncoding.CreateEmptyFontEncoding();
            String[] initialDifferences = (String[])encoding.differences.Clone();
            encoding.SetDifference(256, "a");
            NUnit.Framework.Assert.AreEqual(initialDifferences, encoding.differences);
        }

        [NUnit.Framework.Test]
        public virtual void SetDifferenceToZeroIndexTest() {
            FontEncoding encoding = FontEncoding.CreateEmptyFontEncoding();
            encoding.SetDifference(0, "a");
            NUnit.Framework.Assert.AreEqual("a", encoding.differences[0]);
        }

        [NUnit.Framework.Test]
        public virtual void SetDifferenceTo255IndexTest() {
            FontEncoding encoding = FontEncoding.CreateEmptyFontEncoding();
            encoding.SetDifference(255, "a");
            NUnit.Framework.Assert.AreEqual("a", encoding.differences[255]);
        }

        [NUnit.Framework.Test]
        public virtual void GetNullDifferenceTest() {
            FontEncoding encoding = FontEncoding.CreateEmptyFontEncoding();
            NUnit.Framework.Assert.IsNull(encoding.GetDifference(0));
        }

        [NUnit.Framework.Test]
        public virtual void SetDifferenceAndGetTest() {
            FontEncoding encoding = FontEncoding.CreateEmptyFontEncoding();
            encoding.SetDifference(0, "a");
            NUnit.Framework.Assert.AreEqual("a", encoding.GetDifference(0));
        }

        [NUnit.Framework.Test]
        public virtual void FontSpecificEncodingTest() {
            FontEncoding encoding = FontEncoding.CreateFontSpecificEncoding();
            NUnit.Framework.Assert.IsTrue(encoding.IsFontSpecific());
        }

        [NUnit.Framework.Test]
        public virtual void CreateFontEncodingTest() {
            FontEncoding encoding = FontEncoding.CreateFontEncoding("# full 'A' Aring 0041 'E' Egrave 0045 32 space 0020"
                );
            NUnit.Framework.Assert.AreEqual(3, encoding.unicodeToCode.Size());
        }

        [NUnit.Framework.Test]
        public virtual void NormalizeEncodingTest() {
            NUnit.Framework.Assert.AreEqual(PdfEncodings.WINANSI, FontEncoding.NormalizeEncoding(null));
            NUnit.Framework.Assert.AreEqual(PdfEncodings.WINANSI, FontEncoding.NormalizeEncoding(""));
            NUnit.Framework.Assert.AreEqual(PdfEncodings.WINANSI, FontEncoding.NormalizeEncoding("winansi"));
            NUnit.Framework.Assert.AreEqual(PdfEncodings.WINANSI, FontEncoding.NormalizeEncoding("winansiencoding"));
            NUnit.Framework.Assert.AreEqual(PdfEncodings.MACROMAN, FontEncoding.NormalizeEncoding("macroman"));
            NUnit.Framework.Assert.AreEqual(PdfEncodings.MACROMAN, FontEncoding.NormalizeEncoding("macromanencoding"));
            NUnit.Framework.Assert.AreEqual(PdfEncodings.ZAPFDINGBATS, FontEncoding.NormalizeEncoding("zapfdingbatsencoding"
                ));
            NUnit.Framework.Assert.AreEqual("notknown", FontEncoding.NormalizeEncoding("notknown"));
        }
    }
}

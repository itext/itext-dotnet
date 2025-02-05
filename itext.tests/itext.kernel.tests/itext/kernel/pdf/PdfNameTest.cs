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

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfNameTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SpecialCharactersTest() {
            String str1 = " %()<>";
            String str2 = "[]{}/#";
            PdfName name1 = new PdfName(str1);
            NUnit.Framework.Assert.AreEqual(str1, CreateStringByEscaped(name1.GetInternalContent()));
            PdfName name2 = new PdfName(str2);
            NUnit.Framework.Assert.AreEqual(str2, CreateStringByEscaped(name2.GetInternalContent()));
        }

        [NUnit.Framework.Test]
        public virtual void BasicCompareToTest() {
            // /#C3#9Cberschrift_1
            byte[] name1Content = new byte[] { 35, 67, 51, 35, 57, 67, 98, 101, 114, 115, 99, 104, 114, 105, 102, 116, 
                95, 49 };
            // /TOC-1
            byte[] name2Content = new byte[] { 84, 79, 67, 45, 49 };
            // /NormalParagraphStyle
            byte[] name3Content = new byte[] { 78, 111, 114, 109, 97, 108, 80, 97, 114, 97, 103, 114, 97, 112, 104, 83
                , 116, 121, 108, 101 };
            // /#C3#9Cberschrift_1, Ãberschrift_1
            PdfName name1 = new PdfName(name1Content);
            PdfName name1ContentOnly = new PdfName(name1Content);
            // /TOC-1, TOC-1
            PdfName name2 = new PdfName(name2Content);
            // /NormalParagraphStyle, NormalParagraphStyle
            PdfName name3 = new PdfName(name3Content);
            name1.GenerateValue();
            name2.GenerateValue();
            int oneToTwo = name1.CompareTo(name2);
            int twoToOne = name2.CompareTo(name1);
            int oneToThree = name1.CompareTo(name3);
            int twoToThree = name2.CompareTo(name3);
            int oneToOneContent = name1.CompareTo(name1ContentOnly);
            int oneContentToTwo = name1ContentOnly.CompareTo(name2);
            double delta = 1e-8;
            NUnit.Framework.Assert.AreEqual(Math.Sign(oneToTwo), -Math.Sign(twoToOne), delta);
            NUnit.Framework.Assert.AreEqual(Math.Sign(oneToTwo), Math.Sign(twoToThree), delta);
            NUnit.Framework.Assert.AreEqual(Math.Sign(oneToTwo), Math.Sign(oneToThree), delta);
            NUnit.Framework.Assert.AreEqual(oneToOneContent, 0);
            NUnit.Framework.Assert.AreEqual(Math.Sign(oneToTwo), Math.Sign(oneContentToTwo), delta);
        }
    }
}

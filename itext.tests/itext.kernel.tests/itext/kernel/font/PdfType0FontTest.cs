/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
using iText.IO.Font;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Font {
    public class PdfType0FontTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/font/PdfType0FontTest/";

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontAndCmapConstructorTest() {
            TrueTypeFont ttf = new TrueTypeFont(sourceFolder + "NotoSerif-Regular_v1.7.ttf");
            PdfType0Font type0Font = new PdfType0Font(ttf, PdfEncodings.IDENTITY_H);
            CMapEncoding cmap = type0Font.GetCmap();
            NUnit.Framework.Assert.IsNotNull(cmap);
            NUnit.Framework.Assert.IsTrue(cmap.IsDirect());
            NUnit.Framework.Assert.IsFalse(cmap.HasUniMap());
            NUnit.Framework.Assert.IsNull(cmap.GetUniMapName());
            NUnit.Framework.Assert.AreEqual("Adobe", cmap.GetRegistry());
            NUnit.Framework.Assert.AreEqual("Identity", cmap.GetOrdering());
            NUnit.Framework.Assert.AreEqual(0, cmap.GetSupplement());
            NUnit.Framework.Assert.AreEqual(PdfEncodings.IDENTITY_H, cmap.GetCmapName());
        }

        [NUnit.Framework.Test]
        public virtual void UnsupportedCmapTest() {
            TrueTypeFont ttf = new TrueTypeFont(sourceFolder + "NotoSerif-Regular_v1.7.ttf");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType0Font(ttf, PdfEncodings.
                WINANSI));
            NUnit.Framework.Assert.AreEqual(PdfException.OnlyIdentityCMapsSupportsWithTrueType, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DictionaryConstructorTest() {
            String filePath = sourceFolder + "documentWithType0Noto.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(filePath));
            PdfDictionary fontDict = pdfDocument.GetPage(1).GetResources().GetResource(PdfName.Font).GetAsDictionary(new 
                PdfName("F1"));
            PdfType0Font type0Font = new PdfType0Font(fontDict);
            CMapEncoding cmap = type0Font.GetCmap();
            NUnit.Framework.Assert.IsNotNull(cmap);
            NUnit.Framework.Assert.IsTrue(cmap.IsDirect());
            NUnit.Framework.Assert.IsFalse(cmap.HasUniMap());
            NUnit.Framework.Assert.IsNull(cmap.GetUniMapName());
            NUnit.Framework.Assert.AreEqual("Adobe", cmap.GetRegistry());
            NUnit.Framework.Assert.AreEqual("Identity", cmap.GetOrdering());
            NUnit.Framework.Assert.AreEqual(0, cmap.GetSupplement());
            NUnit.Framework.Assert.AreEqual(PdfEncodings.IDENTITY_H, cmap.GetCmapName());
        }
    }
}

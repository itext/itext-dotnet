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
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfType0FontTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/kernel/font/PdfType0FontTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/font/PdfType0FontTest/";

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontAndCmapConstructorTest() {
            TrueTypeFont ttf = new TrueTypeFont(SOURCE_FOLDER + "NotoSerif-Regular_v1.7.ttf");
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
            TrueTypeFont ttf = new TrueTypeFont(SOURCE_FOLDER + "NotoSerif-Regular_v1.7.ttf");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType0Font(ttf, PdfEncodings.
                WINANSI));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.ONLY_IDENTITY_CMAPS_SUPPORTS_WITH_TRUETYPE, 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DictionaryConstructorTest() {
            String filePath = SOURCE_FOLDER + "documentWithType0Noto.pdf";
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

        [NUnit.Framework.Test]
        public virtual void AppendThreeSurrogatePairsTest() {
            // this text contains three successive surrogate pairs, which should result in three glyphs
            String textWithThreeSurrogatePairs = "\uD800\uDF10\uD800\uDF00\uD800\uDF11";
            PdfFont type0Font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "NotoSansOldItalic-Regular.ttf", PdfEncodings
                .IDENTITY_H);
            IList<Glyph> glyphs = new List<Glyph>();
            type0Font.AppendGlyphs(textWithThreeSurrogatePairs, 0, textWithThreeSurrogatePairs.Length - 1, glyphs);
            NUnit.Framework.Assert.AreEqual(3, glyphs.Count);
        }

        [NUnit.Framework.Test]
        public virtual void GetUniMapFromOrderingTest() {
            NUnit.Framework.Assert.AreEqual("UniCNS-UTF16-H", PdfType0Font.GetUniMapFromOrdering("CNS1", true));
            NUnit.Framework.Assert.AreEqual("UniCNS-UTF16-V", PdfType0Font.GetUniMapFromOrdering("CNS1", false));
            NUnit.Framework.Assert.AreEqual("UniJIS-UTF16-H", PdfType0Font.GetUniMapFromOrdering("Japan1", true));
            NUnit.Framework.Assert.AreEqual("UniJIS-UTF16-V", PdfType0Font.GetUniMapFromOrdering("Japan1", false));
            NUnit.Framework.Assert.AreEqual("UniKS-UTF16-H", PdfType0Font.GetUniMapFromOrdering("Korea1", true));
            NUnit.Framework.Assert.AreEqual("UniKS-UTF16-V", PdfType0Font.GetUniMapFromOrdering("Korea1", false));
            NUnit.Framework.Assert.AreEqual("UniGB-UTF16-H", PdfType0Font.GetUniMapFromOrdering("GB1", true));
            NUnit.Framework.Assert.AreEqual("UniGB-UTF16-V", PdfType0Font.GetUniMapFromOrdering("GB1", false));
            NUnit.Framework.Assert.AreEqual("Identity-H", PdfType0Font.GetUniMapFromOrdering("Identity", true));
            NUnit.Framework.Assert.AreEqual("Identity-V", PdfType0Font.GetUniMapFromOrdering("Identity", false));
        }

        [NUnit.Framework.Test]
        public virtual void DescendantCidFontWithoutOrderingTest() {
            PdfDictionary fontDict = new PdfDictionary();
            PdfArray descendantFonts = new PdfArray();
            PdfDictionary descendantFont = new PdfDictionary();
            descendantFont.Put(PdfName.CIDSystemInfo, new PdfDictionary());
            descendantFonts.Add(descendantFont);
            fontDict.Put(PdfName.DescendantFonts, descendantFonts);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType0Font(fontDict));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.ORDERING_SHOULD_BE_DETERMINED, e.Message);
        }
    }
}

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
using System.IO;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FontProviderTest : ExtendedITextTest {
        private class PdfFontProvider : FontProvider {
            private IList<FontInfo> pdfFontInfos = new List<FontInfo>();

            public virtual void AddPdfFont(PdfFont font, String alias) {
                FontInfo fontInfo = FontInfo.Create(font.GetFontProgram(), null, alias);
                // stored FontInfo will be used in FontSelector collection.
                pdfFontInfos.Add(fontInfo);
                // first of all FOntProvider search PdfFont in pdfFonts.
                pdfFonts.Put(fontInfo, font);
            }

            protected internal override FontSelector CreateFontSelector(ICollection<FontInfo> fonts, IList<String> fontFamilies
                , FontCharacteristics fc) {
                IList<FontInfo> newFonts = new List<FontInfo>(fonts);
                newFonts.AddAll(pdfFontInfos);
                return base.CreateFontSelector(newFonts, fontFamilies, fc);
            }
        }

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FontProviderTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FontProviderTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void StandardAndType3Fonts() {
            String fileName = "taggedDocumentWithType3Font";
            String srcFileName = sourceFolder + "src_" + fileName + ".pdf";
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProviderTest.PdfFontProvider sel = new FontProviderTest.PdfFontProvider();
            sel.AddStandardPdfFonts();
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(new FileStream(srcFileName, FileMode.Open, FileAccess.Read
                )), new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            PdfType3Font pdfType3Font = (PdfType3Font)PdfFontFactory.CreateFont((PdfDictionary)pdfDoc.GetPdfObject(5));
            sel.AddPdfFont(pdfType3Font, "CustomFont");
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            Paragraph paragraph = new Paragraph("Next paragraph contains a triangle, actually Type 3 Font");
            paragraph.SetProperty(Property.FONT, new String[] { StandardFontFamilies.TIMES });
            doc.Add(paragraph);
            paragraph = new Paragraph("A");
            paragraph.SetFontFamily("CustomFont");
            doc.Add(paragraph);
            paragraph = new Paragraph("Next paragraph");
            paragraph.SetProperty(Property.FONT, new String[] { StandardFonts.COURIER });
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CustomFontProvider() {
            String fileName = "customFontProvider.pdf";
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider fontProvider = new FontProvider();
            // TODO DEVSIX-2119 Update if necessary
            fontProvider.GetFontSet().AddFont(StandardFonts.TIMES_ROMAN, null, "times");
            fontProvider.GetFontSet().AddFont(StandardFonts.HELVETICA);
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(fontProvider);
            Paragraph paragraph1 = new Paragraph("Default Helvetica should be selected.");
            doc.Add(paragraph1);
            Paragraph paragraph2 = new Paragraph("Default Helvetica should be selected.").SetFontFamily(StandardFonts.
                COURIER);
            doc.Add(paragraph2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void CustomFontProvider2() {
            String fileName = "customFontProvider2.pdf";
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider fontProvider = new FontProvider();
            // bold font. shouldn't be selected
            // TODO DEVSIX-2119 Update if necessary
            fontProvider.GetFontSet().AddFont(StandardFonts.TIMES_BOLD, null, "times");
            // monospace font. shouldn't be selected
            fontProvider.GetFontSet().AddFont(StandardFonts.COURIER);
            fontProvider.GetFontSet().AddFont(sourceFolder + "../fonts/FreeSans.ttf", PdfEncodings.IDENTITY_H);
            // TODO DEVSIX-2119 Update if necessary
            fontProvider.GetFontSet().AddFont(StandardFonts.TIMES_ROMAN, null, "times");
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(fontProvider);
            Paragraph paragraph = new Paragraph("There is no default font (Helvetica) inside the used FontProvider's instance. So the first font, that has been added, should be selected. Here it's FreeSans."
                ).SetFontFamily("ABRACADABRA_THERE_IS_NO_SUCH_FONT");
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FontProviderNotSetExceptionTest() {
            String fileName = "fontProviderNotSetExceptionTest.pdf";
            String outFileName = destinationFolder + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)))) {
                Document doc = new Document(pdfDoc);
                Paragraph paragraph = new Paragraph("Hello world!").SetFontFamily("ABRACADABRA_NO_FONT_PROVIDER_ANYWAY");
                Exception e = NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => doc.Add(paragraph));
                NUnit.Framework.Assert.AreEqual(LayoutExceptionMessageConstant.FONT_PROVIDER_NOT_SET_FONT_FAMILY_NOT_RESOLVED
                    , e.Message);
            }
        }
    }
}

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
using System.IO;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfua.Checkers {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUATest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUATest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUATest/";

        private static readonly String DOG = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/img/DOG.bmp";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly String FOX = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/img/FOX.bmp";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint01_007_suspectsHasEntryTrue() {
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), PdfUATestPdfDocument
                .CreateWriterProperties()));
            PdfDictionary markInfo = (PdfDictionary)pdfDoc.GetCatalog().GetPdfObject().Get(PdfName.MarkInfo);
            NUnit.Framework.Assert.IsNotNull(markInfo);
            markInfo.Put(PdfName.Suspects, new PdfBoolean(true));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.SUSPECTS_ENTRY_IN_MARK_INFO_DICTIONARY_SHALL_NOT_HAVE_A_VALUE_OF_TRUE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint01_007_suspectsHasEntryFalse() {
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), PdfUATestPdfDocument
                .CreateWriterProperties()));
            PdfDictionary markInfo = (PdfDictionary)pdfDoc.GetCatalog().GetPdfObject().Get(PdfName.MarkInfo);
            markInfo.Put(PdfName.Suspects, new PdfBoolean(false));
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint01_007_suspectsHasNoEntry() {
            // suspects entry is optional so it is ok to not have it according to the spec
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), PdfUATestPdfDocument
                .CreateWriterProperties()));
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
        }

        [NUnit.Framework.Test]
        public virtual void EmptyPageDocument() {
            String outPdf = DESTINATION_FOLDER + "emptyPageDocument.pdf";
            using (PdfDocument pdfDocument = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()))) {
                pdfDocument.AddNewPage();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_emptyPageDocument.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void ManualPdfUaCreation() {
            String outPdf = DESTINATION_FOLDER + "manualPdfUaCreation.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf, new WriterProperties().AddUAXmpMetadata().SetPdfVersion
                (PdfVersion.PDF_1_7)));
            Document document = new Document(pdfDoc, PageSize.A4.Rotate());
            //TAGGED PDF
            //Make document tagged
            pdfDoc.SetTagged();
            //PDF/UA
            //Set document metadata
            pdfDoc.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
            PdfDocumentInfo info = pdfDoc.GetDocumentInfo();
            info.SetTitle("English pangram");
            Paragraph p = new Paragraph();
            //PDF/UA
            //Embed font
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            p.SetFont(font);
            p.Add("The quick brown ");
            iText.Layout.Element.Image img = new Image(ImageDataFactory.Create(FOX));
            //PDF/UA
            //Set alt text
            img.GetAccessibilityProperties().SetAlternateDescription("Fox");
            p.Add(img);
            p.Add(" jumps over the lazy ");
            img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            //PDF/UA
            //Set alt text
            img.GetAccessibilityProperties().SetAlternateDescription("Dog");
            p.Add(img);
            document.Add(p);
            p = new Paragraph("\n\n\n\n\n\n\n\n\n\n\n\n").SetFont(font).SetFontSize(20);
            document.Add(p);
            List list = new List().SetFont(font).SetFontSize(20);
            list.Add(new ListItem("quick"));
            list.Add(new ListItem("brown"));
            list.Add(new ListItem("fox"));
            list.Add(new ListItem("jumps"));
            list.Add(new ListItem("over"));
            list.Add(new ListItem("the"));
            list.Add(new ListItem("lazy"));
            list.Add(new ListItem("dog"));
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_manualPdfUaCreation.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }
        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    }
}

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
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAAppendModeTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public const String testDirName = "PdfAAppendModeTest/";

        public static readonly String cmpFolder = sourceFolder + "cmp/" + testDirName;

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/" + testDirName;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void AddPageInAppendModeTest() {
            String inputFile = destinationFolder + "in_addPageInAppendModeTest.pdf";
            String outputFile = destinationFolder + "out_addPageInAppendModeTest.pdf";
            String cmpFile = cmpFolder + "cmp_addPageInAppendModeTest.pdf";
            CreateInputPdfADocument(inputFile);
            PdfDocument pdfADocument = new PdfADocument(new PdfReader(inputFile), new PdfWriter(outputFile), new StampingProperties
                ().UseAppendMode());
            PdfCanvas canvas = new PdfCanvas(pdfADocument.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf"
                , PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED), 16).ShowText("This page 2").EndText().RestoreState
                ();
            canvas.Release();
            pdfADocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(inputFile));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outputFile));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        private static void CreateInputPdfADocument(String docName) {
            PdfWriter writer = new PdfWriter(docName);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", new FileStream(sourceFolder + "sRGB Color Space Profile.icm"
                , FileMode.Open, FileAccess.Read)));
            pdfDoc.SetTagged();
            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf"
                , PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED), 16).ShowText("This page 1").EndText().RestoreState
                ();
            canvas.Release();
            pdfDoc.Close();
        }
    }
}

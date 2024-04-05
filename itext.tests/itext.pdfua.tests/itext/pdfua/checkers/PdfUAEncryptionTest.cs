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
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAEncryptionTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUAEncryptionTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUAEncryptionTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly byte[] USER_PASSWORD = "user".GetBytes(System.Text.Encoding.UTF8);

        private static readonly byte[] OWNER_PASSWORD = "owner".GetBytes(System.Text.Encoding.UTF8);

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void EncryptWithPassword() {
            String outPdf = DESTINATION_FOLDER + "encryptWithPassword.pdf";
            WriterProperties writerProperties = PdfUATestPdfDocument.CreateWriterProperties().SetStandardEncryption(USER_PASSWORD
                , OWNER_PASSWORD, EncryptionConstants.ALLOW_SCREENREADERS, 3);
            using (PdfWriter writer = new PdfWriter(outPdf, writerProperties)) {
                using (PdfUATestPdfDocument document = new PdfUATestPdfDocument(writer)) {
                    WriteTextToDocument(document);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_" + "encryptWithPassword.pdf"
                , DESTINATION_FOLDER, "diff", USER_PASSWORD, USER_PASSWORD));
        }

        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordWithInvalidPermissionsTest() {
            String outPdf = DESTINATION_FOLDER + "encryptWithPassword2.pdf";
            WriterProperties writerProperties = PdfUATestPdfDocument.CreateWriterProperties().SetStandardEncryption(USER_PASSWORD
                , OWNER_PASSWORD, ~EncryptionConstants.ALLOW_SCREENREADERS, 3);
            PdfUATestPdfDocument document = new PdfUATestPdfDocument(new PdfWriter(outPdf, writerProperties));
            WriteTextToDocument(document);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => document.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.TENTH_BIT_OF_P_VALUE_IN_ENCRYPTION_SHOULD_BE_NON_ZERO
                , e.Message);
        }

        private void WriteTextToDocument(PdfDocument document) {
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P, page));
            PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page, paragraph));
            canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200, 200).ShowText
                ("Hello World!").EndText().RestoreState().CloseTag();
        }
    }
}

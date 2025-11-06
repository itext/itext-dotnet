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
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAEmbeddedFilesCheckTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUAEmbeddedFilesCheckTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void PdfuaWithEmbeddedFilesWithoutFTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(), "description", "file.txt"
                    , null, null, null);
                PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
                fsDict.Remove(PdfName.F);
                pdfDocument.AddFileAttachment("file.txt", fs);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("pdfuaWithEmbeddedFilesWithoutF", PdfUAExceptionMessageConstants.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("pdfuaWithEmbeddedFilesWithoutF", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void PdfuaWithEmbeddedFilesWithoutUFTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                pdfDocument.AddNewPage();
                PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(), "description", "file.txt"
                    , null, null, null);
                PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
                fsDict.Remove(PdfName.UF);
                pdfDocument.AddFileAttachment("file.txt", fs);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("pdfuaWithEmbeddedFilesWithoutUF", PdfUAExceptionMessageConstants.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("pdfuaWithEmbeddedFilesWithoutUF", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void PdfuaWithValidEmbeddedFileTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook(((pdfDocument) => {
                AddEmbeddedFile(pdfDocument, "some test pdf file");
            }
            ));
            framework.AssertBothValid("pdfuaWithValidEmbeddedFile", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void EmbeddedFilesWithFileSpecWithoutDescTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook(((pdfDocument) => {
                AddEmbeddedFile(pdfDocument, null);
            }
            ));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("embeddedFilesWithFileSpecWithoutDesc", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("embeddedFilesWithFileSpecWithoutDesc", PdfUAExceptionMessageConstants.DESC_IS_REQUIRED_ON_ALL_FILE_SPEC_FROM_THE_EMBEDDED_FILES
                        , pdfUAConformance);
                }
            }
        }

        private static void AddEmbeddedFile(PdfDocument pdfDocument, String description) {
            PdfFont font;
            try {
                font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException) {
                // Rethrow as unchecked to fail the test.
                throw new Exception();
            }
            PdfPage page = pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            TagTreePointer tagPointer = new TagTreePointer(pdfDocument).SetPageForTagging(page).AddTag(StandardRoles.P
                );
            canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(100
                , 100).ShowText("Test text.").EndText().RestoreState().CloseTag();
            byte[] somePdf = new byte[35];
            pdfDocument.AddAssociatedFile("some test pdf file", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, somePdf
                , description, "foo.pdf", PdfName.ApplicationPdf, null, new PdfName("Data")));
        }
    }
}

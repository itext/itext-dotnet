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
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Pdfa;

namespace iText.Pdfua.Checkers {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUALayoutTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUALayoutTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUALayoutTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return JavaUtil.ArraysAsList(PdfUAConformance.PDF_UA_1, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleParagraphTest() {
            String outPdf = DESTINATION_FOLDER + "simpleParagraphTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_simpleParagraphTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("Simple layout PDF/UA-1 test").SetFont(font));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void SimpleParagraphWithUnderlineTest() {
            String outPdf = DESTINATION_FOLDER + "simpleParagraphUnderlinesTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_simpleParagraphWithUnderlineTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("Simple layout PDF/UA-1 with underline test").SetFont(font).SetUnderline());
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void SimpleBorderTest() {
            String outPdf = DESTINATION_FOLDER + "simpleBorderTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_simpleBorderTest.pdf";
            using (PdfDocument pdfDocument = new PdfUATestPdfDocument(new PdfWriter(outPdf))) {
                PdfPage page = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.OpenTag(new CanvasTag(PdfName.Artifact));
                new DottedBorder(DeviceRgb.GREEN, 5).Draw(canvas, new Rectangle(350, 700, 100, 100));
                canvas.CloseTag();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void SimpleTableTest() {
            String outPdf = DESTINATION_FOLDER + "simpleTableTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_simpleTableTest.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            Document doc = new Document(pdfDoc);
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            Table table = new Table(new float[] { 50, 50 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1").SetFont(
                font))).AddCell(new Cell().Add(new Paragraph("cell 1, 2").SetFont(font)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddNoteWithoutIdTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_169());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("noteWithoutID", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY, pdfUAConformance
                    );
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    NUnit.Framework.Assert.Catch(typeof(PdfException), () => framework.AssertVeraPdfFail("noteWithoutID", pdfUAConformance
                        ), MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                        , StandardRoles.NOTE, StandardNamespaces.PDF_2_0));
                }
            }
        }

        private sealed class _Generator_169 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_169() {
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUALayoutTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(StandardRoles.NOTE);
                return note;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.NAME_ALREADY_EXISTS_IN_THE_NAME_TREE, Ignore = true)]
        public virtual void AddTwoNotesWithSameIdTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_200(), new _Generator_216());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("twoNotesWithSameId", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.NON_UNIQUE_ID_ENTRY_IN_STRUCT_TREE_ROOT
                    , "123"), false, pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    NUnit.Framework.Assert.Catch(typeof(PdfException), () => framework.AssertVeraPdfFail("twoNotesWithSameId", 
                        pdfUAConformance), MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                        , StandardRoles.NOTE, StandardNamespaces.PDF_2_0));
                }
            }
        }

        private sealed class _Generator_200 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_200() {
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note 1");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUALayoutTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(StandardRoles.NOTE);
                note.GetAccessibilityProperties().SetStructureElementIdString("123");
                return note;
            }
        }

        private sealed class _Generator_216 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_216() {
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note 2");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUALayoutTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(StandardRoles.NOTE);
                note.GetAccessibilityProperties().SetStructureElementIdString("123");
                return note;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddNoteWithValidIdTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_248());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("noteWithValidID", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    NUnit.Framework.Assert.Catch(typeof(PdfException), () => framework.AssertVeraPdfFail("noteWithValidID", pdfUAConformance
                        ), MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                        , StandardRoles.NOTE, StandardNamespaces.PDF_2_0));
                }
            }
        }

        private sealed class _Generator_248 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_248() {
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUALayoutTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(StandardRoles.NOTE);
                note.GetAccessibilityProperties().SetStructureElementIdString("123");
                return note;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddTwoNotesWithDifferentIdTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_278(), new _Generator_294());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("twoNotesWithDifferentId", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    NUnit.Framework.Assert.Catch(typeof(PdfException), () => framework.AssertVeraPdfFail("twoNotesWithDifferentId"
                        , pdfUAConformance), MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                        , StandardRoles.NOTE, StandardNamespaces.PDF_2_0));
                }
            }
        }

        private sealed class _Generator_278 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_278() {
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note 1");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUALayoutTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(StandardRoles.NOTE);
                note.GetAccessibilityProperties().SetStructureElementIdString("123");
                return note;
            }
        }

        private sealed class _Generator_294 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_294() {
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note 2");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUALayoutTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(StandardRoles.NOTE);
                note.GetAccessibilityProperties().SetStructureElementIdString("234");
                return note;
            }
        }
    }
}

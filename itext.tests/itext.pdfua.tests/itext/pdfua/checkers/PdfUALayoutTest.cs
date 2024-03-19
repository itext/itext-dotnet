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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Colors;
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

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleParagraphTest() {
            String outPdf = DESTINATION_FOLDER + "simpleParagraphTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_simpleParagraphTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
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
            String outPdf = DESTINATION_FOLDER + "simpleParagraphTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_simpleParagraphWithUnderlineTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
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
            using (PdfDocument pdfDocument = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()))) {
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
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
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
        [NUnit.Framework.Test]
        public virtual void AddNoteWithoutIdTest() {
            framework.AddSuppliers(new _Generator_163());
            framework.AssertBothFail("noteWithoutID", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY);
        }

        private sealed class _Generator_163 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_163() {
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

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.NAME_ALREADY_EXISTS_IN_THE_NAME_TREE, Count = 2)]
        public virtual void AddTwoNotesWithSameIdTest() {
            framework.AddSuppliers(new _Generator_185(), new _Generator_201());
            framework.AssertBothFail("twoNotesWithSameId", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.NON_UNIQUE_ID_ENTRY_IN_STRUCT_TREE_ROOT
                , "123"), false);
        }

        private sealed class _Generator_185 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_185() {
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

        private sealed class _Generator_201 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_201() {
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

        [NUnit.Framework.Test]
        public virtual void AddNoteWithValidIdTest() {
            framework.AddSuppliers(new _Generator_224());
            framework.AssertBothValid("noteWithValidID");
        }

        private sealed class _Generator_224 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_224() {
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

        [NUnit.Framework.Test]
        public virtual void AddTwoNotesWithDifferentIdTest() {
            framework.AddSuppliers(new _Generator_245(), new _Generator_261());
            framework.AssertBothValid("twoNotesWithDifferentId");
        }

        private sealed class _Generator_245 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_245() {
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

        private sealed class _Generator_261 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_261() {
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

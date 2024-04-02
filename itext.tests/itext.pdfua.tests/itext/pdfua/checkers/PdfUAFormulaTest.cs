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
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAFormulaTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUAFormulaTest/";

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

        [NUnit.Framework.Test]
        public virtual void LayoutTest01() {
            framework.AddSuppliers(new _Generator_74());
            framework.AssertBothFail("layout01");
        }

        private sealed class _Generator_74 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_74() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LayoutTest02() {
            framework.AddSuppliers(new _Generator_87());
            framework.AssertBothValid("layout02");
        }

        private sealed class _Generator_87 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_87() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("Einstein smart boy formula");
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LayoutTest03() {
            framework.AddSuppliers(new _Generator_102());
            framework.AssertBothValid("layout03");
        }

        private sealed class _Generator_102 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_102() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("Einstein smart boy " + "formula");
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LayoutTest04() {
            framework.AddSuppliers(new _Generator_117());
            framework.AssertBothFail("layout04");
        }

        private sealed class _Generator_117 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_117() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("");
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LayoutTest05() {
            framework.AddSuppliers(new _Generator_131());
            framework.AssertBothValid("layout05");
        }

        private sealed class _Generator_131 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_131() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("");
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LayoutTest06() {
            framework.AddSuppliers(new _Generator_145());
            framework.AssertBothFail("layout06", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), false);
        }

        private sealed class _Generator_145 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_145() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("⫊").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("Some character that is not embeded in the font");
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LayoutTest07() {
            framework.AddSuppliers(new _Generator_160());
            framework.AssertBothFail("layout07", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), false);
        }

        private sealed class _Generator_160 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_160() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("⫊").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("Alternate " + "description");
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LayoutWithValidRole() {
            framework.AddSuppliers(new _Generator_175());
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfStructTreeRoot tagStructureContext = pdfDocument.GetStructTreeRoot();
                tagStructureContext.AddRoleMapping("BING", StandardRoles.FORMULA);
            }
            );
            framework.AssertBothValid("layoutWithValidRole");
        }

        private sealed class _Generator_175 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_175() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("e = mc^2").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole("BING");
                p.GetAccessibilityProperties().SetAlternateDescription("Alternate " + "description");
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LayoutWithValidRoleButNoAlternateDescription() {
            framework.AddSuppliers(new _Generator_194());
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfStructTreeRoot tagStructureContext = pdfDocument.GetStructTreeRoot();
                tagStructureContext.AddRoleMapping("BING", StandardRoles.FORMULA);
            }
            );
            framework.AssertBothFail("layoutWithValidRoleButNoDescription");
        }

        private sealed class _Generator_194 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_194() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("e = mc^2").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole("BING");
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void CanvasTest01() {
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfFont font = PdfFontFactory.CreateFont(FONT);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(document.GetFirstPage());
            tagPointer.AddTag(StandardRoles.FORMULA);
            canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).ShowText("E=mc²"
                ).EndText().CloseTag();
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Close();
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void CanvasTest02() {
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfFont font = PdfFontFactory.CreateFont(FONT);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(document.GetFirstPage());
            tagPointer.AddTag(StandardRoles.FORMULA);
            tagPointer.GetProperties().SetActualText("Einstein smart boy");
            canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).ShowText("E=mc²"
                ).EndText().CloseTag();
            NUnit.Framework.Assert.DoesNotThrow(() => {
                document.Close();
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void CanvasTest03() {
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfFont font = PdfFontFactory.CreateFont(FONT);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(document.GetFirstPage());
            tagPointer.AddTag(StandardRoles.FORMULA);
            canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => canvas.ShowText("⫊"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), e.Message);
        }

        private static PdfFont LoadFont(String fontPath) {
            try {
                return PdfFontFactory.CreateFont(fontPath);
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message);
            }
        }
    }
}

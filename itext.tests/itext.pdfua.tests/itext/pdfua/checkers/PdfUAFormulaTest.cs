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
            framework.AddSuppliers(new _Generator_71());
            framework.AssertBothFail("layout01");
        }

        private sealed class _Generator_71 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_71() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void LayoutTest02() {
            framework.AddSuppliers(new _Generator_84());
            framework.AssertBothValid("layout02");
        }

        private sealed class _Generator_84 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_84() {
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
            framework.AddSuppliers(new _Generator_99());
            framework.AssertBothValid("layout03");
        }

        private sealed class _Generator_99 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_99() {
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
            framework.AddSuppliers(new _Generator_114());
            framework.AssertBothFail("layout04");
        }

        private sealed class _Generator_114 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_114() {
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
            framework.AddSuppliers(new _Generator_128());
            framework.AssertBothValid("layout05");
        }

        private sealed class _Generator_128 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_128() {
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
            framework.AddSuppliers(new _Generator_142());
            framework.AssertBothFail("layout06", false);
        }

        private sealed class _Generator_142 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_142() {
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
            framework.AddSuppliers(new _Generator_156());
            framework.AssertBothFail("layout07", false);
        }

        private sealed class _Generator_156 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_156() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("⫊").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("Alternate " + "description");
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
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.ShowText("⫊");
            }
            );
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

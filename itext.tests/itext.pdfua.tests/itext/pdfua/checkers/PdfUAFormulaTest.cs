/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUAFormulaTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<Object> Data() {
            return UaValidationTestFramework2.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest01(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(new _Generator_71());
            if (conformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout01");
            }
            else {
                if (conformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layout01");
                }
            }
        }

        private sealed class _Generator_71 : UaValidationTestFramework2.Generator<IBlockElement> {
            public _Generator_71() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest02(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(new _Generator_91());
            framework.AssertBothValid("layout02");
        }

        private sealed class _Generator_91 : UaValidationTestFramework2.Generator<IBlockElement> {
            public _Generator_91() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("Einstein smart boy formula");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest03(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(new _Generator_107());
            framework.AssertBothValid("layout03");
        }

        private sealed class _Generator_107 : UaValidationTestFramework2.Generator<IBlockElement> {
            public _Generator_107() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("Einstein smart boy " + "formula");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest04(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(new _Generator_123());
            if (conformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout04");
            }
            else {
                if (conformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layout04");
                }
                else {
                    if (conformance == WellTaggedPdfConformance.FOR_REUSE) {
                        framework.AssertBothValid("layout04");
                    }
                }
            }
        }

        private sealed class _Generator_123 : UaValidationTestFramework2.Generator<IBlockElement> {
            public _Generator_123() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest05(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(new _Generator_146());
            framework.AssertBothValid("layout05");
        }

        private sealed class _Generator_146 : UaValidationTestFramework2.Generator<IBlockElement> {
            public _Generator_146() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest06(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(new _Generator_162());
            framework.AssertBothFail("layout06", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), false);
        }

        private sealed class _Generator_162 : UaValidationTestFramework2.Generator<IBlockElement> {
            public _Generator_162() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("⫊").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("Some character that is not embeded in the font");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest07(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(new _Generator_180());
            framework.AssertBothFail("layout07", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), false);
        }

        private sealed class _Generator_180 : UaValidationTestFramework2.Generator<IBlockElement> {
            public _Generator_180() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("⫊").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("Alternate " + "description");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutWithValidRole(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(new _Generator_198());
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (conformance == PdfUAConformance.PDF_UA_2 || conformance == WellTaggedPdfConformance.FOR_REUSE) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("BING", StandardRoles.FORMULA);
                }
                PdfStructTreeRoot tagStructureContext = pdfDocument.GetStructTreeRoot();
                tagStructureContext.AddRoleMapping("BING", StandardRoles.FORMULA);
            }
            );
            framework.AssertBothValid("layoutWithValidRole");
        }

        private sealed class _Generator_198 : UaValidationTestFramework2.Generator<IBlockElement> {
            public _Generator_198() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("e = mc^2").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole("BING");
                p.GetAccessibilityProperties().SetAlternateDescription("Alternate " + "description");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutWithValidRoleButNoAlternateDescription(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers(new _Generator_225());
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (conformance == PdfUAConformance.PDF_UA_2 || conformance == WellTaggedPdfConformance.FOR_REUSE) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("BING", StandardRoles.FORMULA);
                }
                PdfStructTreeRoot tagStructureContext = pdfDocument.GetStructTreeRoot();
                tagStructureContext.AddRoleMapping("BING", StandardRoles.FORMULA);
            }
            );
            if (conformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layoutWithValidRoleButNoDescription");
            }
            else {
                if (conformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layoutWithValidRoleButNoDescription");
                }
                else {
                    if (conformance == WellTaggedPdfConformance.FOR_REUSE) {
                        framework.AssertBothValid("layoutWithValidRoleButNoDescription");
                    }
                }
            }
        }

        private sealed class _Generator_225 : UaValidationTestFramework2.Generator<IBlockElement> {
            public _Generator_225() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("e = mc^2").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole("BING");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CanvasTest01(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                PdfFont font = LoadFont(FONT);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                tagPointer.SetPageForTagging(pdfDoc.GetFirstPage());
                tagPointer.AddTag(StandardRoles.FORMULA);
                canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).ShowText("E=mc²"
                    ).EndText().CloseTag();
            }
            );
            if (conformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("canvasTest01", PdfUAExceptionMessageConstants.FORMULA_SHALL_HAVE_ALT);
            }
            else {
                if (conformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("canvasTest01");
                }
                else {
                    if (conformance == WellTaggedPdfConformance.FOR_REUSE) {
                        framework.AssertBothValid("canvasTest01");
                    }
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CanvasTest02(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                PdfFont font = LoadFont(FONT);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                tagPointer.SetPageForTagging(pdfDoc.GetFirstPage());
                tagPointer.AddTag(StandardRoles.FORMULA);
                tagPointer.GetProperties().SetActualText("Einstein smart boy");
                canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).ShowText("E=mc²"
                    ).EndText().CloseTag();
            }
            );
            framework.AssertBothValid("canvasTest02");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CanvasTest03(Object conformance) {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                PdfFont font = LoadFont(FONT);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                tagPointer.SetPageForTagging(pdfDoc.GetFirstPage());
                tagPointer.AddTag(StandardRoles.FORMULA);
                tagPointer.GetProperties().SetAlternateDescription("Alt descr");
                canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12);
                canvas.ShowText("⫊");
            }
            );
            framework.AssertBothFail("canvasTest03", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), false);
        }

        [NUnit.Framework.Test]
        public virtual void MathStructureElementInvalidUA2Test() {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, PdfUAConformance
                .PDF_UA_2);
            framework.AddSuppliers(new _Generator_326());
            framework.AssertBothFail("mathStructureElementInvalidUA2Test", PdfUAExceptionMessageConstants.MATH_NOT_CHILD_OF_FORMULA
                );
        }

        private sealed class _Generator_326 : UaValidationTestFramework2.Generator<IBlockElement> {
            public _Generator_326() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetNamespace(new PdfNamespace(StandardNamespaces.MATH_ML));
                p.GetAccessibilityProperties().SetRole("math");
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void MathStructureElementValidUA2Test() {
            UaValidationTestFramework2 framework = new UaValidationTestFramework2(DESTINATION_FOLDER, PdfUAConformance
                .PDF_UA_2);
            framework.AddAfterGenerationHook((pdfDocument) => {
                PdfPage page = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                PdfFont font = LoadFont(FONT);
                TagTreePointer tagPointer = new TagTreePointer(pdfDocument);
                tagPointer.SetPageForTagging(pdfDocument.GetFirstPage());
                tagPointer.AddTag(StandardRoles.FORMULA);
                tagPointer.SetNamespaceForNewTags(new PdfNamespace(StandardNamespaces.MATH_ML));
                tagPointer.AddTag("math");
                canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).ShowText("E=mc²"
                    ).EndText().CloseTag();
            }
            );
            framework.AssertBothValid("mathStructureElementValidUA2Test");
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

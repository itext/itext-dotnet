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

        public static IList<PdfUAConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest01(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_70());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout01", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layout01", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_70 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_70() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest02(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_90());
            framework.AssertBothValid("layout02", pdfUAConformance);
        }

        private sealed class _Generator_90 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_90() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("Einstein smart boy formula");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest03(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_106());
            framework.AssertBothValid("layout03", pdfUAConformance);
        }

        private sealed class _Generator_106 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_106() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("Einstein smart boy " + "formula");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest04(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_122());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout04", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layout04", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_122 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_122() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest05(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_143());
            framework.AssertBothValid("layout05", pdfUAConformance);
        }

        private sealed class _Generator_143 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_143() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest06(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_159());
            framework.AssertBothFail("layout06", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), false, pdfUAConformance);
        }

        private sealed class _Generator_159 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_159() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("⫊").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("Some character that is not embeded in the font");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest07(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_177());
            framework.AssertBothFail("layout07", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), false, pdfUAConformance);
        }

        private sealed class _Generator_177 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_177() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("⫊").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("Alternate " + "description");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutWithValidRole(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_195());
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("BING", StandardRoles.FORMULA);
                }
                PdfStructTreeRoot tagStructureContext = pdfDocument.GetStructTreeRoot();
                tagStructureContext.AddRoleMapping("BING", StandardRoles.FORMULA);
            }
            );
            framework.AssertBothValid("layoutWithValidRole", pdfUAConformance);
        }

        private sealed class _Generator_195 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_195() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("e = mc^2").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole("BING");
                p.GetAccessibilityProperties().SetAlternateDescription("Alternate " + "description");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutWithValidRoleButNoAlternateDescription(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_222());
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("BING", StandardRoles.FORMULA);
                }
                PdfStructTreeRoot tagStructureContext = pdfDocument.GetStructTreeRoot();
                tagStructureContext.AddRoleMapping("BING", StandardRoles.FORMULA);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layoutWithValidRoleButNoDescription", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layoutWithValidRoleButNoDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_222 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_222() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("e = mc^2").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole("BING");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CanvasTest01(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("canvasTest01", PdfUAExceptionMessageConstants.FORMULA_SHALL_HAVE_ALT, pdfUAConformance
                    );
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("canvasTest01", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CanvasTest02(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            framework.AssertBothValid("canvasTest02", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CanvasTest03(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                , "⫊"), false, pdfUAConformance);
        }

        [NUnit.Framework.Test]
        public virtual void MathStructureElementInvalidUA2Test() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_319());
            framework.AssertBothFail("mathStructureElementInvalidUA2Test", PdfUAExceptionMessageConstants.MATH_NOT_CHILD_OF_FORMULA
                , PdfUAConformance.PDF_UA_2);
        }

        private sealed class _Generator_319 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_319() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            framework.AssertBothValid("mathStructureElementValidUA2Test", PdfUAConformance.PDF_UA_2);
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

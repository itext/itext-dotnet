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
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest01(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_80());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout01", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layout01", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_80 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_80() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest02(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_99());
            framework.AssertBothValid("layout02", pdfUAConformance);
        }

        private sealed class _Generator_99 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_99() {
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
            framework.AddSuppliers(new _Generator_114());
            framework.AssertBothValid("layout03", pdfUAConformance);
        }

        private sealed class _Generator_114 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_114() {
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
            framework.AddSuppliers(new _Generator_129());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout04", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layout04", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_129 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_129() {
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
            framework.AddSuppliers(new _Generator_149());
            framework.AssertBothValid("layout05", pdfUAConformance);
        }

        private sealed class _Generator_149 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_149() {
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
            framework.AddSuppliers(new _Generator_164());
            framework.AssertBothFail("layout06", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), false, pdfUAConformance);
        }

        private sealed class _Generator_164 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_164() {
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
            framework.AddSuppliers(new _Generator_181());
            framework.AssertBothFail("layout07", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), false, pdfUAConformance);
        }

        private sealed class _Generator_181 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_181() {
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
            framework.AddSuppliers(new _Generator_198());
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

        private sealed class _Generator_198 : UaValidationTestFramework.Generator<IBlockElement> {
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
        public virtual void LayoutWithValidRoleButNoAlternateDescription(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_224());
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

        private sealed class _Generator_224 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_224() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("e = mc^2").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole("BING");
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CanvasTest01(PdfUAConformance pdfUAConformance) {
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
            framework.AddSuppliers(new _Generator_317());
            // TODO DEVSIX-9036. VeraPDF claims the document to be valid, although it's not.
            //  We will need to update this test when veraPDF behavior is fixed and veraPDF version is updated.
            framework.AssertOnlyITextFail("mathStructureElementInvalidUA2Test", PdfUAExceptionMessageConstants.MATH_NOT_CHILD_OF_FORMULA
                , PdfUAConformance.PDF_UA_2);
        }

        private sealed class _Generator_317 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_317() {
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

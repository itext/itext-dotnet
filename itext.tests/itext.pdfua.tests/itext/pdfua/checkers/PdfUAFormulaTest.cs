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
using iText.Kernel.Exceptions;
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

        public static IList<PdfConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest01(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph p = new Paragraph("E=mc²").SetFont(LoadFont(FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                return p;
            }
            );
            if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout01");
            }
            else {
                if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layout01");
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest02(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph p = new Paragraph("E=mc²").SetFont(LoadFont(FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("Einstein smart boy formula");
                return p;
            }
            );
            framework.AssertBothValid("layout02");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest03(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph p = new Paragraph("E=mc²").SetFont(LoadFont(FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("Einstein smart boy " + "formula");
                return p;
            }
            );
            framework.AssertBothValid("layout03");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest04(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph p = new Paragraph("E=mc²").SetFont(LoadFont(FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("");
                return p;
            }
            );
            if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout04");
            }
            else {
                if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layout04");
                }
                else {
                    if (conformance.ConformsTo(WellTaggedPdfConformance.FOR_REUSE)) {
                        framework.AssertBothValid("layout04");
                    }
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest05(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph p = new Paragraph("E=mc²").SetFont(LoadFont(FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("");
                return p;
            }
            );
            framework.AssertBothValid("layout05");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest06(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph p = new Paragraph("⫊").SetFont(LoadFont(FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetActualText("Some character that is not embeded in the font");
                return p;
            }
            );
            framework.AssertBothFail("layout06", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest07(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph p = new Paragraph("⫊").SetFont(LoadFont(FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                p.GetAccessibilityProperties().SetAlternateDescription("Alternate " + "description");
                return p;
            }
            );
            framework.AssertBothFail("layout07", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                , "⫊"), false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutWithValidRole(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph p = new Paragraph("e = mc^2").SetFont(LoadFont(FONT));
                p.GetAccessibilityProperties().SetRole("BING");
                p.GetAccessibilityProperties().SetAlternateDescription("Alternate " + "description");
                return p;
            }
            );
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_2 || conformance.IsWtpdf()) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutWithValidRoleButNoAlternateDescription(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph p = new Paragraph("e = mc^2").SetFont(LoadFont(FONT));
                p.GetAccessibilityProperties().SetRole("BING");
                return p;
            }
            );
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_2 || conformance.ConformsTo(WellTaggedPdfConformance
                    .FOR_REUSE)) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("BING", StandardRoles.FORMULA);
                }
                PdfStructTreeRoot tagStructureContext = pdfDocument.GetStructTreeRoot();
                tagStructureContext.AddRoleMapping("BING", StandardRoles.FORMULA);
            }
            );
            if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layoutWithValidRoleButNoDescription");
            }
            else {
                if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layoutWithValidRoleButNoDescription");
                }
                else {
                    if (conformance.ConformsTo(WellTaggedPdfConformance.FOR_REUSE)) {
                        framework.AssertBothValid("layoutWithValidRoleButNoDescription");
                    }
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CanvasTest01(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("canvasTest01", PdfUAExceptionMessageConstants.FORMULA_SHALL_HAVE_ALT);
            }
            else {
                if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("canvasTest01");
                }
                else {
                    if (conformance.ConformsTo(WellTaggedPdfConformance.FOR_REUSE)) {
                        framework.AssertBothValid("canvasTest01");
                    }
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CanvasTest02(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
        public virtual void CanvasTest03(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, new PdfConformance
                (PdfUAConformance.PDF_UA_2));
            framework.AddSuppliers((document) => {
                Paragraph p = new Paragraph("E=mc²").SetFont(LoadFont(FONT));
                p.GetAccessibilityProperties().SetNamespace(new PdfNamespace(StandardNamespaces.MATH_ML));
                p.GetAccessibilityProperties().SetRole("math");
                return p;
            }
            );
            framework.AssertBothFail("mathStructureElementInvalidUA2Test", PdfUAExceptionMessageConstants.MATH_NOT_CHILD_OF_FORMULA
                );
        }

        [NUnit.Framework.Test]
        public virtual void MathStructureElementValidUA2Test() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, new PdfConformance
                (PdfUAConformance.PDF_UA_2));
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
                throw new PdfException(e.Message);
            }
        }
    }
}

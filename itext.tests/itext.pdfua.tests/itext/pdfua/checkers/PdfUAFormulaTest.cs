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

        public static IList<PdfUAConformance> Data() {
            return JavaUtil.ArraysAsList(PdfUAConformance.PDF_UA_1, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest01(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_83());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout01", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layout01", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_83 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_83() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("E=mc²").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                return p;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LayoutTest02(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_102());
            framework.AssertBothValid("layout02", pdfUAConformance);
        }

        private sealed class _Generator_102 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_102() {
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
            framework.AddSuppliers(new _Generator_117());
            framework.AssertBothValid("layout03", pdfUAConformance);
        }

        private sealed class _Generator_117 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_117() {
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
            framework.AddSuppliers(new _Generator_132());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout04", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("layout04", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_132 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_132() {
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
            framework.AddSuppliers(new _Generator_152());
            framework.AssertBothValid("layout05", pdfUAConformance);
        }

        private sealed class _Generator_152 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_152() {
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
            framework.AddSuppliers(new _Generator_167());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout06", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                    , "⫊"), false, pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    // TODO DEVSIX-8242 The layout level doesn’t throw an error
                    framework.AssertVeraPdfFail("layout06", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_167 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_167() {
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
            framework.AddSuppliers(new _Generator_190());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("layout07", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                    , "⫊"), false, pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    // TODO DEVSIX-8242 The layout level doesn’t throw an error
                    framework.AssertVeraPdfFail("layout07", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_190 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_190() {
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
            framework.AddSuppliers(new _Generator_212());
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

        private sealed class _Generator_212 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_212() {
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
            framework.AddSuppliers(new _Generator_238());
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

        private sealed class _Generator_238 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_238() {
            }

            public IBlockElement Generate() {
                Paragraph p = new Paragraph("e = mc^2").SetFont(PdfUAFormulaTest.LoadFont(PdfUAFormulaTest.FONT));
                p.GetAccessibilityProperties().SetRole("BING");
                return p;
            }
        }

        [NUnit.Framework.Test]
        public virtual void CanvasTest01() {
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream()));
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
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream()));
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
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream()));
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

        [NUnit.Framework.Test]
        public virtual void MathStructureElementInvalidUA2Test() {
            framework.AddSuppliers(new _Generator_326());
            // TODO DEVSIX-9036. VeraPDF claims the document to be valid, although it's not.
            //  We will need to update this test when veraPDF behavior is fixed and veraPDF version is updated.
            framework.AssertVeraPdfValid("mathStructureElementInvalidUA2Test", PdfUAConformance.PDF_UA_2);
            framework.AssertITextFail("mathStructureElementInvalidUA2Test", PdfUAExceptionMessageConstants.MATH_NOT_CHILD_OF_FORMULA
                , PdfUAConformance.PDF_UA_2);
        }

        private sealed class _Generator_326 : UaValidationTestFramework.Generator<IBlockElement> {
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
            framework.AddAfterGenerationHook((pdfDocument) => {
                PdfPage page = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(FONT);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
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

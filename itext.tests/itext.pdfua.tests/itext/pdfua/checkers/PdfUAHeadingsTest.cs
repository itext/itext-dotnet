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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Pdfua.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAHeadingsTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUAHeadingsTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUAHeadingsTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        private static PdfFont LoadFont() {
            try {
                return PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message);
            }
        }

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        // -------- Negative tests --------
        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddH2AsFirstHeaderTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_96());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("addH2FirstHeaderTest", PdfUAExceptionMessageConstants.H1_IS_SKIPPED, pdfUAConformance
                    );
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("addH2FirstHeaderTest", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_96 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_96() {
            }

            public IBlockElement Generate() {
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                return h2;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void BrokenHnParallelSequenceTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_118());
            framework.AddSuppliers(new _Generator_127());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("brokenHnParallelSequenceTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                    .HN_IS_SKIPPED, 2), pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("brokenHnParallelSequenceTest", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_118 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_118() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
        }

        private sealed class _Generator_127 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_127() {
            }

            public IBlockElement Generate() {
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                return h3;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void BrokenHnInheritedSequenceTest1(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_148());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("brokenHnInheritedSequenceTest1", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                    .HN_IS_SKIPPED, 2), pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String expectedMessage = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H3");
                    framework.AssertBothFail("brokenHnInheritedSequenceTest1", expectedMessage, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_148 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_148() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h1.Add(h3);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void BrokenHnMixedSequenceTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_176());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("brokenHnMixedSequenceTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                    .HN_IS_SKIPPED, 3), pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String expectedMessage = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("brokenHnInheritedSequenceTest1", expectedMessage, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_176 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_176() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h5 = new Paragraph("Header level 5");
                h5.SetFont(PdfUAHeadingsTest.LoadFont());
                h5.GetAccessibilityProperties().SetRole(StandardRoles.H5);
                h1.Add(h5);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void BrokenHnMixedSequenceTest2(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_209());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("brokenHnMixedSequenceTest2", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                    .HN_IS_SKIPPED, 3), pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "Div");
                    framework.AssertBothFail("brokenHnMixedSequenceTest2", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_209 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_209() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Div div = new Div();
                div.SetBackgroundColor(ColorConstants.CYAN);
                h1.Add(div);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                div.Add(h2);
                Paragraph h5 = new Paragraph("Header level 5");
                h5.SetFont(PdfUAHeadingsTest.LoadFont());
                h5.GetAccessibilityProperties().SetRole(StandardRoles.H5);
                div.Add(h5);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void FewHInOneNodeTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_246());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("fewHInOneNodeTest", PdfUAExceptionMessageConstants.MORE_THAN_ONE_H_TAG, pdfUAConformance
                    );
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("fewHInOneNodeTest", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG, pdfUAConformance
                        );
                }
            }
        }

        private sealed class _Generator_246 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_246() {
            }

            public IBlockElement Generate() {
                Div div = new Div();
                div.SetBackgroundColor(ColorConstants.CYAN);
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(PdfUAHeadingsTest.LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                div.Add(header1);
                Paragraph header2 = new Paragraph("Header");
                header2.SetFont(PdfUAHeadingsTest.LoadFont());
                header2.GetAccessibilityProperties().SetRole(StandardRoles.H);
                div.Add(header2);
                return div;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void FewHInDocumentTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_277());
            framework.AddSuppliers(new _Generator_286());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("fewHInDocumentTest", PdfUAExceptionMessageConstants.MORE_THAN_ONE_H_TAG, pdfUAConformance
                    );
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("fewHInDocumentTest", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG, pdfUAConformance
                        );
                }
            }
        }

        private sealed class _Generator_277 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_277() {
            }

            public IBlockElement Generate() {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(PdfUAHeadingsTest.LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
        }

        private sealed class _Generator_286 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_286() {
            }

            public IBlockElement Generate() {
                Paragraph header2 = new Paragraph("Header");
                header2.SetFont(PdfUAHeadingsTest.LoadFont());
                header2.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header2;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HAndHnInDocumentTest1(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_308());
            framework.AddSuppliers(new _Generator_317());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("hAndHnInDocumentTest1", PdfUAExceptionMessageConstants.DOCUMENT_USES_BOTH_H_AND_HN
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("hAndHnInDocumentTest1", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG, pdfUAConformance
                        );
                }
            }
        }

        private sealed class _Generator_308 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_308() {
            }

            public IBlockElement Generate() {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(PdfUAHeadingsTest.LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
        }

        private sealed class _Generator_317 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_317() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HAndHnInDocumentTest2(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_339());
            framework.AddSuppliers(new _Generator_348());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("hAndHnInDocumentTest2", PdfUAExceptionMessageConstants.DOCUMENT_USES_BOTH_H_AND_HN
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("hAndHnInDocumentTest2", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG, pdfUAConformance
                        );
                }
            }
        }

        private sealed class _Generator_339 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_339() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
        }

        private sealed class _Generator_348 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_348() {
            }

            public IBlockElement Generate() {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(PdfUAHeadingsTest.LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HAndHnInDocumentTest3(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_370());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("hAndHnInDocumentTest3", PdfUAExceptionMessageConstants.DOCUMENT_USES_BOTH_H_AND_HN
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("hAndHnInDocumentTest3", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG, pdfUAConformance
                        );
                }
            }
        }

        private sealed class _Generator_370 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_370() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(PdfUAHeadingsTest.LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                h2.Add(header1);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void RoleMappingTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_401());
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("header1", StandardRoles.H1);
                    @namespace.AddNamespaceRoleMapping("header5", StandardRoles.H5);
                }
                root.AddRoleMapping("header1", StandardRoles.H1);
                root.AddRoleMapping("header5", StandardRoles.H5);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("rolemappingTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "header5");
                    framework.AssertBothFail("rolemappingTest", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_401 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_401() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole("header1");
                Paragraph h2 = new Paragraph("Header level 5");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole("header5");
                h1.Add(h2);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void RoleMappingTestValid(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_441());
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("header1", StandardRoles.H1);
                    @namespace.AddNamespaceRoleMapping("header5", StandardRoles.H2);
                }
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("header1", StandardRoles.H1);
                root.AddRoleMapping("header5", StandardRoles.H2);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("rolemappingValid", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "header5");
                    framework.AssertBothFail("rolemappingValid", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_441 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_441() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole("header1");
                Paragraph h2 = new Paragraph("Header level 5");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole("header5");
                h1.Add(h2);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void DirectWritingToCanvasTest(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                TagTreePointer pointer = new TagTreePointer(pdfDoc);
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                pointer.SetPageForTagging(page);
                TagTreePointer tmp = pointer.AddTag(StandardRoles.H3);
                canvas.OpenTag(tmp.GetTagReference());
                canvas.WriteLiteral("Heading level 3");
                canvas.CloseTag();
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("directWritingToCanvas", PdfUAExceptionMessageConstants.H1_IS_SKIPPED, pdfUAConformance
                    );
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("directWritingToCanvas", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HInDocumentTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_503());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hInDocumentTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("hInDocumentTest", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG, pdfUAConformance
                        );
                }
            }
        }

        private sealed class _Generator_503 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_503() {
            }

            public IBlockElement Generate() {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(PdfUAHeadingsTest.LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HAndHnInDocumentTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_524());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("hAndHnInDocumentTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("hAndHnInDocumentTest", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG, pdfUAConformance
                        );
                }
            }
        }

        private sealed class _Generator_524 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_524() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(PdfUAHeadingsTest.LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                h2.Add(header1);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void IncorrectHeadingLevelInUA2Test(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_555());
            // Where a heading’s level is evident, the heading level of the structure element enclosing it shall match that
            // heading level, e.g. a heading with the real content “5.1.6.4 Some header” is evidently at heading level 4.
            // This requirement is not checked by both iText and veraPDF.
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("incorrectHeadingLevelInUA2Test", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("incorrectHeadingLevelInUA2Test", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_555 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_555() {
            }

            public IBlockElement Generate() {
                Div div = new Div();
                div.SetBackgroundColor(ColorConstants.CYAN);
                Paragraph h2 = new Paragraph("1.2 Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                div.Add(h2);
                Paragraph h1 = new Paragraph("1.2.3 Header level 3");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                div.Add(h1);
                return h2;
            }
        }

        // -------- Positive tests --------
        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(PdfUALogMessageConstants.PAGE_FLUSHING_DISABLED, Ignore = true)]
        public virtual void FlushPreviousPageTest(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document doc = new Document(pdfDoc);
                String longHeader = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " + "Donec ac malesuada tellus. "
                     + "Quisque a arcu semper, tristique nibh eu, convallis lacus. " + "Donec neque justo, condimentum sed molestie ac, mollis eu nibh. "
                     + "Vivamus pellentesque condimentum fringilla. " + "Nullam euismod ac risus a semper. " + "Etiam hendrerit scelerisque sapien tristique varius.";
                for (int i = 0; i < 10; i++) {
                    Paragraph h1 = new Paragraph(longHeader);
                    h1.SetFont(LoadFont());
                    h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                    Paragraph h2 = new Paragraph(longHeader);
                    h2.SetFont(LoadFont());
                    h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                    h1.Add(h2);
                    Paragraph h3 = new Paragraph(longHeader);
                    h3.SetFont(LoadFont());
                    h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                    h2.Add(h3);
                    Paragraph h4 = new Paragraph(longHeader);
                    h4.SetFont(LoadFont());
                    h4.GetAccessibilityProperties().SetRole(StandardRoles.H4);
                    h3.Add(h4);
                    Paragraph h5 = new Paragraph(longHeader);
                    h5.SetFont(LoadFont());
                    h5.GetAccessibilityProperties().SetRole(StandardRoles.H5);
                    h4.Add(h5);
                    Paragraph h6 = new Paragraph(longHeader);
                    h6.SetFont(LoadFont());
                    h6.GetAccessibilityProperties().SetRole(StandardRoles.H6);
                    h5.Add(h6);
                    doc.Add(h1);
                    if (pdfDoc.GetNumberOfPages() > 1) {
                        pdfDoc.GetPage(pdfDoc.GetNumberOfPages() - 1).Flush();
                    }
                }
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hugeDocumentTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("hugeDocumentTest", MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2"), pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnInheritedSequenceTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_648());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hnInheritedSequenceTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnInheritedSequenceTest", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_648 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_648() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h2.Add(h3);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnCompareWithLastFromAnotherBranchTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_680());
            framework.AddSuppliers(new _Generator_704());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hnInheritedSequenceTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnInheritedSequenceTest", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_680 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_680() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h2.Add(h3);
                Paragraph h4 = new Paragraph("Header level 4");
                h4.SetFont(PdfUAHeadingsTest.LoadFont());
                h4.GetAccessibilityProperties().SetRole(StandardRoles.H4);
                h2.Add(h4);
                return h1;
            }
        }

        private sealed class _Generator_704 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_704() {
            }

            public IBlockElement Generate() {
                Paragraph h5 = new Paragraph("Second Header level 5 in doc");
                h5.SetFont(PdfUAHeadingsTest.LoadFont());
                h5.GetAccessibilityProperties().SetRole(StandardRoles.H5);
                return h5;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnCompareWithLastFromAnotherBranchTest2(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_726());
            framework.AddSuppliers(new _Generator_750());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hnCompareWithLastFromAnotherBranchTest2", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnCompareWithLastFromAnotherBranchTest2", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_726 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_726() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h2.Add(h3);
                Paragraph h4 = new Paragraph("Header level 4");
                h4.SetFont(PdfUAHeadingsTest.LoadFont());
                h4.GetAccessibilityProperties().SetRole(StandardRoles.H4);
                h2.Add(h4);
                return h1;
            }
        }

        private sealed class _Generator_750 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_750() {
            }

            public IBlockElement Generate() {
                Paragraph h33 = new Paragraph("Second Header level 3 in doc");
                h33.SetFont(PdfUAHeadingsTest.LoadFont());
                h33.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                return h33;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnInheritedSequenceTest2(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_772());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hnCompareWithLastFromAnotherBranchTest2", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnCompareWithLastFromAnotherBranchTest2", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_772 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_772() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h2.Add(h3);
                Paragraph secH1 = new Paragraph("Second header level 1");
                secH1.SetFont(PdfUAHeadingsTest.LoadFont());
                secH1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                h3.Add(secH1);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnParallelSequenceTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_809());
            framework.AddSuppliers(new _Generator_818());
            framework.AddSuppliers(new _Generator_827());
            framework.AssertBothValid("hnParallelSequenceTest", pdfUAConformance);
        }

        private sealed class _Generator_809 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_809() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
        }

        private sealed class _Generator_818 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_818() {
            }

            public IBlockElement Generate() {
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                return h2;
            }
        }

        private sealed class _Generator_827 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_827() {
            }

            public IBlockElement Generate() {
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                return h3;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void UsualHTest(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document doc = new Document(pdfDoc);
                Paragraph header = new Paragraph("Header");
                header.SetFont(LoadFont());
                header.GetAccessibilityProperties().SetRole(StandardRoles.H);
                doc.Add(header);
                Div div = new Div();
                div.SetHeight(50);
                div.SetWidth(50);
                div.SetBackgroundColor(ColorConstants.CYAN);
                Paragraph header2 = new Paragraph("Header 2");
                header2.SetFont(LoadFont());
                header2.GetAccessibilityProperties().SetRole(StandardRoles.H);
                div.Add(header2);
                doc.Add(div);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertOnlyVeraPdfFail("usualHTest", pdfUAConformance);
            }
            else {
                framework.AssertBothFail("usualHTest", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG, pdfUAConformance
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void UsualHTest2(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_873());
            framework.AddSuppliers(new _Generator_882());
            // The test code is the same as in usualHTest with one exception:
            // the next line where another grouping element is defined.
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hnParallelSequenceTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("hnParallelSequenceTest", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG, pdfUAConformance
                        );
                }
            }
        }

        private sealed class _Generator_873 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_873() {
            }

            public IBlockElement Generate() {
                Paragraph header = new Paragraph("Header");
                header.SetFont(PdfUAHeadingsTest.LoadFont());
                header.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header;
            }
        }

        private sealed class _Generator_882 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_882() {
            }

            public IBlockElement Generate() {
                Div div = new Div();
                div.SetHeight(50);
                div.SetWidth(50);
                div.SetBackgroundColor(ColorConstants.CYAN);
                div.GetAccessibilityProperties().SetRole(StandardRoles.SECT);
                Paragraph header2 = new Paragraph("Header 2");
                header2.SetFont(PdfUAHeadingsTest.LoadFont());
                header2.GetAccessibilityProperties().SetRole(StandardRoles.H);
                div.Add(header2);
                return div;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnMixedSequenceTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_912());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hnMixedSequenceTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnMixedSequenceTest", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_912 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_912() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Div div = new Div();
                div.SetHeight(50);
                div.SetWidth(50);
                div.SetBackgroundColor(ColorConstants.CYAN);
                h1.Add(div);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h1.Add(h3);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnMixedSequenceTest2(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_950());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hnMixedSequenceTest2", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnMixedSequenceTest2", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_950 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_950() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h1.Add(h3);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnMixedSequenceTest3(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_982());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hnMixedSequenceTest3", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "Div");
                    framework.AssertBothFail("hnMixedSequenceTest3", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_982 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_982() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Div div = new Div();
                div.SetBackgroundColor(ColorConstants.CYAN);
                h1.Add(div);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                div.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                div.Add(h3);
                return h1;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NonSequentialHeadersTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1018());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("nonSequentialHeadersTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("nonSequentialHeadersTest", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1018 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1018() {
            }

            public IBlockElement Generate() {
                Div div = new Div();
                div.SetBackgroundColor(ColorConstants.CYAN);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                div.Add(h2);
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                div.Add(h1);
                return h2;
            }
        }
    }
}

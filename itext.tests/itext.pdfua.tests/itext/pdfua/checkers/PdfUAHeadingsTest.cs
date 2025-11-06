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

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

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

        // -------- Negative tests --------
        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddH2AsFirstHeaderTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_88());
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

        private sealed class _Generator_88 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_88() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_111());
            framework.AddSuppliers(new _Generator_120());
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

        private sealed class _Generator_111 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_111() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
        }

        private sealed class _Generator_120 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_120() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_142());
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

        private sealed class _Generator_142 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_142() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_171());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("brokenHnMixedSequenceTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                    .HN_IS_SKIPPED, 3), pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String expectedMessage = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("brokenHnMixedSequenceTest", expectedMessage, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_171 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_171() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_205());
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

        private sealed class _Generator_205 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_205() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_243());
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

        private sealed class _Generator_243 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_243() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_275());
            framework.AddSuppliers(new _Generator_284());
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

        private sealed class _Generator_275 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_275() {
            }

            public IBlockElement Generate() {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(PdfUAHeadingsTest.LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
        }

        private sealed class _Generator_284 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_284() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_307());
            framework.AddSuppliers(new _Generator_316());
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

        private sealed class _Generator_307 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_307() {
            }

            public IBlockElement Generate() {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(PdfUAHeadingsTest.LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
        }

        private sealed class _Generator_316 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_316() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_371());
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

        private sealed class _Generator_371 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_371() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_403());
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

        private sealed class _Generator_403 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_403() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_444());
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

        private sealed class _Generator_444 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_444() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_508());
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

        private sealed class _Generator_508 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_508() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_530());
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

        private sealed class _Generator_530 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_530() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_562());
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

        private sealed class _Generator_562 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_562() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_657());
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

        private sealed class _Generator_657 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_657() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_690());
            framework.AddSuppliers(new _Generator_714());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hnCompareWithLastFromAnotherBranchTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnCompareWithLastFromAnotherBranchTest", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_690 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_690() {
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

        private sealed class _Generator_714 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_714() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_737());
            framework.AddSuppliers(new _Generator_761());
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

        private sealed class _Generator_737 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_737() {
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

        private sealed class _Generator_761 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_761() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_784());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("hnInheritedSequenceTest2", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnInheritedSequenceTest2", message, pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_784 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_784() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_822());
            framework.AddSuppliers(new _Generator_831());
            framework.AddSuppliers(new _Generator_840());
            framework.AssertBothValid("hnParallelSequenceTest", pdfUAConformance);
        }

        private sealed class _Generator_822 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_822() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
        }

        private sealed class _Generator_831 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_831() {
            }

            public IBlockElement Generate() {
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                return h2;
            }
        }

        private sealed class _Generator_840 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_840() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_888());
            framework.AddSuppliers(new _Generator_897());
            // The test code is the same as in usualHTest with one exception:
            // the next line where another grouping element is defined.
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("usualHTest2", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("usualHTest2", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG, pdfUAConformance
                        );
                }
            }
        }

        private sealed class _Generator_888 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_888() {
            }

            public IBlockElement Generate() {
                Paragraph header = new Paragraph("Header");
                header.SetFont(PdfUAHeadingsTest.LoadFont());
                header.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header;
            }
        }

        private sealed class _Generator_897 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_897() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_928());
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

        private sealed class _Generator_928 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_928() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_967());
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

        private sealed class _Generator_967 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_967() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1000());
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

        private sealed class _Generator_1000 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1000() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1037());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("nonSequentialHeadersTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("nonSequentialHeadersTest", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1037 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1037() {
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

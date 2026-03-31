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

        public static IList<PdfConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        // -------- Negative tests --------
        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddH2AsFirstHeaderTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                return h2;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("addH2FirstHeaderTest", PdfUAExceptionMessageConstants.H1_IS_SKIPPED);
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothValid("addH2FirstHeaderTest");
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void BrokenHnParallelSequenceTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
            );
            framework.AddSuppliers((document) => {
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                return h3;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("brokenHnParallelSequenceTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                    .HN_IS_SKIPPED, 2));
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothValid("brokenHnParallelSequenceTest");
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void BrokenHnInheritedSequenceTest1(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h1.Add(h3);
                return h1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("brokenHnInheritedSequenceTest1", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                    .HN_IS_SKIPPED, 2));
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String expectedMessage = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H3");
                    framework.AssertBothFail("brokenHnInheritedSequenceTest1", expectedMessage);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void BrokenHnMixedSequenceTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h5 = new Paragraph("Header level 5");
                h5.SetFont(LoadFont());
                h5.GetAccessibilityProperties().SetRole(StandardRoles.H5);
                h1.Add(h5);
                return h1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("brokenHnMixedSequenceTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                    .HN_IS_SKIPPED, 3));
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String expectedMessage = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("brokenHnMixedSequenceTest", expectedMessage);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void BrokenHnMixedSequenceTest2(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Div div = new Div();
                div.SetBackgroundColor(ColorConstants.CYAN);
                h1.Add(div);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                div.Add(h2);
                Paragraph h5 = new Paragraph("Header level 5");
                h5.SetFont(LoadFont());
                h5.GetAccessibilityProperties().SetRole(StandardRoles.H5);
                div.Add(h5);
                return h1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("brokenHnMixedSequenceTest2", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                    .HN_IS_SKIPPED, 3));
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "Div");
                    framework.AssertBothFail("brokenHnMixedSequenceTest2", message);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void FewHInOneNodeTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Div div = new Div();
                div.SetBackgroundColor(ColorConstants.CYAN);
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                div.Add(header1);
                Paragraph header2 = new Paragraph("Header");
                header2.SetFont(LoadFont());
                header2.GetAccessibilityProperties().SetRole(StandardRoles.H);
                div.Add(header2);
                return div;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("fewHInOneNodeTest", PdfUAExceptionMessageConstants.MORE_THAN_ONE_H_TAG);
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothFail("fewHInOneNodeTest", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void FewHInDocumentTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
            );
            framework.AddSuppliers((document) => {
                Paragraph header2 = new Paragraph("Header");
                header2.SetFont(LoadFont());
                header2.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header2;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("fewHInDocumentTest", PdfUAExceptionMessageConstants.MORE_THAN_ONE_H_TAG);
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothFail("fewHInDocumentTest", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HAndHnInDocumentTest1(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
            );
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("hAndHnInDocumentTest1", PdfUAExceptionMessageConstants.DOCUMENT_USES_BOTH_H_AND_HN
                    );
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothFail("hAndHnInDocumentTest1", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HAndHnInDocumentTest2(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
            );
            framework.AddSuppliers((document) => {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("hAndHnInDocumentTest2", PdfUAExceptionMessageConstants.DOCUMENT_USES_BOTH_H_AND_HN
                    );
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothFail("hAndHnInDocumentTest2", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HAndHnInDocumentTest3(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                h2.Add(header1);
                return h1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("hAndHnInDocumentTest3", PdfUAExceptionMessageConstants.DOCUMENT_USES_BOTH_H_AND_HN
                    );
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothFail("hAndHnInDocumentTest3", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void RoleMappingTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole("header1");
                Paragraph h2 = new Paragraph("Header level 5");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole("header5");
                h1.Add(h2);
                return h1;
            }
            );
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
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
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("rolemappingTest");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "header5");
                    framework.AssertBothFail("rolemappingTest", message);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void RoleMappingTestValid(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole("header1");
                Paragraph h2 = new Paragraph("Header level 5");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole("header5");
                h1.Add(h2);
                return h1;
            }
            );
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
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
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothValid("rolemappingValid");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "header5");
                    framework.AssertBothFail("rolemappingValid", message);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void DirectWritingToCanvasTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("directWritingToCanvas", PdfUAExceptionMessageConstants.H1_IS_SKIPPED);
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothValid("directWritingToCanvas");
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HInDocumentTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothValid("hInDocumentTest");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothFail("hInDocumentTest", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HAndHnInDocumentTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                h2.Add(header1);
                return h1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("hAndHnInDocumentTest");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothFail("hAndHnInDocumentTest", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void IncorrectHeadingLevelInUA2Test(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Div div = new Div();
                div.SetBackgroundColor(ColorConstants.CYAN);
                Paragraph h2 = new Paragraph("1.2 Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                div.Add(h2);
                Paragraph h1 = new Paragraph("1.2.3 Header level 3");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                div.Add(h1);
                return h2;
            }
            );
            // Where a heading’s level is evident, the heading level of the structure element enclosing it shall match that
            // heading level, e.g. a heading with the real content “5.1.6.4 Some header” is evidently at heading level 4.
            // This requirement is not checked by both iText and veraPDF.
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("incorrectHeadingLevelInUA2Test");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothValid("incorrectHeadingLevelInUA2Test");
                }
            }
        }

        // -------- Positive tests --------
        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(PdfUALogMessageConstants.PAGE_FLUSHING_DISABLED, Ignore = true)]
        public virtual void FlushPreviousPageTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothValid("hugeDocumentTest");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothFail("hugeDocumentTest", MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2"));
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnInheritedSequenceTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h2.Add(h3);
                return h1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothValid("hnInheritedSequenceTest");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnInheritedSequenceTest", message);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnCompareWithLastFromAnotherBranchTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h2.Add(h3);
                Paragraph h4 = new Paragraph("Header level 4");
                h4.SetFont(LoadFont());
                h4.GetAccessibilityProperties().SetRole(StandardRoles.H4);
                h2.Add(h4);
                return h1;
            }
            );
            framework.AddSuppliers((document) => {
                Paragraph h5 = new Paragraph("Second Header level 5 in doc");
                h5.SetFont(LoadFont());
                h5.GetAccessibilityProperties().SetRole(StandardRoles.H5);
                return h5;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothValid("hnCompareWithLastFromAnotherBranchTest");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnCompareWithLastFromAnotherBranchTest", message);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnCompareWithLastFromAnotherBranchTest2(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h2.Add(h3);
                Paragraph h4 = new Paragraph("Header level 4");
                h4.SetFont(LoadFont());
                h4.GetAccessibilityProperties().SetRole(StandardRoles.H4);
                h2.Add(h4);
                return h1;
            }
            );
            framework.AddSuppliers((document) => {
                Paragraph h33 = new Paragraph("Second Header level 3 in doc");
                h33.SetFont(LoadFont());
                h33.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                return h33;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothValid("hnCompareWithLastFromAnotherBranchTest2");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnCompareWithLastFromAnotherBranchTest2", message);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnInheritedSequenceTest2(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h2.Add(h3);
                Paragraph secH1 = new Paragraph("Second header level 1");
                secH1.SetFont(LoadFont());
                secH1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                h3.Add(secH1);
                return h1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothValid("hnInheritedSequenceTest2");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnInheritedSequenceTest2", message);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnParallelSequenceTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
            );
            framework.AddSuppliers((document) => {
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                return h2;
            }
            );
            framework.AddSuppliers((document) => {
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                return h3;
            }
            );
            framework.AssertBothValid("hnParallelSequenceTest");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void UsualHTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertOnlyVeraPdfFail("usualHTest");
            }
            else {
                framework.AssertBothFail("usualHTest", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void UsualHTest2(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph header = new Paragraph("Header");
                header.SetFont(LoadFont());
                header.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header;
            }
            );
            framework.AddSuppliers((document) => {
                Div div = new Div();
                div.SetHeight(50);
                div.SetWidth(50);
                div.SetBackgroundColor(ColorConstants.CYAN);
                // The test code is the same as in usualHTest with one exception:
                // the next line where another grouping element is defined.
                div.GetAccessibilityProperties().SetRole(StandardRoles.SECT);
                Paragraph header2 = new Paragraph("Header 2");
                header2.SetFont(LoadFont());
                header2.GetAccessibilityProperties().SetRole(StandardRoles.H);
                div.Add(header2);
                return div;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothValid("usualHTest2");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothFail("usualHTest2", PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnMixedSequenceTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Div div = new Div();
                div.SetHeight(50);
                div.SetWidth(50);
                div.SetBackgroundColor(ColorConstants.CYAN);
                h1.Add(div);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h1.Add(h3);
                return h1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothValid("hnMixedSequenceTest");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnMixedSequenceTest", message);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnMixedSequenceTest2(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                h1.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                h1.Add(h3);
                return h1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothValid("hnMixedSequenceTest2");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "H2");
                    framework.AssertBothFail("hnMixedSequenceTest2", message);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void HnMixedSequenceTest3(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Div div = new Div();
                div.SetBackgroundColor(ColorConstants.CYAN);
                h1.Add(div);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                div.Add(h2);
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                div.Add(h3);
                return h1;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothValid("hnMixedSequenceTest3");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "H1", "Div");
                    framework.AssertBothFail("hnMixedSequenceTest3", message);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NonSequentialHeadersTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Div div = new Div();
                div.SetBackgroundColor(ColorConstants.CYAN);
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                div.Add(h2);
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                div.Add(h1);
                return h2;
            }
            );
            if (conformance.Equals(PdfConformance.PDF_UA_1)) {
                framework.AssertBothFail("nonSequentialHeadersTest");
            }
            else {
                if (conformance.Equals(PdfConformance.PDF_UA_2)) {
                    framework.AssertBothValid("nonSequentialHeadersTest");
                }
            }
        }

        private static PdfFont LoadFont() {
            try {
                return PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e.Message);
            }
        }
    }
}

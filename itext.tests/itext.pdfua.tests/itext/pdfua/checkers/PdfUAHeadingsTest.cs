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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Pdfua.Logs;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Pdfa;

namespace iText.Pdfua.Checkers {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAHeadingsTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUAHeadingsTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUAHeadingsTest/";

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

        // -------- Negative tests --------
        [NUnit.Framework.Test]
        public virtual void AddH2AsFirstHeaderTest() {
            framework.AddSuppliers(new _Generator_81());
            framework.AssertBothFail("addH2FirstHeaderTest", PdfUAExceptionMessageConstants.H1_IS_SKIPPED);
        }

        private sealed class _Generator_81 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_81() {
            }

            public IBlockElement Generate() {
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                return h2;
            }
        }

        [NUnit.Framework.Test]
        public virtual void BrokenHnParallelSequenceTest() {
            framework.AddSuppliers(new _Generator_97());
            framework.AddSuppliers(new _Generator_106());
            framework.AssertBothFail("brokenHnParallelSequenceTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .HN_IS_SKIPPED, 2));
        }

        private sealed class _Generator_97 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_97() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
        }

        private sealed class _Generator_106 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_106() {
            }

            public IBlockElement Generate() {
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                return h3;
            }
        }

        [NUnit.Framework.Test]
        public virtual void BrokenHnInheritedSequenceTest1() {
            framework.AddSuppliers(new _Generator_121());
            framework.AssertBothFail("brokenHnInheritedSequenceTest1", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .HN_IS_SKIPPED, 2));
        }

        private sealed class _Generator_121 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_121() {
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

        [NUnit.Framework.Test]
        public virtual void BrokenHnMixedSequenceTest() {
            framework.AddSuppliers(new _Generator_141());
            framework.AssertBothFail("brokenHnMixedSequenceTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .HN_IS_SKIPPED, 3));
        }

        private sealed class _Generator_141 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_141() {
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

        [NUnit.Framework.Test]
        public virtual void BrokenHnMixedSequenceTest2() {
            framework.AddSuppliers(new _Generator_166());
            framework.AssertBothFail("brokenHnMixedSequenceTest2", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .HN_IS_SKIPPED, 3));
        }

        private sealed class _Generator_166 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_166() {
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

        [NUnit.Framework.Test]
        public virtual void FewHInOneNodeTest() {
            framework.AddSuppliers(new _Generator_195());
            framework.AssertBothFail("fewHInOneNodeTest", PdfUAExceptionMessageConstants.MORE_THAN_ONE_H_TAG);
        }

        private sealed class _Generator_195 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_195() {
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

        [NUnit.Framework.Test]
        public virtual void FewHInDocumentTest() {
            framework.AddSuppliers(new _Generator_220());
            framework.AddSuppliers(new _Generator_229());
            framework.AssertBothFail("fewHInDocumentTest", PdfUAExceptionMessageConstants.MORE_THAN_ONE_H_TAG);
        }

        private sealed class _Generator_220 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_220() {
            }

            public IBlockElement Generate() {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(PdfUAHeadingsTest.LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
        }

        private sealed class _Generator_229 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_229() {
            }

            public IBlockElement Generate() {
                Paragraph header2 = new Paragraph("Header");
                header2.SetFont(PdfUAHeadingsTest.LoadFont());
                header2.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header2;
            }
        }

        [NUnit.Framework.Test]
        public virtual void HAndHnInDocumentTest1() {
            framework.AddSuppliers(new _Generator_244());
            framework.AddSuppliers(new _Generator_253());
            framework.AssertBothFail("hAndHnInDocumentTest1", PdfUAExceptionMessageConstants.DOCUMENT_USES_BOTH_H_AND_HN
                );
        }

        private sealed class _Generator_244 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_244() {
            }

            public IBlockElement Generate() {
                Paragraph header1 = new Paragraph("Header");
                header1.SetFont(PdfUAHeadingsTest.LoadFont());
                header1.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header1;
            }
        }

        private sealed class _Generator_253 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_253() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
        }

        [NUnit.Framework.Test]
        public virtual void HAndHnInDocumentTest2() {
            framework.AddSuppliers(new _Generator_268());
            framework.AddSuppliers(new _Generator_277());
            framework.AssertBothFail("hAndHnInDocumentTest2", PdfUAExceptionMessageConstants.DOCUMENT_USES_BOTH_H_AND_HN
                );
        }

        private sealed class _Generator_268 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_268() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
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

        [NUnit.Framework.Test]
        public virtual void HAndHnInDocumentTest3() {
            framework.AddSuppliers(new _Generator_292());
            framework.AssertBothFail("hAndHnInDocumentTest3", PdfUAExceptionMessageConstants.DOCUMENT_USES_BOTH_H_AND_HN
                );
        }

        private sealed class _Generator_292 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_292() {
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

        [NUnit.Framework.Test]
        public virtual void RoleMappingTest() {
            framework.AddSuppliers(new _Generator_317());
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("header1", StandardRoles.H1);
                root.AddRoleMapping("header5", StandardRoles.H5);
            }
            );
            framework.AssertBothFail("rolemappingTest");
        }

        private sealed class _Generator_317 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_317() {
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

        [NUnit.Framework.Test]
        public virtual void RoleMappingTestValid() {
            framework.AddSuppliers(new _Generator_342());
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("header1", StandardRoles.H1);
                root.AddRoleMapping("header5", StandardRoles.H2);
            }
            );
            framework.AssertBothValid("rolemappingValid");
        }

        private sealed class _Generator_342 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_342() {
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

        [NUnit.Framework.Test]
        public virtual void DirectWritingToCanvasTest() {
            String outPdf = DESTINATION_FOLDER + "directWritingToCanvasTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            TagTreePointer pointer = new TagTreePointer(pdfDoc);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            pointer.SetPageForTagging(page);
            TagTreePointer tmp = pointer.AddTag(StandardRoles.H3);
            canvas.OpenTag(tmp.GetTagReference());
            canvas.WriteLiteral("Heading level 3");
            canvas.CloseTag();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.H1_IS_SKIPPED, e.Message);
        }

        // -------- Positive tests --------
        [NUnit.Framework.Test]
        [LogMessage(PdfUALogMessageConstants.PAGE_FLUSHING_DISABLED)]
        public virtual void FlushPreviousPageTest() {
            String outPdf = DESTINATION_FOLDER + "hugeDocumentTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_hugeDocumentTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void HnInheritedSequenceTest() {
            framework.AddSuppliers(new _Generator_447());
            framework.AssertBothValid("hnInheritedSequenceTest");
        }

        private sealed class _Generator_447 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_447() {
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

        [NUnit.Framework.Test]
        public virtual void HnCompareWithLastFromAnotherBranchTest() {
            framework.AddSuppliers(new _Generator_471());
            framework.AddSuppliers(new _Generator_495());
            framework.AssertBothValid("hnInheritedSequenceTest");
        }

        private sealed class _Generator_471 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_471() {
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

        private sealed class _Generator_495 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_495() {
            }

            public IBlockElement Generate() {
                Paragraph h5 = new Paragraph("Second Header level 5 in doc");
                h5.SetFont(PdfUAHeadingsTest.LoadFont());
                h5.GetAccessibilityProperties().SetRole(StandardRoles.H5);
                return h5;
            }
        }

        [NUnit.Framework.Test]
        public virtual void HnCompareWithLastFromAnotherBranchTest2() {
            framework.AddSuppliers(new _Generator_509());
            framework.AddSuppliers(new _Generator_533());
            framework.AssertBothValid("hnCompareWithLastFromAnotherBranchTest2");
        }

        private sealed class _Generator_509 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_509() {
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

        private sealed class _Generator_533 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_533() {
            }

            public IBlockElement Generate() {
                Paragraph h33 = new Paragraph("Second Header level 3 in doc");
                h33.SetFont(PdfUAHeadingsTest.LoadFont());
                h33.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                return h33;
            }
        }

        [NUnit.Framework.Test]
        public virtual void HnInheritedSequenceTest2() {
            framework.AddSuppliers(new _Generator_547());
            framework.AssertBothValid("hnCompareWithLastFromAnotherBranchTest2");
        }

        private sealed class _Generator_547 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_547() {
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

        [NUnit.Framework.Test]
        public virtual void HnParallelSequenceTest() {
            framework.AddSuppliers(new _Generator_576());
            framework.AddSuppliers(new _Generator_585());
            framework.AddSuppliers(new _Generator_594());
            framework.AssertBothValid("hnParallelSequenceTest");
        }

        private sealed class _Generator_576 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_576() {
            }

            public IBlockElement Generate() {
                Paragraph h1 = new Paragraph("Header level 1");
                h1.SetFont(PdfUAHeadingsTest.LoadFont());
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                return h1;
            }
        }

        private sealed class _Generator_585 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_585() {
            }

            public IBlockElement Generate() {
                Paragraph h2 = new Paragraph("Header level 2");
                h2.SetFont(PdfUAHeadingsTest.LoadFont());
                h2.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                return h2;
            }
        }

        private sealed class _Generator_594 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_594() {
            }

            public IBlockElement Generate() {
                Paragraph h3 = new Paragraph("Header level 3");
                h3.SetFont(PdfUAHeadingsTest.LoadFont());
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                return h3;
            }
        }

        [NUnit.Framework.Test]
        public virtual void UsualHTest() {
            String outPdf = DESTINATION_FOLDER + "usualHTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_usualHTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        // VeraPdf here throw exception that "A node contains more than one H tag", because
        // it seems that VeraPdf consider div as a not grouping element. See usualHTest2 test
        // with the same code, but div role is replaced by section role
        [NUnit.Framework.Test]
        public virtual void UsualHTest2() {
            framework.AddSuppliers(new _Generator_642());
            framework.AddSuppliers(new _Generator_651());
            // The test code is the same as in usualHTest with one exception:
            // the next line where another grouping element is defined.
            framework.AssertBothValid("hnParallelSequenceTest");
        }

        private sealed class _Generator_642 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_642() {
            }

            public IBlockElement Generate() {
                Paragraph header = new Paragraph("Header");
                header.SetFont(PdfUAHeadingsTest.LoadFont());
                header.GetAccessibilityProperties().SetRole(StandardRoles.H);
                return header;
            }
        }

        private sealed class _Generator_651 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_651() {
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

        [NUnit.Framework.Test]
        public virtual void HnMixedSequenceTest() {
            framework.AddSuppliers(new _Generator_674());
            framework.AssertBothValid("hnMixedSequenceTest");
        }

        private sealed class _Generator_674 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_674() {
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

        [NUnit.Framework.Test]
        public virtual void HnMixedSequenceTest2() {
            framework.AddSuppliers(new _Generator_704());
            framework.AssertBothValid("hnMixedSequenceTest2");
        }

        private sealed class _Generator_704 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_704() {
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

        [NUnit.Framework.Test]
        public virtual void HnMixedSequenceTest3() {
            framework.AddSuppliers(new _Generator_728());
            framework.AssertBothValid("hnMixedSequenceTest3");
        }

        private sealed class _Generator_728 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_728() {
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

        private static PdfFont LoadFont() {
            try {
                return PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message);
            }
        }
    }
}

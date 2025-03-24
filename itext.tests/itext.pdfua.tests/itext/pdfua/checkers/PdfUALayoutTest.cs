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
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfua.Checkers {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUALayoutTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUALayoutTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUALayoutTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return JavaUtil.ArraysAsList(PdfUAConformance.PDF_UA_1, PdfUAConformance.PDF_UA_2);
        }

        public static Object[] RoleData() {
            return new Object[] { new Object[] { StandardRoles.FORM, StandardRoles.FORM, 
                        // Parent role, child role, expected exception
                        false }, new Object[] { StandardRoles.H1, StandardRoles.H1, true }, new Object[] { StandardRoles.P, StandardRoles
                .P, false }, new Object[] { StandardRoles.DIV, StandardRoles.P, false } };
        }

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleParagraphTest() {
            String outPdf = DESTINATION_FOLDER + "simpleParagraphTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_simpleParagraphTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("Simple layout PDF/UA-1 test").SetFont(font));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void SimpleParagraphWithUnderlineTest() {
            String outPdf = DESTINATION_FOLDER + "simpleParagraphUnderlinesTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_simpleParagraphWithUnderlineTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("Simple layout PDF/UA-1 with underline test").SetFont(font).SetUnderline());
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.TestCaseSource("RoleData")]
        public virtual void TestOfIllegalRelations(String parentRole, String childRole, bool expectException) {
            //expectException should take into account repair mechanism
            // in example P:P will be replaced as P:Span so no exceptions should be thrown
            framework.AddSuppliers(new _Generator_135(parentRole, childRole));
            if (expectException) {
                framework.AssertBothFail("testOfIllegalRelation_" + parentRole + "_" + childRole, false, PdfUAConformance.
                    PDF_UA_2);
            }
            else {
                framework.AssertBothValid("testOfIllegalRelation_" + parentRole + "_" + childRole, PdfUAConformance.PDF_UA_2
                    );
            }
        }

        private sealed class _Generator_135 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_135(String parentRole, String childRole) {
                this.parentRole = parentRole;
                this.childRole = childRole;
            }

            public IBlockElement Generate() {
                Div div1 = new Div();
                div1.GetAccessibilityProperties().SetRole(parentRole);
                Div div2 = new Div();
                div2.GetAccessibilityProperties().SetRole(childRole);
                div1.Add(div2);
                return div1;
            }

            private readonly String parentRole;

            private readonly String childRole;
        }

        [NUnit.Framework.Test]
        public virtual void SimpleBorderTest() {
            String outPdf = DESTINATION_FOLDER + "simpleBorderTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_simpleBorderTest.pdf";
            using (PdfDocument pdfDocument = new PdfUATestPdfDocument(new PdfWriter(outPdf))) {
                PdfPage page = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.OpenTag(new CanvasTag(PdfName.Artifact));
                new DottedBorder(DeviceRgb.GREEN, 5).Draw(canvas, new Rectangle(350, 700, 100, 100));
                canvas.CloseTag();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void SimpleTableTest() {
            String outPdf = DESTINATION_FOLDER + "simpleTableTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_simpleTableTest.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            Document doc = new Document(pdfDoc);
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            Table table = new Table(new float[] { 50, 50 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1").SetFont(
                font))).AddCell(new Cell().Add(new Paragraph("cell 1, 2").SetFont(font)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }
        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    }
}

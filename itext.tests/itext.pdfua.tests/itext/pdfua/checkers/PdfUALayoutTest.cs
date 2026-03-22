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
using System.IO;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Contrast;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Validation;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUALayoutTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUALayoutTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        public static IList<Object[]> RoleData() {
            IList<Object[]> data = new List<Object[]>();
            foreach (PdfConformance pdfConformance in UaValidationTestFramework.GetConformanceList()) {
                foreach (Object o in new Object[] { new Object[] { StandardRoles.FORM, StandardRoles.FORM, 
                                // Parent role, child role, expected exception
                                false }, new Object[] { StandardRoles.H1, StandardRoles.H1, true }, new Object[] { StandardRoles.P, StandardRoles
                    .P, false }, new Object[] { StandardRoles.DIV, StandardRoles.P, false } }) {
                    Object[] roles = (Object[])o;
                    data.Add(new Object[] { pdfConformance, roles[0], roles[1], roles[2] });
                }
            }
            return data;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SimpleParagraphTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = LoadFont();
                Document doc = new Document(pdfDoc);
                doc.Add(new Paragraph("Simple layout PDF UA test").SetFont(font));
            }
            );
            framework.AssertBothValid("simpleParagraph");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SimpleParagraphWithUnderlineTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = LoadFont();
                Document doc = new Document(pdfDoc);
                doc.Add(new Paragraph("Simple layout PDF UA with underline test").SetFont(font).SetUnderline());
            }
            );
            framework.AssertBothValid("simpleParagraphWithUnderline");
        }

        [NUnit.Framework.TestCaseSource("RoleData")]
        public virtual void TestOfIllegalRelations(PdfConformance conformance, String parentRole, String childRole
            , bool expectException) {
            if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_1) {
                return;
            }
            //expectException should take into account repair mechanism
            // in example P:P will be replaced as P:Span so no exceptions should be thrown
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Div div1 = new Div();
                div1.GetAccessibilityProperties().SetRole(parentRole);
                Div div2 = new Div();
                div2.GetAccessibilityProperties().SetRole(childRole);
                div1.Add(div2);
                return div1;
            }
            );
            if (expectException) {
                framework.AssertBothFail("testOfIllegalRelation_" + parentRole + "_" + childRole, false);
            }
            else {
                framework.AssertBothValid("testOfIllegalRelation_" + parentRole + "_" + childRole);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SimpleBorderTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfPage page = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.OpenTag(new CanvasTag(PdfName.Artifact));
                new DottedBorder(DeviceRgb.GREEN, 5).Draw(canvas, new Rectangle(350, 700, 100, 100));
                canvas.CloseTag();
            }
            );
            framework.AssertBothValid("simpleBorder");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SimpleTableTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                Document doc = new Document(pdfDocument);
                PdfFont font = LoadFont();
                Table table = new Table(new float[] { 50, 50 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1").SetFont(
                    font))).AddCell(new Cell().Add(new Paragraph("cell 1, 2").SetFont(font)));
                doc.Add(table);
            }
            );
            framework.AssertBothValid("simpleTable");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SimpleParagraphBadContrastThrowsWcagAAAU(PdfConformance conformance) {
            if (!conformance.IsPdfUA()) {
                return;
            }
            PdfDocument pdfDoc = new _PdfUADocument_189(new PdfWriter(new MemoryStream()), new PdfUAConfig(conformance
                .GetUAConformance(), "Hello", "en-US"));
            PdfFont font = LoadFont();
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph("Simple layout PDF UA test").SetFont(font);
            p.SetBackgroundColor(ColorConstants.RED);
            doc.Add(p);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                pdfDoc.Close();
            }
            );
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("not WCAG AAA compliant"));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("has contrast ratio: 5"));
        }

        private sealed class _PdfUADocument_189 : PdfUADocument {
            public _PdfUADocument_189(PdfWriter baseArg1, PdfUAConfig baseArg2)
                : base(baseArg1, baseArg2) {
            }

            protected internal override IList<IValidationChecker> CreateCheckers(PdfUAConformance conformance) {
                ColorContrastChecker contrastChecker = new ColorContrastChecker(false, true);
                contrastChecker.SetCheckWcagAA(false);
                IList<IValidationChecker> validationCheckers = new List<IValidationChecker>();
                validationCheckers.Add(contrastChecker);
                return validationCheckers;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SimpleParagraphBadContrastThrowsWcagAA(PdfConformance conformance) {
            if (!conformance.IsPdfUA()) {
                return;
            }
            PdfUADocument pdfDoc = new _PdfUADocument_218(new PdfWriter(new MemoryStream()), new PdfUAConfig(conformance
                .GetUAConformance(), "Hello", "en-US"));
            PdfFont font = LoadFont();
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph("Simple layout PDF UA test").SetFont(font);
            p.SetFontColor(ColorConstants.PINK);
            p.SetBackgroundColor(ColorConstants.RED);
            doc.Add(p);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                pdfDoc.Close();
            }
            );
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("not WCAG AA compliant"));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("has contrast ratio: 2"));
        }

        private sealed class _PdfUADocument_218 : PdfUADocument {
            public _PdfUADocument_218(PdfWriter baseArg1, PdfUAConfig baseArg2)
                : base(baseArg1, baseArg2) {
            }

            protected internal override IList<IValidationChecker> CreateCheckers(PdfUAConformance uaConformance) {
                ColorContrastChecker contrastChecker = new ColorContrastChecker(false, true);
                contrastChecker.SetCheckWcagAAA(false);
                contrastChecker.SetCheckWcagAA(true);
                IList<IValidationChecker> validationCheckers = new List<IValidationChecker>();
                validationCheckers.Add(contrastChecker);
                return validationCheckers;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage("Page 1: Text: 'Simple layout PDF UA test', with font size: {0} pt " + "has contrast ratio: {1}. It is not WCAG AAA compliant. "
            , Count = 2)]
        public virtual void SimpleParagraphBadContrastLogsByDefaultTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = LoadFont();
                Document doc = new Document(pdfDoc);
                Paragraph p = new Paragraph("Simple layout PDF UA test").SetFont(font);
                p.SetBackgroundColor(ColorConstants.RED);
                doc.Add(p);
            }
            );
            framework.AssertBothValid("simpleParagraphAbc");
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

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
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties.Margins;
using iText.Layout.Testutil;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FootnoteImageFixedPositionTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FootnoteImageFixedPositionTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/FootnoteImageFixedPositionTest/";

        private static readonly float A4_HEIGHT = PageSize.A4.GetHeight();

        private static readonly float A4_WIDTH = PageSize.A4.GetWidth();

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void FixedPositionOnTextFootnoteRenderTest() {
            String fileName = "fixedPositionOnTextFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    footnote.SetFixedPosition(100, A4_HEIGHT / 2f, 300);
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FixedPositionOnTextFootnoteOutsidePageRenderTest() {
            String fileName = "fixedPositionOnTextFootnoteOutsidePage";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    footnote.SetFixedPosition(A4_WIDTH + 50f, A4_HEIGHT + 50f, 200);
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    Paragraph p = new Paragraph("Anchor with footnote fixed outside page.").Add(anchor);
                    document.Add(p);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FixedPositionOnTextFootnoteHugeContentTest() {
            String fileName = "fixedPositionOnTextFootnoteHugeContent";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.RepeatString(TestResourceUtil.GetByronStanza(), 6));
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    footnote.SetFixedPosition(36, 100, A4_WIDTH - 72f);
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor);
                    Div div = new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FixedPositionOnImageFootnoteRenderTest() {
            String fileName = "fixedPositionOnImageFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    iText.Layout.Element.Image image = new Image(ImageDataFactory.Create(SOURCE_FOLDER + "bee.png"));
                    image.SetWidth(80);
                    Footnote footnote = new Footnote(new Paragraph().Add(image));
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    footnote.SetFixedPosition(100, 150, 200);
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FixedPositionOnLargeImageFootnoteRenderTest() {
            String fileName = "fixedPositionOnLargeImageFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + 
                        "bee.png"));
                    image.SetWidth(A4_WIDTH * 0.70f);
                    image.SetHeight(A4_HEIGHT * 0.60f);
                    Footnote footnote = new Footnote(new Paragraph().Add(image));
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    footnote.SetFixedPosition(50, 80, A4_WIDTH * 0.70f);
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor);
                    document.Add(new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FixedPositionOnTextFootnoteAnchorRenderTest() {
            String fileName = "fixedPositionOnTextFootnoteAnchor";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    anchor.SetFixedPosition(200, A4_HEIGHT * 0.60f, 150);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FixedPositionOnTextFootnoteAnchorOutsidePageRenderTest() {
            String fileName = "fixedPositionOnTextFootnoteAnchorOutsidePage";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    anchor.SetFixedPosition(A4_WIDTH + 100f, A4_HEIGHT + 100f, 100);
                    Paragraph p = new Paragraph("Paragraph with anchor fixed outside page.").Add(anchor);
                    document.Add(p);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FixedPositionOnTextFootnoteAnchorAndFootnoteRenderTest() {
            String fileName = "fixedPositionOnBothAnchorAndFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    footnote.SetFixedPosition(50, 200, 250);
                    FootnoteAnchor anchor = new FootnoteAnchor("[1]", footnote);
                    anchor.SetFixedPosition(300, A4_HEIGHT * 0.70f, 100);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FixedPositionOnImageFootnoteAnchorRenderTest() {
            String fileName = "fixedPositionOnImageFootnoteAnchor";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + 
                        "bee.png"));
                    image.SetWidth(15);
                    FootnoteAnchor anchor = new FootnoteAnchor(image, footnote);
                    anchor.SetFixedPosition(200, A4_HEIGHT * 0.55f, 80);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 3)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FixedPositionOnLargeImageFootnoteAnchorRenderTest() {
            String fileName = "fixedPositionOnLargeImageFootnoteAnchor";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Footnote footnote = new Footnote(TestResourceUtil.GetByronStanza());
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    iText.Layout.Element.Image largeImage = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER
                         + "bee.png"));
                    largeImage.SetWidth(A4_WIDTH * 0.70f);
                    largeImage.SetHeight(A4_HEIGHT * 0.60f);
                    FootnoteAnchor anchor = new FootnoteAnchor(largeImage, footnote);
                    anchor.SetFixedPosition(36, 200, A4_WIDTH * 0.70f);
                    Paragraph p = new Paragraph().Add(anchor);
                    document.Add(new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2)));
                    document.Add(new Paragraph("Content after large image anchor with fixed position."));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void FixedPositionOnImageFootnoteAnchorAndImageFootnoteRenderTest() {
            String fileName = "fixedPositionOnImageAnchorAndImageFootnote";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    iText.Layout.Element.Image footnoteImage = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER
                         + "bee.png"));
                    footnoteImage.SetWidth(120);
                    Footnote footnote = new Footnote(new Paragraph().Add(footnoteImage));
                    footnote.SetBorder(new DashedBorder(ColorConstants.YELLOW, 3));
                    footnote.SetFixedPosition(50, 150, 200);
                    iText.Layout.Element.Image anchorImage = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER
                         + "bee.png"));
                    anchorImage.SetWidth(20);
                    FootnoteAnchor anchor = new FootnoteAnchor(anchorImage, footnote);
                    anchor.SetFixedPosition(300, A4_HEIGHT * 0.65f, 100);
                    Paragraph p = new Paragraph(TestResourceUtil.GetByronStanza()).Add(anchor).Add(TestResourceUtil.GetByronStanza
                        ());
                    document.Add(new Div().Add(p).SetBorder(new SolidBorder(ColorConstants.GREEN, 2)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }
    }
}

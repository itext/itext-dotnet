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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ParagraphTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ParagraphTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ParagraphTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CannotPlaceABigChunkOnALineTest01() {
            String outFileName = destinationFolder + "cannotPlaceABigChunkOnALineTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_cannotPlaceABigChunkOnALineTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(ColorConstants.YELLOW, 0));
            p.Add(new Text("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa").SetBorder
                (new SolidBorder(ColorConstants.RED, 0)));
            p.Add(new Text("b").SetFontSize(100).SetBorder(new SolidBorder(ColorConstants.BLUE, 0)));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CannotPlaceABigChunkOnALineTest02() {
            String outFileName = destinationFolder + "cannotPlaceABigChunkOnALineTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_cannotPlaceABigChunkOnALineTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(ColorConstants.YELLOW, 0));
            p.Add(new Text("smaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaall").SetFontSize(5).SetBorder
                (new SolidBorder(ColorConstants.RED, 0)));
            p.Add(new Text("biiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiig"
                ).SetFontSize(20).SetBorder(new SolidBorder(ColorConstants.BLUE, 0)));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ForceOverflowForTextRendererPartialResult01() {
            String outFileName = destinationFolder + "forceOverflowForTextRendererPartialResult01.pdf";
            String cmpFileName = sourceFolder + "cmp_forceOverflowForTextRendererPartialResult01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(ColorConstants.YELLOW, 0)).SetTextAlignment(TextAlignment
                .RIGHT);
            for (int i = 0; i < 5; i++) {
                p.Add(new Text("aaaaaaaaaaaaaaaaaaaaa" + i).SetBorder(new SolidBorder(ColorConstants.BLUE, 0)));
            }
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.RECTANGLE_HAS_NEGATIVE_OR_ZERO_SIZES, LogLevel = LogLevelConstants
            .INFO)]
        public virtual void WordWasSplitAndItWillFitOntoNextLineTest02() {
            // TODO DEVSIX-4622
            String outFileName = destinationFolder + "wordWasSplitAndItWillFitOntoNextLineTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_wordWasSplitAndItWillFitOntoNextLineTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().Add(new Text("Short").SetBackgroundColor(ColorConstants.YELLOW)).Add
                (new Text(" Loooooooooooooooooooong").SetBackgroundColor(ColorConstants.RED)).SetWidth(90).SetBorder(new 
                SolidBorder(1));
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphUsingSvgRenderingModeTest() {
            String outFileName = destinationFolder + "paragraphUsingSvgRenderingMode.pdf";
            String cmpFileName = sourceFolder + "cmp_paragraphUsingSvgRenderingMode.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    Paragraph paragraph1 = new Paragraph().SetBorder(new SolidBorder(ColorConstants.YELLOW, 1));
                    paragraph1.SetWidth(200).SetHorizontalAlignment(HorizontalAlignment.RIGHT);
                    Paragraph paragraph2 = new Paragraph().SetBorder(new SolidBorder(ColorConstants.PINK, 1));
                    paragraph2.SetWidth(200).SetHorizontalAlignment(HorizontalAlignment.RIGHT);
                    paragraph2.SetProperty(Property.RENDERING_MODE, RenderingMode.SVG_MODE);
                    for (int i = 0; i < 5; i++) {
                        Text textChunk = new Text("text" + i).SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                        textChunk.SetRelativePosition(-70 * i, 0, 0, 0);
                        paragraph1.Add(textChunk);
                        paragraph2.Add(textChunk);
                    }
                    document.Add(new Paragraph("Default rendering mode:"));
                    document.Add(paragraph1);
                    document.Add(new Paragraph("SVG rendering mode:"));
                    document.Add(paragraph2);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}

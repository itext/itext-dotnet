/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Text;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OverflowTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/OverflowTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/OverflowTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void TextOverflowTest01() {
            String outFileName = destinationFolder + "textOverflowTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_textOverflowTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < 1000; i++) {
                text.Append("This is a waaaaay tooo long text...");
            }
            Paragraph p = new Paragraph(text.ToString()).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TextOverflowTest02() {
            String outFileName = destinationFolder + "textOverflowTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_textOverflowTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            iText.Layout.Element.Text overflowText = new iText.Layout.Element.Text("This is a long-long and large text which will not overflow"
                ).SetFontSize(19).SetFontColor(ColorConstants.RED);
            iText.Layout.Element.Text followText = new iText.Layout.Element.Text("This is a text which follows overflowed text and will be wrapped"
                );
            document.Add(new Paragraph().Add(overflowText).Add(followText));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TextOverflowTest03() {
            String outFileName = destinationFolder + "textOverflowTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_textOverflowTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            iText.Layout.Element.Text overflowText = new iText.Layout.Element.Text("This is a long-long and large text which will overflow"
                ).SetFontSize(25).SetFontColor(ColorConstants.RED);
            iText.Layout.Element.Text followText = new iText.Layout.Element.Text("This is a text which follows overflowed text and will not be wrapped"
                );
            document.Add(new Paragraph().Add(overflowText).Add(followText));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TextOverflowTest04() {
            String outFileName = destinationFolder + "textOverflowTest04.pdf";
            String cmpFileName = sourceFolder + "cmp_textOverflowTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("ThisIsALongTextWithNoSpacesSoSplittingShouldBeForcedInThisCase").SetFontSize(20
                ));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void AlignedInlineContentOverflowHiddenTest01() {
            String outFileName = destinationFolder + "alignedInlineContentOverflowHiddenTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_alignedInlineContentOverflowHiddenTest01.pdf";
            String imgPath = sourceFolder + "itis.jpg";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div div = new Div().SetHeight(150f).SetWidth(150f).SetBorder(new SolidBorder(5f));
            div.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.HIDDEN);
            div.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.HIDDEN);
            iText.Layout.Element.Image img = new Image(ImageDataFactory.Create(imgPath));
            Paragraph p = new Paragraph().SetTextAlignment(TextAlignment.CENTER);
            p.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            p.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            img.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            img.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            document.Add(div.Add(p.Add(img)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void AlignedInlineContentOverflowHiddenTest02() {
            String outFileName = destinationFolder + "alignedInlineContentOverflowHiddenTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_alignedInlineContentOverflowHiddenTest02.pdf";
            String imgPath = sourceFolder + "itis.jpg";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(imgPath));
            Paragraph p = new Paragraph().SetTextAlignment(TextAlignment.CENTER).SetHeight(150f).SetWidth(150f).SetBorder
                (new SolidBorder(5f));
            p.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.HIDDEN);
            p.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.HIDDEN);
            img.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            img.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            document.Add(p.Add(img));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void OverflowHiddenOnCanvasTest01() {
            String outFileName = destinationFolder + "overflowHiddenOnCanvasTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_overflowHiddenOnCanvasTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            PdfPage page = pdfDocument.AddNewPage();
            iText.Layout.Canvas canvas = new Canvas(new PdfCanvas(page), page.GetPageSize().Clone().ApplyMargins(36, 36
                , 36, 36, false));
            AddParaWithImgSetOverflowX(canvas, OverflowPropertyValue.HIDDEN);
            canvas.Close();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void OverflowHiddenOnCanvasTest02() {
            String outFileName = destinationFolder + "overflowHiddenOnCanvasTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_overflowHiddenOnCanvasTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            PdfPage page = pdfDocument.AddNewPage();
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(page, page.GetPageSize().Clone().ApplyMargins(36, 36, 
                36, 36, false));
            AddParaWithImgSetOverflowX(canvas, OverflowPropertyValue.HIDDEN);
            canvas.Close();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void OverflowVisibleOnCanvasTest01() {
            String outFileName = destinationFolder + "overflowVisibleOnCanvasTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_overflowVisibleOnCanvasTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            PdfPage page = pdfDocument.AddNewPage();
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(new PdfCanvas(page), page.GetPageSize().Clone().ApplyMargins
                (36, 36, 36, 36, false));
            AddParaWithImgSetOverflowX(canvas, OverflowPropertyValue.VISIBLE);
            canvas.Close();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void OverflowVisibleOnCanvasTest02() {
            String outFileName = destinationFolder + "overflowVisibleOnCanvasTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_overflowVisibleOnCanvasTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            PdfPage page = pdfDocument.AddNewPage();
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(page, page.GetPageSize().Clone().ApplyMargins(36, 36, 
                36, 36, false));
            AddParaWithImgSetOverflowX(canvas, OverflowPropertyValue.VISIBLE);
            canvas.Close();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private static void AddParaWithImgSetOverflowX(iText.Layout.Canvas canvas, OverflowPropertyValue? overflowX
            ) {
            String imgPath = sourceFolder + "itis.jpg";
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(imgPath));
            Paragraph p = new Paragraph().SetTextAlignment(TextAlignment.CENTER).SetHeight(150f).SetWidth(150f).SetBorder
                (new SolidBorder(5f));
            p.SetProperty(Property.OVERFLOW_X, overflowX);
            p.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.HIDDEN);
            img.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            img.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            canvas.Add(p.Add(img));
        }

        [NUnit.Framework.Test]
        public virtual void ForcedPlacementTest01() {
            String outFileName = destinationFolder + "forcedPlacementTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_forcedPlacementTest01.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFileName)));
            String text = "Text that is not fitting into single line, but requires several of them. " + "It should be repeated twice and all of it should be shown in the document. ";
            Div div = new Div();
            div.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(750).SetWidth(600);
            div.Add(img);
            div.Add(new Paragraph(text + text));
            // Warning! Property.FORCED_PLACEMENT is for internal usage only!
            // It is highly advised not to use it unless you know what you are doing.
            // It is used here for specific testing purposes.
            div.SetProperty(Property.FORCED_PLACEMENT, true);
            document.Add(div);
            document.Close();
            // TODO DEVSIX-1655: text might be lost later in the element if previously forced placement was applied.
            // This test is really artificial in fact, since FORCED_PLACEMENT is set explicitly. Even though at the moment
            // of test creation such situation in fact really happens during elements layout.
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED)]
        public virtual void ForcedPlacementTest02() {
            String outFileName = destinationFolder + "forcedPlacementTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_forcedPlacementTest02.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFileName)));
            String text = "Text that is not fitting into single line, but requires several of them. " + "It should be repeated twice and all of it should be shown in the document. ";
            Div div = new Div();
            div.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"
                )).SetHeight(750).SetWidth(600);
            div.Add(img);
            div.Add(new Paragraph().Add(text).Add(text));
            // Warning! Property.FORCED_PLACEMENT is for internal usage only!
            // It is highly advised not to use it unless you know what you are doing.
            // It is used here for specific testing purposes.
            div.SetProperty(Property.FORCED_PLACEMENT, true);
            document.Add(div);
            document.Close();
            // TODO DEVSIX-1655: text might be lost later in the element if previously forced placement was applied.
            // This test is really artificial in fact, since FORCED_PLACEMENT is set explicitly. Even though at the moment
            // of test creation such situation in fact really happens during elements layout
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}

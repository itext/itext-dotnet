using System;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Test;

namespace iText.Layout {
    public class MinWidthTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/MinWidthTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/MinWidthTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ParagraphTest() {
            String outFileName = destinationFolder + "paragraphTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_paragraphTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str).SetBorder(new SolidBorder(Color.BLACK, 5))).SetBorder(new SolidBorder
                (Color.BLUE, 5));
            LayoutResult result = MinMaxWidthUtils.TryLayoutWithInfHeight(p.CreateRendererSubTree().SetParent(doc.GetRenderer
                ()), doc.GetPageEffectiveArea(PageSize.A4).GetWidth());
            p.SetWidth(MinMaxWidthUtils.ToEffectiveWidth(p, result.GetMinFullWidth()));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DivTest() {
            String outFileName = destinationFolder + "divTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_divTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(Color.BLACK, 2)).SetMargin
                (3).SetBackgroundColor(Color.LIGHT_GRAY);
            Div d = new Div().SetPadding(4f).SetBorder(new SolidBorder(Color.GREEN, 5)).SetMargin(6);
            d.Add(p);
            LayoutResult result = MinMaxWidthUtils.TryLayoutWithInfHeight(d.CreateRendererSubTree().SetParent(doc.GetRenderer
                ()), doc.GetPageEffectiveArea(PageSize.A4).GetWidth());
            d.SetWidth(MinMaxWidthUtils.ToEffectiveWidth(d, result.GetMinFullWidth()));
            doc.Add(d);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DivWithSmallRotatedParagraph() {
            String outFileName = destinationFolder + "divSmallRotatedParagraphTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_divSmallRotatedParagraphTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(Color.BLACK, 2)).SetMargin
                (3).SetBackgroundColor(Color.LIGHT_GRAY);
            Div d = new Div().SetPadding(4f).SetBorder(new SolidBorder(Color.GREEN, 5)).SetMargin(6);
            d.Add(p);
            d.Add(new Paragraph(("iText")).SetRotationAngle(Math.PI / 8).SetBorder(new SolidBorder(Color.BLUE, 2f)));
            LayoutResult result = MinMaxWidthUtils.TryLayoutWithInfHeight(d.CreateRendererSubTree().SetParent(doc.GetRenderer
                ()), doc.GetPageEffectiveArea(PageSize.A4).GetWidth());
            d.SetWidth(MinMaxWidthUtils.ToEffectiveWidth(d, result.GetMinFullWidth()));
            doc.Add(d);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DivWithBigRotatedParagraph() {
            String outFileName = destinationFolder + "divBigRotatedParagraphTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_divBigRotatedParagraphTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(Color.BLACK, 2)).SetMargin
                (3).SetBackgroundColor(Color.LIGHT_GRAY).SetRotationAngle(Math.PI / 8);
            Div d = new Div().SetPadding(4f).SetBorder(new SolidBorder(Color.GREEN, 5)).SetMargin(6);
            d.Add(p);
            d.Add(new Paragraph(("iText")));
            LayoutResult result = MinMaxWidthUtils.TryLayoutWithInfHeight(d.CreateRendererSubTree().SetParent(doc.GetRenderer
                ()), doc.GetPageEffectiveArea(PageSize.A4).GetWidth());
            d.SetWidth(MinMaxWidthUtils.ToEffectiveWidth(d, result.GetMinFullWidth()));
            doc.Add(d);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DivWithSmallRotatedDiv() {
            String outFileName = destinationFolder + "divSmallRotatedDivTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_divSmallRotatedDivTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(Color.BLACK, 2)).SetMargin
                (3).SetBackgroundColor(Color.LIGHT_GRAY);
            Div d = new Div().SetPadding(4f).SetBorder(new SolidBorder(Color.GREEN, 5)).SetMargin(6);
            d.Add(p);
            Div dRotated = new Div().SetRotationAngle(Math.PI / 8).SetBorder(new SolidBorder(Color.BLUE, 2f));
            d.Add(dRotated.Add(new Paragraph(("iText"))));
            LayoutResult result = MinMaxWidthUtils.TryLayoutWithInfHeight(d.CreateRendererSubTree().SetParent(doc.GetRenderer
                ()), doc.GetPageEffectiveArea(PageSize.A4).GetWidth());
            d.SetWidth(MinMaxWidthUtils.ToEffectiveWidth(d, result.GetMinFullWidth()));
            doc.Add(d);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DivWithBigRotatedDiv() {
            String outFileName = destinationFolder + "divBigRotatedDivTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_divBigRotatedDivTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(Color.BLACK, 2)).SetMargin
                (3).SetBackgroundColor(Color.LIGHT_GRAY);
            Div dRotated = new Div().SetPadding(4f).SetBorder(new SolidBorder(Color.GREEN, 5)).SetMargin(6);
            dRotated.Add(p).SetRotationAngle(Math.PI * 3 / 8);
            Div d = new Div().Add(new Paragraph(("iText"))).Add(dRotated).SetBorder(new SolidBorder(Color.BLUE, 2f));
            LayoutResult result = MinMaxWidthUtils.TryLayoutWithInfHeight(d.CreateRendererSubTree().SetParent(doc.GetRenderer
                ()), doc.GetPageEffectiveArea(PageSize.A4).GetWidth());
            d.SetWidth(MinMaxWidthUtils.ToEffectiveWidth(d, result.GetMinFullWidth()));
            doc.Add(d);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MultipleDivTest() {
            String outFileName = destinationFolder + "multipleDivTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_multipleDivTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Border[] borders = new Border[] { new SolidBorder(Color.BLUE, 2f), new SolidBorder(Color.RED, 2f), new SolidBorder
                (Color.GREEN, 2f) };
            Div externalDiv = new Div().SetPadding(2f).SetBorder(borders[2]);
            Div curr = externalDiv;
            for (int i = 0; i < 100; ++i) {
                Div d = new Div().SetBorder(borders[i % 3]);
                curr.Add(d);
                curr = d;
            }
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str)).SetPadding(1f).SetBorder(new SolidBorder(Color.BLACK, 2)).SetMargin
                (3).SetBackgroundColor(Color.LIGHT_GRAY);
            curr.Add(p);
            LayoutResult result = MinMaxWidthUtils.TryLayoutWithInfHeight(externalDiv.CreateRendererSubTree().SetParent
                (doc.GetRenderer()), doc.GetPageEffectiveArea(PageSize.A4).GetWidth());
            externalDiv.SetWidth(MinMaxWidthUtils.ToEffectiveWidth(externalDiv, result.GetMinFullWidth()));
            doc.Add(externalDiv);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Layout {
    public class PreLayoutTest : ExtendedITextTest {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itext/layout/PreLayoutTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/PreLayoutTest/";

        [NUnit.Framework.TestFixtureSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PreLayoutTest01() {
            String outFileName = destinationFolder + "preLayoutTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_preLayoutTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document document = new Document(pdfDocument, PageSize.Default, false);
            IList<Text> pageNumberTexts = new List<Text>();
            IList<IRenderer> pageNumberRenderers = new List<IRenderer>();
            document.SetProperty(Property.FONT, PdfFontFactory.CreateFont(FontConstants.HELVETICA));
            for (int i = 0; i < 200; i++) {
                document.Add(new Paragraph("This is just junk text"));
                if (i % 10 == 0) {
                    Text pageNumberText = new Text("Page #: {pageNumber}");
                    IRenderer renderer = new TextRenderer(pageNumberText);
                    pageNumberText.SetNextRenderer(renderer);
                    pageNumberRenderers.Add(renderer);
                    Paragraph pageNumberParagraph = new Paragraph().Add(pageNumberText);
                    pageNumberTexts.Add(pageNumberText);
                    document.Add(pageNumberParagraph);
                }
            }
            foreach (IRenderer renderer_1 in pageNumberRenderers) {
                String currentData = renderer_1.ToString().Replace("{pageNumber}", renderer_1.GetOccupiedArea().GetPageNumber
                    ().ToString());
                ((TextRenderer)renderer_1).SetText(currentData);
                ((Text)renderer_1.GetModelElement()).SetNextRenderer(renderer_1);
            }
            document.Relayout();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PreLayoutTest02() {
            String outFileName = destinationFolder + "preLayoutTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_preLayoutTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document document = new Document(pdfDoc, PageSize.Default, false);
            document.Add(new Paragraph("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"));
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < 1200; i++) {
                text.Append("A very long text is here...");
            }
            Paragraph twoColumnParagraph = new Paragraph();
            twoColumnParagraph.SetNextRenderer(new PreLayoutTest.TwoColumnParagraphRenderer(twoColumnParagraph));
            iText.Layout.Element.Text textElement = new iText.Layout.Element.Text(text.ToString());
            twoColumnParagraph.Add(textElement).SetFont(PdfFontFactory.CreateFont(FontConstants.HELVETICA));
            document.Add(twoColumnParagraph);
            document.Add(new Paragraph("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"));
            int paragraphLastPageNumber = -1;
            IList<IRenderer> documentChildRenderers = document.GetRenderer().GetChildRenderers();
            for (int i_1 = documentChildRenderers.Count - 1; i_1 >= 0; i_1--) {
                if (documentChildRenderers[i_1].GetModelElement() == twoColumnParagraph) {
                    paragraphLastPageNumber = documentChildRenderers[i_1].GetOccupiedArea().GetPageNumber();
                    break;
                }
            }
            twoColumnParagraph.SetNextRenderer(new PreLayoutTest.TwoColumnParagraphRenderer(twoColumnParagraph, paragraphLastPageNumber
                ));
            document.Relayout();
            //Close document. Drawing of content is happened on close
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        internal class TwoColumnParagraphRenderer : ParagraphRenderer {
            internal int oneColumnPage = -1;

            public TwoColumnParagraphRenderer(Paragraph modelElement)
                : base(modelElement) {
            }

            public TwoColumnParagraphRenderer(Paragraph modelElement, int oneColumnPage)
                : this(modelElement) {
                this.oneColumnPage = oneColumnPage;
            }

            public override IList<Rectangle> InitElementAreas(LayoutArea area) {
                IList<Rectangle> areas = new List<Rectangle>();
                if (area.GetPageNumber() != oneColumnPage) {
                    Rectangle firstArea = area.GetBBox().Clone();
                    Rectangle secondArea = area.GetBBox().Clone();
                    firstArea.SetWidth(firstArea.GetWidth() / 2 - 5);
                    secondArea.SetX(secondArea.GetX() + secondArea.GetWidth() / 2 + 5);
                    secondArea.SetWidth(firstArea.GetWidth());
                    areas.Add(firstArea);
                    areas.Add(secondArea);
                }
                else {
                    areas.Add(area.GetBBox());
                }
                return areas;
            }

            public override IRenderer GetNextRenderer() {
                return new PreLayoutTest.TwoColumnParagraphRenderer((Paragraph)modelElement, oneColumnPage);
            }
        }
    }
}

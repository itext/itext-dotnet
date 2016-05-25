using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using iTextSharp.IO.Font;
using iTextSharp.Kernel.Font;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Utils;
using iTextSharp.Layout.Element;
using iTextSharp.Layout.Layout;
using iTextSharp.Layout.Renderer;
using iTextSharp.Test;

namespace iTextSharp.Layout
{
	public class PreLayoutTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/layout/PreLayoutTest/";

		public const String destinationFolder = "test/itextsharp/layout/PreLayoutTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void PreLayoutTest01()
		{
			String outFileName = destinationFolder + "preLayoutTest01.pdf";
			String cmpFileName = sourceFolder + "cmp_preLayoutTest01.pdf";
			PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName
				, FileMode.Create)));
			Document document = new Document(pdfDocument, PageSize.Default, false);
			IList<Text> pageNumberTexts = new List<Text>();
			IList<IRenderer> pageNumberRenderers = new List<IRenderer>();
			document.SetProperty(iTextSharp.Layout.Property.Property.FONT, PdfFontFactory.CreateFont
				(FontConstants.HELVETICA));
			for (int i = 0; i < 200; i++)
			{
				document.Add(new Paragraph("This is just junk text"));
				if (i % 10 == 0)
				{
					Text pageNumberText = new Text("Page #: {pageNumber}");
					IRenderer renderer = new TextRenderer(pageNumberText);
					pageNumberText.SetNextRenderer(renderer);
					pageNumberRenderers.Add(renderer);
					Paragraph pageNumberParagraph = new Paragraph().Add(pageNumberText);
					pageNumberTexts.Add(pageNumberText);
					document.Add(pageNumberParagraph);
				}
			}
			foreach (IRenderer renderer_1 in pageNumberRenderers)
			{
				String currentData = renderer_1.ToString().Replace("{pageNumber}", renderer_1.GetOccupiedArea
					().GetPageNumber().ToString());
				((TextRenderer)renderer_1).SetText(currentData);
				((Text)renderer_1.GetModelElement()).SetNextRenderer(renderer_1);
			}
			document.Relayout();
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void PreLayoutTest02()
		{
			String outFileName = destinationFolder + "preLayoutTest02.pdf";
			String cmpFileName = sourceFolder + "cmp_preLayoutTest02.pdf";
			PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode
				.Create)));
			Document document = new Document(pdfDoc, PageSize.Default, false);
			document.Add(new Paragraph("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"));
			StringBuilder text = new StringBuilder();
			for (int i = 0; i < 1200; i++)
			{
				text.Append("A very long text is here...");
			}
			Paragraph twoColumnParagraph = new Paragraph();
			twoColumnParagraph.SetNextRenderer(new _T802745355(this, twoColumnParagraph));
			iTextSharp.Layout.Element.Text textElement = new iTextSharp.Layout.Element.Text(text
				.ToString());
			twoColumnParagraph.Add(textElement).SetFont(PdfFontFactory.CreateFont(FontConstants
				.HELVETICA));
			document.Add(twoColumnParagraph);
			document.Add(new Paragraph("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"));
			int paragraphLastPageNumber = -1;
			IList<IRenderer> documentChildRenderers = document.GetRenderer().GetChildRenderers
				();
			for (int i_1 = documentChildRenderers.Count - 1; i_1 >= 0; i_1--)
			{
				if (documentChildRenderers[i_1].GetModelElement() == twoColumnParagraph)
				{
					paragraphLastPageNumber = documentChildRenderers[i_1].GetOccupiedArea().GetPageNumber
						();
					break;
				}
			}
			twoColumnParagraph.SetNextRenderer(new _T802745355(this, twoColumnParagraph, paragraphLastPageNumber
				));
			document.Relayout();
			//Close document. Drawing of content is happened on close
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		internal class _T802745355 : ParagraphRenderer
		{
			internal int oneColumnPage = -1;

			public _T802745355(PreLayoutTest _enclosing, Paragraph modelElement)
				: base(modelElement)
			{
				this._enclosing = _enclosing;
			}

			public _T802745355(PreLayoutTest _enclosing, Paragraph modelElement, int oneColumnPage
				)
				: this(modelElement)
			{
				this._enclosing = _enclosing;
				this.oneColumnPage = oneColumnPage;
			}

			public override IList<Rectangle> InitElementAreas(LayoutArea area)
			{
				IList<Rectangle> areas = new List<Rectangle>();
				if (area.GetPageNumber() != this.oneColumnPage)
				{
					Rectangle firstArea = area.GetBBox().Clone();
					Rectangle secondArea = area.GetBBox().Clone();
					firstArea.SetWidth(firstArea.GetWidth() / 2 - 5);
					secondArea.SetX(secondArea.GetX() + secondArea.GetWidth() / 2 + 5);
					secondArea.SetWidth(firstArea.GetWidth());
					areas.Add(firstArea);
					areas.Add(secondArea);
				}
				else
				{
					areas.Add(area.GetBBox());
				}
				return areas;
			}

			public override IRenderer GetNextRenderer()
			{
				return new _T802745355(this, (Paragraph)this.modelElement, this.oneColumnPage);
			}

			private readonly PreLayoutTest _enclosing;
		}
	}
}

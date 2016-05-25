using System;
using System.IO;
using NUnit.Framework;
using iTextSharp.IO;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Utils;
using iTextSharp.Layout.Element;
using iTextSharp.Layout.Property;
using iTextSharp.Test;
using iTextSharp.Test.Attributes;

namespace iTextSharp.Layout
{
	public class KeepTogetherTest : ExtendedITextTest
	{
		public static readonly String sourceFolder = TestContext.CurrentContext.TestDirectory
			 + "/../../resources/itextsharp/layout/KeepTogetherTest/";

		public static readonly String destinationFolder = TestContext.CurrentContext.TestDirectory
			 + "/test/itextsharp/layout/KeepTogetherTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void KeepTogetherParagraphTest01()
		{
			String cmpFileName = sourceFolder + "cmp_keepTogetherParagraphTest01.pdf";
			String outFile = destinationFolder + "keepTogetherParagraphTest01.pdf";
			PdfWriter writer = new PdfWriter(new FileStream(outFile, FileMode.Create));
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			for (int i = 0; i < 29; i++)
			{
				doc.Add(new Paragraph("String number" + i));
			}
			String str = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaanasdadasdadaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			Paragraph p1 = new Paragraph(str);
			p1.SetKeepTogether(true);
			doc.Add(p1);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
		public virtual void KeepTogetherParagraphTest02()
		{
			String cmpFileName = sourceFolder + "cmp_keepTogetherParagraphTest02.pdf";
			String outFile = destinationFolder + "keepTogetherParagraphTest02.pdf";
			PdfWriter writer = new PdfWriter(new FileStream(outFile, FileMode.Create));
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			for (int i = 0; i < 28; i++)
			{
				doc.Add(new Paragraph("String number" + i));
			}
			String str = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaanasdadasdadaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			for (int i_1 = 0; i_1 < 5; i_1++)
			{
				str += str;
			}
			Paragraph p1 = new Paragraph(str);
			p1.SetKeepTogether(true);
			doc.Add(p1);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void KeepTogetherListTest01()
		{
			String cmpFileName = sourceFolder + "cmp_keepTogetherListTest01.pdf";
			String outFile = destinationFolder + "keepTogetherListTest01.pdf";
			PdfWriter writer = new PdfWriter(new FileStream(outFile, FileMode.Create));
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			for (int i = 0; i < 28; i++)
			{
				doc.Add(new Paragraph("String number" + i));
			}
			List list = new List();
			list.Add("firstItem").Add("secondItem").Add("thirdItem").SetKeepTogether(true).SetListSymbol
				(ListNumberingType.DECIMAL);
			doc.Add(list);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void KeepTogetherDivTest01()
		{
			String cmpFileName = sourceFolder + "cmp_keepTogetherDivTest01.pdf";
			String outFile = destinationFolder + "keepTogetherDivTest01.pdf";
			PdfWriter writer = new PdfWriter(new FileStream(outFile, FileMode.Create));
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			Paragraph p = new Paragraph("Test String");
			for (int i = 0; i < 28; i++)
			{
				doc.Add(p);
			}
			Div div = new Div();
			div.Add(new Paragraph("first paragraph"));
			div.Add(new Paragraph("second paragraph"));
			div.Add(new Paragraph("third paragraph"));
			div.SetKeepTogether(true);
			doc.Add(div);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
		public virtual void KeepTogetherDivTest02()
		{
			String cmpFileName = sourceFolder + "cmp_keepTogetherDivTest02.pdf";
			String outFile = destinationFolder + "keepTogetherDivTest02.pdf";
			PdfWriter writer = new PdfWriter(new FileStream(outFile, FileMode.Create));
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			Rectangle[] columns = new Rectangle[] { new Rectangle(100, 100, 100, 500), new Rectangle
				(400, 100, 100, 500) };
			doc.SetRenderer(new ColumnDocumentRenderer(doc, columns));
			Div div = new Div();
			doc.Add(new Paragraph("first string"));
			for (int i = 0; i < 130; i++)
			{
				div.Add(new Paragraph("String number " + i));
			}
			div.SetKeepTogether(true);
			doc.Add(div);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName
				, destinationFolder, "diff"));
		}
	}
}

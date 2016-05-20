using System;
using Java.IO;
using NUnit.Framework;
using iTextSharp.IO;
using iTextSharp.IO.Image;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Xobject;
using iTextSharp.Kernel.Utils;
using iTextSharp.Layout.Element;
using iTextSharp.Test;
using iTextSharp.Test.Attributes;

namespace iTextSharp.Layout
{
	public class ImageTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/layout/ImageTest/";

		public const String destinationFolder = "test/itextsharp/layout/ImageTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void ImageTest01()
		{
			String outFileName = destinationFolder + "imageTest01.pdf";
			String cmpFileName = sourceFolder + "cmp_imageTest01.pdf";
			FileOutputStream file = new FileOutputStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder
				 + "Desert.jpg"));
			iTextSharp.Layout.Element.Image image = new iTextSharp.Layout.Element.Image(xObject
				, 100);
			doc.Add(new Paragraph(new Text("First Line")));
			Paragraph p = new Paragraph();
			p.Add(image);
			doc.Add(p);
			doc.Add(new Paragraph(new Text("Second Line")));
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void ImageTest02()
		{
			String outFileName = destinationFolder + "imageTest02.pdf";
			String cmpFileName = sourceFolder + "cmp_imageTest02.pdf";
			FileOutputStream file = new FileOutputStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreateJpeg(new File
				(sourceFolder + "Desert.jpg").ToURI().ToURL()));
			iTextSharp.Layout.Element.Image image = new iTextSharp.Layout.Element.Image(xObject
				, 100);
			Paragraph p = new Paragraph();
			p.Add(new Text("before image"));
			p.Add(image);
			p.Add(new Text("after image"));
			doc.Add(p);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void ImageTest03()
		{
			String outFileName = destinationFolder + "imageTest03.pdf";
			String cmpFileName = sourceFolder + "cmp_imageTest03.pdf";
			FileOutputStream file = new FileOutputStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder
				 + "Desert.jpg"));
			iTextSharp.Layout.Element.Image image = new iTextSharp.Layout.Element.Image(xObject
				, 100);
			doc.Add(new Paragraph(new Text("First Line")));
			Paragraph p = new Paragraph();
			p.Add(image);
			image.SetRotationAngle(Math.PI / 6);
			doc.Add(p);
			doc.Add(new Paragraph(new Text("Second Line")));
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void ImageTest04()
		{
			String outFileName = destinationFolder + "imageTest04.pdf";
			String cmpFileName = sourceFolder + "cmp_imageTest04.pdf";
			FileOutputStream file = new FileOutputStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder
				 + "Desert.jpg"));
			iTextSharp.Layout.Element.Image image = new iTextSharp.Layout.Element.Image(xObject
				, 100);
			Paragraph p = new Paragraph();
			p.Add(new Text("before image"));
			p.Add(image);
			image.SetRotationAngle(Math.PI / 6);
			p.Add(new Text("after image"));
			doc.Add(p);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void ImageTest05()
		{
			String outFileName = destinationFolder + "imageTest05.pdf";
			String cmpFileName = sourceFolder + "cmp_imageTest05.pdf";
			FileOutputStream file = new FileOutputStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder
				 + "Desert.jpg"));
			iTextSharp.Layout.Element.Image image = new iTextSharp.Layout.Element.Image(xObject
				, 100);
			doc.Add(new Paragraph(new Text("First Line")));
			Paragraph p = new Paragraph();
			p.Add(image);
			image.Scale(1, 0.5f);
			doc.Add(p);
			doc.Add(new Paragraph(new Text("Second Line")));
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void ImageTest06()
		{
			String outFileName = destinationFolder + "imageTest06.pdf";
			String cmpFileName = sourceFolder + "cmp_imageTest06.pdf";
			FileOutputStream file = new FileOutputStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder
				 + "Desert.jpg"));
			iTextSharp.Layout.Element.Image image = new iTextSharp.Layout.Element.Image(xObject
				, 100);
			doc.Add(new Paragraph(new Text("First Line")));
			Paragraph p = new Paragraph();
			p.Add(image);
			image.SetMarginLeft(100).SetMarginTop(100);
			doc.Add(p);
			doc.Add(new Paragraph(new Text("Second Line")));
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
		public virtual void ImageTest07()
		{
			String outFileName = destinationFolder + "imageTest07.pdf";
			String cmpFileName = sourceFolder + "cmp_imageTest07.pdf";
			FileOutputStream file = new FileOutputStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			iTextSharp.Layout.Element.Image image = new iTextSharp.Layout.Element.Image(ImageDataFactory
				.Create(sourceFolder + "Desert.jpg"));
			Div div = new Div();
			div.Add(image);
			doc.Add(div);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
		public virtual void ImageTest08()
		{
			String outFileName = destinationFolder + "imageTest08.pdf";
			String cmpFileName = sourceFolder + "cmp_imageTest08.pdf";
			FileOutputStream file = new FileOutputStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			iTextSharp.Layout.Element.Image image = new iTextSharp.Layout.Element.Image(ImageDataFactory
				.Create(sourceFolder + "Desert.jpg"));
			Div div = new Div();
			div.Add(image);
			div.Add(image);
			doc.Add(div);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <summary>Image can be reused in layout, so flushing it on the very first draw is a bad thing.
		/// 	</summary>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FlushOnDrawTest()
		{
			String outFileName = destinationFolder + "flushOnDrawTest.pdf";
			String cmpFileName = sourceFolder + "cmp_flushOnDrawTest.pdf";
			int rowCount = 60;
			FileOutputStream fos = new FileOutputStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document document = new Document(pdfDoc);
			iTextSharp.Layout.Element.Image img = new iTextSharp.Layout.Element.Image(ImageDataFactory
				.Create(sourceFolder + "Desert.jpg"));
			Table table = new Table(8);
			table.SetWidthPercent(100);
			for (int k = 0; k < rowCount; k++)
			{
				for (int j = 0; j < 7; j++)
				{
					table.AddCell("Hello");
				}
				Cell c = new Cell().Add(img.SetWidthPercent(50));
				table.AddCell(c);
			}
			document.Add(table);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <summary>
		/// If an image is flushed automatically on draw, we will later check it for circular references
		/// as it is an XObject.
		/// </summary>
		/// <remarks>
		/// If an image is flushed automatically on draw, we will later check it for circular references
		/// as it is an XObject. This is a test for
		/// <see cref="System.ArgumentNullException"/>
		/// that was caused by getting
		/// a value from flushed image.
		/// </remarks>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FlushOnDrawCheckCircularReferencesTest()
		{
			String outFileName = destinationFolder + "flushOnDrawCheckCircularReferencesTest.pdf";
			String cmpFileName = sourceFolder + "cmp_flushOnDrawCheckCircularReferencesTest.pdf";
			PdfDocument pdf = pdf = new PdfDocument(new PdfWriter(outFileName));
			//Initialize document
			Document document = new Document(pdf);
			iTextSharp.Layout.Element.Image img = new iTextSharp.Layout.Element.Image(ImageDataFactory
				.Create(sourceFolder + "itis.jpg"));
			img.SetAutoScale(true);
			Table table = new Table(4);
			table.SetWidthPercent(100);
			for (int k = 0; k < 5; k++)
			{
				table.AddCell("Hello World from iText7");
				List list = new List().SetListSymbol("-> ");
				list.Add("list item").Add("list item").Add("list item").Add("list item").Add("list item"
					);
				Cell cell = new Cell().Add(list);
				table.AddCell(cell);
				Cell c = new Cell().Add(img);
				table.AddCell(c);
				Table innerTable = new Table(3);
				int j = 0;
				while (j < 9)
				{
					innerTable.AddCell("Hi");
					j++;
				}
				table.AddCell(innerTable);
			}
			document.Add(table);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}
	}
}

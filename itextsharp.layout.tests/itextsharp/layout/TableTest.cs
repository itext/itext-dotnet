using System;
using System.IO;
using iTextSharp.IO;
using iTextSharp.IO.Image;
using iTextSharp.IO.Util;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Xobject;
using iTextSharp.Kernel.Utils;
using iTextSharp.Layout.Border;
using iTextSharp.Layout.Element;
using iTextSharp.Layout.Renderer;
using iTextSharp.Test;
using iTextSharp.Test.Attributes;

namespace iTextSharp.Layout
{
	public class TableTest : ExtendedITextTest
	{
		public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
			.TestDirectory + "/../../resources/itextsharp/layout/TableTest/";

		public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
			.TestDirectory + "/test/itextsharp/layout/TableTest/";

		internal const String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
			 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";

		internal const String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";

		internal const String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
			 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";

		[NUnit.Framework.TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest01()
		{
			String testName = "tableTest01.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			Table table = new Table(new float[] { 50, 50 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1"))).AddCell(new Cell().Add(new Paragraph("cell 1, 2")));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest02()
		{
			String testName = "tableTest02.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			Table table = new Table(new float[] { 50, 50 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1"))).AddCell(new Cell().Add(new Paragraph("cell 1, 2"))).AddCell(new 
				Cell().Add(new Paragraph("cell 2, 1"))).AddCell(new Cell().Add(new Paragraph("cell 2, 2"
				))).AddCell(new Cell().Add(new Paragraph("cell 3, 1"))).AddCell(new Cell());
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest03()
		{
			String testName = "tableTest03.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent1 = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n";
			String textContent2 = "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n"
				 + "Aenean nec lorem. In porttitor. Donec laoreet nonummy augue.\n" + "Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. Mauris eget neque at sem venenatis eleifend. Ut nonummy.\n";
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1\n" + textContent1))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n"
				 + textContent1 + textContent2))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n"
				 + textContent2 + textContent1))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n"
				 + textContent2)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest04()
		{
			String testName = "tableTest04.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1\n" + textContent)));
			table.AddCell(new Cell(3, 1).Add(new Paragraph("cell 1, 2:3\n" + textContent + textContent
				 + textContent)));
			table.AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + textContent)));
			table.AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + textContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest05()
		{
			String testName = "tableTest05.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell(3, 1).Add(new 
				Paragraph("cell 1, 1:3\n" + textContent + textContent + textContent))).AddCell(new 
				Cell().Add(new Paragraph("cell 1, 2\n" + textContent))).AddCell(new Cell().Add(new 
				Paragraph("cell 2, 2\n" + textContent))).AddCell(new Cell().Add(new Paragraph("cell 3, 2\n"
				 + textContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest06()
		{
			String testName = "tableTest06.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1\n" + textContent))).AddCell(new Cell(3, 1).Add(new Paragraph("cell 1, 2:3\n"
				 + textContent + textContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n"
				 + textContent))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + textContent
				)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest07()
		{
			String testName = "tableTest07.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell(3, 1).Add(new 
				Paragraph("cell 1, 1:3\n" + textContent + textContent))).AddCell(new Cell().Add(
				new Paragraph("cell 1, 2\n" + textContent))).AddCell(new Cell().Add(new Paragraph
				("cell 2, 2\n" + textContent))).AddCell(new Cell().Add(new Paragraph("cell 3, 2\n"
				 + textContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest08()
		{
			String testName = "tableTest08.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell(3, 2).Add
				(new Paragraph("cell 1:2, 1:3\n" + textContent + textContent))).AddCell(new Cell
				().Add(new Paragraph("cell 1, 3\n" + textContent))).AddCell(new Cell().Add(new Paragraph
				("cell 2, 3\n" + textContent))).AddCell(new Cell().Add(new Paragraph("cell 3, 3\n"
				 + textContent))).AddCell(new Cell().Add(new Paragraph("cell 4, 1\n" + shortTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n" + shortTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 4, 3\n" + middleTextContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest09()
		{
			String testName = "tableTest09.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell().Add(new 
				Paragraph("cell 1, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph
				("cell 1, 2\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 1, 3\n"
				 + middleTextContent))).AddCell(new Cell(3, 2).Add(new Paragraph("cell 2:2, 1:3\n"
				 + textContent + textContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 3\n"
				 + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 3, 3\n" + middleTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 4, 3\n" + middleTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 5, 1\n" + shortTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 5, 2\n" + shortTextContent))).AddCell(new Cell().Add(
				new Paragraph("cell 5, 3\n" + middleTextContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest10()
		{
			String testName = "tableTest10.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			doc.Add(new Paragraph("Table 1"));
			Table table = new Table(new float[] { 100, 100 }).AddCell(new Cell().Add(new Paragraph
				("1, 1"))).AddCell(new Cell().Add(new Paragraph("1, 2"))).AddCell(new Cell().Add
				(new Paragraph("2, 1"))).AddCell(new Cell().Add(new Paragraph("2, 2")));
			doc.Add(table);
			doc.Add(new Paragraph("Table 2"));
			Table table2 = new Table(new float[] { 50, 50 }).AddCell(new Cell().Add(new Paragraph
				("1, 1"))).AddCell(new Cell().Add(new Paragraph("1, 2"))).AddCell(new Cell().Add
				(new Paragraph("2, 1"))).AddCell(new Cell().Add(new Paragraph("2, 2")));
			doc.Add(table2);
			doc.Add(new Paragraph("Table 3"));
			PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreatePng(UrlUtil.
				ToURL(sourceFolder + "itext.png")));
			iTextSharp.Layout.Element.Image image = new iTextSharp.Layout.Element.Image(xObject
				, 50);
			Table table3 = new Table(new float[] { 100, 100 }).AddCell(new Cell().Add(new Paragraph
				("1, 1"))).AddCell(new Cell().Add(image)).AddCell(new Cell().Add(new Paragraph("2, 1"
				))).AddCell(new Cell().Add(new Paragraph("2, 2")));
			doc.Add(table3);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest11()
		{
			String testName = "tableTest11.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n"
				 + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + shortTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + middleTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 3, 1\n" + shortTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 3, 2\n" + middleTextContent))).AddCell(new Cell().Add
				(new Paragraph("cell 4, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph
				("cell 4, 2\n" + shortTextContent))).AddCell(new Cell().SetKeepTogether(true).Add
				(new Paragraph("cell 5, 1\n" + middleTextContent))).AddCell(new Cell().SetKeepTogether
				(true).Add(new Paragraph("cell 5, 2\n" + shortTextContent))).AddCell(new Cell().
				SetKeepTogether(true).Add(new Paragraph("cell 6, 1\n" + middleTextContent))).AddCell
				(new Cell().SetKeepTogether(true).Add(new Paragraph("cell 6, 2\n" + shortTextContent
				))).AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph("cell 7, 1\n" + middleTextContent
				))).AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph("cell 7, 2\n" + middleTextContent
				)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest12()
		{
			String testName = "tableTest12.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n"
				 + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + shortTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + middleTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 3, 1\n" + shortTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 3, 2\n" + middleTextContent))).AddCell(new Cell().Add
				(new Paragraph("cell 4, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph
				("cell 4, 2\n" + shortTextContent))).AddCell(new Cell().SetKeepTogether(true).Add
				(new Paragraph("cell 5, 1\n" + middleTextContent))).AddCell(new Cell().SetKeepTogether
				(true).Add(new Paragraph("cell 5, 2\n" + shortTextContent))).AddCell(new Cell().
				SetKeepTogether(true).Add(new Paragraph("cell 6, 1\n" + middleTextContent))).AddCell
				(new Cell().SetKeepTogether(true).Add(new Paragraph("cell 6, 2\n" + shortTextContent
				))).AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph("cell 7, 1\n" + middleTextContent
				))).AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph("cell 7, 2\n" + middleTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 8, 1\n" + middleTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 8, 2\n" + shortTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 9, 1\n" + shortTextContent))).AddCell(new Cell().Add(
				new Paragraph("cell 9, 2\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph
				("cell 10, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 10, 2\n"
				 + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 11, 1\n" + shortTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 11, 2\n" + shortTextContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest13()
		{
			String testName = "tableTest13.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n"
				 + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + shortTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + middleTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 3, 1\n" + shortTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 3, 2\n" + middleTextContent))).AddCell(new Cell().Add
				(new Paragraph("cell 4, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph
				("cell 4, 2\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 5, 1\n"
				 + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 5, 2\n" + shortTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 6, 1\n" + middleTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 6, 2\n" + shortTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 7, 1\n" + middleTextContent))).AddCell(new Cell().Add
				(new Paragraph("cell 7, 2\n" + middleTextContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest14()
		{
			String testName = "tableTest14.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell(3, 2).Add
				(new Paragraph("cell 1:2, 1:3\n" + textContent + textContent))).AddCell(new Cell
				().Add(new Paragraph("cell 1, 3\n" + textContent))).AddCell(new Cell().Add(new Paragraph
				("cell 2, 3\n" + textContent))).AddCell(new Cell().Add(new Paragraph("cell 3, 3\n"
				 + textContent))).AddCell(new Cell().Add(new Paragraph("cell 4, 1\n" + shortTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n" + shortTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 4, 3\n" + middleTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 5, 1\n" + shortTextContent))).AddCell(new Cell().Add(
				new Paragraph("cell 5, 2\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph
				("cell 5, 3\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 1\n"
				 + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 2\n" + middleTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 6, 3\n" + middleTextContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest15()
		{
			String testName = "tableTest15.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell().Add(new 
				Paragraph("cell 1, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph
				("cell 1, 2\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 1, 3\n"
				 + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + shortTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + shortTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 2, 3\n" + middleTextContent))).AddCell(new Cell
				(3, 2).Add(new Paragraph("cell 3:2, 1:3\n" + textContent + textContent))).AddCell
				(new Cell().Add(new Paragraph("cell 3, 3\n" + textContent))).AddCell(new Cell().
				Add(new Paragraph("cell 4, 3\n" + textContent))).AddCell(new Cell().Add(new Paragraph
				("cell 5, 3\n" + textContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 1\n"
				 + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 2\n" + shortTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 6, 3\n" + middleTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 7, 1\n" + middleTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 7, 2\n" + middleTextContent))).AddCell(new Cell().Add
				(new Paragraph("cell 7, 3\n" + middleTextContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SimpleTableTest16()
		{
			String testName = "tableTest16.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String longTextContent = "1. " + textContent + "2. " + textContent + "3. " + textContent
				 + "4. " + textContent + "5. " + textContent + "6. " + textContent + "7. " + textContent
				 + "8. " + textContent + "9. " + textContent;
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1\n" + longTextContent))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n"
				 + middleTextContent)).SetBorder(new SolidBorder(iTextSharp.Kernel.Color.Color.RED
				, 2))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + middleTextContent + 
				middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + longTextContent
				)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
		public virtual void SimpleTableTest17()
		{
			String testName = "tableTest17.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			Table table = new Table(new float[] { 50, 50, 50 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1"))).AddCell(new Cell().Add(new Paragraph("cell 1, 2"))).AddCell(new 
				Cell().Add(new Paragraph("cell 1, 3")));
			String longText = "Long text, very long text. ";
			for (int i = 0; i < 4; i++)
			{
				longText += longText;
			}
			table.AddCell(new Cell().Add(new Paragraph("cell 2.1\n" + longText).SetKeepTogether
				(true)));
			table.AddCell("cell 2.2\nShort text.");
			table.AddCell(new Cell().Add(new Paragraph("cell 2.3\n" + longText)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
		public virtual void SimpleTableTest18()
		{
			String testName = "tableTest18.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			doc.Add(new Paragraph(textContent));
			Table table = new Table(new float[] { 50, 50, 50 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1"))).AddCell(new Cell().Add(new Paragraph("cell 1, 2"))).AddCell(new 
				Cell().Add(new Paragraph("cell 1, 3")));
			String longText = "Long text, very long text. ";
			for (int i = 0; i < 4; i++)
			{
				longText += longText;
			}
			table.AddCell(new Cell().Add(new Paragraph("cell 2.1\n" + longText).SetKeepTogether
				(true)));
			table.AddCell("cell 2.2\nShort text.");
			table.AddCell(new Cell().Add(new Paragraph("cell 2.3\n" + longText)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
		public virtual void SimpleTableTest19()
		{
			String testName = "tableTest19.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell(3, 2).Add
				(new Paragraph("cell 1:2, 1:3\n" + textContent + textContent))).AddCell(new Cell
				().Add(new Paragraph("cell 1, 3\n" + textContent))).AddCell(new Cell().Add(new Paragraph
				("cell 2, 3\n" + textContent))).AddCell(new Cell().Add(new Paragraph("cell 3, 3\n"
				 + textContent))).AddCell(new Cell().Add(new iTextSharp.Layout.Element.Image(ImageDataFactory
				.Create(sourceFolder + "red.png")))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n"
				 + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 4, 3\n" + middleTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 5, 1\n" + shortTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 5, 2\n" + shortTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 5, 3\n" + middleTextContent))).AddCell(new Cell().Add
				(new Paragraph("cell 6, 1\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph
				("cell 6, 2\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 3\n"
				 + middleTextContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
		public virtual void SimpleTableTest20()
		{
			String testName = "tableTest20.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell().Add(new 
				iTextSharp.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "red.png"
				)))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n" + shortTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 4, 3\n" + middleTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 5, 1\n" + shortTextContent))).AddCell(new Cell().Add(
				new Paragraph("cell 5, 2\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph
				("cell 5, 3\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 1\n"
				 + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 2\n" + middleTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 6, 3\n" + middleTextContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
		public virtual void SimpleTableTest21()
		{
			String testName = "tableTest21.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			doc.Add(new Paragraph(textContent));
			Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell().Add(new 
				iTextSharp.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "red.png"
				)))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n" + shortTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 4, 3\n" + middleTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 5, 1\n" + shortTextContent))).AddCell(new Cell().Add(
				new Paragraph("cell 5, 2\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph
				("cell 5, 3\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 1\n"
				 + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 2\n" + middleTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 6, 3\n" + middleTextContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void BigRowspanTest01()
		{
			String testName = "bigRowspanTest01.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String longTextContent = "1. " + textContent + "2. " + textContent + "3. " + textContent
				 + "4. " + textContent + "5. " + textContent + "6. " + textContent + "7. " + textContent
				 + "8. " + textContent + "9. " + textContent;
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1\n" + textContent))).AddCell(new Cell(5, 1).Add(new Paragraph("cell 1, 2\n"
				 + longTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + middleTextContent
				))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + middleTextContent))).AddCell
				(new Cell().Add(new Paragraph("cell 4, 1\n" + middleTextContent))).AddCell(new Cell
				().Add(new Paragraph("cell 5, 1\n" + middleTextContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void BigRowspanTest02()
		{
			String testName = "bigRowspanTest02.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String longTextContent = "1. " + textContent + "2. " + textContent + "3. " + textContent
				 + "4. " + textContent + "5. " + textContent + "6. " + textContent + "7. " + textContent
				 + "8. " + textContent + "9. " + textContent;
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1\n" + textContent))).AddCell(new Cell(5, 1).Add(new Paragraph("cell 1, 2\n"
				 + longTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + textContent
				))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + textContent))).AddCell(
				new Cell().Add(new Paragraph("cell 4, 1\n" + textContent))).AddCell(new Cell().Add
				(new Paragraph("cell 5, 1\n" + textContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void BigRowspanTest03()
		{
			String testName = "bigRowspanTest03.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1\n" + textContent))).AddCell(new Cell(5, 1).Add(new Paragraph("cell 1, 2\n"
				 + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + textContent
				))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + textContent))).AddCell(
				new Cell().Add(new Paragraph("cell 4, 1\n" + textContent))).AddCell(new Cell().Add
				(new Paragraph("cell 5, 1\n" + textContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void BigRowspanTest04()
		{
			String testName = "bigRowspanTest04.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
			String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
				 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
			String longTextContent = "1. " + textContent + "2. " + textContent + "3. " + textContent
				 + "4. " + textContent + "5. " + textContent + "6. " + textContent + "7. " + textContent
				 + "8. " + textContent + "9. " + textContent;
			Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph
				("cell 1, 1\n" + textContent))).AddCell(new Cell(5, 1).Add(new Paragraph("cell 1, 2\n"
				 + longTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + textContent
				))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + textContent))).AddCell(
				new Cell().SetKeepTogether(true).Add(new Paragraph("cell 4, 1\n" + textContent))
				).AddCell(new Cell().Add(new Paragraph("cell 5, 1\n" + textContent)));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void DifferentPageOrientationTest01()
		{
			String testName = "differentPageOrientationTest01.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			String textContent1 = "Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document.";
			String textContent2 = "To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries.";
			String textContent3 = "Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme.";
			Table table = new Table(3);
			for (int i = 0; i < 20; i++)
			{
				table.AddCell(new Cell().Add(new Paragraph(textContent1))).AddCell(new Cell().Add
					(new Paragraph(textContent3))).AddCell(new Cell().Add(new Paragraph(textContent2
					))).AddCell(new Cell().Add(new Paragraph(textContent3))).AddCell(new Cell().Add(
					new Paragraph(textContent2))).AddCell(new Cell().Add(new Paragraph(textContent1)
					)).AddCell(new Cell().Add(new Paragraph(textContent2))).AddCell(new Cell().Add(new 
					Paragraph(textContent1))).AddCell(new Cell().Add(new Paragraph(textContent3)));
			}
			doc.SetRenderer(new _DocumentRenderer_863(pdfDoc, doc));
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		private sealed class _DocumentRenderer_863 : DocumentRenderer
		{
			public _DocumentRenderer_863(PdfDocument pdfDoc, Document baseArg1)
				: base(baseArg1)
			{
				this.pdfDoc = pdfDoc;
			}

			protected override PageSize AddNewPage(PageSize customPageSize)
			{
				PageSize pageSize = this.currentPageNumber % 2 == 1 ? PageSize.A4 : PageSize.A4.Rotate
					();
				pdfDoc.AddNewPage(pageSize);
				return pageSize;
			}

			private readonly PdfDocument pdfDoc;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
		[NUnit.Framework.Test]
		public virtual void ToLargeElementWithKeepTogetherPropertyInTableTest01()
		{
			String testName = "toLargeElementWithKeepTogetherPropertyInTableTest01.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode
				.Create)));
			Document doc = new Document(pdfDoc);
			Table table = new Table(1);
			Cell cell = new Cell();
			String str = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
			String result = "";
			for (int i = 0; i < 53; i++)
			{
				result += str;
			}
			Paragraph p = new Paragraph(new Text(result));
			p.SetProperty(iTextSharp.Layout.Property.Property.KEEP_TOGETHER, true);
			cell.Add(p);
			table.AddCell(cell);
			doc.Add(table);
			pdfDoc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
		[NUnit.Framework.Test]
		public virtual void ToLargeElementInTableTest01()
		{
			String testName = "toLargeElementInTableTest01.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(destinationFolder
				 + "toLargeElementInTableTest01.pdf", FileMode.Create)));
			Document doc = new Document(pdfDoc);
			Table table = new Table(new float[] { 5 });
			Cell cell = new Cell();
			Paragraph p = new Paragraph(new Text("a"));
			cell.Add(p);
			table.AddCell(cell);
			doc.Add(table);
			pdfDoc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void NestedTableSkipHeaderFooterTest()
		{
			String testName = "nestedTableSkipHeaderFooter.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileStream file = new FileStream(outFileName, FileMode.Create);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
			Table table = new Table(5);
			table.AddHeaderCell(new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")));
			table.AddFooterCell(new Cell(1, 5).Add(new Paragraph("Continue on next page")));
			table.SetSkipFirstHeader(true);
			table.SetSkipLastFooter(true);
			for (int i = 0; i < 350; i++)
			{
				table.AddCell(new Cell().Add(new Paragraph((i + 1).ToString())));
			}
			Table t = new Table(1);
			t.AddCell(new Cell().SetBorder(new SolidBorder(iTextSharp.Kernel.Color.Color.RED, 
				1)).SetPaddings(3, 3, 3, 3).Add(table));
			doc.Add(t);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
		[NUnit.Framework.Test]
		public virtual void SplitTableOnLowPage()
		{
			String testName = "splitTableOnLowPage.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
			Document doc = new Document(pdfDoc, new PageSize(300, 120));
			doc.Add(new Paragraph("Table with setKeepTogether(true):"));
			Table table = new Table(2);
			table.SetKeepTogether(true);
			table.SetMarginTop(10);
			Cell cell = new Cell(3, 1);
			cell.Add("G");
			cell.Add("R");
			cell.Add("P");
			table.AddCell(cell);
			table.AddCell("row 1");
			table.AddCell("row 2");
			table.AddCell("row 3");
			doc.Add(table);
			doc.Add(new AreaBreak());
			doc.Add(new Paragraph("Table with setKeepTogether(false):"));
			table.SetKeepTogether(false);
			doc.Add(table);
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}
	}
}

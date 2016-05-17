using System;
using Java.IO;
using NUnit.Framework;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Utils;
using iTextSharp.Layout.Element;
using iTextSharp.Test;

namespace iTextSharp.Layout
{
	public class LargeElementTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/layout/LargeElementTest/";

		public const String destinationFolder = "test/itextsharp/layout/LargeElementTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void LargeTableTest01()
		{
			String testName = "largeTableTest01.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileOutputStream file = new FileOutputStream(outFileName);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			Table table = new Table(5, true);
			doc.Add(table);
			for (int i = 0; i < 20; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					table.AddCell(new Cell().Add(new Paragraph(String.Format("Cell {0}, {1}", i + 1, 
						j + 1))));
				}
				if (i % 10 == 0)
				{
					table.Flush();
					// This is a deliberate additional flush.
					table.Flush();
				}
			}
			table.Complete();
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void LargeTableTest02()
		{
			String testName = "largeTableTest02.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileOutputStream file = new FileOutputStream(outFileName);
			PdfWriter writer = new PdfWriter(file);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc);
			Table table = new Table(5, true).SetMargins(20, 20, 20, 20);
			doc.Add(table);
			for (int i = 0; i < 100; i++)
			{
				table.AddCell(new Cell().Add(new Paragraph(String.Format("Cell {0}", i + 1))));
				if (i % 7 == 0)
				{
					table.Flush();
				}
			}
			table.Complete();
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void LargeTableWithHeaderFooterTest01A()
		{
			String testName = "largeTableWithHeaderFooterTest01A.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileOutputStream fos = new FileOutputStream(outFileName);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
			Table table = new Table(5, true);
			doc.Add(table);
			Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)"));
			table.AddHeaderCell(cell);
			cell = new Cell(1, 5).Add(new Paragraph("Continue on next page"));
			table.AddFooterCell(cell);
			table.SetSkipFirstHeader(true);
			table.SetSkipLastFooter(true);
			for (int i = 0; i < 350; i++)
			{
				table.AddCell(new Cell().Add(new Paragraph((i + 1).ToString())));
				table.Flush();
			}
			table.Complete();
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void LargeTableWithHeaderFooterTest01B()
		{
			String testName = "largeTableWithHeaderFooterTest01B.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileOutputStream fos = new FileOutputStream(outFileName);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
			Table table = new Table(5, true);
			doc.Add(table);
			Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)"));
			table.AddHeaderCell(cell);
			cell = new Cell(1, 5).Add(new Paragraph("Continue on next page"));
			table.AddFooterCell(cell);
			table.SetSkipFirstHeader(true);
			table.SetSkipLastFooter(true);
			for (int i = 0; i < 350; i++)
			{
				table.Flush();
				table.AddCell(new Cell().Add(new Paragraph((i + 1).ToString())));
			}
			// That's the trick. complete() is called when table has non-empty content, so the last row is better laid out.
			// Compare with #largeTableWithHeaderFooterTest01A. When we flush last row before calling complete(), we don't yet know
			// if there will be any more rows. Flushing last row implicitly by calling complete solves this problem.
			table.Complete();
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void LargeTableWithHeaderFooterTest02()
		{
			String testName = "largeTableWithHeaderFooterTest02.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileOutputStream fos = new FileOutputStream(outFileName);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
			Table table = new Table(5, true);
			doc.Add(table);
			for (int i = 0; i < 5; i++)
			{
				table.AddHeaderCell(new Cell().Add(new Paragraph("Header1 \n" + i)));
			}
			for (int i_1 = 0; i_1 < 5; i_1++)
			{
				table.AddHeaderCell(new Cell().Add(new Paragraph("Header2 \n" + i_1)));
			}
			for (int i_2 = 0; i_2 < 500; i_2++)
			{
				if (i_2 % 5 == 0)
				{
					table.Flush();
				}
				table.AddCell(new Cell().Add(new Paragraph("Test " + i_2)));
			}
			table.Complete();
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void LargeTableWithHeaderFooterTest03()
		{
			String testName = "largeTableWithHeaderFooterTest03.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileOutputStream fos = new FileOutputStream(outFileName);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
			Table table = new Table(5, true);
			doc.Add(table);
			for (int i = 0; i < 5; i++)
			{
				table.AddHeaderCell(new Cell().Add(new Paragraph("Header \n" + i)));
			}
			for (int i_1 = 0; i_1 < 5; i_1++)
			{
				table.AddFooterCell(new Cell().Add(new Paragraph("Footer \n" + i_1)));
			}
			for (int i_2 = 0; i_2 < 500; i_2++)
			{
				if (i_2 % 5 == 0)
				{
					table.Flush();
				}
				table.AddCell(new Cell().Add(new Paragraph("Test " + i_2)));
			}
			table.Complete();
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void LargeTableWithHeaderFooterTest04()
		{
			String testName = "largeTableWithHeaderFooterTest04.pdf";
			String outFileName = destinationFolder + testName;
			String cmpFileName = sourceFolder + "cmp_" + testName;
			FileOutputStream fos = new FileOutputStream(outFileName);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
			Table table = new Table(5, true);
			doc.Add(table);
			for (int i = 0; i < 5; i++)
			{
				table.AddFooterCell(new Cell().Add(new Paragraph("Footer \n" + i)));
			}
			for (int i_1 = 0; i_1 < 500; i_1++)
			{
				if (i_1 % 5 == 0)
				{
					table.Flush();
				}
				table.AddCell(new Cell().Add(new Paragraph("Test " + i_1)));
			}
			table.Complete();
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, testName + "_diff"));
		}
	}
}

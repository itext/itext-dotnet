using System;
using System.IO;
using Java.IO;
using NUnit.Framework;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Xmp;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfStampingTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/kernel/pdf/PdfStampingTest/";

		public const String destinationFolder = "test/itextsharp/kernel/pdf/PdfStampingTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateOrClearDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping1()
		{
			String filename1 = destinationFolder + "stamping1_1.pdf";
			String filename2 = destinationFolder + "stamping1_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			pdfDoc1.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 6").
				SetTitle("Empty iText 6 Document");
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%Hello World\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2);
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			pdfDoc2.GetDocumentInfo().SetCreator("iText 7").SetTitle("Empty iText 7 Document"
				);
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++)
			{
				pdfDoc3.GetPage(i + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(destinationFolder + "stamping1_2.pdf");
			PdfDocument document = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			PdfDictionary trailer = document.GetTrailer();
			PdfDictionary info = trailer.GetAsDictionary(PdfName.Info);
			PdfString creator = info.GetAsString(PdfName.Creator);
			NUnit.Framework.Assert.AreEqual("iText 7", creator.ToString());
			byte[] bytes = document.GetPage(1).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%Hello World\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			String date = document.GetDocumentInfo().GetPdfObject().GetAsString(PdfName.ModDate
				).GetValue();
			DateTime cl = PdfDate.Decode(date);
			long diff = new GregorianCalendar().GetTimeInMillis() - cl.GetTimeInMillis();
			String message = "Unexpected creation date. Different from now is " + (float)diff
				 / 1000 + "s";
			NUnit.Framework.Assert.IsTrue(diff < 5000, message);
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping2()
		{
			String filename1 = destinationFolder + "stamping2_1.pdf";
			String filename2 = destinationFolder + "stamping2_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2);
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			PdfPage page2 = pdfDoc2.AddNewPage();
			page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"
				));
			page2.Flush();
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++)
			{
				pdfDoc3.GetPage(i + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(destinationFolder + "stamping2_2.pdf");
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 1\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			bytes = pdfDocument.GetPage(2).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 2\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping3()
		{
			String filename1 = destinationFolder + "stamping3_1.pdf";
			String filename2 = destinationFolder + "stamping3_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			PdfPage page2 = pdfDoc2.AddNewPage();
			page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"
				));
			page2.Flush();
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++)
			{
				pdfDoc3.GetPage(i + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 1\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			bytes = pdfDocument.GetPage(2).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 2\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping4()
		{
			String filename1 = destinationFolder + "stamping4_1.pdf";
			String filename2 = destinationFolder + "stamping4_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			int pageCount = 15;
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2);
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			for (int i = 2; i <= pageCount; i++)
			{
				PdfPage page2 = pdfDoc2.AddNewPage();
				page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " 
					+ i + "\n"));
				page2.Flush();
			}
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Page count", pageCount, pdfDocument.GetNumberOfPages
				());
			for (int i_2 = 1; i_2 < pdfDocument.GetNumberOfPages(); i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("%page " + i_2 + "\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
					(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping5()
		{
			String filename1 = destinationFolder + "stamping5_1.pdf";
			String filename2 = destinationFolder + "stamping5_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			int pageCount = 15;
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			for (int i = 2; i <= pageCount; i++)
			{
				PdfPage page2 = pdfDoc2.AddNewPage();
				page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " 
					+ i + "\n"));
				page2.Flush();
			}
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Page count", pageCount, pdfDocument.GetNumberOfPages
				());
			for (int i_2 = 1; i_2 < pdfDocument.GetNumberOfPages(); i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("%page " + i_2 + "\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
					(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping6()
		{
			String filename1 = destinationFolder + "stamping6_1.pdf";
			String filename2 = destinationFolder + "stamping6_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2);
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			PdfPage page2 = pdfDoc2.AddNewPage();
			page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"
				));
			page2.Flush();
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++)
			{
				pdfDoc3.GetPage(i + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 1\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			bytes = pdfDocument.GetPage(2).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 2\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping7()
		{
			String filename1 = destinationFolder + "stamping7_1.pdf";
			String filename2 = destinationFolder + "stamping7_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			PdfPage page2 = pdfDoc2.AddNewPage();
			page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"
				));
			page2.Flush();
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++)
			{
				pdfDoc3.GetPage(i + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 1\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			bytes = pdfDocument.GetPage(2).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 2\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping8()
		{
			String filename1 = destinationFolder + "stamping8_1.pdf";
			String filename2 = destinationFolder + "stamping8_2.pdf";
			int pageCount = 10;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create), new 
				WriterProperties().SetFullCompressionMode(true));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Number of pages", pageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 1; i_2 <= pageCount; i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_2, "%page " + i_2 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping9()
		{
			String filename1 = destinationFolder + "stamping9_1.pdf";
			String filename2 = destinationFolder + "stamping9_2.pdf";
			int pageCount = 10;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(false));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create), new 
				WriterProperties().SetFullCompressionMode(true));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Number of pages", pageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 1; i_2 <= pageCount; i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_2, "%page " + i_2 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping10()
		{
			String filename1 = destinationFolder + "stamping10_1.pdf";
			String filename2 = destinationFolder + "stamping10_2.pdf";
			int pageCount = 10;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create), new 
				WriterProperties().SetFullCompressionMode(false));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Number of pages", pageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 1; i_2 <= pageCount; i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_2, "%page " + i_2 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping11()
		{
			String filename1 = destinationFolder + "stamping11_1.pdf";
			String filename2 = destinationFolder + "stamping11_2.pdf";
			int pageCount = 10;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(false));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create), new 
				WriterProperties().SetFullCompressionMode(false));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Number of pages", pageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 1; i_2 <= pageCount; i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_2, "%page " + i_2 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping12()
		{
			String filename1 = destinationFolder + "stamping12_1.pdf";
			String filename2 = destinationFolder + "stamping12_2.pdf";
			int pageCount = 1010;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			int newPageCount = 10;
			for (int i_1 = pageCount; i_1 > newPageCount; i_1--)
			{
				NUnit.Framework.Assert.IsNotNull("Remove page " + i_1, pdfDoc2.RemovePage(i_1));
			}
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_2 = 1; i_2 <= pdfDoc3.GetNumberOfPages(); i_2++)
			{
				pdfDoc3.GetPage(i_2);
			}
			PdfPage pdfPage = pdfDoc3.GetPage(1);
			PdfDictionary root = pdfPage.GetPdfObject().GetAsDictionary(PdfName.Parent);
			NUnit.Framework.Assert.AreEqual("PdfPages kids count", newPageCount, root.GetAsArray
				(PdfName.Kids).Size());
			NUnit.Framework.Assert.AreEqual("Number of pages", newPageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_3 = 1; i_3 <= pdfDocument.GetNumberOfPages(); i_3++)
			{
				byte[] bytes = pdfDocument.GetPage(i_3).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_3, "%page " + i_3 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Stamping13()
		{
			String filename1 = destinationFolder + "stamping13_1.pdf";
			String filename2 = destinationFolder + "stamping13_2.pdf";
			int pageCount = 1010;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			for (int i_1 = pageCount; i_1 > 1; i_1--)
			{
				NUnit.Framework.Assert.IsNotNull("Remove page " + i_1, pdfDoc2.RemovePage(i_1));
			}
			pdfDoc2.RemovePage(1);
			for (int i_2 = 1; i_2 <= pageCount; i_2++)
			{
				PdfPage page = pdfDoc2.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i_2 + "\n"));
				page.Flush();
			}
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_3 = 1; i_3 <= pdfDoc3.GetNumberOfPages(); i_3++)
			{
				pdfDoc3.GetPage(i_3);
			}
			PdfArray rootKids = pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject().GetAsArray
				(PdfName.Kids);
			NUnit.Framework.Assert.AreEqual("Page root kids count", 2, rootKids.Size());
			NUnit.Framework.Assert.AreEqual("Number of pages", pageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_4 = 1; i_4 <= pageCount; i_4++)
			{
				byte[] bytes = pdfDocument.GetPage(i_4).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_4, "%page " + i_4 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		[Ignore]
		public virtual void Stamping14()
		{
			String filename1 = sourceFolder + "20000PagesDocument.pdf";
			String filename2 = destinationFolder + "stamping14.pdf";
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			for (int i = pdfDoc2.GetNumberOfPages(); i > 3; i--)
			{
				NUnit.Framework.Assert.IsNotNull("Remove page " + i, pdfDoc2.RemovePage(i));
			}
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 1; i_1 <= pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1);
			}
			NUnit.Framework.Assert.IsTrue(pdfDoc3.GetXref().Size() < 20, "Xref size is " + pdfDoc3
				.GetXref().Size());
			NUnit.Framework.Assert.AreEqual("Number of pages", 3, pdfDoc3.GetNumberOfPages());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 1; i_2 <= pdfDocument.GetNumberOfPages(); i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_2, "%page " + i_2 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingStreamsCompression01()
		{
			// by default, old streams should not be recompressed
			String filenameIn = sourceFolder + "stampingStreamsCompression.pdf";
			String filenameOut = destinationFolder + "stampingStreamsCompression01.pdf";
			PdfReader reader = new PdfReader(filenameIn);
			PdfWriter writer = new PdfWriter(filenameOut);
			writer.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
			PdfDocument doc = new PdfDocument(reader, writer);
			PdfStream stream = (PdfStream)doc.GetPdfObject(6);
			int lengthBefore = stream.GetLength();
			doc.Close();
			doc = new PdfDocument(new PdfReader(filenameOut));
			stream = (PdfStream)doc.GetPdfObject(6);
			int lengthAfter = stream.GetLength();
			NUnit.Framework.Assert.IsTrue(lengthBefore == lengthAfter);
			NUnit.Framework.Assert.AreEqual(5731884, lengthBefore);
			NUnit.Framework.Assert.AreEqual(5731884, lengthAfter);
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingStreamsCompression02()
		{
			// if user specified, stream may be uncompressed
			String filenameIn = sourceFolder + "stampingStreamsCompression.pdf";
			String filenameOut = destinationFolder + "stampingStreamsCompression02.pdf";
			PdfReader reader = new PdfReader(filenameIn);
			PdfWriter writer = new PdfWriter(filenameOut);
			PdfDocument doc = new PdfDocument(reader, writer);
			PdfStream stream = (PdfStream)doc.GetPdfObject(6);
			int lengthBefore = stream.GetLength();
			stream.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
			doc.Close();
			doc = new PdfDocument(new PdfReader(filenameOut));
			stream = (PdfStream)doc.GetPdfObject(6);
			int lengthAfter = stream.GetLength();
			NUnit.Framework.Assert.IsTrue(lengthBefore < lengthAfter);
			NUnit.Framework.Assert.AreEqual(5731884, lengthBefore);
			NUnit.Framework.Assert.AreEqual(11321910, lengthAfter);
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingStreamsCompression03()
		{
			// if user specified, stream may be recompressed
			String filenameIn = sourceFolder + "stampingStreamsCompression.pdf";
			String filenameOut = destinationFolder + "stampingStreamsCompression03.pdf";
			PdfReader reader = new PdfReader(filenameIn);
			PdfWriter writer = new PdfWriter(filenameOut);
			PdfDocument doc = new PdfDocument(reader, writer);
			PdfStream stream = (PdfStream)doc.GetPdfObject(6);
			int lengthBefore = stream.GetLength();
			stream.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
			doc.Close();
			doc = new PdfDocument(new PdfReader(filenameOut));
			stream = (PdfStream)doc.GetPdfObject(6);
			int lengthAfter = stream.GetLength();
			NUnit.Framework.Assert.IsTrue(lengthBefore > lengthAfter);
			NUnit.Framework.Assert.AreEqual(5731884, lengthBefore);
			NUnit.Framework.Assert.AreEqual(5729270, lengthAfter);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[Test]
		public virtual void StampingXmp1()
		{
			String filename1 = destinationFolder + "stampingXmp1_1.pdf";
			String filename2 = destinationFolder + "stampingXmp1_2.pdf";
			int pageCount = 10;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create), new 
				WriterProperties().SetFullCompressionMode(false).AddXmpMetadata());
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			pdfDoc2.GetDocumentInfo().SetAuthor("Alexander Chingarev");
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.IsNotNull("XmpMetadata not found", XmpMetaFactory.ParseFromBuffer
				(pdfDoc3.GetXmpMetadata()));
			NUnit.Framework.Assert.AreEqual("Number of pages", pageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 1; i_2 <= pageCount; i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_2, "%page " + i_2 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[Test]
		public virtual void StampingXmp2()
		{
			String filename1 = destinationFolder + "stampingXmp2_1.pdf";
			String filename2 = destinationFolder + "stampingXmp2_2.pdf";
			int pageCount = 10;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(false));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create), new 
				WriterProperties().SetFullCompressionMode(true).AddXmpMetadata());
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
			pdfDoc2.GetDocumentInfo().SetAuthor("Alexander Chingarev");
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.IsNotNull("XmpMetadata not found", XmpMetaFactory.ParseFromBuffer
				(pdfDoc3.GetXmpMetadata()));
			NUnit.Framework.Assert.AreEqual("Number of pages", pageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 1; i_2 <= pageCount; i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_2, "%page " + i_2 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingAppend1()
		{
			String filename1 = destinationFolder + "stampingAppend1_1.pdf";
			String filename2 = destinationFolder + "stampingAppend1_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			pdfDoc1.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 6").
				SetTitle("Empty iText 6 Document");
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%Hello World\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2);
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().
				UseAppendMode());
			pdfDoc2.GetDocumentInfo().SetCreator("iText 7").SetTitle("Empty iText 7 Document"
				);
			pdfDoc2.GetDocumentInfo().SetModified();
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++)
			{
				pdfDoc3.GetPage(i + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			PdfDictionary trailer = pdfDocument.GetTrailer();
			PdfDictionary info = trailer.GetAsDictionary(PdfName.Info);
			PdfString creator = info.GetAsString(PdfName.Creator);
			NUnit.Framework.Assert.AreEqual("iText 7", creator.ToString());
			byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%Hello World\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			String date = pdfDocument.GetDocumentInfo().GetPdfObject().GetAsString(PdfName.ModDate
				).GetValue();
			DateTime cl = PdfDate.Decode(date);
			long diff = new GregorianCalendar().GetTimeInMillis() - cl.GetTimeInMillis();
			String message = "Unexpected creation date. Different from now is " + (float)diff
				 / 1000 + "s";
			NUnit.Framework.Assert.IsTrue(diff < 5000, message);
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingAppend2()
		{
			String filename1 = destinationFolder + "stampingAppend2_1.pdf";
			String filename2 = destinationFolder + "stampingAppend2_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2);
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().
				UseAppendMode());
			PdfPage page2 = pdfDoc2.AddNewPage();
			page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"
				));
			page2.SetModified();
			page2.Flush();
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++)
			{
				pdfDoc3.GetPage(i + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 1\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			bytes = pdfDocument.GetPage(2).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 2\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingAppend3()
		{
			String filename1 = destinationFolder + "stampingAppend3_1.pdf";
			String filename2 = destinationFolder + "stampingAppend3_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2);
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().
				UseAppendMode());
			PdfPage page2 = pdfDoc2.AddNewPage();
			page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"
				));
			page2.Flush();
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++)
			{
				pdfDoc3.GetPage(i + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 1\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			bytes = pdfDocument.GetPage(2).GetContentBytes();
			NUnit.Framework.Assert.AreEqual("%page 2\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
				(bytes));
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingAppend4()
		{
			String filename1 = destinationFolder + "stampingAppend4_1.pdf";
			String filename2 = destinationFolder + "stampingAppend4_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			int pageCount = 15;
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2);
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().
				UseAppendMode());
			for (int i = 2; i <= pageCount; i++)
			{
				PdfPage page2 = pdfDoc2.AddNewPage();
				page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " 
					+ i + "\n"));
				page2.Flush();
			}
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Page count", pageCount, pdfDocument.GetNumberOfPages
				());
			for (int i_2 = 1; i_2 < pdfDocument.GetNumberOfPages(); i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("%page " + i_2 + "\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
					(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingAppend5()
		{
			String filename1 = destinationFolder + "stampingAppend5_1.pdf";
			String filename2 = destinationFolder + "stampingAppend5_2.pdf";
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"
				));
			page1.Flush();
			pdfDoc1.Close();
			int pageCount = 15;
			FileStream fis2 = new FileStream(filename1, FileMode.Open);
			PdfReader reader2 = new PdfReader(fis2);
			FileStream fos2 = new FileStream(filename2, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().
				UseAppendMode());
			for (int i = 2; i <= pageCount; i++)
			{
				PdfPage page2 = pdfDoc2.AddNewPage();
				page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " 
					+ i + "\n"));
				page2.Flush();
			}
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Page count", pageCount, pdfDocument.GetNumberOfPages
				());
			for (int i_2 = 1; i_2 < pdfDocument.GetNumberOfPages(); i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("%page " + i_2 + "\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
					(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingAppend8()
		{
			String filename1 = destinationFolder + "stampingAppend8_1.pdf";
			String filename2 = destinationFolder + "stampingAppend8_2.pdf";
			int pageCount = 10;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().
				UseAppendMode());
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Number of pages", pageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 1; i_2 <= pageCount; i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_2, "%page " + i_2 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingAppend9()
		{
			String filename1 = destinationFolder + "stampingAppend9_1.pdf";
			String filename2 = destinationFolder + "stampingAppend9_2.pdf";
			int pageCount = 10;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(false));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create), new 
				WriterProperties().SetFullCompressionMode(true));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().
				UseAppendMode());
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Number of pages", pageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 1; i_2 <= pageCount; i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_2, "%page " + i_2 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingAppend10()
		{
			String filename1 = destinationFolder + "stampingAppend10_1.pdf";
			String filename2 = destinationFolder + "stampingAppend10_2.pdf";
			int pageCount = 10;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(true));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create), new 
				WriterProperties().SetFullCompressionMode(false));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().
				UseAppendMode());
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Number of pages", pageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 1; i_2 <= pageCount; i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_2, "%page " + i_2 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingAppend11()
		{
			String filename1 = destinationFolder + "stampingAppend11_1.pdf";
			String filename2 = destinationFolder + "stampingAppend11_2.pdf";
			int pageCount = 10;
			FileStream fos1 = new FileStream(filename1, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1, new WriterProperties().SetFullCompressionMode
				(false));
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 1; i <= pageCount; i++)
			{
				PdfPage page = pdfDoc1.AddNewPage();
				page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " +
					 i + "\n"));
				page.Flush();
			}
			pdfDoc1.Close();
			PdfReader reader2 = new PdfReader(new FileStream(filename1, FileMode.Open));
			PdfWriter writer2 = new PdfWriter(new FileStream(filename2, FileMode.Create), new 
				WriterProperties().SetFullCompressionMode(false));
			PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().
				UseAppendMode());
			pdfDoc2.Close();
			PdfReader reader3 = new PdfReader(new FileStream(filename2, FileMode.Open));
			PdfDocument pdfDoc3 = new PdfDocument(reader3);
			for (int i_1 = 0; i_1 < pdfDoc3.GetNumberOfPages(); i_1++)
			{
				pdfDoc3.GetPage(i_1 + 1);
			}
			NUnit.Framework.Assert.AreEqual("Number of pages", pageCount, pdfDoc3.GetNumberOfPages
				());
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader3.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("Fixed", false, reader3.HasFixedXref());
			VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
			pdfDoc3.Close();
			PdfReader reader = new PdfReader(filename2);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 1; i_2 <= pageCount; i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("Page content at page " + i_2, "%page " + i_2 + "\n"
					, iTextSharp.IO.Util.JavaUtil.GetStringForBytes(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingVersionTest01()
		{
			// By default the version of the output file should be the same as the original one
			String @in = sourceFolder + "hello.pdf";
			String @out = destinationFolder + "hello_stamped01.pdf";
			FileStream fis = new FileStream(@in, FileMode.Open);
			PdfReader reader = new PdfReader(fis);
			PdfDocument pdfDoc = new PdfDocument(reader, new PdfWriter(@out));
			NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_1_4, pdfDoc.GetPdfVersion());
			pdfDoc.Close();
			PdfDocument assertPdfDoc = new PdfDocument(new PdfReader(@out));
			NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_1_4, assertPdfDoc.GetPdfVersion());
			assertPdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingVersionTest02()
		{
			// There is a possibility to override version in stamping mode
			String @in = sourceFolder + "hello.pdf";
			String @out = destinationFolder + "hello_stamped02.pdf";
			FileStream fis = new FileStream(@in, FileMode.Open);
			PdfReader reader = new PdfReader(fis);
			PdfDocument pdfDoc = new PdfDocument(reader, new PdfWriter(@out, new WriterProperties
				().SetPdfVersion(PdfVersion.PDF_2_0)));
			NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, pdfDoc.GetPdfVersion());
			pdfDoc.Close();
			PdfDocument assertPdfDoc = new PdfDocument(new PdfReader(@out));
			NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, assertPdfDoc.GetPdfVersion());
			assertPdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingAppendVersionTest01()
		{
			// There is a possibility to override version in stamping mode
			String @in = sourceFolder + "hello.pdf";
			String @out = destinationFolder + "stampingAppendVersionTest01.pdf";
			FileStream fis = new FileStream(@in, FileMode.Open);
			PdfReader reader = new PdfReader(fis);
			PdfWriter writer = new PdfWriter(@out, new WriterProperties().SetPdfVersion(PdfVersion
				.PDF_2_0));
			PdfDocument pdfDoc = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode
				());
			NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, pdfDoc.GetPdfVersion());
			pdfDoc.Close();
			PdfDocument assertPdfDoc = new PdfDocument(new PdfReader(@out));
			NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, assertPdfDoc.GetPdfVersion());
			assertPdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void StampingTestWithTaggedStructure()
		{
			String filename = sourceFolder + "iphone_user_guide.pdf";
			PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open));
			FileStream fos = new FileStream(destinationFolder + "stampingDocWithTaggedStructure.pdf"
				, FileMode.Create);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(reader, writer);
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void StampingTestWithFullCompression01()
		{
			PdfReader reader = new PdfReader(sourceFolder + "fullCompressedDocument.pdf");
			PdfDocument pdfDoc = new PdfDocument(reader, new PdfWriter(destinationFolder + "stampingTestWithFullCompression01.pdf"
				));
			pdfDoc.Close();
			NUnit.Framework.Assert.AreEqual(new File(destinationFolder + "stampingTestWithFullCompression01.pdf"
				).Length(), new File(sourceFolder + "cmp_stampingTestWithFullCompression01.pdf")
				.Length());
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void StampingTestWithFullCompression02()
		{
			PdfReader reader = new PdfReader(sourceFolder + "fullCompressedDocument.pdf");
			PdfDocument pdfDoc = new PdfDocument(reader, new PdfWriter(destinationFolder + "stampingTestWithFullCompression02.pdf"
				, new WriterProperties().SetFullCompressionMode(false)));
			pdfDoc.Close();
			NUnit.Framework.Assert.AreEqual(new File(destinationFolder + "stampingTestWithFullCompression02.pdf"
				).Length(), new File(sourceFolder + "cmp_stampingTestWithFullCompression02.pdf")
				.Length());
		}

		internal static void VerifyPdfPagesCount(PdfObject root)
		{
			if (root.GetObjectType() == PdfObject.INDIRECT_REFERENCE)
			{
				root = ((PdfIndirectReference)root).GetRefersTo();
			}
			PdfDictionary pages = (PdfDictionary)root;
			if (!pages.ContainsKey(PdfName.Kids))
			{
				return;
			}
			PdfNumber count = pages.GetAsNumber(PdfName.Count);
			if (count != null)
			{
				NUnit.Framework.Assert.IsTrue(count.IntValue() > 0, "PdfPages with zero count");
			}
			PdfObject kids = pages.Get(PdfName.Kids);
			if (kids.GetObjectType() == PdfObject.ARRAY)
			{
				foreach (PdfObject kid in (PdfArray)kids)
				{
					VerifyPdfPagesCount(kid);
				}
			}
			else
			{
				VerifyPdfPagesCount(kids);
			}
		}
	}
}

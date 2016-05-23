using System;
using System.IO;
using NUnit.Framework;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Pdf
{
	public class XMPMetadataTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/kernel/pdf/XmpWriterTest/";

		public const String destinationFolder = "test/itextsharp/kernel/pdf/XmpWriterTest/";

		public static TimeZone CURRENT_USER_TIME_ZONE;

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
			CURRENT_USER_TIME_ZONE = TimeZone.GetDefault();
			TimeZone.SetDefault(TimeZone.GetTimeZone("Europe/Brussels"));
		}

		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void CreateEmptyDocumentWithXmp()
		{
			String filename = "emptyDocumentWithXmp.pdf";
			FileStream fos = new FileStream(destinationFolder + filename, FileMode.Create);
			PdfWriter writer = new PdfWriter(fos, new WriterProperties().AddXmpMetadata());
			PdfDocument pdfDoc = new PdfDocument(writer);
			pdfDoc.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 6").SetTitle
				("Empty iText 6 Document");
			pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.CreationDate);
			PdfPage page = pdfDoc.AddNewPage();
			page.Flush();
			pdfDoc.Close();
			PdfReader reader = new PdfReader(destinationFolder + filename);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual(ReadFile(sourceFolder + "emptyDocumentWithXmp.xml"
				).Length, pdfDocument.GetXmpMetadata().Length);
			NUnit.Framework.Assert.IsNotNull(reader.pdfDocument.GetPage(1));
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[Test]
		public virtual void CreateEmptyDocumentWithAbcXmp()
		{
			MemoryStream fos = new MemoryStream();
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			pdfDoc.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 6").SetTitle
				("Empty iText 6 Document");
			pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.CreationDate);
			PdfPage page = pdfDoc.AddNewPage();
			page.Flush();
			pdfDoc.SetXmpMetadata("abc".GetBytes());
			pdfDoc.Close();
			PdfReader reader = new PdfReader(new MemoryStream(fos.ToArray()));
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			NUnit.Framework.Assert.AreEqual("abc".GetBytes(), pdfDocument.GetXmpMetadata());
			NUnit.Framework.Assert.IsNotNull(pdfDocument.GetPage(1));
			reader.Close();
		}

		[AfterClass]
		public static void AfterClass()
		{
			TimeZone.SetDefault(CURRENT_USER_TIME_ZONE);
		}
	}
}

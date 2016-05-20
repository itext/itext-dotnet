using System;
using Java.IO;
using NUnit.Framework;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfStringTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/kernel/pdf/PdfStringTest/";

		public const String destinationFolder = "test/itextsharp/kernel/pdf/PdfStringTest/";

		[SetUp]
		public virtual void Before()
		{
			new File(destinationFolder).Mkdirs();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void TestPdfDocumentInfoStringEncoding01()
		{
			String fileName = "testPdfDocumentInfoStringEncoding01.pdf";
			PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + fileName
				, new WriterProperties().SetCompressionLevel(CompressionConstants.NO_COMPRESSION
				)));
			pdfDocument.AddNewPage();
			String author = "Алексей";
			String title = "Заголовок";
			String subject = "Тема";
			String keywords = "Ключевые слова";
			String creator = "English text";
			pdfDocument.GetDocumentInfo().SetAuthor(author);
			pdfDocument.GetDocumentInfo().SetTitle(title);
			pdfDocument.GetDocumentInfo().SetSubject(subject);
			pdfDocument.GetDocumentInfo().SetKeywords(keywords);
			pdfDocument.GetDocumentInfo().SetCreator(creator);
			pdfDocument.Close();
			PdfDocument readDoc = new PdfDocument(new PdfReader(destinationFolder + fileName)
				);
			NUnit.Framework.Assert.AreEqual(author, readDoc.GetDocumentInfo().GetAuthor());
			NUnit.Framework.Assert.AreEqual(title, readDoc.GetDocumentInfo().GetTitle());
			NUnit.Framework.Assert.AreEqual(subject, readDoc.GetDocumentInfo().GetSubject());
			NUnit.Framework.Assert.AreEqual(keywords, readDoc.GetDocumentInfo().GetKeywords()
				);
			NUnit.Framework.Assert.AreEqual(creator, readDoc.GetDocumentInfo().GetCreator());
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + fileName, sourceFolder + "cmp_" + fileName, destinationFolder, "diff_"));
		}
	}
}

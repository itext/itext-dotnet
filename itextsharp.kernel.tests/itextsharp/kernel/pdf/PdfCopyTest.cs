using System;
using System.IO;
using Java.IO;
using NUnit.Framework;
using iTextSharp.IO;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;
using iTextSharp.Test.Attributes;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfCopyTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/kernel/pdf/PdfCopyTest/";

		public const String destinationFolder = "test/itextsharp/kernel/pdf/PdfCopyTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateOrClearDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
		[LogMessage(LogMessageConstant.MAKE_COPY_OF_CATALOG_DICTIONARY_IS_FORBIDDEN)]
		public virtual void CopySignedDocuments()
		{
			FileStream fis1 = new FileStream(sourceFolder + "hello_signed.pdf", FileMode.Open
				);
			PdfReader reader1 = new PdfReader(fis1);
			PdfDocument pdfDoc1 = new PdfDocument(reader1);
			FileOutputStream fos2 = new FileOutputStream(destinationFolder + "copySignedDocuments.pdf"
				, FileMode.Create);
			PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(fos2));
			pdfDoc1.CopyPagesTo(1, 1, pdfDoc2);
			pdfDoc2.Close();
			pdfDoc1.Close();
			PdfDocument pdfDocument = new PdfDocument(new PdfReader(destinationFolder + "copySignedDocuments.pdf"
				));
			PdfDictionary sig = (PdfDictionary)pdfDocument.GetPdfObject(13);
			PdfDictionary sigRef = sig.GetAsArray(PdfName.Reference).GetAsDictionary(0);
			NUnit.Framework.Assert.IsTrue(PdfName.SigRef.Equals(sigRef.GetAsName(PdfName.Type
				)));
			NUnit.Framework.Assert.IsTrue(sigRef.Get(PdfName.Data).IsNull());
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void Copying1()
		{
			FileOutputStream fos1 = new FileOutputStream(destinationFolder + "copying1_1.pdf"
				, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			pdfDoc1.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 6").
				SetTitle("Empty iText 6 Document");
			pdfDoc1.GetCatalog().Put(new PdfName("a"), ((PdfName)new PdfName("b").MakeIndirect
				(pdfDoc1)));
			PdfPage page1 = pdfDoc1.AddNewPage();
			page1.Flush();
			pdfDoc1.Close();
			FileStream fis1 = new FileStream(destinationFolder + "copying1_1.pdf", FileMode.Open
				);
			PdfReader reader1 = new PdfReader(fis1);
			pdfDoc1 = new PdfDocument(reader1);
			FileOutputStream fos2 = new FileOutputStream(destinationFolder + "copying1_2.pdf"
				, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2);
			PdfDocument pdfDoc2 = new PdfDocument(writer2);
			pdfDoc2.AddNewPage();
			pdfDoc2.GetDocumentInfo().GetPdfObject().Put(new PdfName("a"), pdfDoc1.GetCatalog
				().GetPdfObject().Get(new PdfName("a")).CopyTo(pdfDoc2));
			pdfDoc2.Close();
			pdfDoc1.Close();
			PdfReader reader = new PdfReader(destinationFolder + "copying1_2.pdf");
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			PdfDictionary trailer = pdfDocument.GetTrailer();
			PdfDictionary info = trailer.GetAsDictionary(PdfName.Info);
			PdfName b = info.GetAsName(new PdfName("a"));
			NUnit.Framework.Assert.AreEqual("/b", b.ToString());
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void Copying2()
		{
			FileOutputStream fos1 = new FileOutputStream(destinationFolder + "copying2_1.pdf"
				, FileMode.Create);
			PdfWriter writer1 = new PdfWriter(fos1);
			PdfDocument pdfDoc1 = new PdfDocument(writer1);
			for (int i = 0; i < 10; i++)
			{
				PdfPage page1 = pdfDoc1.AddNewPage();
				page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " 
					+ (i + 1).ToString() + "\n"));
				page1.Flush();
			}
			pdfDoc1.Close();
			FileStream fis1 = new FileStream(destinationFolder + "copying2_1.pdf", FileMode.Open
				);
			PdfReader reader1 = new PdfReader(fis1);
			pdfDoc1 = new PdfDocument(reader1);
			FileOutputStream fos2 = new FileOutputStream(destinationFolder + "copying2_2.pdf"
				, FileMode.Create);
			PdfWriter writer2 = new PdfWriter(fos2);
			PdfDocument pdfDoc2 = new PdfDocument(writer2);
			for (int i_1 = 0; i_1 < 10; i_1++)
			{
				if (i_1 % 2 == 0)
				{
					pdfDoc2.AddPage(pdfDoc1.GetPage(i_1 + 1).CopyTo(pdfDoc2));
				}
			}
			pdfDoc2.Close();
			pdfDoc1.Close();
			PdfReader reader = new PdfReader(destinationFolder + "copying2_2.pdf");
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			for (int i_2 = 0; i_2 < 5; i_2++)
			{
				byte[] bytes = pdfDocument.GetPage(i_2 + 1).GetContentBytes();
				NUnit.Framework.Assert.AreEqual("%page " + (i_2 * 2 + 1).ToString() + "\n", iTextSharp.IO.Util.JavaUtil.GetStringForBytes
					(bytes));
			}
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void Copying3()
		{
			FileOutputStream fos = new FileOutputStream(destinationFolder + "copying3_1.pdf", 
				FileMode.Create);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			PdfDictionary helloWorld = ((PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc
				));
			PdfDictionary helloWorld1 = ((PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc
				));
			helloWorld.Put(new PdfName("Hello"), new PdfString("World"));
			helloWorld.Put(new PdfName("HelloWrld"), helloWorld);
			helloWorld.Put(new PdfName("HelloWrld1"), helloWorld1);
			PdfPage page = pdfDoc.AddNewPage();
			page.GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
			page.GetPdfObject().Put(new PdfName("HelloWorldClone"), (PdfObject)helloWorld.Clone
				());
			pdfDoc.Close();
			PdfReader reader = new PdfReader(destinationFolder + "copying3_1.pdf");
			NUnit.Framework.Assert.AreEqual("Rebuilt", false, reader.HasRebuiltXref());
			pdfDoc = new PdfDocument(reader);
			PdfDictionary dic0 = pdfDoc.GetPage(1).GetPdfObject().GetAsDictionary(new PdfName
				("HelloWorld"));
			NUnit.Framework.Assert.AreEqual(4, dic0.GetIndirectReference().GetObjNumber());
			NUnit.Framework.Assert.AreEqual(0, dic0.GetIndirectReference().GetGenNumber());
			PdfDictionary dic1 = pdfDoc.GetPage(1).GetPdfObject().GetAsDictionary(new PdfName
				("HelloWorldClone"));
			NUnit.Framework.Assert.AreEqual(8, dic1.GetIndirectReference().GetObjNumber());
			NUnit.Framework.Assert.AreEqual(0, dic1.GetIndirectReference().GetGenNumber());
			PdfString str0 = dic0.GetAsString(new PdfName("Hello"));
			PdfString str1 = dic1.GetAsString(new PdfName("Hello"));
			NUnit.Framework.Assert.AreEqual(str0.GetValue(), str1.GetValue());
			NUnit.Framework.Assert.AreEqual(str0.GetValue(), "World");
			PdfDictionary dic01 = dic0.GetAsDictionary(new PdfName("HelloWrld"));
			PdfDictionary dic11 = dic1.GetAsDictionary(new PdfName("HelloWrld"));
			NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetObjNumber(), dic11
				.GetIndirectReference().GetObjNumber());
			NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetGenNumber(), dic11
				.GetIndirectReference().GetGenNumber());
			NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetObjNumber(), 4);
			NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetGenNumber(), 0);
			PdfDictionary dic02 = dic0.GetAsDictionary(new PdfName("HelloWrld1"));
			PdfDictionary dic12 = dic1.GetAsDictionary(new PdfName("HelloWrld1"));
			NUnit.Framework.Assert.AreEqual(dic02.GetIndirectReference().GetObjNumber(), dic12
				.GetIndirectReference().GetObjNumber());
			NUnit.Framework.Assert.AreEqual(dic02.GetIndirectReference().GetGenNumber(), dic12
				.GetIndirectReference().GetGenNumber());
			NUnit.Framework.Assert.AreEqual(dic12.GetIndirectReference().GetObjNumber(), 5);
			NUnit.Framework.Assert.AreEqual(dic12.GetIndirectReference().GetGenNumber(), 0);
			reader.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
		public virtual void CopyDocumentsWithFormFieldsTest()
		{
			String filename = sourceFolder + "fieldsOn2-sPage.pdf";
			PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open));
			FileOutputStream fos = new FileOutputStream(destinationFolder + "copyDocumentsWithFormFields.pdf"
				, FileMode.Create);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument sourceDoc = new PdfDocument(reader);
			PdfDocument pdfDoc = new PdfDocument(writer);
			sourceDoc.InitializeOutlines();
			sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), pdfDoc);
			pdfDoc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + "copyDocumentsWithFormFields.pdf", sourceFolder + "cmp_copyDocumentsWithFormFields.pdf"
				, destinationFolder, "diff_"));
		}
	}
}

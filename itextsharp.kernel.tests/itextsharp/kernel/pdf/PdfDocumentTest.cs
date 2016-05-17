using System;
using System.IO;
using Java.IO;
using NUnit.Framework;
using iTextSharp.IO.Image;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Pdf.Navigation;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfDocumentTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/kernel/pdf/PdfDocumentTest/";

		public const String destinationFolder = "test/itextsharp/kernel/pdf/PdfDocumentTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateOrClearDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void WritingVersionTest01()
		{
			// There is a possibility to override version in stamping mode
			String @out = destinationFolder + "writing_pdf_version.pdf";
			PdfDocument pdfDoc = new PdfDocument(new PdfWriter(@out, new WriterProperties().SetPdfVersion
				(PdfVersion.PDF_2_0)));
			NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, pdfDoc.GetPdfVersion());
			pdfDoc.AddNewPage();
			pdfDoc.Close();
			PdfDocument assertPdfDoc = new PdfDocument(new PdfReader(@out));
			NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, assertPdfDoc.GetPdfVersion());
			assertPdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void AddOutlinesWithNamedDestinations01()
		{
			PdfReader reader = new PdfReader(new FileStream(sourceFolder + "iphone_user_guide.pdf"
				));
			String filename = destinationFolder + "outlinesWithNamedDestinations01.pdf";
			FileOutputStream fos = new FileOutputStream(filename);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(reader, writer);
			PdfArray array1 = new PdfArray();
			array1.Add(pdfDoc.GetPage(2).GetPdfObject());
			array1.Add(PdfName.XYZ);
			array1.Add(new PdfNumber(36));
			array1.Add(new PdfNumber(806));
			array1.Add(new PdfNumber(0));
			PdfArray array2 = new PdfArray();
			array2.Add(pdfDoc.GetPage(3).GetPdfObject());
			array2.Add(PdfName.XYZ);
			array2.Add(new PdfNumber(36));
			array2.Add(new PdfNumber(806));
			array2.Add(new PdfNumber(1.25));
			PdfArray array3 = new PdfArray();
			array3.Add(pdfDoc.GetPage(4).GetPdfObject());
			array3.Add(PdfName.XYZ);
			array3.Add(new PdfNumber(36));
			array3.Add(new PdfNumber(806));
			array3.Add(new PdfNumber(1));
			pdfDoc.AddNamedDestination("test1", array2);
			pdfDoc.AddNamedDestination("test2", array3);
			pdfDoc.AddNamedDestination("test3", array1);
			PdfOutline root = pdfDoc.GetOutlines(false);
			if (root == null)
			{
				root = new PdfOutline(pdfDoc);
			}
			PdfOutline firstOutline = root.AddOutline("Test1");
			firstOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test1")
				));
			PdfOutline secondOutline = root.AddOutline("Test2");
			secondOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test2"
				)));
			PdfOutline thirdOutline = root.AddOutline("Test3");
			thirdOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test3")
				));
			pdfDoc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder
				 + "cmp_outlinesWithNamedDestinations01.pdf", destinationFolder, "diff_"));
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void FreeReferencesInObjectStream()
		{
			PdfReader reader = new PdfReader(sourceFolder + "styledLineArts_Redacted.pdf");
			PdfWriter writer = new PdfWriter(new MemoryStream());
			PdfDocument document = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode
				());
			PdfDictionary dict = new PdfDictionary();
			dict.MakeIndirect(document);
			NUnit.Framework.Assert.IsTrue(dict.GetIndirectReference().GetObjNumber() > 0);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void RemoveUnusedObjectsInWriterModeTest()
		{
			String filename = "removeUnusedObjectsInWriter.pdf";
			PdfWriter writer = new PdfWriter(new FileOutputStream(destinationFolder + filename
				));
			PdfDocument pdfDocument = new PdfDocument(writer);
			pdfDocument.AddNewPage();
			PdfDictionary unusedDictionary = new PdfDictionary();
			PdfArray unusedArray = ((PdfArray)new PdfArray().MakeIndirect(pdfDocument));
			unusedArray.Add(new PdfNumber(42));
			unusedDictionary.Put(new PdfName("testName"), unusedArray);
			unusedDictionary.MakeIndirect(pdfDocument);
			NUnit.Framework.Assert.AreEqual(pdfDocument.GetXref().Size(), 8);
			//on closing, all unused objects shall not be written to resultant document
			pdfDocument.Close();
			PdfReader testerReader = new PdfReader(destinationFolder + filename);
			PdfDocument testerDocument = new PdfDocument(testerReader);
			NUnit.Framework.Assert.AreEqual(testerDocument.GetXref().Size(), 6);
			testerDocument.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void RemoveUnusedObjectsInStampingModeTest()
		{
			String filenameIn = "docWithUnusedObjects_1.pdf";
			String filenameOut = "removeUnusedObjectsInStamping.pdf";
			PdfWriter writer = new PdfWriter(new FileOutputStream(destinationFolder + filenameIn
				));
			PdfDocument pdfDocument = new PdfDocument(writer);
			pdfDocument.AddNewPage();
			PdfDictionary unusedDictionary = new PdfDictionary();
			PdfArray unusedArray = ((PdfArray)new PdfArray().MakeIndirect(pdfDocument));
			unusedArray.Add(new PdfNumber(42));
			unusedDictionary.Put(new PdfName("testName"), unusedArray);
			((PdfDictionary)unusedDictionary.MakeIndirect(pdfDocument)).Flush();
			pdfDocument.Close();
			PdfReader reader = new PdfReader(destinationFolder + filenameIn);
			PdfDocument doc = new PdfDocument(reader, new PdfWriter(new FileOutputStream(destinationFolder
				 + filenameOut)));
			NUnit.Framework.Assert.AreEqual(doc.GetXref().Size(), 8);
			//on closing, all unused objects shall not be written to resultant document
			doc.Close();
			PdfReader testerReader = new PdfReader(destinationFolder + filenameOut);
			PdfDocument testerDocument = new PdfDocument(testerReader);
			NUnit.Framework.Assert.AreEqual(testerDocument.GetXref().Size(), 6);
			testerDocument.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void AddUnusedObjectsInWriterModeTest()
		{
			String filename = "addUnusedObjectsInWriter.pdf";
			PdfWriter writer = new PdfWriter(new FileOutputStream(destinationFolder + filename
				));
			PdfDocument pdfDocument = new PdfDocument(writer);
			pdfDocument.AddNewPage();
			PdfDictionary unusedDictionary = new PdfDictionary();
			PdfArray unusedArray = ((PdfArray)new PdfArray().MakeIndirect(pdfDocument));
			unusedArray.Add(new PdfNumber(42));
			unusedDictionary.Put(new PdfName("testName"), unusedArray);
			unusedDictionary.MakeIndirect(pdfDocument);
			NUnit.Framework.Assert.AreEqual(pdfDocument.GetXref().Size(), 8);
			pdfDocument.SetFlushUnusedObjects(true);
			pdfDocument.Close();
			PdfReader testerReader = new PdfReader(destinationFolder + filename);
			PdfDocument testerDocument = new PdfDocument(testerReader);
			NUnit.Framework.Assert.AreEqual(testerDocument.GetXref().Size(), 8);
			testerDocument.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void AddUnusedObjectsInStampingModeTest()
		{
			String filenameIn = "docWithUnusedObjects_2.pdf";
			String filenameOut = "addUnusedObjectsInStamping.pdf";
			PdfWriter writer = new PdfWriter(new FileOutputStream(destinationFolder + filenameIn
				));
			PdfDocument pdfDocument = new PdfDocument(writer);
			pdfDocument.AddNewPage();
			PdfDictionary unusedDictionary = new PdfDictionary();
			PdfArray unusedArray = ((PdfArray)new PdfArray().MakeIndirect(pdfDocument));
			unusedArray.Add(new PdfNumber(42));
			unusedDictionary.Put(new PdfName("testName"), unusedArray);
			((PdfDictionary)unusedDictionary.MakeIndirect(pdfDocument)).Flush();
			pdfDocument.Close();
			PdfReader reader = new PdfReader(destinationFolder + filenameIn);
			PdfDocument doc = new PdfDocument(reader, new PdfWriter(new FileOutputStream(destinationFolder
				 + filenameOut)));
			NUnit.Framework.Assert.AreEqual(doc.GetXref().Size(), 8);
			doc.SetFlushUnusedObjects(true);
			doc.Close();
			PdfReader testerReader = new PdfReader(destinationFolder + filenameOut);
			PdfDocument testerDocument = new PdfDocument(testerReader);
			NUnit.Framework.Assert.AreEqual(testerDocument.GetXref().Size(), 8);
			testerDocument.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void AddUnusedStreamObjectsTest()
		{
			String filenameIn = "docWithUnusedObjects_3.pdf";
			PdfWriter writer = new PdfWriter(new FileOutputStream(destinationFolder + filenameIn
				));
			PdfDocument pdfDocument = new PdfDocument(writer);
			pdfDocument.AddNewPage();
			PdfDictionary unusedDictionary = new PdfDictionary();
			PdfArray unusedArray = ((PdfArray)new PdfArray().MakeIndirect(pdfDocument));
			unusedArray.Add(new PdfNumber(42));
			PdfStream stream = new PdfStream(new byte[] { 1, 2, 34, 45 }, 0);
			unusedArray.Add(stream);
			unusedDictionary.Put(new PdfName("testName"), unusedArray);
			((PdfDictionary)unusedDictionary.MakeIndirect(pdfDocument)).Flush();
			pdfDocument.SetFlushUnusedObjects(true);
			pdfDocument.Close();
			PdfReader testerReader = new PdfReader(destinationFolder + filenameIn);
			PdfDocument testerDocument = new PdfDocument(testerReader);
			NUnit.Framework.Assert.AreEqual(testerDocument.GetXref().Size(), 9);
			testerDocument.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void TestImageCompressLevel()
		{
			byte[] b = ImageDataFactory.Create(sourceFolder + "berlin2013.jpg").GetData();
			ByteArrayOutputStream image = new ByteArrayOutputStream();
			image.AssignBytes(b, b.Length);
			MemoryStream byteArrayStream1 = new ByteArrayOutputStream();
			DeflaterOutputStream zip = new DeflaterOutputStream(byteArrayStream1, 9);
			image.WriteTo(zip);
			zip.Close();
			MemoryStream byteArrayStream2 = new ByteArrayOutputStream();
			DeflaterOutputStream zip2 = new DeflaterOutputStream(byteArrayStream2, -1);
			image.WriteTo(zip2);
			zip2.Close();
			NUnit.Framework.Assert.IsTrue(byteArrayStream1.Length == byteArrayStream2.Length);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestFreeReference()
		{
			PdfWriter writer = new PdfWriter(destinationFolder + "freeReference.pdf", new WriterProperties
				().SetFullCompressionMode(false));
			PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "baseFreeReference.pdf"
				), writer);
			pdfDocument.GetPage(1).GetResources().GetPdfObject().GetAsArray(new PdfName("d"))
				.Get(0).GetIndirectReference().SetFree();
			PdfStream pdfStream = new PdfStream();
			pdfStream.SetData(new byte[] { 24, 23, 67 });
			pdfStream.MakeIndirect(pdfDocument);
			pdfDocument.GetPage(1).GetResources().GetPdfObject().GetAsArray(new PdfName("d"))
				.Add(pdfStream);
			pdfDocument.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + "freeReference.pdf", sourceFolder + "cmp_freeReference.pdf", destinationFolder
				, "diff_"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void CheckAndResolveCircularReferences()
		{
			PdfReader pdfReader = new PdfReader(sourceFolder + "datasheet.pdf");
			PdfDocument pdfDocument = new PdfDocument(pdfReader, new PdfWriter(destinationFolder
				 + "datasheet_mode.pdf"));
			PdfDictionary pdfObject = (PdfDictionary)pdfDocument.GetPdfObject(53);
			pdfDocument.GetPage(1).GetResources().AddForm(pdfObject);
			pdfDocument.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + "datasheet_mode.pdf", sourceFolder + "cmp_datasheet_mode.pdf", "d:/", "diff_"
				));
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void ReadEncryptedDocumentWithFullCompression()
		{
			PdfReader reader = new PdfReader(new FileStream(sourceFolder + "source.pdf"), new 
				ReaderProperties().SetPassword("123".GetBytes()));
			PdfDocument pdfDocument = new PdfDocument(reader);
			PdfDictionary form = pdfDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName
				.AcroForm);
			PdfDictionary field = form.GetAsArray(PdfName.Fields).GetAsDictionary(0);
			NUnit.Framework.Assert.AreEqual("ch", field.GetAsString(PdfName.T).ToUnicodeString
				());
			NUnit.Framework.Assert.AreEqual("SomeStringValueInDictionary", field.GetAsDictionary
				(new PdfName("TestDic")).GetAsString(new PdfName("TestString")).ToUnicodeString(
				));
			NUnit.Framework.Assert.AreEqual("SomeStringValueInArray", field.GetAsArray(new PdfName
				("TestArray")).GetAsString(0).ToUnicodeString());
			pdfDocument.Close();
		}
	}
}

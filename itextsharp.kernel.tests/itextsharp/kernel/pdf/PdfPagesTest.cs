using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.IO;
using iTextSharp.Kernel;
using iTextSharp.Test;
using iTextSharp.Test.Attributes;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfPagesTest : ExtendedITextTest
	{
		public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
			.TestDirectory + "/test/itextsharp/kernel/pdf/PdfPagesTest/";

		public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
			.TestDirectory + "/../../resources/itextsharp/kernel/pdf/PdfPagesTest/";

		internal static readonly PdfName PageNum = new PdfName("PageNum");

		internal static readonly PdfName PageNum5 = new PdfName("PageNum");

		[NUnit.Framework.TestFixtureSetUp]
		public static void Setup()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void SimplePagesTest()
		{
			String filename = "simplePagesTest.pdf";
			int pageCount = 111;
			FileStream fos = new FileStream(destinationFolder + filename, FileMode.Create);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = 0; i < pageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				page.GetPdfObject().Put(PageNum, new PdfNumber(i + 1));
				page.Flush();
			}
			pdfDoc.Close();
			VerifyPagesOrder(destinationFolder + filename, pageCount);
		}

		//    @Test
		//    public void simpleClonePagesTest() throws IOException {
		//        String filename = "simpleClonePagesTest.pdf";
		//        int pageCount = 111;
		//
		//        FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
		//        PdfWriter writer = new PdfWriter(fos);
		//        PdfDocument pdfDoc = new PdfDocument(writer);
		//
		//        for (int i = 0; i < pageCount; i++) {
		//            PdfPage page = pdfDoc.addNewPage();
		//            page.getPdfObject().put(PageNum, new PdfNumber(i + 1));
		//        }
		//        for (int i = 0; i < pageCount; i++) {
		//            PdfPage page = pdfDoc.addPage((PdfPage)pdfDoc.getPage(i + 1).clone());
		//            page.getPdfObject().put(PageNum, new PdfNumber(pageCount + i + 1));
		//            pdfDoc.getPage(i + 1).flush();
		//            page.flush();
		//        }
		//        pdfDoc.close();
		//        verifyPagesOrder(destinationFolder + filename, pageCount);
		//    }
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void ReversePagesTest()
		{
			String filename = "reversePagesTest.pdf";
			int pageCount = 111;
			FileStream fos = new FileStream(destinationFolder + filename, FileMode.Create);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = pageCount; i > 0; i--)
			{
				PdfPage page = new PdfPage(pdfDoc, pdfDoc.GetDefaultPageSize());
				pdfDoc.AddPage(1, page);
				page.GetPdfObject().Put(PageNum, new PdfNumber(i));
				page.Flush();
			}
			pdfDoc.Close();
			VerifyPagesOrder(destinationFolder + filename, pageCount);
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void RandomObjectPagesTest()
		{
			String filename = "randomObjectPagesTest.pdf";
			int pageCount = 10000;
			int[] indexes = new int[pageCount];
			for (int i = 0; i < indexes.Length; i++)
			{
				indexes[i] = i + 1;
			}
			Random rnd = new Random();
			for (int i_1 = indexes.Length - 1; i_1 > 0; i_1--)
			{
				int index = rnd.Next(i_1 + 1);
				int a = indexes[index];
				indexes[index] = indexes[i_1];
				indexes[i_1] = a;
			}
			FileStream fos = new FileStream(destinationFolder + filename, FileMode.Create);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument document = new PdfDocument(writer);
			PdfPage[] pages = new PdfPage[pageCount];
			for (int i_2 = 0; i_2 < indexes.Length; i_2++)
			{
				PdfPage page = document.AddNewPage();
				page.GetPdfObject().Put(PageNum, new PdfNumber(indexes[i_2]));
				//page.flush();
				pages[indexes[i_2] - 1] = page;
			}
			int xrefSize = document.GetXref().Size();
			PdfPage testPage = document.RemovePage(1000);
			NUnit.Framework.Assert.IsTrue(testPage.GetPdfObject().GetIndirectReference() == null
				);
			document.AddPage(1000, testPage);
			NUnit.Framework.Assert.IsTrue(testPage.GetPdfObject().GetIndirectReference().GetObjNumber
				() < xrefSize);
			for (int i_3 = 0; i_3 < pages.Length; i_3++)
			{
				NUnit.Framework.Assert.AreEqual(true, document.RemovePage(pages[i_3]), "Remove page"
					);
				document.AddPage(i_3 + 1, pages[i_3]);
			}
			document.Close();
			VerifyPagesOrder(destinationFolder + filename, pageCount);
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void RandomNumberPagesTest()
		{
			String filename = "randomNumberPagesTest.pdf";
			int pageCount = 3000;
			int[] indexes = new int[pageCount];
			for (int i = 0; i < indexes.Length; i++)
			{
				indexes[i] = i + 1;
			}
			Random rnd = new Random();
			for (int i_1 = indexes.Length - 1; i_1 > 0; i_1--)
			{
				int index = rnd.Next(i_1 + 1);
				int a = indexes[index];
				indexes[index] = indexes[i_1];
				indexes[i_1] = a;
			}
			FileStream fos = new FileStream(destinationFolder + filename, FileMode.Create);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i_2 = 0; i_2 < indexes.Length; i_2++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				page.GetPdfObject().Put(PageNum, new PdfNumber(indexes[i_2]));
			}
			for (int i_3 = 1; i_3 < pageCount; i_3++)
			{
				for (int j = i_3 + 1; j <= pageCount; j++)
				{
					int j_page = pdfDoc.GetPage(j).GetPdfObject().GetAsNumber(PageNum).IntValue();
					int i_page = pdfDoc.GetPage(i_3).GetPdfObject().GetAsNumber(PageNum).IntValue();
					if (j_page < i_page)
					{
						PdfPage page = pdfDoc.RemovePage(j);
						pdfDoc.AddPage(i_3 + 1, page);
						page = pdfDoc.RemovePage(i_3);
						pdfDoc.AddPage(j, page);
					}
				}
				NUnit.Framework.Assert.IsTrue(VerifyIntegrity(pdfDoc.GetCatalog().GetPageTree()) 
					== -1);
			}
			pdfDoc.Close();
			VerifyPagesOrder(destinationFolder + filename, pageCount);
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED)]
		public virtual void InsertFlushedPageTest()
		{
			PdfWriter writer = new PdfWriter(new MemoryStream());
			PdfDocument pdfDoc = new PdfDocument(writer);
			PdfPage page = pdfDoc.AddNewPage();
			bool error = false;
			try
			{
				page.Flush();
				pdfDoc.RemovePage(page);
				pdfDoc.AddPage(1, page);
				pdfDoc.Close();
			}
			catch (PdfException e)
			{
				if (PdfException.FlushedPageCannotBeAddedOrInserted.Equals(e.Message))
				{
					error = true;
				}
			}
			NUnit.Framework.Assert.IsTrue(error);
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED)]
		public virtual void AddFlushedPageTest()
		{
			PdfWriter writer = new PdfWriter(new MemoryStream());
			PdfDocument pdfDoc = new PdfDocument(writer);
			PdfPage page = pdfDoc.AddNewPage();
			bool error = false;
			try
			{
				page.Flush();
				pdfDoc.RemovePage(page);
				pdfDoc.AddPage(page);
				pdfDoc.Close();
			}
			catch (PdfException e)
			{
				if (PdfException.FlushedPageCannotBeAddedOrInserted.Equals(e.Message))
				{
					error = true;
				}
			}
			NUnit.Framework.Assert.IsTrue(error);
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED, Count = 2)]
		public virtual void RemoveFlushedPage()
		{
			String filename = "removeFlushedPage.pdf";
			int pageCount = 10;
			FileStream fos = new FileStream(destinationFolder + filename, FileMode.Create);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			PdfPage removedPage = pdfDoc.AddNewPage();
			int removedPageObjectNumber = removedPage.GetPdfObject().GetIndirectReference().GetObjNumber
				();
			removedPage.Flush();
			pdfDoc.RemovePage(removedPage);
			for (int i = 0; i < pageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				page.GetPdfObject().Put(PageNum, new PdfNumber(i + 1));
				page.Flush();
			}
			NUnit.Framework.Assert.AreEqual(true, pdfDoc.RemovePage(pdfDoc.GetPage(pageCount)
				), "Remove last page");
			NUnit.Framework.Assert.AreEqual(true, pdfDoc.GetXref().Get(removedPageObjectNumber
				).CheckState(PdfObject.FREE), "Free reference");
			pdfDoc.Close();
			VerifyPagesOrder(destinationFolder + filename, pageCount - 1);
		}

		/// <exception cref="System.IO.IOException"/>
		internal virtual void VerifyPagesOrder(String filename, int numOfPages)
		{
			PdfReader reader = new PdfReader(filename);
			PdfDocument pdfDocument = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
			for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
			{
				PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
				NUnit.Framework.Assert.IsNotNull(page);
				PdfNumber number = page.GetAsNumber(PageNum5);
				NUnit.Framework.Assert.AreEqual(i, number.IntValue(), "Page number");
			}
			NUnit.Framework.Assert.AreEqual(numOfPages, pdfDocument.GetNumberOfPages(), "Number of pages"
				);
			reader.Close();
		}

		internal virtual int VerifyIntegrity(PdfPagesTree pagesTree)
		{
			IList<PdfPages> parents = pagesTree.GetParents();
			int from = 0;
			for (int i = 0; i < parents.Count; i++)
			{
				if (parents[i].GetFrom() != from)
				{
					return i;
				}
				from = parents[i].GetFrom() + parents[i].GetCount();
			}
			return -1;
		}

		//    @Test@Ignore
		//    public void testInheritedResources() throws IOException {
		//        String inputFileName1 = sourceFolder + "veraPDF-A003-a-pass.pdf";
		//        PdfReader reader1 = new PdfReader(inputFileName1);
		//        PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
		//        PdfPage page = inputPdfDoc1.getPage(1);
		//        List<PdfFont> list = page.getResources().getFonts(true);
		//        Assert.assertEquals(1, list.size());
		//        Assert.assertEquals("ASJKFO+Arial-BoldMT", list.get(0).getFontProgram().getFontNames().getFontName());
		//    }
		//
		//    @Test(expected = PdfException.class)
		//    public void testCircularReferencesInResources() throws IOException {
		//        String inputFileName1 = sourceFolder + "circularReferencesInResources.pdf";
		//        PdfReader reader1 = new PdfReader(inputFileName1);
		//        PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
		//        PdfPage page = inputPdfDoc1.getPage(1);
		//        List<PdfFont> list = page.getResources().getFonts(true);
		//    }
		//
		//    @Test@Ignore
		//    public void testInheritedResourcesUpdate() throws IOException {
		//        String inputFileName1 = sourceFolder + "veraPDF-A003-a-pass.pdf";
		//        PdfReader reader1 = new PdfReader(inputFileName1);
		//
		//        FileOutputStream fos = new FileOutputStream(destinationFolder + "veraPDF-A003-a-pass_new.pdf");
		//        PdfWriter writer = new PdfWriter(fos);
		//        writer.setCompressionLevel(PdfOutputStream.NO_COMPRESSION);
		//        PdfDocument pdfDoc = new PdfDocument(reader1, writer);
		//        pdfDoc.getPage(1).getResources().getFonts(true);
		//        PdfFont f = PdfFont.createFont((PdfDictionary) pdfDoc.getPdfObject(6));
		//        pdfDoc.getPage(1).getResources().addFont(pdfDoc, f);
		//        int fontCount = pdfDoc.getPage(1).getResources().getFonts(false).size();
		//        pdfDoc.getPage(1).flush();
		//        pdfDoc.close();
		//
		//        Assert.assertEquals(2, fontCount);
		//    }
		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void GetPageByDictionary()
		{
			String filename = sourceFolder + "1000PagesDocument.pdf";
			PdfReader reader = new PdfReader(filename);
			PdfDocument pdfDoc = new PdfDocument(reader);
			PdfObject[] pageDictionaries = new PdfObject[] { pdfDoc.GetPdfObject(4), pdfDoc.GetPdfObject
				(255), pdfDoc.GetPdfObject(512), pdfDoc.GetPdfObject(1023), pdfDoc.GetPdfObject(
				2049), pdfDoc.GetPdfObject(3100) };
			foreach (PdfObject pageObject in pageDictionaries)
			{
				PdfDictionary pageDictionary = (PdfDictionary)pageObject;
				NUnit.Framework.Assert.AreEqual(PdfName.Page, pageDictionary.Get(PdfName.Type));
				PdfPage page = pdfDoc.GetPage(pageDictionary);
				NUnit.Framework.Assert.AreEqual(pageDictionary, page.GetPdfObject());
			}
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void RemovePageWithFormFieldsTest()
		{
			String filename = sourceFolder + "docWithFields.pdf";
			PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
			pdfDoc.RemovePage(1);
			PdfArray fields = pdfDoc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm
				).GetAsArray(PdfName.Fields);
			PdfDictionary field = (PdfDictionary)fields.Get(0);
			PdfDictionary kid = (PdfDictionary)field.GetAsArray(PdfName.Kids).Get(0);
			NUnit.Framework.Assert.AreEqual(6, kid.KeySet().Count);
			NUnit.Framework.Assert.AreEqual(3, fields.Size());
		}
	}
}

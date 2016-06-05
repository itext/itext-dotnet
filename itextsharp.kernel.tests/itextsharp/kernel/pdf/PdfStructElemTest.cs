using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.IO;
using iTextSharp.IO.Font;
using iTextSharp.Kernel.Font;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf.Annot;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Pdf.Tagging;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;
using iTextSharp.Test.Attributes;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfStructElemTest : ExtendedITextTest
	{
		public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
			.TestDirectory + "/../../resources/itextsharp/kernel/pdf/PdfStructElemTest/";

		public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
			.TestDirectory + "/test/itextsharp/kernel/pdf/PdfStructElemTest/";

		[NUnit.Framework.TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateOrClearDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void StructElemTest01()
		{
			FileStream fos = new FileStream(destinationFolder + "structElemTest01.pdf", FileMode
				.Create);
			PdfWriter writer = new PdfWriter(fos);
			writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
			PdfDocument document = new PdfDocument(writer);
			document.SetTagged();
			PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document
				, PdfName.Document));
			PdfPage page1 = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page1);
			canvas.BeginText();
			canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 24);
			canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
			PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
			PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, 
				page1));
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page1, span1))));
			canvas.ShowText("Hello ");
			canvas.CloseTag();
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrDictionary(page1, span1))));
			canvas.ShowText("World");
			canvas.CloseTag();
			canvas.EndText();
			canvas.Release();
			PdfPage page2 = document.AddNewPage();
			canvas = new PdfCanvas(page2);
			canvas.BeginText();
			canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA), 24);
			canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
			paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
			span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page2));
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page2, span1))));
			canvas.ShowText("Hello ");
			canvas.CloseTag();
			PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, 
				page2));
			canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page2, span2))));
			canvas.ShowText("World");
			canvas.CloseTag();
			canvas.EndText();
			canvas.Release();
			page1.Flush();
			page2.Flush();
			document.Close();
			CompareResult("structElemTest01.pdf", "cmp_structElemTest01.pdf", "diff_structElem_01_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void StructElemTest02()
		{
			FileStream fos = new FileStream(destinationFolder + "structElemTest02.pdf", FileMode
				.Create);
			PdfWriter writer = new PdfWriter(fos);
			writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
			PdfDocument document = new PdfDocument(writer);
			document.SetTagged();
			document.GetStructTreeRoot().GetRoleMap().Put(new PdfName("Chunk"), PdfName.Span);
			PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document
				, PdfName.Document));
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.BeginText();
			canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 24);
			canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
			PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
			PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, 
				page));
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))));
			canvas.ShowText("Hello ");
			canvas.CloseTag();
			PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, new PdfName("Chunk"
				), page));
			canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page, span2))));
			canvas.ShowText("World");
			canvas.CloseTag();
			canvas.EndText();
			canvas.Release();
			page.Flush();
			document.Close();
			CompareResult("structElemTest02.pdf", "cmp_structElemTest02.pdf", "diff_structElem_02_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void StructElemTest03()
		{
			FileStream fos = new FileStream(destinationFolder + "structElemTest03.pdf", FileMode
				.Create);
			PdfWriter writer = new PdfWriter(fos);
			writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
			PdfDocument document = new PdfDocument(writer);
			document.SetTagged();
			document.GetStructTreeRoot().GetRoleMap().Put(new PdfName("Chunk"), PdfName.Span);
			PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document
				, PdfName.Document));
			PdfPage page1 = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page1);
			canvas.BeginText();
			canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 24);
			canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
			PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
			PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, 
				page1));
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page1, span1))));
			canvas.ShowText("Hello ");
			canvas.CloseTag();
			PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, new PdfName("Chunk"
				), page1));
			canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page1, span2))));
			canvas.ShowText("World");
			canvas.CloseTag();
			canvas.EndText();
			canvas.Release();
			PdfPage page2 = document.AddNewPage();
			canvas = new PdfCanvas(page2);
			canvas.BeginText();
			canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA), 24);
			canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
			paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
			span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page2));
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page2, span1))));
			canvas.ShowText("Hello ");
			canvas.CloseTag();
			span2 = paragraph.AddKid(new PdfStructElem(document, new PdfName("Chunk"), page2)
				);
			canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page2, span2))));
			canvas.ShowText("World");
			canvas.CloseTag();
			canvas.EndText();
			canvas.Release();
			page1.Flush();
			page2.Flush();
			document.Close();
			PdfReader reader = new PdfReader(new FileStream(destinationFolder + "structElemTest03.pdf"
				, FileMode.Open, FileAccess.Read));
			document = new PdfDocument(reader);
			NUnit.Framework.Assert.AreEqual(2, (int)document.GetNextStructParentIndex());
			PdfPage page = document.GetPage(1);
			NUnit.Framework.Assert.AreEqual(0, page.GetStructParentIndex());
			NUnit.Framework.Assert.AreEqual(2, page.GetNextMcid());
			document.Close();
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void StructElemTest04()
		{
			MemoryStream baos = new MemoryStream();
			PdfWriter writer = new PdfWriter(baos);
			writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
			PdfDocument document = new PdfDocument(writer);
			document.SetTagged();
			document.GetStructTreeRoot().GetRoleMap().Put(new PdfName("Chunk"), PdfName.Span);
			PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document
				, PdfName.Document));
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.BeginText();
			canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 24);
			canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
			PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
			PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, 
				page));
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))));
			canvas.ShowText("Hello ");
			canvas.CloseTag();
			PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, new PdfName("Chunk"
				), page));
			canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page, span2))));
			canvas.ShowText("World");
			canvas.CloseTag();
			canvas.EndText();
			canvas.Release();
			page.Flush();
			document.Close();
			byte[] bytes = baos.ToArray();
			PdfReader reader = new PdfReader(new MemoryStream(bytes));
			writer = new PdfWriter(new FileStream(destinationFolder + "structElemTest04.pdf", 
				FileMode.Create));
			writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
			document = new PdfDocument(reader, writer);
			page = document.GetPage(1);
			canvas = new PdfCanvas(page);
			PdfStructElem p = (PdfStructElem)document.GetStructTreeRoot().GetKids()[0].GetKids
				()[0];
			canvas.BeginText();
			canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 24);
			canvas.SetTextMatrix(1, 0, 0, 1, 32, 490);
			//Inserting span between of 2 existing ones.
			span1 = p.AddKid(1, new PdfStructElem(document, PdfName.Span, page));
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))));
			canvas.ShowText("text1");
			canvas.CloseTag();
			//Inserting span at the end.
			span1 = p.AddKid(new PdfStructElem(document, PdfName.Span, page));
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))));
			canvas.ShowText("text2");
			canvas.CloseTag();
			canvas.EndText();
			canvas.Release();
			page.Flush();
			document.Close();
			CompareResult("structElemTest04.pdf", "cmp_structElemTest04.pdf", "diff_structElem_04_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void StructElemTest05()
		{
			FileStream fos = new FileStream(destinationFolder + "structElemTest05.pdf", FileMode
				.Create);
			PdfWriter writer = new PdfWriter(fos);
			writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
			PdfDocument document = new PdfDocument(writer);
			document.SetTagged();
			PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document
				, PdfName.Document));
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.BeginText();
			canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 14);
			canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
			PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
			PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, 
				page));
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))));
			canvas.ShowText("Click ");
			canvas.CloseTag();
			PdfStructElem link = paragraph.AddKid(new PdfStructElem(document, PdfName.Link, page
				));
			canvas.OpenTag(new CanvasTag(link.AddKid(new PdfMcrNumber(page, link))));
			canvas.SetFillColorRgb(0, 0, 1).ShowText("here");
			PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(new Rectangle(80, 508, 40
				, 18));
			linkAnnotation.SetColor(new float[] { 0, 0, 1 }).SetBorder(new PdfArray(new float
				[] { 0, 0, 1 }));
			page.AddAnnotation(-1, linkAnnotation, false);
			link.AddKid(new PdfObjRef(linkAnnotation, link));
			canvas.CloseTag();
			PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, 
				page));
			canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page, span2))));
			canvas.SetFillColorRgb(0, 0, 0);
			canvas.ShowText(" to visit iText site.");
			canvas.CloseTag();
			canvas.EndText();
			canvas.Release();
			document.Close();
			CompareResult("structElemTest05.pdf", "cmp_structElemTest05.pdf", "diff_structElem_05_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void StructElemTest06()
		{
			FileStream fos = new FileStream(destinationFolder + "structElemTest06.pdf", FileMode
				.Create);
			PdfWriter writer = new PdfWriter(fos);
			writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
			PdfDocument document = new PdfDocument(writer);
			document.SetTagged();
			PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document
				, PdfName.Document));
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.BeginText();
			canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 14);
			canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
			PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
			PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, 
				page));
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))).AddProperty
				(PdfName.Lang, new PdfString("en-US")).AddProperty(PdfName.ActualText, new PdfString
				("The actual text is: Text with property list")));
			canvas.ShowText("Text with property list");
			canvas.CloseTag();
			canvas.EndText();
			canvas.Release();
			document.Close();
			CompareResult("structElemTest06.pdf", "cmp_structElemTest06.pdf", "diff_structElem_06_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
		public virtual void StructTreeCopyingTest01()
		{
			FileStream fis = new FileStream(sourceFolder + "iphone_user_guide.pdf", FileMode.
				Open, FileAccess.Read);
			PdfReader reader = new PdfReader(fis);
			PdfDocument source = new PdfDocument(reader);
			PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "structTreeCopyingTest01.pdf"
				, FileMode.Create));
			PdfDocument destination = new PdfDocument(writer);
			destination.SetTagged();
			destination.InitializeOutlines();
			List<int> pagesToCopy = new List<int>();
			pagesToCopy.Add(3);
			pagesToCopy.Add(4);
			pagesToCopy.Add(10);
			pagesToCopy.Add(11);
			source.CopyPagesTo(pagesToCopy, destination);
			source.CopyPagesTo(50, 52, destination);
			destination.Close();
			source.Close();
			CompareResult("structTreeCopyingTest01.pdf", "cmp_structTreeCopyingTest01.pdf", "diff_copying_01_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
		public virtual void StructTreeCopyingTest02()
		{
			FileStream fis = new FileStream(sourceFolder + "iphone_user_guide.pdf", FileMode.
				Open, FileAccess.Read);
			PdfReader reader = new PdfReader(fis);
			PdfDocument source = new PdfDocument(reader);
			PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "structTreeCopyingTest02.pdf"
				, FileMode.Create));
			PdfDocument destination = new PdfDocument(writer);
			destination.SetTagged();
			destination.InitializeOutlines();
			source.CopyPagesTo(6, source.GetNumberOfPages(), destination);
			source.CopyPagesTo(1, 5, destination);
			destination.Close();
			source.Close();
			CompareResult("structTreeCopyingTest02.pdf", "cmp_structTreeCopyingTest02.pdf", "diff_copying_02_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
		public virtual void StructTreeCopyingTest03()
		{
			FileStream fis = new FileStream(sourceFolder + "iphone_user_guide.pdf", FileMode.
				Open, FileAccess.Read);
			PdfReader reader = new PdfReader(fis);
			PdfDocument source = new PdfDocument(reader);
			PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "structTreeCopyingTest03.pdf"
				, FileMode.Create));
			PdfDocument destination = new PdfDocument(writer);
			destination.InitializeOutlines();
			source.CopyPagesTo(6, source.GetNumberOfPages(), destination);
			source.CopyPagesTo(1, 5, destination);
			destination.Close();
			source.Close();
			// we don't compare tag structures, because resultant document is not tagged
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + "structTreeCopyingTest03.pdf", sourceFolder + "cmp_structTreeCopyingTest03.pdf"
				, destinationFolder, "diff_copying_03_"));
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
		public virtual void StructTreeCopyingTest04()
		{
			FileStream fis = new FileStream(sourceFolder + "iphone_user_guide.pdf", FileMode.
				Open, FileAccess.Read);
			PdfReader reader = new PdfReader(fis);
			PdfDocument source = new PdfDocument(reader);
			PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "structTreeCopyingTest04.pdf"
				, FileMode.Create));
			PdfDocument destination = new PdfDocument(writer);
			destination.SetTagged();
			destination.InitializeOutlines();
			for (int i = 1; i <= source.GetNumberOfPages(); i++)
			{
				source.CopyPagesTo(i, i, destination);
			}
			destination.Close();
			source.Close();
			CompareResult("structTreeCopyingTest04.pdf", "cmp_structTreeCopyingTest04.pdf", "diff_copying_04_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void StructTreeCopyingTest05()
		{
			PdfReader reader = new PdfReader(new FileStream(sourceFolder + "iphone_user_guide.pdf"
				, FileMode.Open, FileAccess.Read));
			PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "structTreeCopyingTest05.pdf"
				, FileMode.Create));
			PdfDocument document = new PdfDocument(reader, writer);
			PdfReader reader1 = new PdfReader(new FileStream(sourceFolder + "quick-brown-fox.pdf"
				, FileMode.Open, FileAccess.Read));
			PdfDocument document1 = new PdfDocument(reader1);
			document1.CopyPagesTo(1, 1, document, 2);
			PdfReader reader2 = new PdfReader(new FileStream(sourceFolder + "quick-brown-fox-table.pdf"
				, FileMode.Open, FileAccess.Read));
			PdfDocument document2 = new PdfDocument(reader2);
			document2.CopyPagesTo(1, 3, document, 4);
			document.Close();
			document1.Close();
			document2.Close();
			CompareResult("structTreeCopyingTest05.pdf", "cmp_structTreeCopyingTest05.pdf", "diff_copying_05_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		[LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
		public virtual void StructTreeCopyingTest06()
		{
			FileStream fis = new FileStream(sourceFolder + "iphone_user_guide.pdf", FileMode.
				Open, FileAccess.Read);
			PdfReader reader = new PdfReader(fis);
			PdfDocument source = new PdfDocument(reader);
			PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "structTreeCopyingTest06.pdf"
				, FileMode.Create));
			PdfDocument destination = new PdfDocument(writer);
			destination.SetTagged();
			destination.InitializeOutlines();
			source.CopyPagesTo(1, source.GetNumberOfPages(), destination);
			destination.Close();
			source.Close();
			CompareResult("structTreeCopyingTest06.pdf", "cmp_structTreeCopyingTest06.pdf", "diff_copying_06_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void StructTreeCopyingTest07()
		{
			PdfReader reader = new PdfReader(new FileStream(sourceFolder + "quick-brown-fox.pdf"
				, FileMode.Open, FileAccess.Read));
			PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "structTreeCopyingTest07.pdf"
				, FileMode.Create));
			PdfDocument document = new PdfDocument(writer);
			document.SetTagged();
			PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document
				, PdfName.Document));
			PdfPage page1 = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page1);
			canvas.BeginText();
			canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.COURIER), 24);
			canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
			PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
			PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, 
				page1));
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page1, span1))));
			canvas.ShowText("Hello ");
			canvas.CloseTag();
			canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrDictionary(page1, span1))));
			canvas.ShowText("World");
			canvas.CloseTag();
			canvas.EndText();
			canvas.Release();
			PdfDocument document1 = new PdfDocument(reader);
			document1.InitializeOutlines();
			document1.CopyPagesTo(1, 1, document);
			document.Close();
			document1.Close();
			CompareResult("structTreeCopyingTest07.pdf", "cmp_structTreeCopyingTest07.pdf", "diff_copying_07_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void StructTreeCopyingTest08()
		{
			PdfReader reader = new PdfReader(new FileStream(sourceFolder + "quick-brown-fox-table.pdf"
				, FileMode.Open, FileAccess.Read));
			PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "structTreeCopyingTest08.pdf"
				, FileMode.Create));
			PdfDocument document = new PdfDocument(reader, writer);
			PdfReader reader1 = new PdfReader(new FileStream(sourceFolder + "quick-brown-fox.pdf"
				, FileMode.Open, FileAccess.Read));
			PdfDocument document1 = new PdfDocument(reader1);
			document1.InitializeOutlines();
			document1.CopyPagesTo(1, 1, document, 2);
			document.Close();
			document1.Close();
			CompareResult("structTreeCopyingTest08.pdf", "cmp_structTreeCopyingTest08.pdf", "diff_copying_08_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void StructTreeCopyingTest09()
		{
			PdfReader reader = new PdfReader(new FileStream(sourceFolder + "quick-brown-fox-table.pdf"
				, FileMode.Open, FileAccess.Read));
			PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "structTreeCopyingTest09.pdf"
				, FileMode.Create));
			PdfDocument document = new PdfDocument(reader, writer);
			PdfReader reader1 = new PdfReader(new FileStream(sourceFolder + "quick-brown-fox.pdf"
				, FileMode.Open, FileAccess.Read));
			PdfDocument document1 = new PdfDocument(reader1);
			document1.InitializeOutlines();
			document1.CopyPagesTo(1, 1, document, 2);
			document1.CopyPagesTo(1, 1, document, 4);
			document.Close();
			document1.Close();
			CompareResult("structTreeCopyingTest09.pdf", "cmp_structTreeCopyingTest09.pdf", "diff_copying_09_"
				);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void StructTreeCopyingTest10()
		{
			PdfReader reader = new PdfReader(new FileStream(sourceFolder + "88th_Academy_Awards.pdf"
				, FileMode.Open, FileAccess.Read));
			PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "structTreeCopyingTest10.pdf"
				, FileMode.Create));
			PdfDocument document = new PdfDocument(reader, writer);
			PdfReader reader1 = new PdfReader(new FileStream(sourceFolder + "quick-brown-fox-table.pdf"
				, FileMode.Open, FileAccess.Read));
			PdfDocument document1 = new PdfDocument(reader1);
			document1.InitializeOutlines();
			document1.CopyPagesTo(1, 3, document, 2);
			PdfReader reader2 = new PdfReader(new FileStream(sourceFolder + "quick-brown-fox.pdf"
				, FileMode.Open, FileAccess.Read));
			PdfDocument document2 = new PdfDocument(reader2);
			document2.InitializeOutlines();
			document2.CopyPagesTo(1, 1, document, 4);
			document.Close();
			document1.Close();
			document2.Close();
			CompareResult("structTreeCopyingTest10.pdf", "cmp_structTreeCopyingTest10.pdf", "diff_copying_10_"
				);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		/// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
		/// <exception cref="Org.Xml.Sax.SAXException"/>
		private void CompareResult(String outFileName, String cmpFileName, String diffNamePrefix
			)
		{
			CompareTool compareTool = new CompareTool();
			String outPdf = destinationFolder + outFileName;
			String cmpPdf = sourceFolder + cmpFileName;
			String contentDifferences = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder
				, diffNamePrefix);
			String taggedStructureDifferences = compareTool.CompareTagStructures(outPdf, cmpPdf
				);
			String errorMessage = "";
			errorMessage += taggedStructureDifferences == null ? "" : taggedStructureDifferences
				 + "\n";
			errorMessage += contentDifferences == null ? "" : contentDifferences;
			if (errorMessage.Length > 0)
			{
				NUnit.Framework.Assert.Fail(errorMessage);
			}
		}
	}
}

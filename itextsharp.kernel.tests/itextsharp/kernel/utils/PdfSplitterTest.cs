using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using iTextSharp.IO;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Test;
using iTextSharp.Test.Attributes;

namespace iTextSharp.Kernel.Utils
{
	public class PdfSplitterTest : ExtendedITextTest
	{
		public static readonly String sourceFolder = TestContext.CurrentContext.TestDirectory
			 + "/../../resources/itextsharp/kernel/utils/PdfSplitterTest/";

		public static readonly String destinationFolder = TestContext.CurrentContext.TestDirectory
			 + "/test/itextsharp/kernel/utils/PdfSplitterTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateOrClearDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		[LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY, Count = 3
			)]
		public virtual void SplitDocumentTest01()
		{
			String inputFileName = sourceFolder + "iphone_user_guide.pdf";
			PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
			IList<int> pageNumbers = iTextSharp.IO.Util.JavaUtil.ArraysAsList(30, 100);
			IList<PdfDocument> splitDocuments = new _PdfSplitter_44(inputPdfDoc).SplitByPageNumbers
				(pageNumbers);
			foreach (PdfDocument doc in splitDocuments)
			{
				doc.Close();
			}
			for (int i = 1; i <= 3; i++)
			{
				NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
					 + "splitDocument1_" + i.ToString() + ".pdf", sourceFolder + "cmp/" + "splitDocument1_"
					 + i.ToString() + ".pdf", destinationFolder, "diff_"));
			}
		}

		private sealed class _PdfSplitter_44 : PdfSplitter
		{
			public _PdfSplitter_44(PdfDocument baseArg1)
				: base(baseArg1)
			{
				this.partNumber = 1;
			}

			internal int partNumber;

			protected internal override PdfWriter GetNextPdfWriter(PageRange documentPageRange
				)
			{
				try
				{
					return new PdfWriter(new FileStream(PdfSplitterTest.destinationFolder + "splitDocument1_"
						 + (this.partNumber++).ToString() + ".pdf", FileMode.Create));
				}
				catch (FileNotFoundException)
				{
					throw new Exception();
				}
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		[LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY, Count = 3
			)]
		public virtual void SplitDocumentTest02()
		{
			String inputFileName = sourceFolder + "iphone_user_guide.pdf";
			PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
			new _PdfSplitter_74(inputPdfDoc).SplitByPageCount(60, new _IDocumentReadyListener_85
				());
			for (int i = 1; i <= 3; i++)
			{
				NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
					 + "splitDocument2_" + i.ToString() + ".pdf", sourceFolder + "cmp/" + "splitDocument2_"
					 + i.ToString() + ".pdf", destinationFolder, "diff_"));
			}
		}

		private sealed class _PdfSplitter_74 : PdfSplitter
		{
			public _PdfSplitter_74(PdfDocument baseArg1)
				: base(baseArg1)
			{
				this.partNumber = 1;
			}

			internal int partNumber;

			protected internal override PdfWriter GetNextPdfWriter(PageRange documentPageRange
				)
			{
				try
				{
					return new PdfWriter(new FileStream(PdfSplitterTest.destinationFolder + "splitDocument2_"
						 + (this.partNumber++).ToString() + ".pdf", FileMode.Create));
				}
				catch (FileNotFoundException)
				{
					throw new Exception();
				}
			}
		}

		private sealed class _IDocumentReadyListener_85 : PdfSplitter.IDocumentReadyListener
		{
			public _IDocumentReadyListener_85()
			{
			}

			public void DocumentReady(PdfDocument pdfDocument, PageRange pageRange)
			{
				if (new PageRange("61-120").Equals(pageRange))
				{
					pdfDocument.GetDocumentInfo().SetAuthor("Modified Author");
				}
				pdfDocument.Close();
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		[LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY, Count = 2
			)]
		public virtual void SplitDocumentTest03()
		{
			String inputFileName = sourceFolder + "iphone_user_guide.pdf";
			PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
			PageRange pageRange1 = new PageRange().AddPageSequence(4, 15).AddSinglePage(18).AddPageSequence
				(1, 2);
			PageRange pageRange2 = new PageRange().AddSinglePage(99).AddSinglePage(98).AddPageSequence
				(70, 99);
			IList<PdfDocument> splitDocuments = new _PdfSplitter_113(inputPdfDoc).ExtractPageRanges
				(iTextSharp.IO.Util.JavaUtil.ArraysAsList(pageRange1, pageRange2));
			foreach (PdfDocument pdfDocument in splitDocuments)
			{
				pdfDocument.Close();
			}
			for (int i = 1; i <= 2; i++)
			{
				NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
					 + "splitDocument3_" + i + ".pdf", sourceFolder + "cmp/" + "splitDocument3_" + i
					.ToString() + ".pdf", destinationFolder, "diff_"));
			}
		}

		private sealed class _PdfSplitter_113 : PdfSplitter
		{
			public _PdfSplitter_113(PdfDocument baseArg1)
				: base(baseArg1)
			{
				this.partNumber = 1;
			}

			internal int partNumber;

			protected internal override PdfWriter GetNextPdfWriter(PageRange documentPageRange
				)
			{
				try
				{
					return new PdfWriter(new FileStream(PdfSplitterTest.destinationFolder + "splitDocument3_"
						 + (this.partNumber++).ToString() + ".pdf", FileMode.Create));
				}
				catch (FileNotFoundException)
				{
					throw new Exception();
				}
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		[LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY, Count = 2
			)]
		public virtual void SplitDocumentByOutlineTest()
		{
			String inputFileName = sourceFolder + "iphone_user_guide.pdf";
			PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
			PdfSplitter splitter = new PdfSplitter(inputPdfDoc);
			IList<String> listTitles = new List<String>();
			listTitles.Add("Syncing iPod Content from Your iTunes Library");
			listTitles.Add("Restoring or Transferring Your iPhone Settings");
			IList<PdfDocument> list = splitter.SplitByOutlines(listTitles);
			NUnit.Framework.Assert.AreEqual(1, list[0].GetNumberOfPages());
			NUnit.Framework.Assert.AreEqual(2, list[1].GetNumberOfPages());
			list[0].Close();
			list[1].Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void SplitDocumentBySize()
		{
			String inputFileName = sourceFolder + "splitBySize.pdf";
			PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
			PdfSplitter splitter = new _PdfSplitter_159(inputPdfDoc);
			IList<PdfDocument> documents = splitter.SplitBySize(100000);
			foreach (PdfDocument doc in documents)
			{
				doc.Close();
			}
			for (int i = 1; i <= 4; ++i)
			{
				NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
					 + "splitBySize_part" + i + ".pdf", sourceFolder + "cmp/" + "splitBySize_part" +
					 i + ".pdf", destinationFolder, "diff_"));
			}
		}

		private sealed class _PdfSplitter_159 : PdfSplitter
		{
			public _PdfSplitter_159(PdfDocument baseArg1)
				: base(baseArg1)
			{
				this.partNumber = 1;
			}

			internal int partNumber;

			protected internal override PdfWriter GetNextPdfWriter(PageRange documentPageRange
				)
			{
				try
				{
					return new PdfWriter(new FileStream(PdfSplitterTest.destinationFolder + "splitBySize_part"
						 + (this.partNumber++).ToString() + ".pdf", FileMode.Create));
				}
				catch (FileNotFoundException)
				{
					throw new Exception();
				}
			}
		}
	}
}

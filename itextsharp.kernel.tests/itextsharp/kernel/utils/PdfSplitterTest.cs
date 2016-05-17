using System;
using System.Collections;
using System.Collections.Generic;
using Java.IO;
using NUnit.Framework;
using iTextSharp.Kernel;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Utils
{
	public class PdfSplitterTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/kernel/utils/PdfSplitterTest/";

		public const String destinationFolder = "test/itextsharp/kernel/utils/PdfSplitterTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateOrClearDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SplitDocumentTest01()
		{
			String inputFileName = sourceFolder + "iphone_user_guide.pdf";
			PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
			IList<int> pageNumbers = iTextSharp.IO.Util.JavaUtil.ArraysAsList(30, 100);
			IList<PdfDocument> splitDocuments = new _PdfSplitter_45(inputPdfDoc).SplitByPageNumbers
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

		private sealed class _PdfSplitter_45 : PdfSplitter
		{
			public _PdfSplitter_45(PdfDocument baseArg1)
				: base(baseArg1)
			{
				this.partNumber = 1;
			}

			internal int partNumber;

			protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
			{
				try
				{
					return new PdfWriter(new FileOutputStream(PdfSplitterTest.destinationFolder + "splitDocument1_"
						 + (this.partNumber++).ToString() + ".pdf"));
				}
				catch (FileNotFoundException)
				{
					throw new Exception();
				}
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SplitDocumentTest02()
		{
			String inputFileName = sourceFolder + "iphone_user_guide.pdf";
			PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
			new _PdfSplitter_75(inputPdfDoc).SplitByPageCount(60, new _IDocumentReadyListener_86
				());
			for (int i = 1; i <= 3; i++)
			{
				NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
					 + "splitDocument2_" + i.ToString() + ".pdf", sourceFolder + "cmp/" + "splitDocument2_"
					 + i.ToString() + ".pdf", destinationFolder, "diff_"));
			}
		}

		private sealed class _PdfSplitter_75 : PdfSplitter
		{
			public _PdfSplitter_75(PdfDocument baseArg1)
				: base(baseArg1)
			{
				this.partNumber = 1;
			}

			internal int partNumber;

			protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
			{
				try
				{
					return new PdfWriter(new FileOutputStream(PdfSplitterTest.destinationFolder + "splitDocument2_"
						 + (this.partNumber++).ToString() + ".pdf"));
				}
				catch (FileNotFoundException)
				{
					throw new Exception();
				}
			}
		}

		private sealed class _IDocumentReadyListener_86 : PdfSplitter.IDocumentReadyListener
		{
			public _IDocumentReadyListener_86()
			{
			}

			public void DocumentReady(PdfDocument pdfDocument, PageRange pageRange)
			{
				try
				{
					if (new PageRange("61-120").Equals(pageRange))
					{
						pdfDocument.GetDocumentInfo().SetAuthor("Modified Author");
					}
					pdfDocument.Close();
				}
				catch (PdfException e)
				{
					iTextSharp.PrintStackTrace(e);
				}
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SplitDocumentTest03()
		{
			String inputFileName = sourceFolder + "iphone_user_guide.pdf";
			PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
			PageRange pageRange1 = new PageRange().AddPageSequence(4, 15).AddSinglePage(18).AddPageSequence
				(1, 2);
			PageRange pageRange2 = new PageRange().AddSinglePage(99).AddSinglePage(98).AddPageSequence
				(70, 99);
			IList<PdfDocument> splitDocuments = new _PdfSplitter_118(inputPdfDoc).ExtractPageRanges
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

		private sealed class _PdfSplitter_118 : PdfSplitter
		{
			public _PdfSplitter_118(PdfDocument baseArg1)
				: base(baseArg1)
			{
				this.partNumber = 1;
			}

			internal int partNumber;

			protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
			{
				try
				{
					return new PdfWriter(new FileOutputStream(PdfSplitterTest.destinationFolder + "splitDocument3_"
						 + (this.partNumber++).ToString() + ".pdf"));
				}
				catch (FileNotFoundException)
				{
					throw new Exception();
				}
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void SplitDocumentByOutlineTest()
		{
			String inputFileName = sourceFolder + "iphone_user_guide.pdf";
			PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
			PdfSplitter splitter = new PdfSplitter(inputPdfDoc);
			IList listTitles = new ArrayList();
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
		[NUnit.Framework.Test]
		public virtual void SplitDocumentBySize()
		{
			String inputFileName = sourceFolder + "splitBySize.pdf";
			PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
			PdfSplitter splitter = new _PdfSplitter_164(inputPdfDoc);
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

		private sealed class _PdfSplitter_164 : PdfSplitter
		{
			public _PdfSplitter_164(PdfDocument baseArg1)
				: base(baseArg1)
			{
				this.partNumber = 1;
			}

			internal int partNumber;

			protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
			{
				try
				{
					return new PdfWriter(new FileOutputStream(PdfSplitterTest.destinationFolder + "splitBySize_part"
						 + (this.partNumber++).ToString() + ".pdf"));
				}
				catch (FileNotFoundException)
				{
					throw new Exception();
				}
			}
		}
	}
}

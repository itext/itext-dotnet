using System;
using System.Text;
using Java.IO;
using NUnit.Framework;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfPrimitivesTest
	{
		internal const String destinationFolder = "test/itextsharp/kernel/pdf/PdfPrimitivesTest/";

		internal static readonly PdfName TestArray = new PdfName("TestArray");

		internal const int DefaultArraySize = 64;

		internal const int PageCount = 1000;

		public class RandomString
		{
			private static readonly char[] symbols;

			private readonly Random random = new Random();

			private readonly char[] buf;

			static RandomString()
			{
				StringBuilder tmp = new StringBuilder();
				for (char ch = 'A'; ch <= 'Z'; ++ch)
				{
					tmp.Append(ch);
				}
				for (char ch_1 = 'a'; ch_1 <= 'z'; ++ch_1)
				{
					tmp.Append(ch_1);
				}
				for (char ch_2 = '0'; ch_2 <= '9'; ++ch_2)
				{
					tmp.Append(ch_2);
				}
				symbols = tmp.ToString().ToCharArray();
			}

			public RandomString(int length)
			{
				if (length < 1)
				{
					throw new ArgumentException("length < 1: " + length);
				}
				buf = new char[length];
			}

			public virtual String NextString()
			{
				for (int idx = 0; idx < buf.Length; ++idx)
				{
					buf[idx] = symbols[random.Next(symbols.Length)];
				}
				return new String(buf);
			}
		}

		[SetUp]
		public virtual void Setup()
		{
			new File(destinationFolder).Mkdirs();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PrimitivesFloatNumberTest()
		{
			String filename = "primitivesFloatNumberTest.pdf";
			FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = 0; i < PageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				PdfArray array = GeneratePdfArrayWithFloatNumbers(null, false);
				page.GetPdfObject().Put(TestArray, array);
				array.Flush();
				page.Flush();
			}
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PrimitivesIntNumberTest()
		{
			String filename = "primitivesIntNumberTest.pdf";
			FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = 0; i < PageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				PdfArray array = GeneratePdfArrayWithIntNumbers(null, false);
				page.GetPdfObject().Put(TestArray, array);
				array.Flush();
				page.Flush();
			}
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PrimitivesNameTest()
		{
			String filename = "primitivesNameTest.pdf";
			FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = 0; i < PageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				PdfArray array = GeneratePdfArrayWithNames(null, false);
				page.GetPdfObject().Put(TestArray, array);
				array.Flush();
				page.Flush();
			}
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PrimitivesStringTest()
		{
			String filename = "primitivesStringTest.pdf";
			FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = 0; i < PageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				PdfArray array = GeneratePdfArrayWithStrings(null, false);
				page.GetPdfObject().Put(TestArray, array);
				array.Flush();
				page.Flush();
			}
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PrimitivesBooleanTest()
		{
			String filename = "primitivesBooleanTest.pdf";
			FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = 0; i < PageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithBooleans(null, false));
				page.Flush();
			}
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PrimitivesFloatNumberIndirectTest()
		{
			String filename = "primitivesFloatNumberIndirectTest.pdf";
			FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = 0; i < PageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithFloatNumbers(pdfDoc, true)
					);
				page.Flush();
			}
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PrimitivesIntNumberIndirectTest()
		{
			String filename = "primitivesIntNumberIndirectTest.pdf";
			FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = 0; i < PageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithIntNumbers(pdfDoc, true));
				page.Flush();
			}
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PrimitivesStringIndirectTest()
		{
			String filename = "primitivesStringIndirectTest.pdf";
			FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = 0; i < PageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithStrings(pdfDoc, true));
				page.Flush();
			}
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PrimitivesNameIndirectTest()
		{
			String filename = "primitivesNameIndirectTest.pdf";
			FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = 0; i < PageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithNames(pdfDoc, true));
				page.Flush();
			}
			pdfDoc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void PrimitivesBooleanIndirectTest()
		{
			String filename = "primitivesBooleanIndirectTest.pdf";
			FileOutputStream fos = new FileOutputStream(destinationFolder + filename);
			PdfWriter writer = new PdfWriter(fos);
			PdfDocument pdfDoc = new PdfDocument(writer);
			for (int i = 0; i < PageCount; i++)
			{
				PdfPage page = pdfDoc.AddNewPage();
				page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithBooleans(pdfDoc, true));
				page.Flush();
			}
			pdfDoc.Close();
		}

		[Test]
		public virtual void PdfNamesTest()
		{
			PdfPrimitivesTest.RandomString rnd = new PdfPrimitivesTest.RandomString(16);
			for (int i = 0; i < 10000000; i++)
			{
				new PdfName(rnd.NextString());
			}
		}

		private PdfArray GeneratePdfArrayWithFloatNumbers(PdfDocument doc, bool indirects
			)
		{
			PdfArray array = ((PdfArray)new PdfArray().MakeIndirect(doc));
			Random rnd = new Random();
			for (int i = 0; i < DefaultArraySize; i++)
			{
				PdfNumber num = new PdfNumber(rnd.NextFloat());
				if (indirects)
				{
					num.MakeIndirect(doc);
				}
				array.Add(num);
			}
			return array;
		}

		private PdfArray GeneratePdfArrayWithIntNumbers(PdfDocument doc, bool indirects)
		{
			PdfArray array = ((PdfArray)new PdfArray().MakeIndirect(doc));
			Random rnd = new Random();
			for (int i = 0; i < DefaultArraySize; i++)
			{
				array.Add(((PdfNumber)new PdfNumber(rnd.NextInt()).MakeIndirect(indirects ? doc : 
					null)));
			}
			return array;
		}

		private PdfArray GeneratePdfArrayWithStrings(PdfDocument doc, bool indirects)
		{
			PdfArray array = ((PdfArray)new PdfArray().MakeIndirect(doc));
			PdfPrimitivesTest.RandomString rnd = new PdfPrimitivesTest.RandomString(16);
			for (int i = 0; i < DefaultArraySize; i++)
			{
				array.Add(((PdfString)new PdfString(rnd.NextString()).MakeIndirect(indirects ? doc
					 : null)));
			}
			return array;
		}

		private PdfArray GeneratePdfArrayWithNames(PdfDocument doc, bool indirects)
		{
			PdfArray array = ((PdfArray)new PdfArray().MakeIndirect(doc));
			PdfPrimitivesTest.RandomString rnd = new PdfPrimitivesTest.RandomString(6);
			for (int i = 0; i < DefaultArraySize; i++)
			{
				array.Add(((PdfName)new PdfName(rnd.NextString()).MakeIndirect(indirects ? doc : 
					null)));
			}
			return array;
		}

		private PdfArray GeneratePdfArrayWithBooleans(PdfDocument doc, bool indirects)
		{
			PdfArray array = ((PdfArray)new PdfArray().MakeIndirect(doc));
			Random rnd = new Random();
			for (int i = 0; i < DefaultArraySize; i++)
			{
				array.Add(((PdfBoolean)new PdfBoolean(rnd.NextBoolean()).MakeIndirect(indirects ? 
					doc : null)));
			}
			return array;
		}
	}
}

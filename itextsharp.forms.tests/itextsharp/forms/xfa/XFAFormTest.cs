using System;
using System.IO;
using NUnit.Framework;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Forms.Xfa
{
	public class XFAFormTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/forms/xfa/XFAFormTest/";

		public const String destinationFolder = "test/itextsharp/forms/xfa/XFAFormTest/";

		public const String XML = sourceFolder + "xfa.xml";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void CreateEmptyXFAFormTest01()
		{
			String outFileName = destinationFolder + "createEmptyXFAFormTest01.pdf";
			String cmpFileName = sourceFolder + "cmp_createEmptyXFAFormTest01.pdf";
			PdfDocument doc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode
				.Create)));
			XfaForm xfa = new XfaForm(doc);
			XfaForm.SetXfaForm(xfa, doc);
			doc.AddNewPage();
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void CreateEmptyXFAFormTest02()
		{
			String outFileName = destinationFolder + "createEmptyXFAFormTest02.pdf";
			String cmpFileName = sourceFolder + "cmp_createEmptyXFAFormTest02.pdf";
			PdfDocument doc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode
				.Create)));
			XfaForm xfa = new XfaForm();
			XfaForm.SetXfaForm(xfa, doc);
			doc.AddNewPage();
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void CreateXFAFormTest()
		{
			String outFileName = destinationFolder + "createXFAFormTest.pdf";
			String cmpFileName = sourceFolder + "cmp_createXFAFormTest.pdf";
			PdfDocument doc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode
				.Create)));
			XfaForm xfa = new XfaForm(new FileStream(XML, FileMode.Open));
			xfa.Write(doc);
			doc.AddNewPage();
			doc.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName
				, destinationFolder, "diff"));
		}
	}
}

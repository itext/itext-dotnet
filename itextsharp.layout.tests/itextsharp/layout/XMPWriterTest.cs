using System;
using System.IO;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Utils;
using iTextSharp.Kernel.Xmp;
using iTextSharp.Kernel.Xmp.Options;
using iTextSharp.Layout.Element;
using iTextSharp.Test;

namespace iTextSharp.Layout
{
	public class XMPWriterTest : ExtendedITextTest
	{
		public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
			.TestDirectory + "/../../resources/itextsharp/layout/XMPWriterTest/";

		public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
			.TestDirectory + "/test/itextsharp/layout/XMPWriterTest/";

		[NUnit.Framework.TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[NUnit.Framework.Test]
		public virtual void CreatePdfTest()
		{
			String fileName = "xmp_metadata.pdf";
			// step 1
			PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(destinationFolder
				 + "xmp_metadata.pdf", FileMode.Create)));
			Document document = new Document(pdfDocument);
			// step 2
			ByteArrayOutputStream os = new ByteArrayOutputStream();
			XmpMeta xmp = XmpMetaFactory.Create();
			xmp.AppendArrayItem(XmpConst.NS_DC, "subject", new PropertyOptions(PropertyOptions
				.ARRAY), "Hello World", null);
			xmp.AppendArrayItem(XmpConst.NS_DC, "subject", new PropertyOptions(PropertyOptions
				.ARRAY), "XMP & Metadata", null);
			xmp.AppendArrayItem(XmpConst.NS_DC, "subject", new PropertyOptions(PropertyOptions
				.ARRAY), "Metadata", null);
			pdfDocument.SetXmpMetadata(xmp);
			// step 4
			document.Add(new Paragraph("Hello World"));
			// step 5
			document.Close();
			CompareTool ct = new CompareTool();
			NUnit.Framework.Assert.IsNull(ct.CompareXmp(destinationFolder + fileName, sourceFolder
				 + "cmp_" + fileName, true));
		}
	}
}

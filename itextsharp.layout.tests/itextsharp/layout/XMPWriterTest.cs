using System;
using Java.IO;
using NUnit.Framework;
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
		public const String sourceFolder = "../../resources/itextsharp/layout/XMPWriterTest/";

		public const String destinationFolder = "test/itextsharp/layout/XMPWriterTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[NUnit.Framework.Test]
		public virtual void CreatePdfTest()
		{
			String fileName = "xmp_metadata.pdf";
			// step 1
			PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileOutputStream(destinationFolder
				 + "xmp_metadata.pdf", FileMode.Create)));
			Document document = new Document(pdfDocument);
			// step 2
			ByteArrayOutputStream os = new ByteArrayOutputStream();
			XMPMeta xmp = XMPMetaFactory.Create();
			xmp.AppendArrayItem(XMPConst.NS_DC, "subject", new PropertyOptions(PropertyOptions
				.ARRAY), "Hello World", null);
			xmp.AppendArrayItem(XMPConst.NS_DC, "subject", new PropertyOptions(PropertyOptions
				.ARRAY), "XMP & Metadata", null);
			xmp.AppendArrayItem(XMPConst.NS_DC, "subject", new PropertyOptions(PropertyOptions
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

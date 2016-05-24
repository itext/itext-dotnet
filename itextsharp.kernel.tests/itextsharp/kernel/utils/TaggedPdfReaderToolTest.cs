using System;
using System.IO;
using NUnit.Framework;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Utils
{
	public class TaggedPdfReaderToolTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/kernel/utils/TaggedPdfReaderToolTest/";

		public const String destinationFolder = "test/itextsharp/kernel/utils/TaggedPdfReaderToolTest/";

		[SetUp]
		public virtual void SetUp()
		{
			CreateOrClearDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.Exception"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
		/// <exception cref="Org.Xml.Sax.SAXException"/>
		[Test]
		public virtual void TaggedPdfReaderToolTest01()
		{
			String filename = "iphone_user_guide.pdf";
			String outXmlPath = destinationFolder + "outXml01.xml";
			String cmpXmlPath = sourceFolder + "cmpXml01.xml";
			PdfReader reader = new PdfReader(sourceFolder + filename);
			PdfDocument document = new PdfDocument(reader);
			FileStream outXml = new FileStream(outXmlPath, FileMode.Create);
			TaggedPdfReaderTool tool = new TaggedPdfReaderTool(document);
			tool.SetRootTag("root");
			tool.ConvertToXml(outXml);
			outXml.Close();
			document.Close();
			CompareTool compareTool = new CompareTool();
			if (!compareTool.CompareXmls(outXmlPath, cmpXmlPath))
			{
				NUnit.Framework.Assert.Fail("Resultant xml is different.");
			}
		}
	}
}

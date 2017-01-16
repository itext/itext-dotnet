using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils {
    public class TaggedPdfReaderToolTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/utils/TaggedPdfReaderToolTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/utils/TaggedPdfReaderToolTest/";

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void TaggedPdfReaderToolTest01() {
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
            if (!compareTool.CompareXmls(outXmlPath, cmpXmlPath)) {
                NUnit.Framework.Assert.Fail("Resultant xml is different.");
            }
        }
    }
}

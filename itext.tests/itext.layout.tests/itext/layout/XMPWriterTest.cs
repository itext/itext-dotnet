using System;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    public class XMPWriterTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/XMPWriterTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/XMPWriterTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void CreatePdfTest() {
            String fileName = "xmp_metadata.pdf";
            // step 1
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "xmp_metadata.pdf"));
            Document document = new Document(pdfDocument);
            // step 2
            ByteArrayOutputStream os = new ByteArrayOutputStream();
            XMPMeta xmp = XMPMetaFactory.Create();
            xmp.AppendArrayItem(XMPConst.NS_DC, "subject", new PropertyOptions(PropertyOptions.ARRAY), "Hello World", 
                null);
            xmp.AppendArrayItem(XMPConst.NS_DC, "subject", new PropertyOptions(PropertyOptions.ARRAY), "XMP & Metadata"
                , null);
            xmp.AppendArrayItem(XMPConst.NS_DC, "subject", new PropertyOptions(PropertyOptions.ARRAY), "Metadata", null
                );
            pdfDocument.SetXmpMetadata(xmp);
            // step 4
            document.Add(new Paragraph("Hello World"));
            // step 5
            document.Close();
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(destinationFolder + fileName, sourceFolder + "cmp_" + fileName
                , true));
        }
    }
}

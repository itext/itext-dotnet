using System;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Utils;
using iTextSharp.Layout.Element;
using iTextSharp.Test;

namespace iTextSharp.Layout {
    public class DestinationTest : ExtendedITextTest {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itextsharp/layout/DestinationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itextsharp/layout/DestinationTest/";

        [NUnit.Framework.TestFixtureSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DestinationTest01() {
            String outFileName = destinationFolder + "destinationTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_destinationTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Text text = new Text(String.Format("Page {0}", 10));
            text.SetProperty(iTextSharp.Layout.Property.Property.DESTINATION, "p10");
            doc.Add(new Paragraph(text).SetFixedPosition(1, 549, 742, 40));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}

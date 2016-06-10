using System;
using System.IO;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Action;
using iTextSharp.Kernel.Pdf.Navigation;
using iTextSharp.Kernel.Utils;
using iTextSharp.Layout.Element;
using iTextSharp.Test;

namespace iTextSharp.Layout
{
    public class LinkTest : ExtendedITextTest
    {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itextsharp/layout/LinkTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itextsharp/layout/LinkTest/";

        [NUnit.Framework.TestFixtureSetUp]
        public static void BeforeClass()
        {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LinkTest01()
        {
            String outFileName = destinationFolder + "linkTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_linkTest01.pdf";
            FileStream file = new FileStream(outFileName, FileMode.Create);
            PdfWriter writer = new PdfWriter(file);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com/", false);
            Link link = new Link("TestLink", action);
            doc.Add(new Paragraph(link));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LinkTest02()
        {
            String outFileName = destinationFolder + "linkTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_linkTest02.pdf";
            FileStream file = new FileStream(outFileName, FileMode.Create);
            PdfWriter writer = new PdfWriter(file);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            doc.Add(new AreaBreak()).Add(new AreaBreak());
            PdfArray array = new PdfArray();
            array.Add(doc.GetPdfDocument().GetPage(1).GetPdfObject());
            array.Add(PdfName.XYZ);
            array.Add(new PdfNumber(36));
            array.Add(new PdfNumber(100));
            array.Add(new PdfNumber(1));
            PdfDestination dest = PdfDestination.MakeDestination(array);
            PdfAction action = PdfAction.CreateGoTo(dest);
            Link link = new Link("TestLink", action);
            doc.Add(new Paragraph(link));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}

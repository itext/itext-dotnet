using System;
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class LinkTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/LinkTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/LinkTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LinkTest01() {
            String outFileName = destinationFolder + "linkTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_linkTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
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
        public virtual void LinkTest02() {
            String outFileName = destinationFolder + "linkTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_linkTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ACTION_WAS_SET_TO_LINK_ANNOTATION_WITH_DESTINATION)]
        public virtual void LinkTest03() {
            String outFileName = destinationFolder + "linkTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_linkTest03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            PdfArray array = new PdfArray();
            array.Add(doc.GetPdfDocument().AddNewPage().GetPdfObject());
            array.Add(PdfName.XYZ);
            array.Add(new PdfNumber(36));
            array.Add(new PdfNumber(100));
            array.Add(new PdfNumber(1));
            PdfDestination dest = PdfDestination.MakeDestination(array);
            Link link = new Link("TestLink", dest);
            link.SetAction(PdfAction.CreateURI("http://itextpdf.com/", false));
            doc.Add(new Paragraph(link));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BorderedLinkTest() {
            String outFileName = destinationFolder + "borderedLinkTest.pdf";
            String cmpFileName = sourceFolder + "cmp_borderedLinkTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            Link link = new Link("Link with orange border", PdfAction.CreateURI("http://itextpdf.com"));
            link.SetBorder(new SolidBorder(Color.ORANGE, 5));
            doc.Add(new Paragraph(link));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <summary>
        /// <a href="http://stackoverflow.com/questions/34408764/create-local-link-in-rotated-pdfpcell-in-itextsharp">
        /// Stack overflow: Create local link in rotated PdfPCell in iTextSharp
        /// </a>
        /// <p>
        /// This is the equivalent Java code for iText 7 of the C# code for iTextSharp 5
        /// in the question.
        /// </summary>
        /// <remarks>
        /// <a href="http://stackoverflow.com/questions/34408764/create-local-link-in-rotated-pdfpcell-in-itextsharp">
        /// Stack overflow: Create local link in rotated PdfPCell in iTextSharp
        /// </a>
        /// <p>
        /// This is the equivalent Java code for iText 7 of the C# code for iTextSharp 5
        /// in the question.
        /// </p>
        /// Author: mkl.
        /// </remarks>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestCreateLocalLinkInRotatedCell() {
            String outFileName = destinationFolder + "linkInRotatedCell.pdf";
            String cmpFileName = sourceFolder + "cmp_linkInRotatedCell.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Table table = new Table(2);
            Link chunk = new Link("Click here", PdfAction.CreateURI("http://itextpdf.com/"));
            table.AddCell(new Cell().Add(new Paragraph().Add(chunk)).SetRotationAngle(Math.PI / 2));
            chunk = new Link("Click here 2", PdfAction.CreateURI("http://itextpdf.com/"));
            table.AddCell(new Paragraph().Add(chunk));
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RotatedLinkAtFixedPosition() {
            String outFileName = destinationFolder + "rotatedLinkAtFixedPosition.pdf";
            String cmpFileName = sourceFolder + "cmp_rotatedLinkAtFixedPosition.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com/", false);
            Link link = new Link("TestLink", action);
            doc.Add(new Paragraph(link).SetMargin(0).SetRotationAngle(Math.PI / 4).SetFixedPosition(300, 623, 100));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        [NUnit.Framework.Test]
        public virtual void RotatedLinkInnerRotation() {
            String outFileName = destinationFolder + "rotatedLinkInnerRotation.pdf";
            String cmpFileName = sourceFolder + "cmp_rotatedLinkInnerRotation.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com/", false);
            Link link = new Link("TestLink", action);
            Paragraph p = new Paragraph(link).SetRotationAngle(Math.PI / 4).SetBackgroundColor(Color.RED);
            Div div = new Div().Add(p).SetRotationAngle(Math.PI / 3).SetBackgroundColor(Color.BLUE);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SimpleMarginsTest01() {
            String outFileName = destinationFolder + "simpleMarginsTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_simpleMarginsTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com/", false);
            Link link = new Link("TestLink", action);
            link.SetBorder(new SolidBorder(Color.BLUE, 20));
            link.SetProperty(Property.MARGIN_LEFT, 50);
            link.SetProperty(Property.MARGIN_RIGHT, 50);
            doc.Add(new Paragraph(link).SetBorder(new SolidBorder(10)));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}

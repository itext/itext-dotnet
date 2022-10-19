/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LinkTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/LinkTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/LinkTest/";

        private const String LONG_TEXT = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam nec condimentum odio. Duis sed ipsum semper, imperdiet risus sit amet, pellentesque leo. Proin eget libero quis orci sagittis efficitur et a justo. Phasellus ac ipsum id lacus fermentum malesuada. Morbi vulputate ultricies ligula a pretium. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Etiam eget leo maximus velit placerat condimentum. Nulla in fermentum ex, in fermentum risus. Phasellus gravida ante sit amet magna porta fermentum. Nunc nec urna quis enim facilisis scelerisque. Praesent risus est, efficitur eget quam nec, dignissim mollis nunc. Mauris in sodales nulla.\n"
             + "Sed sodales pharetra sapien, eget tristique magna fringilla at. Quisque ligula eros, auctor sit amet varius a, tincidunt non mauris. Sed diam mi, dignissim id magna accumsan, viverra scelerisque risus. Etiam blandit condimentum quam non bibendum. Sed vehicula justo quis lectus consequat, sit amet tempor sem mollis. Sed turpis nibh, luctus in arcu mattis, consequat laoreet est. Integer tempor, ante a gravida efficitur, velit libero dapibus nibh, et scelerisque diam nulla a orci. Vestibulum eleifend rutrum elit, sed pellentesque arcu lacinia nec. Nam semper, velit eget rhoncus efficitur, odio libero molestie mi, ut eleifend libero purus ut ex. Quisque hendrerit vehicula hendrerit. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam quis elit eu dolor pellentesque viverra non eget purus. Nam nisi erat, efficitur sed malesuada ut, ornare sit amet risus. Nunc eu vestibulum turpis.\n"
             + "Duis ultricies et dui nec pharetra. Cras sagittis felis risus, vel vulputate diam blandit non. Vestibulum sed neque quis massa rutrum luctus. Nulla vitae leo ornare, elementum dolor sit amet, fringilla enim. Vestibulum efficitur, diam in molestie tincidunt, tellus purus ultricies nisl, ut bibendum purus augue et mi. Mauris eget leo aliquam metus egestas dapibus eget sit amet risus. Cras eget felis porttitor, ornare est congue, venenatis ipsum. Suspendisse accumsan eget elit efficitur malesuada. Quisque porttitor efficitur lorem in placerat. Nunc sit amet mattis ante. Vestibulum eget quam et ex tempus iaculis. Duis pharetra posuere erat, vitae imperdiet ipsum lacinia in. Aenean nunc quam, consectetur vel nibh sit amet, sollicitudin porta purus.\n"
             + "Curabitur non nunc in libero pretium dictum rutrum at lorem. Suspendisse nec magna id libero bibendum porta. Nullam urna tellus, ornare nec massa quis, fringilla fermentum leo. Vestibulum ac velit pulvinar ex feugiat varius vel eu nunc. Mauris vitae purus porttitor, sagittis elit eu, volutpat quam. Nunc mattis pretium arcu, vitae pellentesque mauris tincidunt vitae. Proin congue sem eget commodo pulvinar. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer eu augue tortor. Vestibulum porta enim eget neque semper scelerisque. Nulla et enim ac nulla luctus viverra sed nec risus. Aliquam blandit, lorem non consectetur auctor, ex ipsum blandit ipsum, ut faucibus orci sem non odio. Nulla ut condimentum ante. Proin dignissim risus vitae arcu tristique, ac ultricies lacus lobortis. Aliquam sodales orci justo, vitae imperdiet elit volutpat id. Nullam vitae interdum erat.\n"
             + "Donec fringilla sapien sed neque finibus, non luctus justo lobortis. Praesent commodo pellentesque ligula, vel fringilla odio commodo id. Nam ultrices justo a dignissim congue. Nullam imperdiet sem eget placerat aliquam. Suspendisse non faucibus libero. Aenean purus arcu, auctor vitae tincidunt in, tincidunt at ante. Pellentesque euismod, velit vel vulputate faucibus, dolor erat consectetur sapien, ut elementum dui turpis nec lacus. In hac habitasse platea dictumst. Aenean vel elit ultrices, varius mi quis, congue erat."
             + "Curabitur sit amet nunc porttitor, congue elit vestibulum, vestibulum sapien. Fusce ut arcu consequat, scelerisque sapien vitae, dignissim ligula. Duis gravida mollis volutpat. Maecenas condimentum pulvinar urna in cursus. Nulla ornare est non tellus elementum auctor. Mauris ornare, elit non ornare lobortis, risus augue consectetur orci, ac efficitur ex nunc nec leo. Aenean dictum mattis magna vitae bibendum.";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

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

        [NUnit.Framework.Test]
        public virtual void LinkTest02() {
            String outFileName = destinationFolder + "linkTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_linkTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new AreaBreak()).Add(new AreaBreak());
            PdfDestination dest = PdfExplicitDestination.CreateXYZ(pdfDoc.GetPage(1), 36, 100, 1);
            PdfAction action = PdfAction.CreateGoTo(dest);
            Link link = new Link("TestLink", action);
            doc.Add(new Paragraph(link));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ACTION_WAS_SET_TO_LINK_ANNOTATION_WITH_DESTINATION)]
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

        [NUnit.Framework.Test]
        public virtual void BorderedLinkTest() {
            String outFileName = destinationFolder + "borderedLinkTest.pdf";
            String cmpFileName = sourceFolder + "cmp_borderedLinkTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            Link link = new Link("Link with orange border", PdfAction.CreateURI("http://itextpdf.com"));
            link.SetBorder(new SolidBorder(ColorConstants.ORANGE, 5));
            doc.Add(new Paragraph(link));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <summary>
        /// <a href="http://stackoverflow.com/questions/34408764/create-local-link-in-rotated-pdfpcell-in-itextsharp">
        /// Stack overflow: Create local link in rotated PdfPCell in iTextSharp
        /// </a>
        /// </summary>
        /// <remarks>
        /// <a href="http://stackoverflow.com/questions/34408764/create-local-link-in-rotated-pdfpcell-in-itextsharp">
        /// Stack overflow: Create local link in rotated PdfPCell in iTextSharp
        /// </a>
        /// <para />
        /// This is the equivalent Java code for iText 7 of the C# code for iTextSharp 5
        /// in the question.
        /// <para />
        /// </remarks>
        /// <author>mkl</author>
        [NUnit.Framework.Test]
        public virtual void TestCreateLocalLinkInRotatedCell() {
            String outFileName = destinationFolder + "linkInRotatedCell.pdf";
            String cmpFileName = sourceFolder + "cmp_linkInRotatedCell.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2 }));
            Link chunk = new Link("Click here", PdfAction.CreateURI("http://itextpdf.com/"));
            table.AddCell(new Cell().Add(new Paragraph().Add(chunk)).SetRotationAngle(Math.PI / 2));
            chunk = new Link("Click here 2", PdfAction.CreateURI("http://itextpdf.com/"));
            table.AddCell(new Paragraph().Add(chunk));
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

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

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void RotatedLinkInnerRotation() {
            String outFileName = destinationFolder + "rotatedLinkInnerRotation.pdf";
            String cmpFileName = sourceFolder + "cmp_rotatedLinkInnerRotation.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com/", false);
            Link link = new Link("TestLink", action);
            Paragraph p = new Paragraph(link).SetRotationAngle(Math.PI / 4).SetBackgroundColor(ColorConstants.RED);
            Div div = new Div().Add(p).SetRotationAngle(Math.PI / 3).SetBackgroundColor(ColorConstants.BLUE);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleMarginsTest01() {
            String outFileName = destinationFolder + "simpleMarginsTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_simpleMarginsTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com/", false);
            Link link = new Link("TestLink", action);
            link.SetBorder(new SolidBorder(ColorConstants.BLUE, 20));
            link.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePointValue(50));
            link.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePointValue(50));
            doc.Add(new Paragraph(link).SetBorder(new SolidBorder(10)));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MultiLineLinkTest01() {
            String outFileName = destinationFolder + "multiLineLinkTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_multiLineLinkTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com/", false);
            String text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut "
                 + "labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco " + "laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate "
                 + "velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in "
                 + "culpa qui officia deserunt mollit anim id est laborum.";
            Link link = new Link(text, action);
            doc.Add(new Paragraph(link));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableHeaderLinkTest01() {
            String outFileName = destinationFolder + "tableHeaderLinkTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_tableHeaderLinkTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com/", false);
            int numCols = 3;
            int numRows = 24;
            Table table = new Table(numCols);
            for (int x = 0; x < numCols; x++) {
                Cell headerCell = new Cell();
                String cellContent = "Header cell\n" + (x + 1);
                Link link = new Link(cellContent, action);
                link.SetFontColor(ColorConstants.BLUE);
                headerCell.Add(new Paragraph().Add(link));
                table.AddHeaderCell(headerCell);
            }
            for (int x = 0; x < numRows; x++) {
                table.AddCell(new Cell().SetHeight(100f).Add(new Paragraph("Content cell " + (x + 1))));
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LinkWithCustomRectangleTest01() {
            String outFileName = destinationFolder + "linkWithCustomRectangleTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_linkWithCustomRectangleTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String text = "Hello World";
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com");
            PdfLinkAnnotation annotation = new PdfLinkAnnotation(new Rectangle(1, 1)).SetAction(action);
            Link linkByAnnotation = new Link(text, annotation);
            doc.Add(new Paragraph(linkByAnnotation));
            annotation.SetRectangle(new PdfArray(new Rectangle(100, 100, 20, 20)));
            Link linkByChangedAnnotation = new Link(text, annotation);
            doc.Add(new Paragraph(linkByChangedAnnotation));
            Link linkByAction = new Link(text, action);
            doc.Add(new Paragraph(linkByAction));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SplitLinkTest01() {
            String outFileName = destinationFolder + "splitLinkTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_splitLinkTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com");
            PdfLinkAnnotation annotation = new PdfLinkAnnotation(new Rectangle(1, 1)).SetAction(action);
            Link linkByAnnotation = new Link(LONG_TEXT, annotation);
            doc.Add(new Div().SetHeight(700).SetBackgroundColor(ColorConstants.RED));
            // This paragraph is so long that it will be present on the first, second and third pages
            doc.Add(new Paragraph(linkByAnnotation));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationOnDivSplitTest01() {
            String outFileName = destinationFolder + "linkAnnotationOnDivSplitTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_linkAnnotationOnDivSplitTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com");
            PdfLinkAnnotation annotation = new PdfLinkAnnotation(new Rectangle(1, 1)).SetAction(action);
            Div div = new Div().SetHeight(2000).SetBackgroundColor(ColorConstants.RED);
            div.SetProperty(Property.LINK_ANNOTATION, annotation);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void LinkActionOnDivSplitTest01() {
            String outFileName = destinationFolder + "linkActionOnDivSplitTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_linkActionOnDivSplitTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            PdfAction action = PdfAction.CreateURI("http://itextpdf.com");
            Div div = new Div().SetHeight(2000).SetBackgroundColor(ColorConstants.RED);
            div.SetAction(action);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }
    }
}

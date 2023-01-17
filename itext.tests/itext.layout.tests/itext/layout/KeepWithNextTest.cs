/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class KeepWithNextTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/KeepWithNextTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/KeepWithNextTest/";

        private const String MIDDLE_TEXT = "Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document. To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries. Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme. Save time in Word with new buttons that show up where you need them.";

        private const String SHORT_TEXT = "Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device.";

        private const String LONG_TEXT = "Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document.\n"
             + "To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries.\n"
             + "Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme.\n"
             + "Save time in Word with new buttons that show up where you need them. To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign.\n"
             + "Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device.\n"
             + "Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document. To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries. Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme. Save time in Word with new buttons that show up where you need them.\n"
             + "To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign. Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device. Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document. To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar.\n"
             + "Click Insert and then choose the elements you want from the different galleries. Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme. Save time in Word with new buttons that show up where you need them. To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign. Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device.\n"
             + "Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document. To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries. Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme. Save time in Word with new buttons that show up where you need them.\n"
             + "To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign. Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device. Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document. To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar.\n"
             + "Click Insert and then choose the elements you want from the different galleries. Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme. Save time in Word with new buttons that show up where you need them. To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign. Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device.\n"
             + "Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document. To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries. Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme. Save time in Word with new buttons that show up where you need them.\n";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void KeepWithNextTest01() {
            String outFileName = destinationFolder + "keepWithNextTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_keepWithNextTest01.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf, PageSize.A4);
            for (int i = 0; i < 28; i++) {
                document.Add(new Paragraph("dummy"));
            }
            Paragraph title = new Paragraph("THIS IS THE TITLE OF A CHAPTER THAT FITS A PAGE");
            title.SetKeepWithNext(true);
            document.Add(title);
            for (int i = 0; i < 20; i++) {
                document.Add(new Paragraph("content of chapter " + i));
            }
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepWithNextTest02() {
            String outFileName = destinationFolder + "keepWithNextTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_keepWithNextTest02.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf, PageSize.A4);
            for (int i = 0; i < 28; i++) {
                document.Add(new Paragraph("dummy"));
            }
            Paragraph title = new Paragraph("THIS IS THE TITLE OF A CHAPTER THAT FITS A PAGE");
            title.SetKeepWithNext(true);
            document.Add(title);
            document.Add(new Paragraph(LONG_TEXT));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepWithNextTest03() {
            String outFileName = destinationFolder + "keepWithNextTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_keepWithNextTest03.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf, PageSize.A4);
            for (int i = 0; i < 27; i++) {
                document.Add(new Paragraph("dummy"));
            }
            Paragraph title = new Paragraph("THIS IS THE TITLE OF A CHAPTER THAT FITS A PAGE");
            title.SetKeepWithNext(true);
            document.Add(title);
            document.Add(new Paragraph(LONG_TEXT));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepWithNextTest04() {
            String outFileName = destinationFolder + "keepWithNextTest04.pdf";
            String cmpFileName = sourceFolder + "cmp_keepWithNextTest04.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf, PageSize.A4);
            for (int i = 0; i < 22; i++) {
                document.Add(new Paragraph("dummy"));
            }
            document.SetProperty(Property.FIRST_LINE_INDENT, 20f);
            Paragraph title = new Paragraph(MIDDLE_TEXT);
            title.SetKeepWithNext(true);
            document.Add(title);
            document.Add(new Paragraph(LONG_TEXT));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepWithNextTest05() {
            String outFileName = destinationFolder + "keepWithNextTest05.pdf";
            String cmpFileName = sourceFolder + "cmp_keepWithNextTest05.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf, PageSize.A4);
            for (int i = 0; i < 22; i++) {
                document.Add(new Paragraph("dummy"));
            }
            document.SetProperty(Property.FIRST_LINE_INDENT, 20f);
            Paragraph title = new Paragraph(MIDDLE_TEXT);
            title.SetKeepTogether(true);
            title.SetKeepWithNext(true);
            document.Add(title);
            document.Add(new Paragraph(LONG_TEXT));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepWithNextTest06() {
            String outFileName = destinationFolder + "keepWithNextTest06.pdf";
            String cmpFileName = sourceFolder + "cmp_keepWithNextTest06.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf, PageSize.A4);
            document.Add(new Paragraph(LONG_TEXT).SetKeepWithNext(true));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepWithNextTest07() {
            String outFileName = destinationFolder + "keepWithNextTest07.pdf";
            String cmpFileName = sourceFolder + "cmp_keepWithNextTest07.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf, PageSize.A4);
            document.SetProperty(Property.FIRST_LINE_INDENT, 20f);
            document.Add(new Paragraph(LONG_TEXT).SetKeepWithNext(true));
            document.Add(new Paragraph(LONG_TEXT));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepWithNextTest08() {
            String outFileName = destinationFolder + "keepWithNextTest08.pdf";
            String cmpFileName = sourceFolder + "cmp_keepWithNextTest08.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf, PageSize.A4);
            for (int i = 0; i < 25; i++) {
                document.Add(new Paragraph("dummy"));
            }
            document.Add(new Paragraph("Title").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetKeepWithNext
                (true));
            List list = new List(ListNumberingType.DECIMAL);
            for (int i = 0; i < 10; i++) {
                list.Add("item");
            }
            list.SetKeepTogether(true);
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepWithNextTest09() {
            String outFileName = destinationFolder + "keepWithNextTest09.pdf";
            String cmpFileName = sourceFolder + "cmp_keepWithNextTest09.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf, PageSize.A4);
            for (int i = 0; i < 28; i++) {
                document.Add(new Paragraph("dummy"));
            }
            document.Add(new Paragraph("Title").SetFontSize(20).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD
                )).SetKeepWithNext(true));
            List list = new List(ListNumberingType.DECIMAL);
            for (int i = 0; i < 10; i++) {
                list.Add("item");
            }
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepWithNextTest10() {
            String outFileName = destinationFolder + "keepWithNextTest10.pdf";
            String cmpFileName = sourceFolder + "cmp_keepWithNextTest10.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf, PageSize.A4);
            for (int i = 0; i < 25; i++) {
                document.Add(new Paragraph("dummy"));
            }
            document.Add(new Paragraph("Title").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetKeepWithNext
                (true));
            List list = new List(ListNumberingType.DECIMAL);
            for (int i = 0; i < 10; i++) {
                list.Add("item");
            }
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void KeepWithNextTest11() {
            String outFileName = destinationFolder + "keepWithNextTest11.pdf";
            String cmpFileName = sourceFolder + "cmp_keepWithNextTest11.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            Style style = new Style();
            style.SetProperty(Property.KEEP_WITH_NEXT, true);
            document.Add(new Paragraph("A").AddStyle(style));
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorderTop(new SolidBorder
                (2)).SetBorderBottom(new SolidBorder(2));
            table.AddCell("Body").AddHeaderCell("Header");
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}

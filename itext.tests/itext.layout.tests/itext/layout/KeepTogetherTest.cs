/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class KeepTogetherTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/KeepTogetherTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/KeepTogetherTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void KeepTogetherParagraphTest01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherParagraphTest01.pdf";
            String outFile = destinationFolder + "keepTogetherParagraphTest01.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            for (int i = 0; i < 29; i++) {
                doc.Add(new Paragraph("String number" + i));
            }
            String str = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaanasdadasdadaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            Paragraph p1 = new Paragraph(str);
            p1.SetKeepTogether(true);
            doc.Add(p1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherParagraphTest02() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherParagraphTest02.pdf";
            String outFile = destinationFolder + "keepTogetherParagraphTest02.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            for (int i = 0; i < 28; i++) {
                doc.Add(new Paragraph("String number" + i));
            }
            String str = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaanasdadasdadaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            for (int i = 0; i < 5; i++) {
                str += str;
            }
            Paragraph p1 = new Paragraph(str);
            p1.SetKeepTogether(true);
            doc.Add(p1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void KeepTogetherListTest01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherListTest01.pdf";
            String outFile = destinationFolder + "keepTogetherListTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            for (int i = 0; i < 28; i++) {
                doc.Add(new Paragraph("String number" + i));
            }
            List list = new List();
            list.Add("firstItem").Add("secondItem").Add("thirdItem").SetKeepTogether(true).SetListSymbol(ListNumberingType
                .DECIMAL);
            doc.Add(list);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void KeepTogetherDivTest01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherDivTest01.pdf";
            String outFile = destinationFolder + "keepTogetherDivTest01.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph("Test String");
            for (int i = 0; i < 28; i++) {
                doc.Add(p);
            }
            Div div = new Div();
            div.Add(new Paragraph("first paragraph"));
            div.Add(new Paragraph("second paragraph"));
            div.Add(new Paragraph("third paragraph"));
            div.SetKeepTogether(true);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void KeepTogetherMinHeightTest() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherMinHeightTest.pdf";
            String outFile = destinationFolder + "keepTogetherMinHeightTest.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph("Test String");
            for (int i = 0; i < 15; i++) {
                doc.Add(p);
            }
            Div div = new Div();
            div.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            div.SetMinHeight(500);
            div.SetKeepTogether(true);
            div.Add(new Paragraph("Hello"));
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherDivTest02() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherDivTest02.pdf";
            String outFile = destinationFolder + "keepTogetherDivTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            Rectangle[] columns = new Rectangle[] { new Rectangle(100, 100, 100, 500), new Rectangle(400, 100, 100, 500
                ) };
            doc.SetRenderer(new ColumnDocumentRenderer(doc, columns));
            Div div = new Div();
            doc.Add(new Paragraph("first string"));
            for (int i = 0; i < 130; i++) {
                div.Add(new Paragraph("String number " + i));
            }
            div.SetKeepTogether(true);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherDefaultTest01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherDefaultTest01.pdf";
            String outFile = destinationFolder + "keepTogetherDefaultTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            Div div = new KeepTogetherTest.KeepTogetherDiv();
            doc.Add(new Paragraph("first string"));
            for (int i = 0; i < 130; i++) {
                div.Add(new Paragraph("String number " + i));
            }
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        private class KeepTogetherDiv : Div {
            public override T1 GetDefaultProperty<T1>(int property) {
                if (property == Property.KEEP_TOGETHER) {
                    return (T1)(Object)true;
                }
                return base.GetDefaultProperty<T1>(property);
            }
        }
    }
}

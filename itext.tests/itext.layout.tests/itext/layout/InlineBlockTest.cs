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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class InlineBlockTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/InlineBlockTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/InlineBlockTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void InlineTableTest01() {
            // TODO DEVSIX-1967
            String name = "inlineTableTest01.pdf";
            String outFileName = destinationFolder + name;
            String cmpFileName = sourceFolder + "cmp_" + name;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph().SetMultipliedLeading(0);
            p.Add(new Paragraph("This is inline table: ").SetBorder(new SolidBorder(1)).SetMultipliedLeading(0));
            Table inlineTable = new Table(1);
            int commonPadding = 10;
            Cell cell = new Cell();
            Paragraph paragraph = new Paragraph("Cell 1");
            inlineTable.AddCell(cell.Add(paragraph.SetMultipliedLeading(0)).SetPadding(commonPadding).SetWidth(33));
            Div div = new Div();
            p.Add(div.Add(inlineTable).SetPadding(commonPadding)).Add(new Paragraph(". Was it fun?").SetBorder(new SolidBorder
                (1)).SetMultipliedLeading(0));
            SolidBorder border = new SolidBorder(1);
            doc.Add(p);
            Paragraph p1 = new Paragraph().Add(p).SetBorder(border);
            doc.Add(p1);
            Paragraph p2 = new Paragraph().Add(p1).SetBorder(border);
            doc.Add(p2);
            Paragraph p3 = new Paragraph().Add(p2).SetBorder(border);
            doc.Add(p3);
            Paragraph p4 = new Paragraph().Add(p3).SetBorder(border);
            doc.Add(p4);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DeepNestingInlineBlocksTest01() {
            // TODO DEVSIX-1963
            String name = "deepNestingInlineBlocksTest01.pdf";
            String outFileName = destinationFolder + name;
            String cmpFileName = sourceFolder + "cmp_" + name;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Color[] colors = new Color[] { ColorConstants.BLUE, ColorConstants.RED, ColorConstants.LIGHT_GRAY, ColorConstants
                .ORANGE };
            int w = 60;
            int n = 6;
            Paragraph p = new Paragraph("hello world").SetWidth(w);
            for (int i = 0; i < n; ++i) {
                Paragraph currP = new Paragraph().SetWidth(i == 0 ? w * 1.1f * 3 : 450 + 5 * i);
                currP.Add(p).Add(p).Add(p).SetBorder(new DashedBorder(colors[i % colors.Length], 0.5f));
                p = currP;
            }
            long start = SystemUtil.GetRelativeTimeMillis();
            doc.Add(p);
            // 606 on local machine (including jvm warming up)
            System.Console.Out.WriteLine(SystemUtil.GetRelativeTimeMillis() - start);
            p = new Paragraph("hello world");
            for (int i = 0; i < n; ++i) {
                Paragraph currP = new Paragraph();
                currP.Add(p).Add(p).Add(p).SetBorder(new DashedBorder(colors[i % colors.Length], 0.5f));
                p = currP;
            }
            start = SystemUtil.GetRelativeTimeMillis();
            doc.Add(p);
            // 4656 on local machine
            System.Console.Out.WriteLine(SystemUtil.GetRelativeTimeMillis() - start);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void WrappingAfter100PercentWidthFloatTest() {
            String name = "wrappingAfter100PercentWidthFloatTest.pdf";
            String output = destinationFolder + name;
            String cmp = sourceFolder + "cmp_" + name;
            using (Document doc = new Document(new PdfDocument(new PdfWriter(output)))) {
                Div floatingDiv = new Div().SetWidth(UnitValue.CreatePercentValue(100)).SetHeight(10).SetBorder(new SolidBorder
                    (1)).SetBackgroundColor(ColorConstants.RED);
                floatingDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
                floatingDiv.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                floatingDiv.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
                Div inlineDiv = new Div().SetWidth(UnitValue.CreatePercentValue(100)).SetHeight(10).SetBorder(new SolidBorder
                    (1))
                                // gold color
                                .SetBackgroundColor(new DeviceRgb(255, 215, 0));
                inlineDiv.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                inlineDiv.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
                doc.Add(new Div().Add(floatingDiv).Add(new Paragraph().Add(inlineDiv)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, destinationFolder));
        }
    }
}

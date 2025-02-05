/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LineSeparatorTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/LineSeparatorTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/LineSeparatorTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void LineSeparatorWidthPercentageTest01() {
            String outFileName = destinationFolder + "lineSeparatorWidthPercentageTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_lineSeparatorWidthPercentageTest01.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            ILineDrawer line1 = new SolidLine();
            line1.SetColor(ColorConstants.RED);
            ILineDrawer line2 = new SolidLine();
            document.Add(new LineSeparator(line1).SetWidth(50).SetMarginBottom(10));
            document.Add(new LineSeparator(line2).SetWidth(UnitValue.CreatePercentValue(50)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LineSeparatorBackgroundTest01() {
            String outFileName = destinationFolder + "lineSeparatorBackgroundTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_lineSeparatorBackgroundTest01.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            Style style = new Style();
            style.SetBackgroundColor(ColorConstants.YELLOW);
            style.SetMargin(10);
            document.Add(new LineSeparator(new SolidLine()).AddStyle(style));
            document.Add(new LineSeparator(new DashedLine()).SetBackgroundColor(ColorConstants.RED));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void RotatedLineSeparatorTest01() {
            String outFileName = destinationFolder + "rotatedLineSeparatorTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_rotatedLineSeparatorTest01.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            document.Add(new LineSeparator(new DashedLine()).SetBackgroundColor(ColorConstants.RED).SetRotationAngle(Math
                .PI / 2));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void RotatedLineSeparatorTest02() {
            String outFileName = destinationFolder + "rotatedLineSeparatorTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_rotatedLineSeparatorTest02.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            document.Add(new Paragraph("Hello"));
            document.Add(new LineSeparator(new DashedLine()).SetWidth(100).SetHorizontalAlignment(HorizontalAlignment.
                CENTER).SetBackgroundColor(ColorConstants.GREEN).SetRotationAngle(Math.PI / 4));
            document.Add(new Paragraph("World"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}

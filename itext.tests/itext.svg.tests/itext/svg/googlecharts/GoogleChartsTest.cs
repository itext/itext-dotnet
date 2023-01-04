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
using iText.Kernel.Geom;
using iText.Kernel.Utils;
using iText.Svg.Renderers;
using iText.Svg.Utils;
using iText.Test;

namespace iText.Svg.Googlecharts {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GoogleChartsTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/GoogleChartsTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/googlecharts/GoogleChartsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void BarChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "barChart");
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "annotationChart");
        }

        [NUnit.Framework.Test]
        public virtual void AreaChart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "areaChart.pdf", sourceFolder + "areaChart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "areaChart.pdf", sourceFolder
                 + "cmp_areaChart.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void BubbleChart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "bubbleChart.pdf", sourceFolder + "bubbleChart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "bubbleChart.pdf", sourceFolder
                 + "cmp_bubbleChart.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CalendarChart() {
            //TODO DEVSIX-4857 support stroke-linecap attribute
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "calendarChart.pdf", sourceFolder + "calendarChart.svg", 1, 
                pageSize);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "calendarChart.pdf", 
                sourceFolder + "cmp_calendarChart.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CandlestickChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "candlestickChart");
        }

        [NUnit.Framework.Test]
        public virtual void ComboChart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "comboChart.pdf", sourceFolder + "comboChart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "comboChart.pdf", sourceFolder
                 + "cmp_comboChart.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DiffChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "diffChart");
        }

        [NUnit.Framework.Test]
        public virtual void DonutChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "donutChart");
        }

        [NUnit.Framework.Test]
        public virtual void WaterfallChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "waterfallChart");
        }

        [NUnit.Framework.Test]
        public virtual void HistogramChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "histogramChart");
        }
    }
}

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

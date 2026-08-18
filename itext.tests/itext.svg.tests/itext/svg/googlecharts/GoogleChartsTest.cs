/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/GoogleChartsTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/googlecharts/GoogleChartsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BarChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "barChart");
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "annotationChart");
        }

        [NUnit.Framework.Test]
        public virtual void AreaChart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(DESTINATION_FOLDER + "areaChart.pdf", SOURCE_FOLDER + "areaChart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "areaChart.pdf", SOURCE_FOLDER
                 + "cmp_areaChart.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void BubbleChart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(DESTINATION_FOLDER + "bubbleChart.pdf", SOURCE_FOLDER + "bubbleChart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "bubbleChart.pdf", SOURCE_FOLDER
                 + "cmp_bubbleChart.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CalendarChart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(DESTINATION_FOLDER + "calendarChart.pdf", SOURCE_FOLDER + "calendarChart.svg", 1
                , pageSize);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "calendarChart.pdf", 
                SOURCE_FOLDER + "cmp_calendarChart.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CandlestickChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "candlestickChart");
        }

        [NUnit.Framework.Test]
        public virtual void ComboChart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(DESTINATION_FOLDER + "comboChart.pdf", SOURCE_FOLDER + "comboChart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "comboChart.pdf", SOURCE_FOLDER
                 + "cmp_comboChart.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DiffChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "diffChart");
        }

        [NUnit.Framework.Test]
        public virtual void DonutChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "donutChart");
        }

        [NUnit.Framework.Test]
        public virtual void WaterfallChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "waterfallChart");
        }

        [NUnit.Framework.Test]
        public virtual void HistogramChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "histogramChart");
        }
    }
}

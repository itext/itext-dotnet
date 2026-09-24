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
    public class TimelineChartsTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/TimelineChartsTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/googlecharts/TimelineChartsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TimelineAdvancedChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "timelineAdvancedChart");
        }

        [NUnit.Framework.Test]
        public virtual void TimelineChart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(DESTINATION_FOLDER + "timelineChart.pdf", SOURCE_FOLDER + "timelineChart.svg", 1
                , pageSize);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "timelineChart.pdf", 
                SOURCE_FOLDER + "cmp_timelineChart.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TimelineLabeledChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "timelineLabeledChart");
        }
    }
}

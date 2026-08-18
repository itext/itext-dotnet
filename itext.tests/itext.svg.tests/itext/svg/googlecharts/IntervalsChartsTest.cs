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
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Googlecharts {
    [NUnit.Framework.Category("IntegrationTest")]
    public class IntervalsChartsTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/IntervalsChartsTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/googlecharts/IntervalsChartsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsAreaChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsAreaChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsBackgroundBoxChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsBackgroundBoxChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsBoxChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsBoxChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsBoxPlotChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsBoxPlotChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsBoxThickChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsBoxThickChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsCombiningChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsCombiningChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsLineChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsLineChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsPointsChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsPointsChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsPointsWhiskersChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsPointsWhiskersChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsStickChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsStickChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsSticksHorizontalChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsSticksHorizontalChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsTailoredChart() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "intervalsTailoredChart");
        }
    }
}

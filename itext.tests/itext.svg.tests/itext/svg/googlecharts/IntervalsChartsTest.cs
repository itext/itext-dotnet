/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/IntervalsChartsTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/googlecharts/IntervalsChartsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsAreaChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsAreaChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsBackgroundBoxChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsBackgroundBoxChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsBoxChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsBoxChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsBoxPlotChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsBoxPlotChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsBoxThickChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsBoxThickChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsCombiningChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsCombiningChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsLineChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsLineChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsPointsChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsPointsChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsPointsWhiskersChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsPointsWhiskersChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsStickChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsStickChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsSticksHorizontalChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsSticksHorizontalChart");
        }

        [NUnit.Framework.Test]
        public virtual void IntervalsTailoredChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "intervalsTailoredChart");
        }
    }
}

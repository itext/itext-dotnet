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

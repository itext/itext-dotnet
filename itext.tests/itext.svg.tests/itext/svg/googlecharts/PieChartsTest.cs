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
    public class PieChartsTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/PieChartsTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/googlecharts/PieChartsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PieChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pieChart");
        }

        [NUnit.Framework.Test]
        public virtual void Pie3DChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pie3DChart");
        }

        [NUnit.Framework.Test]
        public virtual void PieHoleChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pieHoleChart");
        }

        [NUnit.Framework.Test]
        public virtual void RotatingPieChart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rotatingPieChart");
        }

        [NUnit.Framework.Test]
        public virtual void Pie1Chart() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pie1Chart");
        }
    }
}

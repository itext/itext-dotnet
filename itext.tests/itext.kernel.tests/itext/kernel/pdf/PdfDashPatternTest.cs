/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfDashPatternTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructorNoParamTest() {
            PdfDashPattern dashPattern = new PdfDashPattern();
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetDash(), 0.0001);
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetGap(), 0.0001);
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetPhase(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorOneParamTest() {
            PdfDashPattern dashPattern = new PdfDashPattern(10);
            NUnit.Framework.Assert.AreEqual(10, dashPattern.GetDash(), 0.0001);
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetGap(), 0.0001);
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetPhase(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorTwoParamsTest() {
            PdfDashPattern dashPattern = new PdfDashPattern(10, 20);
            NUnit.Framework.Assert.AreEqual(10, dashPattern.GetDash(), 0.0001);
            NUnit.Framework.Assert.AreEqual(20, dashPattern.GetGap(), 0.0001);
            NUnit.Framework.Assert.AreEqual(-1, dashPattern.GetPhase(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorThreeParamsTest() {
            PdfDashPattern dashPattern = new PdfDashPattern(10, 20, 30);
            NUnit.Framework.Assert.AreEqual(10, dashPattern.GetDash(), 0.0001);
            NUnit.Framework.Assert.AreEqual(20, dashPattern.GetGap(), 0.0001);
            NUnit.Framework.Assert.AreEqual(30, dashPattern.GetPhase(), 0.0001);
        }
    }
}

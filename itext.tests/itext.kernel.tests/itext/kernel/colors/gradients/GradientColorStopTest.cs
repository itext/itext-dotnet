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
using iText.Test;

namespace iText.Kernel.Colors.Gradients {
    [NUnit.Framework.Category("UnitTest")]
    public class GradientColorStopTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NormalizationTest() {
            GradientColorStop stopToTest = new GradientColorStop(new float[] { -0.5f, 1.5f, 0.5f, 0.5f }, 1.5, GradientColorStop.OffsetType
                .AUTO).SetHint(1.5, GradientColorStop.HintOffsetType.NONE);
            iText.Test.TestUtil.AreEqual(new float[] { 0f, 1f, 0.5f }, stopToTest.GetRgbArray(), 1e-10f);
            NUnit.Framework.Assert.AreEqual(0, stopToTest.GetOffset(), 1e-10);
            NUnit.Framework.Assert.AreEqual(GradientColorStop.OffsetType.AUTO, stopToTest.GetOffsetType());
            NUnit.Framework.Assert.AreEqual(0, stopToTest.GetHintOffset(), 1e-10);
            NUnit.Framework.Assert.AreEqual(GradientColorStop.HintOffsetType.NONE, stopToTest.GetHintOffsetType());
        }

        [NUnit.Framework.Test]
        public virtual void CornerCasesTest() {
            GradientColorStop stopToTest = new GradientColorStop((float[])null, 1.5, GradientColorStop.OffsetType.AUTO
                ).SetHint(1.5, GradientColorStop.HintOffsetType.NONE);
            iText.Test.TestUtil.AreEqual(new float[] { 0f, 0f, 0f }, stopToTest.GetRgbArray(), 1e-10f);
            NUnit.Framework.Assert.AreEqual(0, stopToTest.GetOffset(), 1e-10);
            NUnit.Framework.Assert.AreEqual(GradientColorStop.OffsetType.AUTO, stopToTest.GetOffsetType());
            NUnit.Framework.Assert.AreEqual(0, stopToTest.GetHintOffset(), 1e-10);
            NUnit.Framework.Assert.AreEqual(GradientColorStop.HintOffsetType.NONE, stopToTest.GetHintOffsetType());
        }
    }
}

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
using iText.Kernel.Colors;
using iText.Test;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class ColorUtilsTest : ExtendedITextTest {
        private const float EPSILON = 0.000001f;

        [NUnit.Framework.Test]
        public virtual void CalculateSvgLuminanceTest() {
            AssertSvgLuminance(0f, 0f, 0f, 0f);
            AssertSvgLuminance(1f, 1f, 1f, 1f);
            AssertSvgLuminance(1f, 0f, 0f, 0.2125f);
            AssertSvgLuminance(0f, 1f, 0f, 0.7154f);
            AssertSvgLuminance(0f, 0f, 1f, 0.0721f);
            AssertSvgLuminance(0.5f, 0.5f, 0.5f, 0.5f);
            AssertSvgLuminance(0.2f, 0.4f, 0.6f, 0.37192f);
        }

        [NUnit.Framework.Test]
        public virtual void RgbPrimariesAreConvertedUsingSvgLuminanceCoefficientsTest() {
            AssertGray(0.2125f, new DeviceRgb(1f, 0f, 0f));
            AssertGray(0.7154f, new DeviceRgb(0f, 1f, 0f));
            AssertGray(0.0721f, new DeviceRgb(0f, 0f, 1f));
            AssertGray(1, new DeviceRgb(1f, 1f, 1f));
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGrayColorIsPreservedTest() {
            DeviceGray gray = new DeviceGray(0.4f);
            NUnit.Framework.Assert.AreSame(gray, ColorUtils.ToDeviceGrayForSvgLuminanceMode(gray));
        }

        [NUnit.Framework.Test]
        public virtual void DeviceCmykColorIsPreservedTest() {
            DeviceCmyk cmyk = new DeviceCmyk(0.1f, 0.2f, 0.3f, 0.4f);
            NUnit.Framework.Assert.AreSame(cmyk, ColorUtils.ToDeviceGrayForSvgLuminanceMode(cmyk));
        }

        [NUnit.Framework.Test]
        public virtual void NullColorIsPreservedTest() {
            NUnit.Framework.Assert.IsNull(ColorUtils.ToDeviceGrayForSvgLuminanceMode(null));
        }

        private static void AssertGray(float expected, DeviceRgb rgb) {
            Color converted = ColorUtils.ToDeviceGrayForSvgLuminanceMode(rgb);
            NUnit.Framework.Assert.AreEqual(1, converted.GetNumberOfComponents());
            NUnit.Framework.Assert.AreEqual(expected, converted.GetColorValue()[0], EPSILON);
        }

        private static void AssertSvgLuminance(float red, float green, float blue, float expected) {
            float[] rgb = new float[] { red, green, blue };
            NUnit.Framework.Assert.AreEqual(expected, ColorUtils.CalculateSvgLuminance(rgb), EPSILON);
            NUnit.Framework.Assert.AreEqual(new float[] { red, green, blue }, rgb);
        }
    }
}

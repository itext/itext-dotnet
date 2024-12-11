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
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Colors {
    [NUnit.Framework.Category("UnitTest")]
    public class DeviceRgbTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void MakeDarkerTest() {
            DeviceRgb rgbColor = new DeviceRgb(50, 100, 150);
            DeviceRgb darkerRgbColor = DeviceRgb.MakeDarker(rgbColor);
            // check the resultant darkness of RGB items with using this multiplier
            float multiplier = Math.Max(0f, (150f / 255 - 0.33f) / (150f / 255));
            NUnit.Framework.Assert.AreEqual(multiplier * (50f / 255), darkerRgbColor.GetColorValue()[0], 0.0001);
            NUnit.Framework.Assert.AreEqual(multiplier * (100f / 255), darkerRgbColor.GetColorValue()[1], 0.0001);
            NUnit.Framework.Assert.AreEqual(multiplier * (150f / 255), darkerRgbColor.GetColorValue()[2], 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void MakeLighterTest() {
            DeviceRgb rgbColor = new DeviceRgb(50, 100, 150);
            DeviceRgb darkerRgbColor = DeviceRgb.MakeLighter(rgbColor);
            // check the resultant darkness of RGB items with using this multiplier
            float multiplier = Math.Min(1f, 150f / 255 + 0.33f) / (150f / 255);
            NUnit.Framework.Assert.AreEqual(multiplier * (50f / 255), darkerRgbColor.GetColorValue()[0], 0.0001);
            NUnit.Framework.Assert.AreEqual(multiplier * (100f / 255), darkerRgbColor.GetColorValue()[1], 0.0001);
            NUnit.Framework.Assert.AreEqual(multiplier * (150f / 255), darkerRgbColor.GetColorValue()[2], 0.0001);
        }

        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        [iText.Commons.Utils.NoopAnnotation]
        // java.awt is not compatible with graalvm
        [NUnit.Framework.Test]
        public virtual void ColorByAWTColorConstantTest() {
            // RED
            DeviceRgb rgbColor = new DeviceRgb(System.Drawing.Color.Red);
            float[] rgbColorValue = rgbColor.GetColorValue();
            NUnit.Framework.Assert.AreEqual(1, rgbColorValue[0], 0.0001);
            NUnit.Framework.Assert.AreEqual(0, rgbColorValue[1], 0.0001);
            NUnit.Framework.Assert.AreEqual(0, rgbColorValue[2], 0.0001);
            // GREEN
            rgbColor = new DeviceRgb(System.Drawing.Color.Lime);
            rgbColorValue = rgbColor.GetColorValue();
            NUnit.Framework.Assert.AreEqual(0, rgbColorValue[0], 0.0001);
            NUnit.Framework.Assert.AreEqual(1, rgbColorValue[1], 0.0001);
            NUnit.Framework.Assert.AreEqual(0, rgbColorValue[2], 0.0001);
            // BLUE
            rgbColor = new DeviceRgb(System.Drawing.Color.Blue);
            rgbColorValue = rgbColor.GetColorValue();
            NUnit.Framework.Assert.AreEqual(0, rgbColorValue[0], 0.0001);
            NUnit.Framework.Assert.AreEqual(0, rgbColorValue[1], 0.0001);
            NUnit.Framework.Assert.AreEqual(1, rgbColorValue[2], 0.0001);
        }

        [iText.Commons.Utils.NoopAnnotation]
        // java.awt is not compatible with graalvm
        [NUnit.Framework.Test]
        public virtual void ColorByAWTColorTest() {
            System.Drawing.Color color = System.Drawing.Color.FromArgb(50, 100, 150);
            DeviceRgb rgbColor = new DeviceRgb(color);
            float[] rgbColorValue = rgbColor.GetColorValue();
            NUnit.Framework.Assert.AreEqual(50f / 255, rgbColorValue[0], 0.0001);
            NUnit.Framework.Assert.AreEqual(100f / 255, rgbColorValue[1], 0.0001);
            NUnit.Framework.Assert.AreEqual(150f / 255, rgbColorValue[2], 0.0001);
        }

        // Android-Conversion-Skip-Block-End
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.COLORANT_INTENSITIES_INVALID, Count = 14)]
        public virtual void InvalidConstructorArgumentsTest() {
            NUnit.Framework.Assert.AreEqual(0, GetSumOfColorValues(new DeviceRgb(-2f, 0f, 0f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(0, GetSumOfColorValues(new DeviceRgb(0f, -2f, 0f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(0, GetSumOfColorValues(new DeviceRgb(0f, 0f, -2f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(1, GetSumOfColorValues(new DeviceRgb(2f, 0f, 0f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(1, GetSumOfColorValues(new DeviceRgb(0f, 2f, 0f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(1, GetSumOfColorValues(new DeviceRgb(0f, 0f, 2f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(0, GetSumOfColorValues(new DeviceRgb(-2f, -2f, 0f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(0, GetSumOfColorValues(new DeviceRgb(-2f, 0f, -2f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(0, GetSumOfColorValues(new DeviceRgb(0f, -2f, -2f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(2, GetSumOfColorValues(new DeviceRgb(2f, 2f, 0f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(2, GetSumOfColorValues(new DeviceRgb(2f, 0f, 2f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(2, GetSumOfColorValues(new DeviceRgb(0f, 2f, 2f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(0, GetSumOfColorValues(new DeviceRgb(-2f, -2f, -2f)), 0.001f);
            NUnit.Framework.Assert.AreEqual(3, GetSumOfColorValues(new DeviceRgb(2f, 2f, 2f)), 0.001f);
        }

        private float GetSumOfColorValues(DeviceRgb deviceRgb) {
            float sum = 0;
            float[] values = deviceRgb.GetColorValue();
            for (int i = 0; i < values.Length; i++) {
                sum += values[i];
            }
            return sum;
        }
    }
}

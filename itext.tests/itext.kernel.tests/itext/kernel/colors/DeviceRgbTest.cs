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

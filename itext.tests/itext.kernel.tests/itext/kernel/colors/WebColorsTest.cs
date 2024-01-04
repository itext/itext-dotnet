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

namespace iText.Kernel.Colors {
    [NUnit.Framework.Category("UnitTest")]
    public class WebColorsTest : ExtendedITextTest {
        private const double RGB_MAX_VAL = 255.0;

        [NUnit.Framework.Test]
        public virtual void GetRGBColorBySupportedNameTest() {
            String colorName = "violet";
            DeviceRgb cmpRgb = new DeviceRgb(0xee, 0x82, 0xee);
            DeviceRgb resultRgb = WebColors.GetRGBColor(colorName);
            NUnit.Framework.Assert.AreEqual(cmpRgb, resultRgb);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBColorByUnsupportedNameTest() {
            String colorName = "tangerine";
            DeviceRgb cmpRgb = new DeviceRgb(0, 0, 0);
            DeviceRgb resultRgb = WebColors.GetRGBColor(colorName);
            NUnit.Framework.Assert.AreEqual(cmpRgb, resultRgb);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBAColorByNameTest() {
            String colorName = "violet";
            float[] cmpRgba = new float[] { (float)(238 / RGB_MAX_VAL), (float)(130 / RGB_MAX_VAL), (float)(238 / RGB_MAX_VAL
                ), (float)(1.0) };
            float delta = (float)(0.0001);
            float[] resultRgba = WebColors.GetRGBAColor(colorName);
            iText.Test.TestUtil.AreEqual(cmpRgba, resultRgba, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBAColorByCodeWithHashTest() {
            //corresponding color name = "violet"
            String hashHex = "#EE82EE";
            float[] cmpRgba = new float[] { (float)(238 / RGB_MAX_VAL), (float)(130 / RGB_MAX_VAL), (float)(238 / RGB_MAX_VAL
                ), (float)(1.0) };
            float delta = (float)(0.0001);
            float[] resultRgba = WebColors.GetRGBAColor(hashHex);
            iText.Test.TestUtil.AreEqual(cmpRgba, resultRgba, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBAColorByCode6DigitsTest() {
            //corresponding color name = "violet"
            String hexString = "EE82EE";
            float[] cmpRgba = new float[] { (float)(238 / RGB_MAX_VAL), (float)(130 / RGB_MAX_VAL), (float)(238 / RGB_MAX_VAL
                ), (float)(1.0) };
            float delta = (float)(0.0001);
            float[] resultRgba = WebColors.GetRGBAColor(hexString);
            iText.Test.TestUtil.AreEqual(cmpRgba, resultRgba, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBAColorByCode3DigitsTest() {
            //corresponding full hex = #990000, rgb(153,0,0)
            String hexString = "900";
            float[] cmpRgba = new float[] { (float)(153 / RGB_MAX_VAL), (float)(0.0), (float)(0.0), (float)(1.0) };
            float delta = (float)(0.0001);
            float[] resultRgba = WebColors.GetRGBAColor(hexString);
            iText.Test.TestUtil.AreEqual(cmpRgba, resultRgba, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBAColorByCodeWrongDigitsNumberTest() {
            String hexString = "9000";
            float[] resultRgba = WebColors.GetRGBAColor(hexString);
            NUnit.Framework.Assert.IsNull(resultRgba);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBAColorByRgbObjectTest() {
            //corresponding color name = "violet"
            String rgbString = "rgb(238,130,238)";
            float[] cmpRgba = new float[] { (float)(238 / RGB_MAX_VAL), (float)(130 / RGB_MAX_VAL), (float)(238 / RGB_MAX_VAL
                ), (float)(1.0) };
            float delta = (float)(0.0001);
            float[] resultRgba = WebColors.GetRGBAColor(rgbString);
            iText.Test.TestUtil.AreEqual(cmpRgba, resultRgba, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBAColorByRgbaObjectTest() {
            //corresponding color name = "violet"
            String rgbaString = "rgba(238,130,238,255)";
            float[] cmpRgba = new float[] { (float)(238 / RGB_MAX_VAL), (float)(130 / RGB_MAX_VAL), (float)(238 / RGB_MAX_VAL
                ), (float)(1.0) };
            float delta = (float)(0.0001);
            float[] resultRgba = WebColors.GetRGBAColor(rgbaString);
            iText.Test.TestUtil.AreEqual(cmpRgba, resultRgba, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetCMYKColorByDeviceCmykTest() {
            //corresponding color name = "violet"
            String cmykString = "device-cmyk(44% 100% 0% 0%)";
            float[] cmpCmyk = new float[] { 0.44f, 1f, 0f, 0f, 1f };
            float delta = (float)(0.0001);
            float[] resultCmyk = WebColors.GetCMYKArray(cmykString);
            iText.Test.TestUtil.AreEqual(cmpCmyk, resultCmyk, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetCMYKColorByDeviceCmykWithOpacityTest() {
            //corresponding color name = "violet"
            String cmykString = "device-cmyk(44% 100% 0% 0% / .8)";
            float[] cmpCmyk = new float[] { 0.44f, 1f, 0f, 0f, 0.8f };
            float delta = (float)(0.0001);
            float[] resultCmyk = WebColors.GetCMYKArray(cmykString);
            iText.Test.TestUtil.AreEqual(cmpCmyk, resultCmyk, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetCMYKColorByDeviceCmykWithOpacityAndFallbackTest() {
            //corresponding color name = "violet"
            String cmykString = "device-cmyk(44% 100% 0% 0% / .8 rgb(238,130,238))";
            float[] cmpCmyk = new float[] { 0.44f, 1f, 0f, 0f, 0.8f };
            float delta = (float)(0.0001);
            float[] resultCmyk = WebColors.GetCMYKArray(cmykString);
            iText.Test.TestUtil.AreEqual(cmpCmyk, resultCmyk, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetCMYKColorWithNoBlack() {
            String cmykString = "device-cmyk(44% 100% 0%))";
            float[] cmpCmyk = new float[] { 0.44f, 1f, 0f, 1f, 1f };
            float delta = (float)(0.0001);
            float[] resultCmyk = WebColors.GetCMYKArray(cmykString);
            iText.Test.TestUtil.AreEqual(cmpCmyk, resultCmyk, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetCMYKColorWithInvalidDeviceCmykDefinition() {
            String cmykString = "cmyk(44% 100% 0% 1%))";
            float[] resultCmyk = WebColors.GetCMYKArray(cmykString);
            NUnit.Framework.Assert.IsNull(resultCmyk);
        }

        [NUnit.Framework.Test]
        public virtual void GetCMYKColorWithExceptionDuringParsing() {
            float[] resultCmyk = WebColors.GetCMYKArray(null);
            NUnit.Framework.Assert.IsNull(resultCmyk);
        }

        [NUnit.Framework.Test]
        public virtual void GetCMYKColorTest() {
            //corresponding color name = "violet"
            String cmykString = "device-cmyk(44% 100% 0% 0%)";
            float[] cmpCmyk = new float[] { 0.44f, 1f, 0f, 0f };
            float delta = (float)(0.0001);
            DeviceCmyk resultCmyk = WebColors.GetCMYKColor(cmykString);
            iText.Test.TestUtil.AreEqual(cmpCmyk, resultCmyk.colorValue, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetCMYKColorFloatTest() {
            String cmykString = "device-cmyk(0.44 1 0.2 0.2)";
            float[] cmpCmyk = new float[] { 0.44f, 1f, 0.2f, 0.2f };
            float delta = (float)(0.0001);
            DeviceCmyk resultCmyk = WebColors.GetCMYKColor(cmykString);
            iText.Test.TestUtil.AreEqual(cmpCmyk, resultCmyk.colorValue, delta);
        }

        [NUnit.Framework.Test]
        public virtual void GetCMYKColorNullTest() {
            String cmykString = "null";
            float[] cmpCmyk = new float[] { 0f, 0f, 0f, 1f };
            float delta = (float)(0.0001);
            DeviceCmyk resultCmyk = WebColors.GetCMYKColor(cmykString);
            iText.Test.TestUtil.AreEqual(cmpCmyk, resultCmyk.colorValue, delta);
        }
    }
}

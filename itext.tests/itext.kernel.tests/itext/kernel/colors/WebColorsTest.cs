/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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

namespace iText.Kernel.Colors {
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
            NUnit.Framework.Assert.AreEqual(cmpRgba, resultRgba);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBAColorByCodeWithHashTest() {
            //corresponding color name = "violet"
            String hashHex = "#EE82EE";
            float[] cmpRgba = new float[] { (float)(238 / RGB_MAX_VAL), (float)(130 / RGB_MAX_VAL), (float)(238 / RGB_MAX_VAL
                ), (float)(1.0) };
            float delta = (float)(0.0001);
            float[] resultRgba = WebColors.GetRGBAColor(hashHex);
            NUnit.Framework.Assert.AreEqual(cmpRgba, resultRgba);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBAColorByCode6DigitsTest() {
            //corresponding color name = "violet"
            String hexString = "EE82EE";
            float[] cmpRgba = new float[] { (float)(238 / RGB_MAX_VAL), (float)(130 / RGB_MAX_VAL), (float)(238 / RGB_MAX_VAL
                ), (float)(1.0) };
            float delta = (float)(0.0001);
            float[] resultRgba = WebColors.GetRGBAColor(hexString);
            NUnit.Framework.Assert.AreEqual(cmpRgba, resultRgba);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBAColorByCode3DigitsTest() {
            //corresponding full hex = #990000, rgb(153,0,0)
            String hexString = "900";
            float[] cmpRgba = new float[] { (float)(153 / RGB_MAX_VAL), (float)(0.0), (float)(0.0), (float)(1.0) };
            float delta = (float)(0.0001);
            float[] resultRgba = WebColors.GetRGBAColor(hexString);
            NUnit.Framework.Assert.AreEqual(cmpRgba, resultRgba);
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
            NUnit.Framework.Assert.AreEqual(cmpRgba, resultRgba);
        }

        [NUnit.Framework.Test]
        public virtual void GetRGBAColorByRgbaObjectTest() {
            //corresponding color name = "violet"
            String rgbaString = "rgba(238,130,238,255)";
            float[] cmpRgba = new float[] { (float)(238 / RGB_MAX_VAL), (float)(130 / RGB_MAX_VAL), (float)(238 / RGB_MAX_VAL
                ), (float)(1.0) };
            float delta = (float)(0.0001);
            float[] resultRgba = WebColors.GetRGBAColor(rgbaString);
            NUnit.Framework.Assert.AreEqual(cmpRgba, resultRgba);
        }
    }
}

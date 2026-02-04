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
using System;
using iText.Kernel.Colors;
using iText.Test;

namespace iText.Kernel.Contrast {
    [NUnit.Framework.Category("UnitTest")]
    public class ColorContrastCalculatorTest : ExtendedITextTest {
        public static Object[][] Colors() {
            // As per https://webaim.org/resources/contrastchecker/
            return new Object[][] { new Object[] { new DeviceRgb(0, 0, 0), new DeviceRgb(255, 255, 255), 21.0 }, new Object
                [] { new DeviceRgb(255, 0, 0), new DeviceRgb(0, 0, 255), 2.15 }, new Object[] { new DeviceRgb(255, 255
                , 0), new DeviceRgb(0, 0, 255), 8 }, new Object[] { new DeviceRgb(128, 128, 128), new DeviceRgb(0, 0, 
                255), 2.17 }, new Object[] { new DeviceRgb(128, 128, 128), new DeviceRgb(192, 192, 192), 2.17 }, new Object
                [] { new DeviceRgb(0, 128, 0), new DeviceRgb(255, 255, 255), 5.13 }, new Object[] { new DeviceRgb(255, 
                165, 0), new DeviceRgb(0, 0, 0), 10.63 } };
        }

        [NUnit.Framework.TestCaseSource("Colors")]
        public virtual void CalculateContrastShouldBeTheSameAsWebAimContrastTool(DeviceRgb color1, DeviceRgb color2
            , double expectedContrast) {
            double calculatedContrast = ColorContrastCalculator.ContrastRatio(color1, color2);
            NUnit.Framework.Assert.AreEqual(expectedContrast, calculatedContrast, 0.01);
        }

        [NUnit.Framework.Test]
        public virtual void ContrastColor1NullShouldThrowException() {
            DeviceRgb color2 = new DeviceRgb(255, 255, 255);
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                ColorContrastCalculator.ContrastRatio(null, color2);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContrastColor2NullShouldThrowException() {
            DeviceRgb color1 = new DeviceRgb(0, 0, 0);
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                ColorContrastCalculator.ContrastRatio(color1, null);
            }
            );
        }
    }
}

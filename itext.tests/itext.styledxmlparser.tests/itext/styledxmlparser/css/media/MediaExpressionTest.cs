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

namespace iText.StyledXmlParser.Css.Media {
    [NUnit.Framework.Category("UnitTest")]
    public class MediaExpressionTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void MediaExpressionTestTest01() {
            MediaExpression minWidth = new MediaExpression("min-width", "600px");
            MediaExpression minHeight = new MediaExpression("min-height", "600px");
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            deviceDescription.SetWidth(600);
            deviceDescription.SetHeight(600);
            NUnit.Framework.Assert.IsTrue(minHeight.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(minWidth.Matches(deviceDescription));
        }

        [NUnit.Framework.Test]
        public virtual void MediaExpressionTestTest02() {
            MediaExpression maxWidth = new MediaExpression("max-width", "600px");
            MediaExpression maxHeight = new MediaExpression("max-height", "600px");
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            deviceDescription.SetWidth(450);
            deviceDescription.SetHeight(450);
            NUnit.Framework.Assert.IsTrue(maxHeight.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(maxWidth.Matches(deviceDescription));
        }

        [NUnit.Framework.Test]
        public virtual void MediaExpressionTestTest03() {
            MediaExpression orientation = new MediaExpression("orientation", "landscape");
            MediaExpression resolution = new MediaExpression("resolution", "150dpi");
            MediaExpression grid = new MediaExpression("grid", "0");
            MediaExpression colorIndex = new MediaExpression("max-color-index", "15000");
            MediaExpression monochrome = new MediaExpression("monochrome", "0");
            MediaExpression scan = new MediaExpression("scan", "interlace");
            MediaExpression color = new MediaExpression("color", "15000");
            MediaExpression minAspectRatio = new MediaExpression("max-aspect-ratio", "8/5");
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            deviceDescription.SetOrientation("landscape");
            deviceDescription.SetResolution(150);
            deviceDescription.SetGrid(false);
            deviceDescription.SetColorIndex(15000);
            deviceDescription.SetMonochrome(0);
            deviceDescription.SetScan("interlace");
            deviceDescription.SetBitsPerComponent(15000);
            deviceDescription.SetWidth(32);
            deviceDescription.SetHeight(20);
            NUnit.Framework.Assert.IsTrue(resolution.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(orientation.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(grid.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(colorIndex.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(monochrome.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(scan.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(color.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(minAspectRatio.Matches(deviceDescription));
        }

        [NUnit.Framework.Test]
        public virtual void MediaExpressionTestTest04() {
            MediaExpression minColorIndex = new MediaExpression("min-color-index", "15000");
            MediaExpression minResolution = new MediaExpression("min-resolution", "150dpi");
            MediaExpression minColor = new MediaExpression("min-color", "8");
            MediaExpression minAspectRatio = new MediaExpression("min-aspect-ratio", "8/5");
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            deviceDescription.SetColorIndex(15000);
            deviceDescription.SetBitsPerComponent(8);
            deviceDescription.SetResolution(150);
            deviceDescription.SetWidth(32);
            deviceDescription.SetHeight(20);
            NUnit.Framework.Assert.IsTrue(minAspectRatio.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(minColorIndex.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(minColor.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(minResolution.Matches(deviceDescription));
        }

        [NUnit.Framework.Test]
        public virtual void MediaExpressionTestTest05() {
            MediaExpression maxColorIndex = new MediaExpression("max-color-index", null);
            MediaExpression maxColor = new MediaExpression("max-color", null);
            MediaExpression maxWidth = new MediaExpression("width", "600ex");
            MediaExpression maxHeight = new MediaExpression("height", "600ex");
            MediaExpression maxMonochrome = new MediaExpression("max-monochrome", "0");
            MediaExpression maxResolution = new MediaExpression("max-resolution", "150dpi");
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            deviceDescription.SetHeight(450);
            deviceDescription.SetWidth(450);
            deviceDescription.SetColorIndex(15000);
            deviceDescription.SetBitsPerComponent(8);
            deviceDescription.SetMonochrome(0);
            deviceDescription.SetResolution(150);
            NUnit.Framework.Assert.IsTrue(maxMonochrome.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(maxHeight.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(maxWidth.Matches(deviceDescription));
            NUnit.Framework.Assert.IsFalse(maxColorIndex.Matches(deviceDescription));
            NUnit.Framework.Assert.IsFalse(maxColor.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(maxResolution.Matches(deviceDescription));
        }

        [NUnit.Framework.Test]
        public virtual void MediaExpressionTestTest06() {
            MediaExpression minColorIndex = new MediaExpression("min-color-index", null);
            MediaExpression minColor = new MediaExpression("min-color", null);
            MediaExpression colorIndex = new MediaExpression("color-index", "1500");
            MediaExpression minMonochrome = new MediaExpression("min-monochrome", "0");
            MediaExpression resolution = new MediaExpression("resolution", "150dpi");
            MediaExpression defaultExpression = new MediaExpression("none", "none");
            MediaExpression aspectRatio = new MediaExpression("aspect-ratio", "8/8");
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            deviceDescription.SetColorIndex(15000);
            deviceDescription.SetBitsPerComponent(8);
            deviceDescription.SetMonochrome(0);
            deviceDescription.SetWidth(1.99999999f);
            deviceDescription.SetHeight(2.00000000f);
            deviceDescription.SetColorIndex(15000);
            NUnit.Framework.Assert.IsTrue(aspectRatio.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(minMonochrome.Matches(deviceDescription));
            NUnit.Framework.Assert.IsFalse(minColorIndex.Matches(deviceDescription));
            NUnit.Framework.Assert.IsFalse(minColor.Matches(deviceDescription));
            NUnit.Framework.Assert.IsFalse(resolution.Matches(deviceDescription));
            NUnit.Framework.Assert.IsFalse(defaultExpression.Matches(deviceDescription));
            NUnit.Framework.Assert.IsFalse(colorIndex.Matches(deviceDescription));
        }
    }
}

/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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

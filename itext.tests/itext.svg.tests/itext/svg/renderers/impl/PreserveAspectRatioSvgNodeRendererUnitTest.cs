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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Svg;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class PreserveAspectRatioSvgNodeRendererUnitTest : ExtendedITextTest {
        private static readonly Rectangle VIEWPORT_VALUE = PageSize.DEFAULT;

        private static readonly Rectangle VIEWBOX_VALUE = new Rectangle(0, 0, 300, 400);

        [NUnit.Framework.Test]
        public virtual void RetrieveAlignAndMeetXMinYMinMeet() {
            String align = SvgConstants.Values.XMIN_YMIN;
            String meet = SvgConstants.Values.MEET;
            String[] cmpAlignAndMeet = new String[] { align, meet };
            String[] outAlignAndMeet = RetrieveAlignAndMeet(align, meet);
            NUnit.Framework.Assert.AreEqual(cmpAlignAndMeet, outAlignAndMeet);
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveAlignAndMeetXMinYMinSlice() {
            String align = SvgConstants.Values.XMIN_YMIN;
            String meet = SvgConstants.Values.SLICE;
            String[] cmpAlignAndMeet = new String[] { align, meet };
            String[] outAlignAndMeet = RetrieveAlignAndMeet(align, meet);
            NUnit.Framework.Assert.AreEqual(cmpAlignAndMeet, outAlignAndMeet);
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveAlignAndMeetXMinYMinNone() {
            String align = SvgConstants.Values.XMIN_YMIN;
            String meet = SvgConstants.Values.MEET;
            String[] cmpAlignAndMeet = new String[] { align, meet };
            String[] outAlignAndMeet = RetrieveAlignAndMeet(align, "");
            NUnit.Framework.Assert.AreEqual(cmpAlignAndMeet, outAlignAndMeet);
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveAlignAndMeetEmptyMeet() {
            String align = SvgConstants.Values.DEFAULT_ASPECT_RATIO;
            String meet = SvgConstants.Values.MEET;
            String[] cmpAlignAndMeet = new String[] { align, meet };
            String[] outAlignAndMeet = RetrieveAlignAndMeet("", meet);
            //should fail, because align attribute must be present
            NUnit.Framework.Assert.IsFalse(JavaUtil.ArraysEquals(cmpAlignAndMeet, outAlignAndMeet));
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveAlignAndMeetEmptySlice() {
            String align = SvgConstants.Values.DEFAULT_ASPECT_RATIO;
            String meet = SvgConstants.Values.SLICE;
            String[] cmpAlignAndMeet = new String[] { align, meet };
            String[] outAlignAndMeet = RetrieveAlignAndMeet("", meet);
            //should fail, because align attribute must be present
            NUnit.Framework.Assert.IsFalse(JavaUtil.ArraysEquals(cmpAlignAndMeet, outAlignAndMeet));
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveAlignAndMeetNoneMeet() {
            String align = SvgConstants.Values.NONE;
            String meet = SvgConstants.Values.MEET;
            String[] cmpAlignAndMeet = new String[] { align, meet };
            String[] outAlignAndMeet = RetrieveAlignAndMeet(align, meet);
            NUnit.Framework.Assert.AreEqual(cmpAlignAndMeet, outAlignAndMeet);
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveAlignAndMeetNoneSlice() {
            String align = SvgConstants.Values.NONE;
            String meet = SvgConstants.Values.SLICE;
            String[] cmpAlignAndMeet = new String[] { align, meet };
            String[] outAlignAndMeet = RetrieveAlignAndMeet(align, meet);
            NUnit.Framework.Assert.AreEqual(cmpAlignAndMeet, outAlignAndMeet);
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveAlignAndMeetAllDefault() {
            String align = SvgConstants.Values.DEFAULT_ASPECT_RATIO;
            String meet = SvgConstants.Values.MEET;
            String[] cmpAlignAndMeet = new String[] { align, meet };
            String[] outAlignAndMeet = RetrieveAlignAndMeet("", "");
            NUnit.Framework.Assert.AreEqual(cmpAlignAndMeet, outAlignAndMeet);
        }

        private String[] RetrieveAlignAndMeet(String align, String meet) {
            AbstractBranchSvgNodeRenderer renderer = new SvgTagSvgNodeRenderer();
            IDictionary<String, String> attributesAndStyles = new Dictionary<String, String>();
            if (!"".Equals(align) || !"".Equals(meet)) {
                attributesAndStyles.Put(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO, align + " " + meet);
            }
            renderer.SetAttributesAndStyles(attributesAndStyles);
            return renderer.RetrieveAlignAndMeet();
        }
    }
}

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
using System.Collections.Generic;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class StopSvgNodeRendererUnitTest : ExtendedITextTest {
        private const float DELTA = 0;

        [NUnit.Framework.Test]
        public virtual void GetOffsetPercentageValueTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Attributes.OFFSET, "50%");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            double expected = 0.5;
            NUnit.Framework.Assert.AreEqual(expected, renderer.GetOffset(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetOffsetNumericValueTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Attributes.OFFSET, "0.5");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            double expected = 0.5;
            NUnit.Framework.Assert.AreEqual(expected, renderer.GetOffset(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetOffsetMoreThanOneValueTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Attributes.OFFSET, "2");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            double expected = 1;
            NUnit.Framework.Assert.AreEqual(expected, renderer.GetOffset(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetOffsetLessThanZeroValueTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Attributes.OFFSET, "-2");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            double expected = 0;
            NUnit.Framework.Assert.AreEqual(expected, renderer.GetOffset(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetOffsetNoneValueTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Attributes.OFFSET, null);
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            double expected = 0;
            NUnit.Framework.Assert.AreEqual(expected, renderer.GetOffset(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetOffsetRandomStringValueTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Attributes.OFFSET, "Hello");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            double expected = 0;
            NUnit.Framework.Assert.AreEqual(expected, renderer.GetOffset(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetOffsetEmptyStringValueTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Attributes.OFFSET, "");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            double expected = 0;
            NUnit.Framework.Assert.AreEqual(expected, renderer.GetOffset(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetStopColorTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Tags.STOP_COLOR, "red");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            float[] expected = new float[] { 1, 0, 0, 1 };
            float[] actual = renderer.GetStopColor();
            iText.Test.TestUtil.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetStopColorNoneValueTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Tags.STOP_COLOR, null);
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            float[] expected = new float[] { 0, 0, 0, 1 };
            float[] actual = renderer.GetStopColor();
            iText.Test.TestUtil.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetStopColorRandomStringValueTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Tags.STOP_COLOR, "Hello");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            float[] expected = new float[] { 0, 0, 0, 1 };
            float[] actual = renderer.GetStopColor();
            iText.Test.TestUtil.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetStopColorEmptyStringTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Tags.STOP_COLOR, "");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            float[] expected = new float[] { 0, 0, 0, 1 };
            float[] actual = renderer.GetStopColor();
            iText.Test.TestUtil.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetStopColorOpacityTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Tags.STOP_COLOR, "rgba(0.5, 0.5, 0.5, 0.5)");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            float[] expected = new float[] { 0, 0, 0, 1 };
            float[] actual = renderer.GetStopColor();
            iText.Test.TestUtil.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetStopOpacityTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Tags.STOP_OPACITY, "1");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            float expected = 1;
            NUnit.Framework.Assert.AreEqual(expected, renderer.GetStopOpacity(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetStopOpacityNoneValueTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Tags.STOP_OPACITY, null);
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            float expected = 1;
            NUnit.Framework.Assert.AreEqual(expected, renderer.GetStopOpacity(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetStopOpacityRandomStringValueTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Tags.STOP_OPACITY, "Hello");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            float expected = 1;
            NUnit.Framework.Assert.AreEqual(expected, renderer.GetStopOpacity(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetStopOpacityEmptyStringTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Tags.STOP_OPACITY, "");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            float expected = 1;
            NUnit.Framework.Assert.AreEqual(expected, renderer.GetStopOpacity(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void CreateDeepCopyTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put(SvgConstants.Attributes.OFFSET, "0.5");
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            renderer.SetAttributesAndStyles(styles);
            ISvgNodeRenderer copy = renderer.CreateDeepCopy();
            NUnit.Framework.Assert.AreNotSame(renderer, copy);
            NUnit.Framework.Assert.AreEqual(renderer.GetType(), copy.GetType());
            NUnit.Framework.Assert.AreEqual(renderer.GetAttributeMapCopy(), copy.GetAttributeMapCopy());
        }

        [NUnit.Framework.Test]
        public virtual void DoDrawTest() {
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            Exception e = NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => renderer.DoDraw(new SvgDrawContext
                (null, null)));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.DRAW_NO_DRAW, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NoObjectBoundingBoxTest() {
            StopSvgNodeRenderer renderer = new StopSvgNodeRenderer();
            NUnit.Framework.Assert.IsNull(renderer.GetObjectBoundingBox(null));
        }
    }
}

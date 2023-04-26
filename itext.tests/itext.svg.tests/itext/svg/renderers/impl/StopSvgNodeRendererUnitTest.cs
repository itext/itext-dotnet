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

/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Geom;
using iText.Test;

namespace iText.Layout.Properties {
    [NUnit.Framework.Category("UnitTest")]
    public class TransformTest : ExtendedITextTest {
        // AffineTransform.TYPE_UNKNOWN
        private const float type = -1;

        [NUnit.Framework.Test]
        public virtual void CreateDefaultSingleTransformTest() {
            Transform.SingleTransform defaultSingleTransform = new Transform.SingleTransform();
            UnitValue[] defaultUnitValues = defaultSingleTransform.GetUnitValues();
            iText.Test.TestUtil.AreEqual(new float[] { 1f, 0f, 0f, 1f }, defaultSingleTransform.GetFloats(), 0);
            NUnit.Framework.Assert.AreEqual(2, defaultUnitValues.Length);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(0), defaultUnitValues[0]);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(0), defaultUnitValues[1]);
        }

        [NUnit.Framework.Test]
        public virtual void GetAffineTransformPercentPointUnitValuesTest() {
            NUnit.Framework.Assert.AreEqual(new AffineTransform(new float[] { -2f, 3f, -4f, -5f, 12f, 30f, type }), GetAffineTransform
                (UnitValue.PERCENT, UnitValue.POINT));
        }

        [NUnit.Framework.Test]
        public virtual void GetAffineTransformPointPercentUnitValuesTest() {
            NUnit.Framework.Assert.AreEqual(new AffineTransform(new float[] { -2f, 3f, -4f, -5f, 20f, 24f, type }), GetAffineTransform
                (UnitValue.POINT, UnitValue.PERCENT));
        }

        [NUnit.Framework.Test]
        public virtual void GetAffineTransformPercentPercentUnitValuesTest() {
            NUnit.Framework.Assert.AreEqual(new AffineTransform(new float[] { -2f, 3f, -4f, -5f, 12f, 24f, type }), GetAffineTransform
                (UnitValue.PERCENT, UnitValue.PERCENT));
        }

        [NUnit.Framework.Test]
        public virtual void GetAffineTransformPointPointUnitValuesTest() {
            NUnit.Framework.Assert.AreEqual(new AffineTransform(new float[] { -2f, 3f, -4f, -5f, 20f, 30f, type }), GetAffineTransform
                (UnitValue.POINT, UnitValue.POINT));
        }

        [NUnit.Framework.Test]
        public virtual void GetAffineTransformDiffSingleTransformTest() {
            float txUnitValue = 20f;
            float tyUnitValue2 = 30f;
            Transform transform = new Transform(4);
            transform.AddSingleTransform(CreateSingleTransform(UnitValue.CreatePercentValue(txUnitValue), UnitValue.CreatePointValue
                (tyUnitValue2)));
            transform.AddSingleTransform(CreateSingleTransform(UnitValue.CreatePointValue(txUnitValue), UnitValue.CreatePercentValue
                (tyUnitValue2)));
            transform.AddSingleTransform(CreateSingleTransform(UnitValue.CreatePercentValue(txUnitValue), UnitValue.CreatePercentValue
                (tyUnitValue2)));
            transform.AddSingleTransform(CreateSingleTransform(UnitValue.CreatePointValue(txUnitValue), UnitValue.CreatePointValue
                (tyUnitValue2)));
            NUnit.Framework.Assert.AreEqual(new AffineTransform(new float[] { -524f, -105f, 140f, -419f, -788f, 2220f, 
                type }), Transform.GetAffineTransform(transform, 60f, 80f));
        }

        [NUnit.Framework.Test]
        public virtual void GetAffineTransformOneSingleTransformFewTimesTest() {
            Transform transform = new Transform(4);
            Transform.SingleTransform singleTransform = CreateSingleTransform(UnitValue.CreatePointValue(20f), UnitValue
                .CreatePointValue(30f));
            transform.AddSingleTransform(singleTransform);
            transform.AddSingleTransform(singleTransform);
            transform.AddSingleTransform(singleTransform);
            transform.AddSingleTransform(singleTransform);
            NUnit.Framework.Assert.AreEqual(new AffineTransform(new float[] { -524f, -105f, 140f, -419f, -700f, 2100f, 
                type }), Transform.GetAffineTransform(transform, 60f, 60f));
        }

        [NUnit.Framework.Test]
        public virtual void GetAffineTransformDifferentWidthHeightTest() {
            Transform transform = new Transform(1);
            transform.AddSingleTransform(CreateSingleTransform(UnitValue.CreatePercentValue(20f), UnitValue.CreatePercentValue
                (30f)));
            NUnit.Framework.Assert.AreEqual(new AffineTransform(new float[] { -2f, 3f, -4f, -5f, -10f, -6f, type }), Transform
                .GetAffineTransform(transform, -50f, -20f));
            NUnit.Framework.Assert.AreEqual(new AffineTransform(new float[] { -2f, 3f, -4f, -5f, 10f, -6f, type }), Transform
                .GetAffineTransform(transform, 50f, -20f));
            NUnit.Framework.Assert.AreEqual(new AffineTransform(new float[] { -2f, 3f, -4f, -5f, -10f, 6f, type }), Transform
                .GetAffineTransform(transform, -50f, 20f));
            NUnit.Framework.Assert.AreEqual(new AffineTransform(new float[] { -2f, 3f, -4f, -5f, 10f, 6f, type }), Transform
                .GetAffineTransform(transform, 50f, 20f));
        }

        private static AffineTransform GetAffineTransform(int txUnitValueType, int tyUnitValueType) {
            float txUnitValue = 20f;
            float tyUnitValue = 30f;
            float width = 60f;
            float height = 80f;
            // create Transform
            Transform transform = new Transform(1);
            transform.AddSingleTransform(CreateSingleTransform(new UnitValue(txUnitValueType, txUnitValue), new UnitValue
                (tyUnitValueType, tyUnitValue)));
            // get AffineTransform
            return Transform.GetAffineTransform(transform, width, height);
        }

        private static Transform.SingleTransform CreateSingleTransform(UnitValue xUnitVal, UnitValue yUnitVal) {
            float a = -2f;
            float b = 3f;
            float c = -4f;
            float d = -5f;
            return new Transform.SingleTransform(a, b, c, d, xUnitVal, yUnitVal);
        }
    }
}

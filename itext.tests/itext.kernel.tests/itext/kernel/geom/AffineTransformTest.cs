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
using iText.Test;

namespace iText.Kernel.Geom {
    [NUnit.Framework.Category("UnitTest")]
    public class AffineTransformTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SelfTest() {
            AffineTransform affineTransform = new AffineTransform();
            NUnit.Framework.Assert.IsTrue(affineTransform.Equals(affineTransform));
        }

        [NUnit.Framework.Test]
        public virtual void NullTest() {
            AffineTransform affineTransform = new AffineTransform();
            NUnit.Framework.Assert.IsFalse(affineTransform.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void OtherClassTest() {
            AffineTransform affineTransform = new AffineTransform();
            String @string = "Test";
            NUnit.Framework.Assert.IsFalse(affineTransform.Equals(@string));
        }

        [NUnit.Framework.Test]
        public virtual void SameValuesTest() {
            AffineTransform affineTransform1 = new AffineTransform(0d, 1d, 2d, 3d, 4d, 5d);
            AffineTransform affineTransform2 = new AffineTransform(0d, 1d, 2d, 3d, 4d, 5d);
            int hash1 = affineTransform1.GetHashCode();
            int hash2 = affineTransform2.GetHashCode();
            NUnit.Framework.Assert.IsFalse(affineTransform1 == affineTransform2);
            NUnit.Framework.Assert.AreEqual(hash1, hash2);
            NUnit.Framework.Assert.IsTrue(affineTransform1.Equals(affineTransform2));
        }

        [NUnit.Framework.Test]
        public virtual void DifferentValuesTest() {
            AffineTransform affineTransform1 = new AffineTransform(0d, 1d, 2d, 3d, 4d, 5d);
            AffineTransform affineTransform2 = new AffineTransform(5d, 4d, 3d, 2d, 1d, 1d);
            int hash1 = affineTransform1.GetHashCode();
            int hash2 = affineTransform2.GetHashCode();
            NUnit.Framework.Assert.IsFalse(affineTransform1 == affineTransform2);
            NUnit.Framework.Assert.AreNotEqual(hash1, hash2);
            NUnit.Framework.Assert.IsFalse(affineTransform1.Equals(affineTransform2));
        }

        [NUnit.Framework.Test]
        public virtual void GetRotateInstanceTest() {
            AffineTransform rotateOne = AffineTransform.GetRotateInstance(Math.PI / 2);
            AffineTransform expected = new AffineTransform(0, 1, -1, 0, 0, 0);
            NUnit.Framework.Assert.AreEqual(rotateOne, expected);
        }

        [NUnit.Framework.Test]
        public virtual void GetRotateInstanceTranslateTest() {
            AffineTransform rotateTranslate = AffineTransform.GetRotateInstance(Math.PI / 2, 10, 5);
            AffineTransform expected = new AffineTransform(0, 1, -1, 0, 15, -5);
            NUnit.Framework.Assert.AreEqual(rotateTranslate, expected);
        }

        [NUnit.Framework.Test]
        public virtual void CloneTest() {
            AffineTransform original = new AffineTransform();
            AffineTransform clone = original.Clone();
            NUnit.Framework.Assert.IsTrue(original != clone);
            NUnit.Framework.Assert.IsTrue(original.Equals(clone));
        }

        [NUnit.Framework.Test]
        public virtual void GetTransformValuesTest() {
            float[] matrix = new float[] { 0f, 1f, 2f, 3f, 4f, 5f };
            AffineTransform affineTransform = new AffineTransform(matrix);
            NUnit.Framework.Assert.AreEqual(matrix[0], affineTransform.GetScaleX(), 0.0);
            NUnit.Framework.Assert.AreEqual(matrix[3], affineTransform.GetScaleY(), 0.0);
            NUnit.Framework.Assert.AreEqual(matrix[2], affineTransform.GetShearX(), 0.0);
            NUnit.Framework.Assert.AreEqual(matrix[1], affineTransform.GetShearY(), 0.0);
            NUnit.Framework.Assert.AreEqual(matrix[4], affineTransform.GetTranslateX(), 0.0);
            NUnit.Framework.Assert.AreEqual(matrix[5], affineTransform.GetTranslateY(), 0.0);
            NUnit.Framework.Assert.AreEqual(32, affineTransform.GetTransformType(), 0.0);
        }

        [NUnit.Framework.Test]
        public virtual void CreateAffineTransformFromOtherATTest() {
            AffineTransform template = new AffineTransform(0, 1, 2, 3, 4, 5);
            AffineTransform result = new AffineTransform(template);
            NUnit.Framework.Assert.AreNotSame(template, result);
            NUnit.Framework.Assert.AreEqual(template, result);
        }

        [NUnit.Framework.Test]
        public virtual void CreateAffineTransformFromFloatArrayTest() {
            float[] matrix = new float[] { 0f, 1f, 2f, 3f, 4f, 5f };
            AffineTransform expected = new AffineTransform(matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix
                [5]);
            AffineTransform result = new AffineTransform(matrix);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void CreateAffineTransformFromDoubleArrayTest() {
            double[] matrix = new double[] { 0d, 1d, 2d, 3d, 4d, 5d };
            AffineTransform expected = new AffineTransform(matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix
                [5]);
            AffineTransform result = new AffineTransform(matrix);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SetTransformTest() {
            float[] matrix = new float[] { 0f, 1f, 2f, 3f, 4f, 5f };
            AffineTransform expected = new AffineTransform(matrix);
            AffineTransform result = new AffineTransform();
            result.SetTransform(matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SetToIdentityTest() {
            AffineTransform expected = new AffineTransform(1, 0, 0, 1, 0, 0);
            AffineTransform result = new AffineTransform();
            result.SetToIdentity();
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SetToShearTypeIdentityTest() {
            double shx = 0d;
            double shy = 0d;
            AffineTransform expected = new AffineTransform(1, shx, shy, 1, 0, 0);
            AffineTransform result = new AffineTransform();
            result.SetToShear(shx, shy);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SetToShearTypeUnknownTest() {
            double shx = 1d;
            double shy = 1d;
            AffineTransform expected = new AffineTransform(1, shx, shy, 1, 0, 0);
            AffineTransform result = new AffineTransform();
            result.SetToShear(shx, shy);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void GetShearInstanceTest() {
            double shx = 1d;
            double shy = 1d;
            AffineTransform expected = new AffineTransform(1, shx, shy, 1, 0, 0);
            AffineTransform result = AffineTransform.GetShearInstance(shx, shy);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void ShearTest() {
            double shx = 1d;
            double shy = 1d;
            AffineTransform expected = new AffineTransform(4d, 6d, 4d, 6d, 5d, 6d);
            AffineTransform result = new AffineTransform(1d, 2d, 3d, 4d, 5d, 6d);
            result.Shear(shx, shy);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void RotateTest() {
            double angle = Math.PI / 2;
            AffineTransform expected = new AffineTransform(3d, 4d, -1d, -2d, 5d, 6d);
            AffineTransform result = new AffineTransform(1d, 2d, 3d, 4d, 5d, 6d);
            result.Rotate(angle);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void PreConcatenateTest() {
            AffineTransform expected = new AffineTransform(6d, 6d, 14d, 14d, 24d, 24d);
            AffineTransform result = new AffineTransform(1d, 2d, 3d, 4d, 5d, 6d);
            AffineTransform template = new AffineTransform(2d, 2d, 2d, 2d, 2d, 2d);
            result.PreConcatenate(template);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void TransformDoubleArrayTest() {
            AffineTransform affineTransform = new AffineTransform(1d, 2d, 3d, 4d, 5d, 6d);
            double[] expected = new double[] { 0d, 13d, 18d, 13d, 18d, 0d };
            double[] src = new double[] { 2d, 2d, 2d, 2d, 2d, 2d };
            double[] dest = new double[6];
            affineTransform.Transform(src, 1, dest, 1, 2);
            iText.Test.TestUtil.AreEqual(expected, dest, 0);
        }

        [NUnit.Framework.Test]
        public virtual void TransformDoubleArraySourceDestEqualsTest() {
            AffineTransform affineTransform = new AffineTransform(1d, 2d, 3d, 4d, 5d, 6d);
            double[] expected = new double[] { 2d, 2d, 13d, 18d, 13d, 18d };
            double[] src = new double[] { 2d, 2d, 2d, 2d, 2d, 2d };
            affineTransform.Transform(src, 1, src, 2, 2);
            iText.Test.TestUtil.AreEqual(expected, src, 0);
        }

        [NUnit.Framework.Test]
        public virtual void TransformFloatArrayTest() {
            AffineTransform affineTransform = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f);
            float[] expected = new float[] { 0f, 13f, 18f, 13f, 18f, 0f };
            float[] src = new float[] { 2f, 2f, 2f, 2f, 2f, 2f };
            float[] dest = new float[6];
            affineTransform.Transform(src, 1, dest, 1, 2);
            iText.Test.TestUtil.AreEqual(expected, dest, 0);
        }

        [NUnit.Framework.Test]
        public virtual void TransformFloatArraySourceDestEqualsTest() {
            AffineTransform affineTransform = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f);
            float[] expected = new float[] { 2f, 2f, 13f, 18f, 13f, 18f };
            float[] src = new float[] { 2f, 2f, 2f, 2f, 2f, 2f };
            affineTransform.Transform(src, 1, src, 2, 2);
            iText.Test.TestUtil.AreEqual(expected, src, 0);
        }

        [NUnit.Framework.Test]
        public virtual void TransformFloatToDoubleTest() {
            AffineTransform affineTransform = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f);
            double[] expected = new double[] { 0d, 13d, 18d, 13d, 18d, 0d };
            float[] src = new float[] { 2f, 2f, 2f, 2f, 2f, 2f };
            double[] dest = new double[6];
            affineTransform.Transform(src, 1, dest, 1, 2);
            iText.Test.TestUtil.AreEqual(expected, dest, 0);
        }

        [NUnit.Framework.Test]
        public virtual void TransformDoubleToFloatTest() {
            AffineTransform affineTransform = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f);
            float[] expected = new float[] { 0f, 13f, 18f, 13f, 18f, 0f };
            double[] src = new double[] { 2d, 2d, 2d, 2d, 2d, 2d };
            float[] dest = new float[6];
            affineTransform.Transform(src, 1, dest, 1, 2);
            iText.Test.TestUtil.AreEqual(expected, dest, 0);
        }

        [NUnit.Framework.Test]
        public virtual void DeltaTransformPointTest() {
            AffineTransform affineTransform = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f);
            Point src = new Point(2, 2);
            Point dest = new Point();
            Point expected = new Point(8, 12);
            affineTransform.DeltaTransform(src, dest);
            NUnit.Framework.Assert.AreEqual(expected, dest);
        }

        [NUnit.Framework.Test]
        public virtual void DeltaTransformPointNullDestTest() {
            AffineTransform affineTransform = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f);
            Point src = new Point(2, 2);
            Point expected = new Point(8, 12);
            Point dest = affineTransform.DeltaTransform(src, null);
            NUnit.Framework.Assert.AreEqual(expected, dest);
        }

        [NUnit.Framework.Test]
        public virtual void DeltaTransformDoubleArrayTest() {
            AffineTransform affineTransform = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f);
            double[] expected = new double[] { 0d, 8d, 12d, 8d, 12d, 0d };
            double[] src = new double[] { 2d, 2d, 2d, 2d, 2d, 2d };
            double[] dest = new double[6];
            affineTransform.DeltaTransform(src, 1, dest, 1, 2);
            iText.Test.TestUtil.AreEqual(expected, dest, 0);
        }

        [NUnit.Framework.Test]
        public virtual void InverseTransformPointTest() {
            AffineTransform affineTransform = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f);
            Point src = new Point(2, 2);
            Point dest = new Point();
            Point expected = new Point(0, -1);
            affineTransform.InverseTransform(src, dest);
            NUnit.Framework.Assert.AreEqual(expected, dest);
        }

        [NUnit.Framework.Test]
        public virtual void InverseTransformPointNullTest() {
            AffineTransform affineTransform = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f);
            Point src = new Point(2, 2);
            Point expected = new Point(0, -1);
            Point dest = affineTransform.InverseTransform(src, null);
            NUnit.Framework.Assert.AreEqual(expected, dest);
        }

        [NUnit.Framework.Test]
        public virtual void InverseTransformDoubleArrayTest() {
            AffineTransform affineTransform = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f);
            double[] expected = new double[] { 0d, -0d, -1d, -0d, -1d, 0d };
            double[] src = new double[] { 2d, 2d, 2d, 2d, 2d, 2d };
            double[] dest = new double[6];
            affineTransform.InverseTransform(src, 1, dest, 1, 2);
            iText.Test.TestUtil.AreEqual(expected, dest, 0);
        }

        [NUnit.Framework.Test]
        public virtual void InverseTransformFloatArrayTest() {
            AffineTransform affineTransform = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f);
            float[] expected = new float[] { 0f, -0f, -1f, -0f, -1f, 0f };
            float[] src = new float[] { 2f, 2f, 2f, 2f, 2f, 2f };
            float[] dest = new float[6];
            affineTransform.InverseTransform(src, 1, dest, 1, 2);
            iText.Test.TestUtil.AreEqual(expected, dest, 0);
        }
    }
}

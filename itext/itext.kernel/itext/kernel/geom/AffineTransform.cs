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
using iText.Commons.Utils;

namespace iText.Kernel.Geom {
    /// <summary>
    /// The
    /// <see cref="AffineTransform"/>
    /// class represents an affine transformation,
    /// which is a combination of linear transformations such as translation,
    /// scaling, rotation, and shearing which allows preservation of the straightness of lines.
    /// </summary>
    public class AffineTransform {
        /// <summary>The type of affine transformation.</summary>
        /// <remarks>
        /// The type of affine transformation. See
        /// <see cref="GetTransformType()"/>.
        /// </remarks>
        public const int TYPE_IDENTITY = 0;

        /// <summary>The type of affine transformation.</summary>
        /// <remarks>
        /// The type of affine transformation. See
        /// <see cref="GetTransformType()"/>.
        /// </remarks>
        public const int TYPE_TRANSLATION = 1;

        /// <summary>The type of affine transformation.</summary>
        /// <remarks>
        /// The type of affine transformation. See
        /// <see cref="GetTransformType()"/>.
        /// </remarks>
        public const int TYPE_UNIFORM_SCALE = 2;

        /// <summary>The type of affine transformation.</summary>
        /// <remarks>
        /// The type of affine transformation. See
        /// <see cref="GetTransformType()"/>.
        /// </remarks>
        public const int TYPE_GENERAL_SCALE = 4;

        /// <summary>The type of affine transformation.</summary>
        /// <remarks>
        /// The type of affine transformation. See
        /// <see cref="GetTransformType()"/>.
        /// </remarks>
        public const int TYPE_QUADRANT_ROTATION = 8;

        /// <summary>The type of affine transformation.</summary>
        /// <remarks>
        /// The type of affine transformation. See
        /// <see cref="GetTransformType()"/>.
        /// </remarks>
        public const int TYPE_GENERAL_ROTATION = 16;

        /// <summary>The type of affine transformation.</summary>
        /// <remarks>
        /// The type of affine transformation. See
        /// <see cref="GetTransformType()"/>.
        /// </remarks>
        public const int TYPE_GENERAL_TRANSFORM = 32;

        /// <summary>The type of affine transformation.</summary>
        /// <remarks>
        /// The type of affine transformation. See
        /// <see cref="GetTransformType()"/>.
        /// </remarks>
        public const int TYPE_FLIP = 64;

        /// <summary>The type of affine transformation.</summary>
        /// <remarks>
        /// The type of affine transformation. See
        /// <see cref="GetTransformType()"/>.
        /// </remarks>
        public const int TYPE_MASK_SCALE = TYPE_UNIFORM_SCALE | TYPE_GENERAL_SCALE;

        /// <summary>The type of affine transformation.</summary>
        /// <remarks>
        /// The type of affine transformation. See
        /// <see cref="GetTransformType()"/>.
        /// </remarks>
        public const int TYPE_MASK_ROTATION = TYPE_QUADRANT_ROTATION | TYPE_GENERAL_ROTATION;

//\cond DO_NOT_DOCUMENT
        /// <summary>The <c>TYPE_UNKNOWN</c> is an initial type value.</summary>
        internal const int TYPE_UNKNOWN = -1;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The min value equivalent to zero.</summary>
        /// <remarks>The min value equivalent to zero. If absolute value less then ZERO it considered as zero.</remarks>
        internal const double ZERO = 1E-10;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The values of transformation matrix</summary>
        internal double m00;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal double m10;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal double m01;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal double m11;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal double m02;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal double m12;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>The transformation <c>type</c></summary>
        internal int type;
//\endcond

        /// <summary>
        /// Create an empty
        /// <see cref="AffineTransform"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Create an empty
        /// <see cref="AffineTransform"/>
        /// instance.
        /// The default type is for the transformation is
        /// <c>TYPE_IDENTITY</c>
        /// </remarks>
        public AffineTransform() {
            type = TYPE_IDENTITY;
            m00 = m11 = 1;
            m10 = m01 = m02 = m12 = 0;
        }

        /// <summary>
        /// Will create a new
        /// <see cref="AffineTransform"/>
        /// instance with the values provided from the original
        /// <see cref="AffineTransform"/>
        /// instance.
        /// </summary>
        /// <param name="t">The AffineTransform class to be used.</param>
        public AffineTransform(iText.Kernel.Geom.AffineTransform t) {
            this.type = t.type;
            this.m00 = t.m00;
            this.m10 = t.m10;
            this.m01 = t.m01;
            this.m11 = t.m11;
            this.m02 = t.m02;
            this.m12 = t.m12;
        }

        /// <summary>
        /// Create an
        /// <see cref="AffineTransform"/>
        /// instance with the values provided.
        /// </summary>
        /// <remarks>
        /// Create an
        /// <see cref="AffineTransform"/>
        /// instance with the values provided.
        /// The default type is for the transformation is
        /// <c>TYPE_UNKNOWN</c>
        /// </remarks>
        /// <param name="m00">The value of the first row and first column of the matrix.</param>
        /// <param name="m10">The value of the second row and first column of the matrix.</param>
        /// <param name="m01">The value of the first row and second column of the matrix.</param>
        /// <param name="m11">The value of the second row and second column of the matrix.</param>
        /// <param name="m02">The value of the first row and third column of the matrix.</param>
        /// <param name="m12">The value of the second row and third column of the matrix.</param>
        public AffineTransform(double m00, double m10, double m01, double m11, double m02, double m12) {
            this.type = TYPE_UNKNOWN;
            this.m00 = m00;
            this.m10 = m10;
            this.m01 = m01;
            this.m11 = m11;
            this.m02 = m02;
            this.m12 = m12;
        }

        /// <summary>
        /// Create an
        /// <see cref="AffineTransform"/>
        /// instance with the values provided.
        /// </summary>
        /// <remarks>
        /// Create an
        /// <see cref="AffineTransform"/>
        /// instance with the values provided.
        /// The default type is for the transformation is
        /// <c>TYPE_UNKNOWN</c>
        /// </remarks>
        /// <param name="matrix">The array of values to be used for the transformation matrix.</param>
        public AffineTransform(float[] matrix) {
            this.type = TYPE_UNKNOWN;
            m00 = matrix[0];
            m10 = matrix[1];
            m01 = matrix[2];
            m11 = matrix[3];
            if (matrix.Length > 4) {
                m02 = matrix[4];
                m12 = matrix[5];
            }
        }

        /// <summary>
        /// Create an
        /// <see cref="AffineTransform"/>
        /// instance with the values provided.
        /// </summary>
        /// <remarks>
        /// Create an
        /// <see cref="AffineTransform"/>
        /// instance with the values provided.
        /// The default type is for the transformation is
        /// <c>TYPE_UNKNOWN</c>
        /// </remarks>
        /// <param name="matrix">The array of values to be used for the transformation matrix.</param>
        public AffineTransform(double[] matrix) {
            this.type = TYPE_UNKNOWN;
            m00 = matrix[0];
            m10 = matrix[1];
            m01 = matrix[2];
            m11 = matrix[3];
            if (matrix.Length > 4) {
                m02 = matrix[4];
                m12 = matrix[5];
            }
        }

        /// <summary>Method returns type of affine transformation.</summary>
        /// <remarks>
        /// Method returns type of affine transformation.
        /// <para />
        /// Transform matrix is
        /// m00 m01 m02
        /// m10 m11 m12
        /// <para />
        /// According analytic geometry new basis vectors are (m00, m01) and (m10, m11),
        /// translation vector is (m02, m12). Original basis vectors are (1, 0) and (0, 1).
        /// Type transformations classification:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="TYPE_IDENTITY"/>
        /// - new basis equals original one and zero translation
        /// </description></item>
        /// <item><description>
        /// <see cref="TYPE_TRANSLATION"/>
        /// - translation vector isn't zero
        /// </description></item>
        /// <item><description>
        /// <see cref="TYPE_UNIFORM_SCALE"/>
        /// - vectors length of new basis equals
        /// </description></item>
        /// <item><description>
        /// <see cref="TYPE_GENERAL_SCALE"/>
        /// - vectors length of new basis doesn't equal
        /// </description></item>
        /// <item><description>
        /// <see cref="TYPE_FLIP"/>
        /// - new basis vector orientation differ from original one
        /// </description></item>
        /// <item><description>
        /// <see cref="TYPE_QUADRANT_ROTATION"/>
        /// - new basis is rotated by 90, 180, 270, or 360 degrees
        /// </description></item>
        /// <item><description>
        /// <see cref="TYPE_GENERAL_ROTATION"/>
        /// - new basis is rotated by arbitrary angle
        /// </description></item>
        /// <item><description>
        /// <see cref="TYPE_GENERAL_TRANSFORM"/>
        /// - transformation can't be inversed
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <returns>the type of this AffineTransform</returns>
        public virtual int GetTransformType() {
            if (this.type != TYPE_UNKNOWN) {
                return this.type;
            }
            int type = 0;
            if (m00 * m01 + m10 * m11 != 0.0) {
                type |= TYPE_GENERAL_TRANSFORM;
                return type;
            }
            if (m02 != 0.0 || m12 != 0.0) {
                type |= TYPE_TRANSLATION;
            }
            else {
                if (m00 == 1.0 && m11 == 1.0 && m01 == 0.0 && m10 == 0.0) {
                    type = TYPE_IDENTITY;
                    return type;
                }
            }
            if (m00 * m11 - m01 * m10 < 0.0) {
                type |= TYPE_FLIP;
            }
            double dx = m00 * m00 + m10 * m10;
            double dy = m01 * m01 + m11 * m11;
            if (dx != dy) {
                type |= TYPE_GENERAL_SCALE;
            }
            else {
                if (dx != 1.0) {
                    type |= TYPE_UNIFORM_SCALE;
                }
            }
            if ((m00 == 0.0 && m11 == 0.0) || (m10 == 0.0 && m01 == 0.0 && (m00 < 0.0 || m11 < 0.0))) {
                type |= TYPE_QUADRANT_ROTATION;
            }
            else {
                if (m01 != 0.0 || m10 != 0.0) {
                    type |= TYPE_GENERAL_ROTATION;
                }
            }
            return type;
        }

        /// <summary>Gets the scale factor of the x-axis.</summary>
        /// <returns>the scale factor of the x-axis.</returns>
        public virtual double GetScaleX() {
            return m00;
        }

        /// <summary>Gets the scale factor of the y-axis.</summary>
        /// <returns>the scale factor of the y-axis.</returns>
        public virtual double GetScaleY() {
            return m11;
        }

        /// <summary>Gets the shear factor of the x-axis.</summary>
        /// <returns>the shear factor of the x-axis.</returns>
        public virtual double GetShearX() {
            return m01;
        }

        /// <summary>Gets the shear factor of the y-axis.</summary>
        /// <returns>the shear factor of the y-axis.</returns>
        public virtual double GetShearY() {
            return m10;
        }

        /// <summary>Gets translation factor of the x-axis.</summary>
        /// <returns>the translation factor of the x-axis.</returns>
        public virtual double GetTranslateX() {
            return m02;
        }

        /// <summary>Gets translation factor of the y-axis.</summary>
        /// <returns>the translation factor of the y-axis.</returns>
        public virtual double GetTranslateY() {
            return m12;
        }

        /// <summary>
        /// Gets whether this
        /// <see cref="AffineTransform"/>
        /// is an identity transformation.
        /// </summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this
        /// <see cref="AffineTransform"/>
        /// is an identity transformation,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool IsIdentity() {
            return GetTransformType() == TYPE_IDENTITY;
        }

        /// <summary>
        /// Fills the matrix parameter with the values of this
        /// <see cref="AffineTransform"/>
        /// instance.
        /// </summary>
        /// <param name="matrix">
        /// the array to be filled with the values of this
        /// <see cref="AffineTransform"/>
        /// instance.
        /// </param>
        public virtual void GetMatrix(float[] matrix) {
            matrix[0] = (float)m00;
            matrix[1] = (float)m10;
            matrix[2] = (float)m01;
            matrix[3] = (float)m11;
            if (matrix.Length > 4) {
                matrix[4] = (float)m02;
                matrix[5] = (float)m12;
            }
        }

        /// <summary>
        /// Fills the matrix parameter with the values of this
        /// <see cref="AffineTransform"/>
        /// instance.
        /// </summary>
        /// <param name="matrix">
        /// the array to be filled with the values of this
        /// <see cref="AffineTransform"/>
        /// instance.
        /// </param>
        public virtual void GetMatrix(double[] matrix) {
            matrix[0] = m00;
            matrix[1] = m10;
            matrix[2] = m01;
            matrix[3] = m11;
            if (matrix.Length > 4) {
                matrix[4] = m02;
                matrix[5] = m12;
            }
        }

        /// <summary>
        /// Gets the determinant of the matrix representation of this
        /// <see cref="AffineTransform"/>.
        /// </summary>
        /// <returns>
        /// the determinant of the matrix representation of this
        /// <see cref="AffineTransform"/>.
        /// </returns>
        public virtual double GetDeterminant() {
            return m00 * m11 - m01 * m10;
        }

        /// <summary>
        /// Sets the values of this
        /// <see cref="AffineTransform"/>
        /// instance to the values provided.
        /// </summary>
        /// <remarks>
        /// Sets the values of this
        /// <see cref="AffineTransform"/>
        /// instance to the values provided.
        /// The type of the transformation is set to
        /// <c>TYPE_UNKNOWN</c>.
        /// </remarks>
        /// <param name="m00">The value of the first row and first column of the matrix.</param>
        /// <param name="m10">The value of the second row and first column of the matrix.</param>
        /// <param name="m01">The value of the first row and second column of the matrix.</param>
        /// <param name="m11">The value of the second row and second column of the matrix.</param>
        /// <param name="m02">The value of the first row and third column of the matrix.</param>
        /// <param name="m12">The value of the second row and third column of the matrix.</param>
        public virtual void SetTransform(float m00, float m10, float m01, float m11, float m02, float m12) {
            this.type = TYPE_UNKNOWN;
            this.m00 = m00;
            this.m10 = m10;
            this.m01 = m01;
            this.m11 = m11;
            this.m02 = m02;
            this.m12 = m12;
        }

        /// <summary>
        /// Sets the values of this
        /// <see cref="AffineTransform"/>
        /// instance to the values provided.
        /// </summary>
        /// <remarks>
        /// Sets the values of this
        /// <see cref="AffineTransform"/>
        /// instance to the values provided.
        /// The type of the transformation is set to
        /// <c>TYPE_UNKNOWN</c>.
        /// </remarks>
        /// <param name="m00">The value of the first row and first column of the matrix.</param>
        /// <param name="m10">The value of the second row and first column of the matrix.</param>
        /// <param name="m01">The value of the first row and second column of the matrix.</param>
        /// <param name="m11">The value of the second row and second column of the matrix.</param>
        /// <param name="m02">The value of the first row and third column of the matrix.</param>
        /// <param name="m12">The value of the second row and third column of the matrix.</param>
        public virtual void SetTransform(double m00, double m10, double m01, double m11, double m02, double m12) {
            this.type = TYPE_UNKNOWN;
            this.m00 = m00;
            this.m10 = m10;
            this.m01 = m01;
            this.m11 = m11;
            this.m02 = m02;
            this.m12 = m12;
        }

        /// <summary>
        /// Sets the values of this
        /// <see cref="AffineTransform"/>
        /// instance to the values provided.
        /// </summary>
        /// <param name="t">
        /// The
        /// <see cref="AffineTransform"/>
        /// instance to be used.
        /// </param>
        public virtual void SetTransform(iText.Kernel.Geom.AffineTransform t) {
            type = t.type;
            SetTransform(t.m00, t.m10, t.m01, t.m11, t.m02, t.m12);
        }

        /// <summary>
        /// Sets this
        /// <see cref="AffineTransform"/>
        /// to the identity transformation.
        /// </summary>
        public virtual void SetToIdentity() {
            type = TYPE_IDENTITY;
            m00 = m11 = 1;
            m10 = m01 = m02 = m12 = 0;
        }

        /// <summary>
        /// Sets this
        /// <see cref="AffineTransform"/>
        /// to represent a translation transformation.
        /// </summary>
        /// <param name="mx">The value of the translation on the x-axis.</param>
        /// <param name="my">The value of the translation on the y-axis.</param>
        public virtual void SetToTranslation(double mx, double my) {
            m00 = m11 = 1;
            m01 = m10 = 0;
            m02 = mx;
            m12 = my;
            if (mx == 0 && my == 0) {
                type = TYPE_IDENTITY;
            }
            else {
                type = TYPE_TRANSLATION;
            }
        }

        /// <summary>
        /// Sets this
        /// <see cref="AffineTransform"/>
        /// to represent a scale transformation.
        /// </summary>
        /// <param name="scx">The value of the scale factor on the x-axis.</param>
        /// <param name="scy">The value of the scale factor on the y-axis.</param>
        public virtual void SetToScale(double scx, double scy) {
            m00 = scx;
            m11 = scy;
            m10 = m01 = m02 = m12 = 0;
            if (scx != 1.0 || scy != 1) {
                type = TYPE_UNKNOWN;
            }
            else {
                type = TYPE_IDENTITY;
            }
        }

        /// <summary>
        /// Sets this
        /// <see cref="AffineTransform"/>
        /// to represent a shear transformation.
        /// </summary>
        /// <param name="shx">The value of the shear factor on the x-axis.</param>
        /// <param name="shy">The value of the shear factor on the y-axis.</param>
        public virtual void SetToShear(double shx, double shy) {
            m00 = m11 = 1;
            m02 = m12 = 0;
            m01 = shx;
            m10 = shy;
            if (shx != 0.0 || shy != 0.0) {
                type = TYPE_UNKNOWN;
            }
            else {
                type = TYPE_IDENTITY;
            }
        }

        /// <summary>Set this affine transformation to represent a rotation over the passed angle</summary>
        /// <param name="angle">angle to rotate over in radians</param>
        public virtual void SetToRotation(double angle) {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            if (Math.Abs(cos) < ZERO) {
                cos = 0.0;
                sin = sin > 0.0 ? 1.0 : -1.0;
            }
            else {
                if (Math.Abs(sin) < ZERO) {
                    sin = 0.0;
                    cos = cos > 0.0 ? 1.0 : -1.0;
                }
            }
            m00 = m11 = (float)cos;
            m01 = (float)-sin;
            m10 = (float)sin;
            m02 = m12 = 0;
            type = TYPE_UNKNOWN;
        }

        /// <summary>
        /// Set this affine transformation to represent a rotation over the passed angle,
        /// using the passed point as the center of rotation
        /// </summary>
        /// <param name="angle">angle to rotate over in radians</param>
        /// <param name="px">x-coordinate of center of rotation</param>
        /// <param name="py">y-coordinate of center of rotation</param>
        public virtual void SetToRotation(double angle, double px, double py) {
            SetToRotation(angle);
            m02 = px * (1 - m00) + py * m10;
            m12 = py * (1 - m00) - px * m10;
            type = TYPE_UNKNOWN;
        }

        /// <summary>
        /// Get a new
        /// <see cref="AffineTransform"/>
        /// instance representing a translation over the passed values
        /// </summary>
        /// <param name="mx">x-coordinate of translation</param>
        /// <param name="my">y-coordinate of translation</param>
        /// <returns>
        /// 
        /// <see cref="AffineTransform"/>
        /// representing the translation
        /// </returns>
        public static iText.Kernel.Geom.AffineTransform GetTranslateInstance(double mx, double my) {
            iText.Kernel.Geom.AffineTransform t = new iText.Kernel.Geom.AffineTransform();
            t.SetToTranslation(mx, my);
            return t;
        }

        /// <summary>
        /// Get a new
        /// <see cref="AffineTransform"/>
        /// instance representing a scale over the passed values
        /// </summary>
        /// <param name="scx">scale factor on the x-axis</param>
        /// <param name="scY">scale factor on the y-axis</param>
        /// <returns>
        /// 
        /// <see cref="AffineTransform"/>
        /// representing the scale
        /// </returns>
        public static iText.Kernel.Geom.AffineTransform GetScaleInstance(double scx, double scY) {
            iText.Kernel.Geom.AffineTransform t = new iText.Kernel.Geom.AffineTransform();
            t.SetToScale(scx, scY);
            return t;
        }

        /// <summary>
        /// Get a new
        /// <see cref="AffineTransform"/>
        /// instance representing a shear over the passed values
        /// </summary>
        /// <param name="shx">shear factor on the x-axis</param>
        /// <param name="shy">shear factor on the y-axis</param>
        /// <returns>
        /// 
        /// <see cref="AffineTransform"/>
        /// representing the shear
        /// </returns>
        public static iText.Kernel.Geom.AffineTransform GetShearInstance(double shx, double shy) {
            iText.Kernel.Geom.AffineTransform m = new iText.Kernel.Geom.AffineTransform();
            m.SetToShear(shx, shy);
            return m;
        }

        /// <summary>Get an affine transformation representing a counter-clockwise rotation over the passed angle</summary>
        /// <param name="angle">angle in radians to rotate over</param>
        /// <returns>
        /// 
        /// <see cref="AffineTransform"/>
        /// representing the rotation
        /// </returns>
        public static iText.Kernel.Geom.AffineTransform GetRotateInstance(double angle) {
            iText.Kernel.Geom.AffineTransform t = new iText.Kernel.Geom.AffineTransform();
            t.SetToRotation(angle);
            return t;
        }

        /// <summary>
        /// Get an affine transformation representing a counter-clockwise rotation over the passed angle,
        /// using the passed point as the center of rotation
        /// </summary>
        /// <param name="angle">angle in radians to rotate over</param>
        /// <param name="x">x-coordinate of center of rotation</param>
        /// <param name="y">y-coordinate of center of rotation</param>
        /// <returns>
        /// 
        /// <see cref="AffineTransform"/>
        /// representing the rotation
        /// </returns>
        public static iText.Kernel.Geom.AffineTransform GetRotateInstance(double angle, double x, double y) {
            iText.Kernel.Geom.AffineTransform t = new iText.Kernel.Geom.AffineTransform();
            t.SetToRotation(angle, x, y);
            return t;
        }

        public virtual void Translate(double mx, double my) {
            Concatenate(iText.Kernel.Geom.AffineTransform.GetTranslateInstance(mx, my));
        }

        public virtual void Scale(double scx, double scy) {
            Concatenate(iText.Kernel.Geom.AffineTransform.GetScaleInstance(scx, scy));
        }

        public virtual void Shear(double shx, double shy) {
            Concatenate(iText.Kernel.Geom.AffineTransform.GetShearInstance(shx, shy));
        }

        /// <summary>Add a counter-clockwise rotation to this transformation</summary>
        /// <param name="angle">angle in radians to rotate over</param>
        public virtual void Rotate(double angle) {
            Concatenate(iText.Kernel.Geom.AffineTransform.GetRotateInstance(angle));
        }

        /// <summary>
        /// Add a counter-clockwise rotation to this transformation,
        /// using the passed point as the center of rotation
        /// </summary>
        /// <param name="angle">angle in radians to rotate over</param>
        /// <param name="px">x-coordinate of center of rotation</param>
        /// <param name="py">y-coordinate of center of rotation</param>
        public virtual void Rotate(double angle, double px, double py) {
            Concatenate(iText.Kernel.Geom.AffineTransform.GetRotateInstance(angle, px, py));
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Multiply matrix of two AffineTransform objects</summary>
        /// <param name="t1">- the AffineTransform object is a multiplicand.</param>
        /// <param name="t2">- the AffineTransform object is a multiplier.</param>
        /// <returns>an AffineTransform object that is a result of t1 multiplied by matrix t2.</returns>
        internal virtual iText.Kernel.Geom.AffineTransform Multiply(iText.Kernel.Geom.AffineTransform t1, iText.Kernel.Geom.AffineTransform
             t2) {
            return new iText.Kernel.Geom.AffineTransform(t1.m00 * t2.m00 + t1.m10 * t2.m01, t1.m00 * t2.m10 + t1.m10 *
                 t2.m11, t1.m01 * t2.m00 + t1.m11 * t2.m01, t1.m01 * t2.m10 + t1.m11 * t2.m11, t1.m02 * t2.m00 + t1.m12
                 * t2.m01 + t2.m02, t1.m02 * t2.m10 + t1.m12 * t2.m11 + t2.m12);
        }
//\endcond

        /// <summary>Multiply matrix of two AffineTransform objects</summary>
        /// <param name="t">- the AffineTransform object is a multiplier.</param>
        public virtual void Concatenate(iText.Kernel.Geom.AffineTransform t) {
            SetTransform(Multiply(t, this));
        }

        /// <summary>Multiply matrix of two AffineTransform objects</summary>
        /// <param name="t">- the AffineTransform object is a multiplicand.</param>
        public virtual void PreConcatenate(iText.Kernel.Geom.AffineTransform t) {
            SetTransform(Multiply(this, t));
        }

        /// <summary>
        /// Creates a new
        /// <see cref="AffineTransform"/>
        /// object that is invert of this
        /// <see cref="AffineTransform"/>
        /// object.
        /// </summary>
        /// <returns>
        /// a new
        /// <see cref="AffineTransform"/>
        /// object that is invert of this
        /// <see cref="AffineTransform"/>
        /// object.
        /// </returns>
        public virtual iText.Kernel.Geom.AffineTransform CreateInverse() {
            double det = GetDeterminant();
            if (Math.Abs(det) < ZERO) {
                // awt.204=Determinant is zero
                //$NON-NLS-1$
                throw new NoninvertibleTransformException(NoninvertibleTransformException.DETERMINANT_IS_ZERO_CANNOT_INVERT_TRANSFORMATION
                    );
            }
            return new iText.Kernel.Geom.AffineTransform(m11 / det, -m10 / det, -m01 / det, m00 / det, (m01 * m12 - m11
                 * m02) / det, (m10 * m02 - m00 * m12) / det);
        }

        /// <summary>
        /// Transform the point according to the values of this
        /// <see cref="AffineTransform"/>
        /// object.
        /// </summary>
        /// <param name="src">The point to be transformed.</param>
        /// <param name="dst">The point that will hold the result of the transformation.</param>
        /// <returns>The point that holds the result of the transformation.</returns>
        public virtual Point Transform(Point src, Point dst) {
            if (dst == null) {
                dst = new Point();
            }
            double x = src.GetX();
            double y = src.GetY();
            dst.SetLocation(x * m00 + y * m01 + m02, x * m10 + y * m11 + m12);
            return dst;
        }

        /// <summary>
        /// Transform the array of points according to the values of this
        /// <see cref="AffineTransform"/>
        /// object.
        /// </summary>
        /// <param name="src">The array of points to be transformed.</param>
        /// <param name="srcOff">The offset of the first point in the array.</param>
        /// <param name="dst">The array of points that will hold the result of the transformation.</param>
        /// <param name="dstOff">The offset of the first point in the destination array.</param>
        /// <param name="length">The number of points to be transformed.</param>
        public virtual void Transform(Point[] src, int srcOff, Point[] dst, int dstOff, int length) {
            while (--length >= 0) {
                Point srcPoint = src[srcOff++];
                double x = srcPoint.GetX();
                double y = srcPoint.GetY();
                Point dstPoint = dst[dstOff];
                if (dstPoint == null) {
                    dstPoint = new Point();
                }
                dstPoint.SetLocation(x * m00 + y * m01 + m02, x * m10 + y * m11 + m12);
                dst[dstOff++] = dstPoint;
            }
        }

        /// <summary>
        /// Transform the array of points according to the values of this
        /// <see cref="AffineTransform"/>
        /// object.
        /// </summary>
        /// <param name="src">The array of points to be transformed.</param>
        /// <param name="srcOff">The offset of the first point in the array.</param>
        /// <param name="dst">The array of points that will hold the result of the transformation.</param>
        /// <param name="dstOff">The offset of the first point in the destination array.</param>
        /// <param name="length">The number of points to be transformed.</param>
        public virtual void Transform(double[] src, int srcOff, double[] dst, int dstOff, int length) {
            int step = 2;
            if (src == dst && srcOff < dstOff && dstOff < srcOff + length * 2) {
                srcOff = srcOff + length * 2 - 2;
                dstOff = dstOff + length * 2 - 2;
                step = -2;
            }
            while (--length >= 0) {
                double x = src[srcOff + 0];
                double y = src[srcOff + 1];
                dst[dstOff + 0] = x * m00 + y * m01 + m02;
                dst[dstOff + 1] = x * m10 + y * m11 + m12;
                srcOff += step;
                dstOff += step;
            }
        }

        /// <summary>
        /// Transform the array of points according to the values of this
        /// <see cref="AffineTransform"/>
        /// object.
        /// </summary>
        /// <param name="src">The array of points to be transformed.</param>
        /// <param name="srcOff">The offset of the first point in the array.</param>
        /// <param name="dst">The array of points that will hold the result of the transformation.</param>
        /// <param name="dstOff">The offset of the first point in the destination array.</param>
        /// <param name="length">The number of points to be transformed.</param>
        public virtual void Transform(float[] src, int srcOff, float[] dst, int dstOff, int length) {
            int step = 2;
            if (src == dst && srcOff < dstOff && dstOff < srcOff + length * 2) {
                srcOff = srcOff + length * 2 - 2;
                dstOff = dstOff + length * 2 - 2;
                step = -2;
            }
            while (--length >= 0) {
                float x = src[srcOff + 0];
                float y = src[srcOff + 1];
                dst[dstOff + 0] = (float)(x * m00 + y * m01 + m02);
                dst[dstOff + 1] = (float)(x * m10 + y * m11 + m12);
                srcOff += step;
                dstOff += step;
            }
        }

        /// <summary>
        /// Transform the array of points according to the values of this
        /// <see cref="AffineTransform"/>
        /// object.
        /// </summary>
        /// <param name="src">The array of points to be transformed.</param>
        /// <param name="srcOff">The offset of the first point in the array.</param>
        /// <param name="dst">The array of points that will hold the result of the transformation.</param>
        /// <param name="dstOff">The offset of the first point in the destination array.</param>
        /// <param name="length">The number of points to be transformed.</param>
        public virtual void Transform(float[] src, int srcOff, double[] dst, int dstOff, int length) {
            while (--length >= 0) {
                float x = src[srcOff++];
                float y = src[srcOff++];
                dst[dstOff++] = x * m00 + y * m01 + m02;
                dst[dstOff++] = x * m10 + y * m11 + m12;
            }
        }

        /// <summary>
        /// Transform the array of points according to the values of this
        /// <see cref="AffineTransform"/>
        /// object.
        /// </summary>
        /// <param name="src">The array of points to be transformed.</param>
        /// <param name="srcOff">The offset of the first point in the array.</param>
        /// <param name="dst">The array of points that will hold the result of the transformation.</param>
        /// <param name="dstOff">The offset of the first point in the destination array.</param>
        /// <param name="length">The number of points to be transformed.</param>
        public virtual void Transform(double[] src, int srcOff, float[] dst, int dstOff, int length) {
            while (--length >= 0) {
                double x = src[srcOff++];
                double y = src[srcOff++];
                dst[dstOff++] = (float)(x * m00 + y * m01 + m02);
                dst[dstOff++] = (float)(x * m10 + y * m11 + m12);
            }
        }

        /// <summary>Performs  the transformation on the source point and stores the result in the destination point.</summary>
        /// <param name="src">The source point to be transformed.</param>
        /// <param name="dst">The destination point that will hold the result of the transformation.</param>
        /// <returns>The modified destination point.</returns>
        public virtual Point DeltaTransform(Point src, Point dst) {
            if (dst == null) {
                dst = new Point();
            }
            double x = src.GetX();
            double y = src.GetY();
            dst.SetLocation(x * m00 + y * m01, x * m10 + y * m11);
            return dst;
        }

        /// <summary>
        /// Performs the delta transformation on the source array of points and stores the result in
        /// the destination array of points.
        /// </summary>
        /// <param name="src">The source array of data to be transformed.</param>
        /// <param name="srcOff">The offset of the first point in the source array.</param>
        /// <param name="dst">The destination array of data that will hold the result of the transformation.</param>
        /// <param name="dstOff">The offset of the first point in the destination array.</param>
        /// <param name="length">The number of points to be transformed.</param>
        public virtual void DeltaTransform(double[] src, int srcOff, double[] dst, int dstOff, int length) {
            while (--length >= 0) {
                double x = src[srcOff++];
                double y = src[srcOff++];
                dst[dstOff++] = x * m00 + y * m01;
                dst[dstOff++] = x * m10 + y * m11;
            }
        }

        /// <summary>Performs the inverse transformation on the source point and stores the result in the destination point.
        ///     </summary>
        /// <param name="src">The source point to be transformed.</param>
        /// <param name="dst">The destination point that will hold the result of the transformation.</param>
        /// <returns>The modified destination point.</returns>
        public virtual Point InverseTransform(Point src, Point dst) {
            double det = GetDeterminant();
            if (Math.Abs(det) < ZERO) {
                // awt.204=Determinant is zero
                //$NON-NLS-1$
                throw new NoninvertibleTransformException(NoninvertibleTransformException.DETERMINANT_IS_ZERO_CANNOT_INVERT_TRANSFORMATION
                    );
            }
            if (dst == null) {
                dst = new Point();
            }
            double x = src.GetX() - m02;
            double y = src.GetY() - m12;
            dst.SetLocation((x * m11 - y * m01) / det, (y * m00 - x * m10) / det);
            return dst;
        }

        /// <summary>
        /// Performs the inverse transformation on the source array of points and stores the result
        /// in the destination array of points.
        /// </summary>
        /// <param name="src">The source array of data to be transformed.</param>
        /// <param name="srcOff">The offset of the first point in the source array.</param>
        /// <param name="dst">The destination array of data that will hold the result of the transformation.</param>
        /// <param name="dstOff">The offset of the first point in the destination array.</param>
        /// <param name="length">The number of points to be transformed.</param>
        public virtual void InverseTransform(double[] src, int srcOff, double[] dst, int dstOff, int length) {
            double det = GetDeterminant();
            if (Math.Abs(det) < ZERO) {
                // awt.204=Determinant is zero
                //$NON-NLS-1$
                throw new NoninvertibleTransformException(NoninvertibleTransformException.DETERMINANT_IS_ZERO_CANNOT_INVERT_TRANSFORMATION
                    );
            }
            while (--length >= 0) {
                double x = src[srcOff++] - m02;
                double y = src[srcOff++] - m12;
                dst[dstOff++] = (x * m11 - y * m01) / det;
                dst[dstOff++] = (y * m00 - x * m10) / det;
            }
        }

        /// <summary>
        /// Performs the inverse transformation on the source array of points and stores the result
        /// in the destination array of points.
        /// </summary>
        /// <param name="src">The source array of data to be transformed.</param>
        /// <param name="srcOff">The offset of the first point in the source array.</param>
        /// <param name="dst">The destination array of data that will hold the result of the transformation.</param>
        /// <param name="dstOff">The offset of the first point in the destination array.</param>
        /// <param name="length">The number of points to be transformed.</param>
        public virtual void InverseTransform(float[] src, int srcOff, float[] dst, int dstOff, int length) {
            float det = (float)GetDeterminant();
            if (Math.Abs(det) < ZERO) {
                // awt.204=Determinant is zero
                //$NON-NLS-1$
                throw new NoninvertibleTransformException(NoninvertibleTransformException.DETERMINANT_IS_ZERO_CANNOT_INVERT_TRANSFORMATION
                    );
            }
            while (--length >= 0) {
                float x = (float)(src[srcOff++] - m02);
                float y = (float)(src[srcOff++] - m12);
                dst[dstOff++] = (float)((x * m11 - y * m01) / det);
                dst[dstOff++] = (float)((y * m00 - x * m10) / det);
            }
        }

        /// <summary>
        /// Creates a "deep copy" of this AffineTransform, meaning the object returned by this method will be independent
        /// of the object being cloned.
        /// </summary>
        /// <returns>the copied AffineTransform.</returns>
        public virtual iText.Kernel.Geom.AffineTransform Clone() {
            return (iText.Kernel.Geom.AffineTransform) MemberwiseClone();
        }

        /// <summary>Compares this AffineTransform with the specified Object.</summary>
        /// <remarks>
        /// Compares this AffineTransform with the specified Object.
        /// If the object is the same as this AffineTransform, this method returns true.
        /// Otherwise, this method checks if the Object is an instance of AffineTransform and if the values of the two
        /// AffineTransforms are equal.
        /// </remarks>
        /// <param name="o">The object to compare this AffineTransform with.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the object is the same as this AffineTransform,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Kernel.Geom.AffineTransform that = (iText.Kernel.Geom.AffineTransform)o;
            return JavaUtil.DoubleCompare(that.m00, m00) == 0 && JavaUtil.DoubleCompare(that.m10, m10) == 0 && JavaUtil.DoubleCompare
                (that.m01, m01) == 0 && JavaUtil.DoubleCompare(that.m11, m11) == 0 && JavaUtil.DoubleCompare(that.m02, 
                m02) == 0 && JavaUtil.DoubleCompare(that.m12, m12) == 0;
        }

        /// <summary>Returns a hash code value for the object.</summary>
        /// <returns>a hash code value for this object.</returns>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(m00, m10, m01, m11, m02, m12);
        }
    }
}

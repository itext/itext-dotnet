/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
    /// <summary>Represents a vector (i.e. a point in space).</summary>
    /// <remarks>
    /// Represents a vector (i.e. a point in space).  This class is completely
    /// unrelated to the
    /// <see cref="System.Collections.ArrayList{E}"/>
    /// class in the standard JRE.
    /// <br /><br />
    /// For many PDF related operations, the z coordinate is specified as 1
    /// This is to support the coordinate transformation calculations.  If it
    /// helps, just think of all PDF drawing operations as occurring in a single plane
    /// with z=1.
    /// </remarks>
    public class Vector {
        /// <summary>index of the X coordinate</summary>
        public const int I1 = 0;

        /// <summary>index of the Y coordinate</summary>
        public const int I2 = 1;

        /// <summary>index of the Z coordinate</summary>
        public const int I3 = 2;

        /// <summary>the values inside the vector</summary>
        private readonly float[] vals = new float[] { 0, 0, 0 };

        /// <summary>Creates a new Vector</summary>
        /// <param name="x">the X coordinate</param>
        /// <param name="y">the Y coordinate</param>
        /// <param name="z">the Z coordinate</param>
        public Vector(float x, float y, float z) {
            vals[I1] = x;
            vals[I2] = y;
            vals[I3] = z;
        }

        /// <summary>Gets the value from a coordinate of the vector</summary>
        /// <param name="index">the index of the value to get (I1, I2 or I3)</param>
        /// <returns>a coordinate value</returns>
        public virtual float Get(int index) {
            return vals[index];
        }

        /// <summary>Computes the cross product of this vector and the specified matrix</summary>
        /// <param name="by">the matrix to cross this vector with</param>
        /// <returns>the result of the cross product</returns>
        public virtual iText.Kernel.Geom.Vector Cross(Matrix by) {
            float x = vals[I1] * by.Get(Matrix.I11) + vals[I2] * by.Get(Matrix.I21) + vals[I3] * by.Get(Matrix.I31);
            float y = vals[I1] * by.Get(Matrix.I12) + vals[I2] * by.Get(Matrix.I22) + vals[I3] * by.Get(Matrix.I32);
            float z = vals[I1] * by.Get(Matrix.I13) + vals[I2] * by.Get(Matrix.I23) + vals[I3] * by.Get(Matrix.I33);
            return new iText.Kernel.Geom.Vector(x, y, z);
        }

        /// <summary>Computes the difference between this vector and the specified vector</summary>
        /// <param name="v">the vector to subtract from this one</param>
        /// <returns>the results of the subtraction</returns>
        public virtual iText.Kernel.Geom.Vector Subtract(iText.Kernel.Geom.Vector v) {
            float x = vals[I1] - v.vals[I1];
            float y = vals[I2] - v.vals[I2];
            float z = vals[I3] - v.vals[I3];
            return new iText.Kernel.Geom.Vector(x, y, z);
        }

        /// <summary>Computes the cross product of this vector and the specified vector</summary>
        /// <param name="with">the vector to cross this vector with</param>
        /// <returns>the cross product</returns>
        public virtual iText.Kernel.Geom.Vector Cross(iText.Kernel.Geom.Vector with) {
            float x = vals[I2] * with.vals[I3] - vals[I3] * with.vals[I2];
            float y = vals[I3] * with.vals[I1] - vals[I1] * with.vals[I3];
            float z = vals[I1] * with.vals[I2] - vals[I2] * with.vals[I1];
            return new iText.Kernel.Geom.Vector(x, y, z);
        }

        /// <summary>Normalizes the vector (i.e. returns the unit vector in the same orientation as this vector)</summary>
        /// <returns>the unit vector</returns>
        public virtual iText.Kernel.Geom.Vector Normalize() {
            float l = this.Length();
            float x = vals[I1] / l;
            float y = vals[I2] / l;
            float z = vals[I3] / l;
            return new iText.Kernel.Geom.Vector(x, y, z);
        }

        /// <summary>Multiplies the vector by a scalar</summary>
        /// <param name="by">the scalar to multiply by</param>
        /// <returns>the result of the scalar multiplication</returns>
        public virtual iText.Kernel.Geom.Vector Multiply(float by) {
            float x = vals[I1] * by;
            float y = vals[I2] * by;
            float z = vals[I3] * by;
            return new iText.Kernel.Geom.Vector(x, y, z);
        }

        /// <summary>Computes the dot product of this vector with the specified vector</summary>
        /// <param name="with">the vector to dot product this vector with</param>
        /// <returns>the dot product</returns>
        public virtual float Dot(iText.Kernel.Geom.Vector with) {
            return vals[I1] * with.vals[I1] + vals[I2] * with.vals[I2] + vals[I3] * with.vals[I3];
        }

        /// <summary>Computes the length of this vector</summary>
        /// <remarks>
        /// Computes the length of this vector
        /// <br />
        /// <b>Note:</b> If you are working with raw vectors from PDF, be careful -
        /// the Z axis will generally be set to 1.  If you want to compute the
        /// length of a vector, subtract it from the origin first (this will set
        /// the Z axis to 0).
        /// <br />
        /// For example:
        /// <c>aVector.subtract(originVector).length();</c>
        /// </remarks>
        /// <returns>the length of this vector</returns>
        public virtual float Length() {
            return (float)Math.Sqrt(LengthSquared());
        }

        /// <summary>Computes the length squared of this vector.</summary>
        /// <remarks>
        /// Computes the length squared of this vector.
        /// The square of the length is less expensive to compute, and is often
        /// useful without taking the square root.
        /// <br /><br />
        /// <b>Note:</b> See the important note under
        /// <see cref="Length()"/>
        /// </remarks>
        /// <returns>the square of the length of the vector</returns>
        public virtual float LengthSquared() {
            return vals[I1] * vals[I1] + vals[I2] * vals[I2] + vals[I3] * vals[I3];
        }

        /// <seealso cref="System.Object.ToString()"/>
        public override String ToString() {
            return vals[I1] + "," + vals[I2] + "," + vals[I3];
        }

        /// <summary>Calculates the hashcode using the values.</summary>
        public override int GetHashCode() {
            int prime = 31;
            int result = 1;
            result = prime * result + JavaUtil.ArraysHashCode(vals);
            return result;
        }

        /// <seealso cref="System.Object.Equals(System.Object)"/>
        public override bool Equals(Object obj) {
            if (this == obj) {
                return true;
            }
            if (obj == null) {
                return false;
            }
            if (GetType() != obj.GetType()) {
                return false;
            }
            iText.Kernel.Geom.Vector other = (iText.Kernel.Geom.Vector)obj;
            if (!JavaUtil.ArraysEquals(vals, other.vals)) {
                return false;
            }
            return true;
        }
    }
}

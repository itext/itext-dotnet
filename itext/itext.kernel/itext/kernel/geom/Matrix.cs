/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using iText.Commons.Utils;

namespace iText.Kernel.Geom {
    /// <summary>
    /// Keeps all the values of a 3 by 3 matrix and allows you to
    /// do some math with matrices.
    /// </summary>
    public class Matrix {
        /// <summary>the row=1, col=1 position ('a') in the matrix.</summary>
        public const int I11 = 0;

        /// <summary>the row=1, col=2 position ('b') in the matrix.</summary>
        public const int I12 = 1;

        /// <summary>the row=1, col=3 position (always 0 for 2-D) in the matrix.</summary>
        public const int I13 = 2;

        /// <summary>the row=2, col=1 position ('c') in the matrix.</summary>
        public const int I21 = 3;

        /// <summary>the row=2, col=2 position ('d') in the matrix.</summary>
        public const int I22 = 4;

        /// <summary>the row=2, col=3 position (always 0 for 2-D) in the matrix.</summary>
        public const int I23 = 5;

        /// <summary>the row=3, col=1 ('e', or X translation) position in the matrix.</summary>
        public const int I31 = 6;

        /// <summary>the row=3, col=2 ('f', or Y translation) position in the matrix.</summary>
        public const int I32 = 7;

        /// <summary>the row=3, col=3 position (always 1 for 2-D) in the matrix.</summary>
        public const int I33 = 8;

        /// <summary>The values inside the matrix (the identity matrix by default).</summary>
        /// <remarks>
        /// The values inside the matrix (the identity matrix by default).
        /// <para />
        /// For reference, the indeces are as follows:
        /// <br />I11 I12 I13
        /// <br />I21 I22 I23
        /// <br />I31 I32 I33
        /// </remarks>
        private readonly float[] vals = new float[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 };

        /// <summary>constructs a new Matrix with identity.</summary>
        public Matrix() {
        }

        /// <summary>Constructs a matrix that represents translation.</summary>
        /// <param name="tx">x-axis translation</param>
        /// <param name="ty">y-axis translation</param>
        public Matrix(float tx, float ty) {
            vals[I31] = tx;
            vals[I32] = ty;
        }

        /// <summary>Creates a Matrix with 9 specified entries.</summary>
        /// <param name="e11">element at position (1,1)</param>
        /// <param name="e12">element at position (1,2)</param>
        /// <param name="e13">element at position (1,3)</param>
        /// <param name="e21">element at position (2,1)</param>
        /// <param name="e22">element at position (2,2)</param>
        /// <param name="e23">element at position (2,3)</param>
        /// <param name="e31">element at position (3,1)</param>
        /// <param name="e32">element at position (3,2)</param>
        /// <param name="e33">element at position (3,3)</param>
        public Matrix(float e11, float e12, float e13, float e21, float e22, float e23, float e31, float e32, float
             e33) {
            vals[I11] = e11;
            vals[I12] = e12;
            vals[I13] = e13;
            vals[I21] = e21;
            vals[I22] = e22;
            vals[I23] = e23;
            vals[I31] = e31;
            vals[I32] = e32;
            vals[I33] = e33;
        }

        /// <summary>Creates a Matrix with 6 specified entries.</summary>
        /// <remarks>
        /// Creates a Matrix with 6 specified entries.
        /// The third column will always be [0 0 1]
        /// (row, column)
        /// </remarks>
        /// <param name="a">element at (1,1)</param>
        /// <param name="b">element at (1,2)</param>
        /// <param name="c">element at (2,1)</param>
        /// <param name="d">element at (2,2)</param>
        /// <param name="e">element at (3,1)</param>
        /// <param name="f">element at (3,2)</param>
        public Matrix(float a, float b, float c, float d, float e, float f) {
            vals[I11] = a;
            vals[I12] = b;
            vals[I13] = 0;
            vals[I21] = c;
            vals[I22] = d;
            vals[I23] = 0;
            vals[I31] = e;
            vals[I32] = f;
            vals[I33] = 1;
        }

        /// <summary>Gets a specific value inside the matrix.</summary>
        /// <remarks>
        /// Gets a specific value inside the matrix.
        /// <para />
        /// For reference, the indeces are as follows:
        /// <br />I11 I12 I13
        /// <br />I21 I22 I23
        /// <br />I31 I32 I33
        /// </remarks>
        /// <param name="index">an array index corresponding with a value inside the matrix</param>
        /// <returns>the value at that specific position.</returns>
        public virtual float Get(int index) {
            return vals[index];
        }

        /// <summary>multiplies this matrix by 'b' and returns the result.</summary>
        /// <remarks>
        /// multiplies this matrix by 'b' and returns the result.
        /// See http://en.wikipedia.org/wiki/Matrix_multiplication
        /// </remarks>
        /// <param name="by">The matrix to multiply by</param>
        /// <returns>the resulting matrix</returns>
        public virtual iText.Kernel.Geom.Matrix Multiply(iText.Kernel.Geom.Matrix by) {
            iText.Kernel.Geom.Matrix rslt = new iText.Kernel.Geom.Matrix();
            float[] a = vals;
            float[] b = by.vals;
            float[] c = rslt.vals;
            c[I11] = a[I11] * b[I11] + a[I12] * b[I21] + a[I13] * b[I31];
            c[I12] = a[I11] * b[I12] + a[I12] * b[I22] + a[I13] * b[I32];
            c[I13] = a[I11] * b[I13] + a[I12] * b[I23] + a[I13] * b[I33];
            c[I21] = a[I21] * b[I11] + a[I22] * b[I21] + a[I23] * b[I31];
            c[I22] = a[I21] * b[I12] + a[I22] * b[I22] + a[I23] * b[I32];
            c[I23] = a[I21] * b[I13] + a[I22] * b[I23] + a[I23] * b[I33];
            c[I31] = a[I31] * b[I11] + a[I32] * b[I21] + a[I33] * b[I31];
            c[I32] = a[I31] * b[I12] + a[I32] * b[I22] + a[I33] * b[I32];
            c[I33] = a[I31] * b[I13] + a[I32] * b[I23] + a[I33] * b[I33];
            return rslt;
        }

        /// <summary>Adds a matrix from this matrix and returns the results.</summary>
        /// <param name="arg">the matrix to subtract from this matrix</param>
        /// <returns>a Matrix object</returns>
        public virtual iText.Kernel.Geom.Matrix Add(iText.Kernel.Geom.Matrix arg) {
            iText.Kernel.Geom.Matrix rslt = new iText.Kernel.Geom.Matrix();
            float[] a = vals;
            float[] b = arg.vals;
            float[] c = rslt.vals;
            c[I11] = a[I11] + b[I11];
            c[I12] = a[I12] + b[I12];
            c[I13] = a[I13] + b[I13];
            c[I21] = a[I21] + b[I21];
            c[I22] = a[I22] + b[I22];
            c[I23] = a[I23] + b[I23];
            c[I31] = a[I31] + b[I31];
            c[I32] = a[I32] + b[I32];
            c[I33] = a[I33] + b[I33];
            return rslt;
        }

        /// <summary>Subtracts a matrix from this matrix and returns the results.</summary>
        /// <param name="arg">the matrix to subtract from this matrix</param>
        /// <returns>a Matrix object</returns>
        public virtual iText.Kernel.Geom.Matrix Subtract(iText.Kernel.Geom.Matrix arg) {
            iText.Kernel.Geom.Matrix rslt = new iText.Kernel.Geom.Matrix();
            float[] a = vals;
            float[] b = arg.vals;
            float[] c = rslt.vals;
            c[I11] = a[I11] - b[I11];
            c[I12] = a[I12] - b[I12];
            c[I13] = a[I13] - b[I13];
            c[I21] = a[I21] - b[I21];
            c[I22] = a[I22] - b[I22];
            c[I23] = a[I23] - b[I23];
            c[I31] = a[I31] - b[I31];
            c[I32] = a[I32] - b[I32];
            c[I33] = a[I33] - b[I33];
            return rslt;
        }

        /// <summary>Computes the determinant of the matrix.</summary>
        /// <returns>the determinant of the matrix</returns>
        public virtual float GetDeterminant() {
            // ref http://en.wikipedia.org/wiki/Determinant
            // note that in PDF, I13 and I23 are always 0 and I33 is always 1
            // so this could be simplified/faster
            return vals[I11] * vals[I22] * vals[I33] + vals[I12] * vals[I23] * vals[I31] + vals[I13] * vals[I21] * vals
                [I32] - vals[I11] * vals[I23] * vals[I32] - vals[I12] * vals[I21] * vals[I33] - vals[I13] * vals[I22] 
                * vals[I31];
        }

        /// <summary>Checks equality of matrices.</summary>
        /// <param name="obj">the other Matrix that needs to be compared with this matrix.</param>
        /// <returns>true if both matrices are equal</returns>
        /// <seealso cref="System.Object.Equals(System.Object)"/>
        public override bool Equals(Object obj) {
            if (!(obj is iText.Kernel.Geom.Matrix)) {
                return false;
            }
            return JavaUtil.ArraysEquals(vals, ((iText.Kernel.Geom.Matrix)obj).vals);
        }

        /// <summary>Generates a hash code for this object.</summary>
        /// <returns>the hash code of this object</returns>
        /// <seealso cref="System.Object.GetHashCode()"/>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(vals);
        }

        /// <summary>Generates a String representation of the matrix.</summary>
        /// <returns>the values, delimited with tabs and newlines.</returns>
        /// <seealso cref="System.Object.ToString()"/>
        public override String ToString() {
            return vals[I11] + "\t" + vals[I12] + "\t" + vals[I13] + "\n" + vals[I21] + "\t" + vals[I22] + "\t" + vals
                [I23] + "\n" + vals[I31] + "\t" + vals[I32] + "\t" + vals[I33];
        }
    }
}

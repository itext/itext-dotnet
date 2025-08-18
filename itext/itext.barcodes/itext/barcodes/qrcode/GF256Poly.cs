/*
* Copyright 2007 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
using System.Text;

namespace iText.Barcodes.Qrcode {
//\cond DO_NOT_DOCUMENT
    /// <summary>Represents a polynomial whose coefficients are elements of GF(256).</summary>
    /// <remarks>
    /// Represents a polynomial whose coefficients are elements of GF(256).
    /// Instances of this class are immutable.
    /// <para />
    /// Much credit is due to William Rucklidge since portions of this code are an indirect
    /// port of his C++ Reed-Solomon implementation.
    /// </remarks>
    internal sealed class GF256Poly {
        private readonly GF256 field;

        private readonly int[] coefficients;

//\cond DO_NOT_DOCUMENT
        /// <param name="field">
        /// the
        /// <see cref="GF256"/>
        /// instance representing the field to use
        /// to perform computations
        /// </param>
        /// <param name="coefficients">
        /// coefficients as ints representing elements of GF(256), arranged
        /// from most significant (highest-power term) coefficient to least significant
        /// </param>
        internal GF256Poly(GF256 field, int[] coefficients) {
            if (coefficients == null || coefficients.Length == 0) {
                throw new ArgumentException();
            }
            this.field = field;
            int coefficientsLength = coefficients.Length;
            if (coefficientsLength > 1 && coefficients[0] == 0) {
                // Leading term must be non-zero for anything except the constant polynomial "0"
                int firstNonZero = 1;
                while (firstNonZero < coefficientsLength && coefficients[firstNonZero] == 0) {
                    firstNonZero++;
                }
                if (firstNonZero == coefficientsLength) {
                    this.coefficients = field.GetZero().coefficients;
                }
                else {
                    this.coefficients = new int[coefficientsLength - firstNonZero];
                    Array.Copy(coefficients, firstNonZero, this.coefficients, 0, this.coefficients.Length);
                }
            }
            else {
                this.coefficients = coefficients;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int[] GetCoefficients() {
            return coefficients;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>degree of this polynomial</returns>
        internal int GetDegree() {
            return coefficients.Length - 1;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>true iff this polynomial is the monomial "0"</returns>
        internal bool IsZero() {
            return coefficients[0] == 0;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>coefficient of x^degree term in this polynomial</returns>
        internal int GetCoefficient(int degree) {
            return coefficients[coefficients.Length - 1 - degree];
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <returns>evaluation of this polynomial at a given point</returns>
        internal int EvaluateAt(int a) {
            if (a == 0) {
                // Just return the x^0 coefficient
                return GetCoefficient(0);
            }
            int size = coefficients.Length;
            if (a == 1) {
                // Just the sum of the coefficients
                int result = 0;
                for (int i = 0; i < size; i++) {
                    result = GF256.AddOrSubtract(result, coefficients[i]);
                }
                return result;
            }
            int result_1 = coefficients[0];
            for (int i = 1; i < size; i++) {
                result_1 = GF256.AddOrSubtract(field.Multiply(a, result_1), coefficients[i]);
            }
            return result_1;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>GF addition or subtraction (they are identical for a GF(2^n)</summary>
        /// <param name="other">the other GF-poly</param>
        /// <returns>new GF256Poly obtained by summing this GF and other</returns>
        internal iText.Barcodes.Qrcode.GF256Poly AddOrSubtract(iText.Barcodes.Qrcode.GF256Poly other) {
            if (!field.Equals(other.field)) {
                throw new ArgumentException("GF256Polys do not have same GF256 field");
            }
            if (IsZero()) {
                return other;
            }
            if (other.IsZero()) {
                return this;
            }
            int[] smallerCoefficients = this.coefficients;
            int[] largerCoefficients = other.coefficients;
            if (smallerCoefficients.Length > largerCoefficients.Length) {
                int[] temp = smallerCoefficients;
                smallerCoefficients = largerCoefficients;
                largerCoefficients = temp;
            }
            int[] sumDiff = new int[largerCoefficients.Length];
            int lengthDiff = largerCoefficients.Length - smallerCoefficients.Length;
            // Copy high-order terms only found in higher-degree polynomial's coefficients
            Array.Copy(largerCoefficients, 0, sumDiff, 0, lengthDiff);
            for (int i = lengthDiff; i < largerCoefficients.Length; i++) {
                sumDiff[i] = GF256.AddOrSubtract(smallerCoefficients[i - lengthDiff], largerCoefficients[i]);
            }
            return new iText.Barcodes.Qrcode.GF256Poly(field, sumDiff);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>GF multiplication</summary>
        /// <param name="other">the other GF-poly</param>
        /// <returns>new GF-poly obtained by multiplying this  with other</returns>
        internal iText.Barcodes.Qrcode.GF256Poly Multiply(iText.Barcodes.Qrcode.GF256Poly other) {
            if (!field.Equals(other.field)) {
                throw new ArgumentException("GF256Polys do not have same GF256 field");
            }
            if (IsZero() || other.IsZero()) {
                return field.GetZero();
            }
            int[] aCoefficients = this.coefficients;
            int aLength = aCoefficients.Length;
            int[] bCoefficients = other.coefficients;
            int bLength = bCoefficients.Length;
            int[] product = new int[aLength + bLength - 1];
            for (int i = 0; i < aLength; i++) {
                int aCoeff = aCoefficients[i];
                for (int j = 0; j < bLength; j++) {
                    product[i + j] = GF256.AddOrSubtract(product[i + j], field.Multiply(aCoeff, bCoefficients[j]));
                }
            }
            return new iText.Barcodes.Qrcode.GF256Poly(field, product);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>GF scalar multiplication</summary>
        /// <param name="scalar">scalar</param>
        /// <returns>new GF-poly obtained by multiplying every element of this with the scalar.</returns>
        internal iText.Barcodes.Qrcode.GF256Poly Multiply(int scalar) {
            if (scalar == 0) {
                return field.GetZero();
            }
            if (scalar == 1) {
                return this;
            }
            int size = coefficients.Length;
            int[] product = new int[size];
            for (int i = 0; i < size; i++) {
                product[i] = field.Multiply(coefficients[i], scalar);
            }
            return new iText.Barcodes.Qrcode.GF256Poly(field, product);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal iText.Barcodes.Qrcode.GF256Poly MultiplyByMonomial(int degree, int coefficient) {
            if (degree < 0) {
                throw new ArgumentException();
            }
            if (coefficient == 0) {
                return field.GetZero();
            }
            int size = coefficients.Length;
            int[] product = new int[size + degree];
            for (int i = 0; i < size; i++) {
                product[i] = field.Multiply(coefficients[i], coefficient);
            }
            return new iText.Barcodes.Qrcode.GF256Poly(field, product);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal iText.Barcodes.Qrcode.GF256Poly[] Divide(iText.Barcodes.Qrcode.GF256Poly other) {
            if (!field.Equals(other.field)) {
                throw new ArgumentException("GF256Polys do not have same GF256 field");
            }
            if (other.IsZero()) {
                throw new ArgumentException("Divide by 0");
            }
            iText.Barcodes.Qrcode.GF256Poly quotient = field.GetZero();
            iText.Barcodes.Qrcode.GF256Poly remainder = this;
            int denominatorLeadingTerm = other.GetCoefficient(other.GetDegree());
            int inverseDenominatorLeadingTerm = field.Inverse(denominatorLeadingTerm);
            while (remainder.GetDegree() >= other.GetDegree() && !remainder.IsZero()) {
                int degreeDifference = remainder.GetDegree() - other.GetDegree();
                int scale = field.Multiply(remainder.GetCoefficient(remainder.GetDegree()), inverseDenominatorLeadingTerm);
                iText.Barcodes.Qrcode.GF256Poly term = other.MultiplyByMonomial(degreeDifference, scale);
                iText.Barcodes.Qrcode.GF256Poly iterationQuotient = field.BuildMonomial(degreeDifference, scale);
                quotient = quotient.AddOrSubtract(iterationQuotient);
                remainder = remainder.AddOrSubtract(term);
            }
            return new iText.Barcodes.Qrcode.GF256Poly[] { quotient, remainder };
        }
//\endcond

        /// <returns>String representation of the Galois Field polynomial.</returns>
        public override String ToString() {
            StringBuilder result = new StringBuilder(8 * GetDegree());
            for (int degree = GetDegree(); degree >= 0; degree--) {
                int coefficient = GetCoefficient(degree);
                if (coefficient != 0) {
                    if (coefficient < 0) {
                        result.Append(" - ");
                        coefficient = -coefficient;
                    }
                    else {
                        if (result.Length > 0) {
                            result.Append(" + ");
                        }
                    }
                    if (degree == 0 || coefficient != 1) {
                        int alphaPower = field.Log(coefficient);
                        if (alphaPower == 0) {
                            result.Append('1');
                        }
                        else {
                            if (alphaPower == 1) {
                                result.Append('a');
                            }
                            else {
                                result.Append("a^");
                                result.Append(alphaPower);
                            }
                        }
                    }
                    if (degree != 0) {
                        if (degree == 1) {
                            result.Append('x');
                        }
                        else {
                            result.Append("x^");
                            result.Append(degree);
                        }
                    }
                }
            }
            return result.ToString();
        }
    }
//\endcond
}

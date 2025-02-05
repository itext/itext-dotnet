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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Function {
    /// <summary>
    /// This class represents Pdf type 2 function that defines an exponential
    /// interpolation of one input value to n output values.
    /// </summary>
    /// <remarks>
    /// This class represents Pdf type 2 function that defines an exponential
    /// interpolation of one input value to n output values.
    /// <para />
    /// For more info see ISO 32000-1, section 7.10.3 "Type 2 (Exponential Interpolation) Functions".
    /// </remarks>
    public class PdfType2Function : AbstractPdfFunction<PdfDictionary> {
        private double[] c0;

        private double[] c1;

        private double n;

        /// <summary>Instantiates a new PdfType2Function instance based on passed PdfDictionary instance.</summary>
        /// <param name="dict">the function dictionary</param>
        public PdfType2Function(PdfDictionary dict)
            : base(dict) {
            PdfNumber nObj = dict.GetAsNumber(PdfName.N);
            if (nObj == null) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_N);
            }
            n = nObj.DoubleValue();
            if (base.GetDomain().Length < 2) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_DOMAIN);
            }
            if (n != Math.Floor(n) && base.GetDomain()[0] < 0) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_N_NOT_INTEGER);
            }
            if (n < 0 && base.ClipInput(new double[] { 0 })[0] == 0) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_N_NEGATIVE);
            }
            PdfArray c0Obj = dict.GetAsArray(PdfName.C0);
            PdfArray c1Obj = dict.GetAsArray(PdfName.C1);
            PdfArray rangeObj = dict.GetAsArray(PdfName.Range);
            c0 = InitializeCArray(c0Obj, c1Obj, rangeObj, 0);
            c1 = InitializeCArray(c1Obj, c0Obj, rangeObj, 1);
            if (c0.Length != c1.Length || (base.GetRange() != null && c0.Length != base.GetRange().Length / 2)) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_OUTPUT_SIZE);
            }
        }

        public PdfType2Function(double[] domain, double[] range, double[] c0, double[] c1, double n)
            : base(new PdfDictionary(), PdfFunctionFactory.FUNCTION_TYPE_2, domain, range) {
            SetC0(c0);
            SetC1(c1);
            SetN(n);
        }

        public PdfType2Function(float[] domain, float[] range, float[] c0, float[] c1, double n)
            : this(ConvertFloatArrayToDoubleArray(domain), ConvertFloatArrayToDoubleArray(range), ConvertFloatArrayToDoubleArray
                (c0), ConvertFloatArrayToDoubleArray(c1), n) {
        }

        public override double[] Calculate(double[] input) {
            if (input == null || input.Length != 1) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_INPUT_FOR_TYPE_2_FUNCTION);
            }
            double[] clipped = ClipInput(input);
            double x = clipped[0];
            int outputSize = GetOutputSize();
            double[] output = new double[outputSize];
            for (int i = 0; i < outputSize; i++) {
                output[i] = c0[i] + Math.Pow(x, n) * (c1[i] - c0[i]);
            }
            return ClipOutput(output);
        }

        /// <summary>Gets output size of function.</summary>
        /// <remarks>
        /// Gets output size of function.
        /// <para />
        /// If Range field is absent, the size of C0 array will be returned.
        /// </remarks>
        /// <returns>output size of function</returns>
        public sealed override int GetOutputSize() {
            return GetRange() == null ? c0.Length : (GetRange().Length / 2);
        }

        /// <summary>Gets values of C0 array.</summary>
        /// <returns>the values of C0 array</returns>
        public double[] GetC0() {
            return c0;
        }

        /// <summary>Sets values of C0 array.</summary>
        /// <param name="value">the values of C0 array</param>
        public void SetC0(double[] value) {
            GetPdfObject().Put(PdfName.C0, new PdfArray(value));
            c0 = value;
        }

        /// <summary>Gets values of C1 array.</summary>
        /// <returns>the values of C1 array</returns>
        public double[] GetC1() {
            return c1;
        }

        /// <summary>Sets values of C1 array.</summary>
        /// <param name="value">the values of C1 array</param>
        public void SetC1(double[] value) {
            GetPdfObject().Put(PdfName.C1, new PdfArray(value));
            c1 = value;
        }

        /// <summary>Gets value of N field.</summary>
        /// <returns>the value of N field</returns>
        public double GetN() {
            return n;
        }

        /// <summary>sets value of N field.</summary>
        /// <param name="value">the value of N field</param>
        public void SetN(double value) {
            GetPdfObject().Put(PdfName.N, new PdfNumber(value));
            n = value;
        }

        private static double[] InitializeCArray(PdfArray c, PdfArray otherC, PdfArray range, double defaultValue) {
            if (c != null) {
                return c.ToDoubleArray();
            }
            double[] result;
            if (otherC == null) {
                if (range == null) {
                    result = new double[1];
                }
                else {
                    result = new double[range.Size() / 2];
                }
            }
            else {
                result = new double[otherC.Size()];
            }
            for (int i = 0; i < result.Length; i++) {
                result[i] = defaultValue;
            }
            return result;
        }
    }
}

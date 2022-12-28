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

        public PdfType2Function(double[] domain, double[] range, double[] c0, double[] c1, int n)
            : base(new PdfDictionary(), PdfFunctionFactory.FUNCTION_TYPE_2, domain, range) {
            SetC0(c0);
            SetC1(c1);
            SetN(n);
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
        public void SetN(int value) {
            GetPdfObject().Put(PdfName.N, new PdfNumber(value));
            n = value;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
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

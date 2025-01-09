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
using System.Linq;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function.Utils;

namespace iText.Kernel.Pdf.Function {
    public class PdfType0Function : AbstractPdfFunction<PdfStream> {
        private int[] size;

        private int order;

        private int[] encode;

        private double[] decode;

        private int bitsPerSample;

        private AbstractSampleExtractor sampleExtractor = null;

        private byte[] samples;

        private int outputDimension;

        private long decodeLimit;

        private bool isValidated = false;

        private String errorMessage = null;

        private double[][] derivatives = null;

        public PdfType0Function(PdfStream pdfObject)
            : base(pdfObject) {
            PdfArray sizeObj = pdfObject.GetAsArray(PdfName.Size);
            if (base.GetDomain() == null || base.GetRange() == null || sizeObj == null) {
                SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_NOT_NULL_PARAMETERS);
                return;
            }
            size = sizeObj.ToIntArray();
            PdfNumber orderObj = pdfObject.GetAsNumber(PdfName.Order);
            order = orderObj == null ? 1 : orderObj.IntValue();
            PdfArray encodeObj = pdfObject.GetAsArray(PdfName.Encode);
            InitializeEncoding(encodeObj);
            PdfArray decodeObj = pdfObject.GetAsArray(PdfName.Decode);
            if (decodeObj == null) {
                decode = base.GetRange();
            }
            else {
                decode = decodeObj.ToDoubleArray();
            }
            outputDimension = base.GetRange().Length >> 1;
            PdfNumber bitsPerSampleObj = pdfObject.GetAsNumber(PdfName.BitsPerSample);
            bitsPerSample = bitsPerSampleObj == null ? 0 : bitsPerSampleObj.IntValue();
            decodeLimit = (1L << bitsPerSample) - 1;
            samples = pdfObject.GetBytes(true);
            try {
                sampleExtractor = AbstractSampleExtractor.CreateExtractor(bitsPerSample);
            }
            catch (ArgumentException e) {
                SetErrorMessage(e.Message);
            }
        }

        public PdfType0Function(double[] domain, int[] size, double[] range, int order, int bitsPerSample, byte[] 
            samples)
            : this(domain, size, range, order, null, null, bitsPerSample, samples) {
        }

        public PdfType0Function(float[] domain, int[] size, float[] range, int order, int bitsPerSample, byte[] samples
            )
            : this(ConvertFloatArrayToDoubleArray(domain), size, ConvertFloatArrayToDoubleArray(range), order, bitsPerSample
                , samples) {
        }

        public PdfType0Function(double[] domain, int[] size, double[] range, int order, int[] encode, double[] decode
            , int bitsPerSample, byte[] samples)
            : base(new PdfStream(samples), PdfFunctionFactory.FUNCTION_TYPE_0, domain, range) {
            if (size != null) {
                this.size = JavaUtil.ArraysCopyOf(size, size.Length);
            }
            if (base.GetDomain() == null || base.GetRange() == null || size == null) {
                SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_NOT_NULL_PARAMETERS);
                return;
            }
            this.size = JavaUtil.ArraysCopyOf(size, size.Length);
            base.GetPdfObject().Put(PdfName.Size, new PdfArray(size));
            this.order = order;
            base.GetPdfObject().Put(PdfName.Order, new PdfNumber(order));
            InitializeEncoding(encode);
            base.GetPdfObject().Put(PdfName.Encode, new PdfArray(this.encode));
            if (decode == null) {
                this.decode = JavaUtil.ArraysCopyOf(range, range.Length);
            }
            else {
                this.decode = JavaUtil.ArraysCopyOf(decode, decode.Length);
            }
            base.GetPdfObject().Put(PdfName.Decode, new PdfArray(this.decode));
            this.bitsPerSample = bitsPerSample;
            base.GetPdfObject().Put(PdfName.BitsPerSample, new PdfNumber(bitsPerSample));
            this.outputDimension = base.GetRange().Length >> 1;
            this.decodeLimit = (1L << bitsPerSample) - 1;
            this.samples = JavaUtil.ArraysCopyOf(samples, samples.Length);
            try {
                sampleExtractor = AbstractSampleExtractor.CreateExtractor(bitsPerSample);
            }
            catch (ArgumentException e) {
                SetErrorMessage(e.Message);
            }
            if (IsInvalid()) {
                throw new ArgumentException(errorMessage);
            }
        }

        public virtual int GetOrder() {
            return order;
        }

        public virtual void SetOrder(int order) {
            this.order = order;
            GetPdfObject().Put(PdfName.Order, new PdfNumber(order));
            isValidated = false;
        }

        public virtual int[] GetSize() {
            return size;
        }

        public virtual void SetSize(int[] size) {
            this.size = size;
            GetPdfObject().Put(PdfName.Size, new PdfArray(size));
            isValidated = false;
        }

        public virtual int[] GetEncode() {
            return encode;
        }

        public virtual void SetEncode(int[] encode) {
            InitializeEncoding(encode);
            GetPdfObject().Put(PdfName.Encode, new PdfArray(encode));
            isValidated = false;
        }

        public virtual double[] GetDecode() {
            return decode;
        }

        public virtual void SetDecode(double[] decode) {
            this.decode = decode;
            GetPdfObject().Put(PdfName.Decode, new PdfArray(decode));
            isValidated = false;
        }

        public override bool CheckCompatibilityWithColorSpace(PdfColorSpace alternateSpace) {
            return GetInputSize() == 1 && GetOutputSize() == alternateSpace.GetNumberOfComponents();
        }

        public override void SetDomain(double[] domain) {
            base.SetDomain(domain);
            isValidated = false;
        }

        public override void SetRange(double[] range) {
            base.SetRange(range);
            isValidated = false;
        }

        public override double[] Calculate(double[] input) {
            if (IsInvalid()) {
                throw new ArgumentException(errorMessage);
            }
            double[] normal = Normalize(input, GetDomain());
            int[] floor = GetFloor(normal, encode);
            double[] result;
            if (order == 3 && size.Length == 1 && encode[1] - encode[0] > 1) {
                result = InterpolateByCubicSpline(normal[0], floor[0]);
            }
            else {
                result = Interpolate(normal, floor);
            }
            return Clip(result, GetRange());
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Encode normalized input value.</summary>
        /// <param name="normal">input normalized value</param>
        /// <param name="encodeMin">encode min value</param>
        /// <param name="encodeMax">encode max value</param>
        /// <returns>encoded value</returns>
        internal static double Encode(double normal, int encodeMin, int encodeMax) {
            return encodeMin + normal * (encodeMax - encodeMin);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Calculates floor sample coordinates for the normalized  input array.</summary>
        /// <param name="normal">input array normalized to domain</param>
        /// <param name="encode">encode mapping</param>
        /// <returns>encoded sample coordinates of the nearest left interpolation point</returns>
        internal static int[] GetFloor(double[] normal, int[] encode) {
            int[] result = new int[normal.Length];
            for (int i = 0; i < normal.Length; ++i) {
                int j = i << 1;
                int floor = (int)Encode(normal[i], encode[j], encode[j + 1]);
                result[i] = Math.Min(Math.Max(0, encode[j + 1] - 1), floor);
            }
            return result;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Maps sample coordinates to linear position in samples table.</summary>
        /// <param name="sample">sample encoded coordinates</param>
        /// <param name="size">number of samples in each input dimension</param>
        /// <returns>position in samples table</returns>
        internal static int GetSamplePosition(int[] sample, int[] size) {
            int position = sample[size.Length - 1];
            for (int i = size.Length - 2; i >= 0; --i) {
                position = sample[i] + size[i] * position;
            }
            return position;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Calculated component-by-component normalized distances from input array to the nearest left interpolation point.
        ///     </summary>
        /// <remarks>
        /// Calculated component-by-component normalized distances from input array to the nearest left interpolation point.
        /// Input array shall be normalized to domain
        /// </remarks>
        /// <param name="normal">input array normalized to domain</param>
        /// <param name="encode">encode mapping</param>
        /// <returns>component-by-component normalized distances from input array to the nearest left interpolation point
        ///     </returns>
        internal static double[] GetFloorWeights(double[] normal, int[] encode) {
            double[] result = new double[normal.Length];
            for (int i = 0; i < normal.Length; i++) {
                result[i] = GetFloorWeight(normal[i], encode[2 * i], encode[2 * i + 1]);
            }
            return result;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Calculates normalized distance from input value to the nearest left interpolation point.</summary>
        /// <remarks>
        /// Calculates normalized distance from input value to the nearest left interpolation point.
        /// Input value shall be normalized to domain component
        /// </remarks>
        /// <param name="normal">input value normalized to domain component</param>
        /// <param name="encodeMin">encode min value</param>
        /// <param name="encodeMax">encode max value</param>
        /// <returns>normalized distance from input value to the nearest left interpolation point</returns>
        internal static double GetFloorWeight(double normal, int encodeMin, int encodeMax) {
            double value = Encode(normal, encodeMin, encodeMax);
            return value - Math.Min(encodeMax - 1, (int)value);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Solves the system of linear equations by sweep method where the matrix is 3-diagonal.</summary>
        /// <remarks>
        /// Solves the system of linear equations by sweep method where the matrix is 3-diagonal.
        /// Main diagonal elements are 4, lower and upper diagonals: 1.
        /// <para />
        /// x[0] = 0,
        /// x[0]   + 4*x[1] + x[2]   = f[0],
        /// x[1]   + 4*x[2] + x[3]   = f[1],
        /// ...
        /// x[n-1] + 4*x[n] + x[n+1] = f[n-1],
        /// x[n] = 0
        /// </remarks>
        /// <param name="f">right hand side</param>
        /// <returns>solution, first and last values are zeroes</returns>
        internal static double[] SpecialSweepMethod(double[] f) {
            System.Diagnostics.Debug.Assert((f.Length > 0));
            double[] x = new double[f.Length + 2];
            x[1] = 4;
            for (int i = 1; i < f.Length; ++i) {
                x[0] = 1 / x[i];
                x[i + 1] = 4 - x[0];
                f[i] = f[i] - x[0] * f[i - 1];
            }
            x[f.Length] = f[f.Length - 1] / x[f.Length];
            for (int i = f.Length - 1; i > 0; --i) {
                x[i] = (f[i - 1] - x[i + 1]) / x[i];
            }
            x[0] = x[x.Length - 1] = 0;
            return x;
        }
//\endcond

        private void InitializeEncoding(PdfArray encodeObj) {
            if (encodeObj == null) {
                encode = GetDefaultEncoding();
            }
            else {
                encode = encodeObj.ToIntArray();
                for (int i = 0; i < size.Length; ++i) {
                    int j = i << 1;
                    encode[j] = Math.Max(0, encode[j]);
                    encode[j + 1] = Math.Min(size[i] - 1, encode[j + 1]);
                }
            }
        }

        private void InitializeEncoding(int[] encode) {
            if (encode == null) {
                this.encode = GetDefaultEncoding();
            }
            else {
                this.encode = new int[encode.Length];
                for (int i = 0; i < size.Length; ++i) {
                    int j = i << 1;
                    this.encode[j] = Math.Max(0, encode[j]);
                    this.encode[j + 1] = Math.Min(size[i] - 1, encode[j + 1]);
                }
            }
        }

        private int[] GetDefaultEncoding() {
            int[] result = new int[this.size.Length << 1];
            int i = 0;
            foreach (int sizeItem in size) {
                result[i++] = 0;
                result[i++] = sizeItem - 1;
            }
            return result;
        }

        private double[] Interpolate(double[] normal, int[] floor) {
            int floorPosition = GetSamplePosition(floor, size);
            double[] x = GetFloorWeights(normal, encode);
            int[] steps = GetInputDimensionSteps();
            double[] result = new double[outputDimension];
            switch (order) {
                case 1: {
                    for (int dim = 0; dim < outputDimension; dim++) {
                        result[dim] = InterpolateOrder1(x, floorPosition, steps, steps.Length, dim);
                    }
                    return result;
                }

                case 3: {
                    for (int dim = 0; dim < outputDimension; dim++) {
                        result[dim] = InterpolateOrder3(x, floor, floorPosition, steps, steps.Length, dim);
                    }
                    return result;
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_ORDER);
                }
            }
        }

        private double InterpolateOrder1(double[] x, int floorPosition, int[] steps, int inDim, int outDim) {
            if (inDim == 0) {
                return GetValue(outDim, floorPosition);
            }
            int step = steps[--inDim];
            int encodeIndex = inDim << 1;
            double value0 = InterpolateOrder1(x, floorPosition, steps, inDim, outDim);
            if (encode[encodeIndex] == encode[encodeIndex + 1]) {
                return value0;
            }
            int ceilPosition = floorPosition + step;
            double value1 = InterpolateOrder1(x, ceilPosition, steps, inDim, outDim);
            return CalculateLinearInterpolationFormula(x[inDim], value0, value1);
        }

        private double InterpolateOrder3(double[] x, int[] floor, int floorPosition, int[] steps, int inDim, int outDim
            ) {
            if (inDim == 0) {
                return GetValue(outDim, floorPosition);
            }
            int step = steps[--inDim];
            int encodeIndex = inDim << 1;
            double value1 = InterpolateOrder3(x, floor, floorPosition, steps, inDim, outDim);
            if (encode[encodeIndex] == encode[encodeIndex + 1]) {
                return value1;
            }
            int ceilPosition = floorPosition + step;
            double value2 = InterpolateOrder3(x, floor, ceilPosition, steps, inDim, outDim);
            if (encode[encodeIndex + 1] - encode[encodeIndex] == 1) {
                return CalculateLinearInterpolationFormula(x[inDim], value1, value2);
            }
            double value0;
            if (floor[inDim] > encode[encodeIndex]) {
                value0 = InterpolateOrder3(x, floor, floorPosition - step, steps, inDim, outDim);
            }
            else {
                value0 = 2 * value1 - value2;
            }
            double value3;
            if (floor[inDim] < encode[encodeIndex + 1] - encode[encodeIndex] - 1) {
                value3 = InterpolateOrder3(x, floor, ceilPosition + step, steps, inDim, outDim);
            }
            else {
                value3 = 2 * value2 - value1;
            }
            return CalculateCubicInterpolationFormula(x[inDim], value0, value1, value2, value3);
        }

        private double[] InterpolateByCubicSpline(double normal, int position) {
            if (derivatives == null) {
                CalculateSecondDerivatives();
            }
            double x = GetFloorWeight(normal, encode[0], encode[1]);
            return CalculateCubicSplineFormula(x, position);
        }

        private double[] CalculateCubicSplineFormula(double x, int position) {
            double[] result = new double[outputDimension];
            for (int dim = 0; dim < outputDimension; dim++) {
                result[dim] = CalculateCubicSplineFormula(x, GetValue(dim, position), GetValue(dim, position + 1), derivatives
                    [dim][position - encode[0]], derivatives[dim][position - encode[0] + 1]);
            }
            return result;
        }

        /// <summary>Calculates second derivatives at each interpolation point by sweep method with 3-diagonal matrix.
        ///     </summary>
        private void CalculateSecondDerivatives() {
            derivatives = new double[outputDimension][];
            for (int dim = 0; dim < outputDimension; ++dim) {
                double[] f = new double[encode[1] - encode[0] - 1];
                for (int pos = encode[0]; pos < encode[1] - 1; ++pos) {
                    f[pos - encode[0]] = 6 * (GetValue(dim, pos) - 2 * GetValue(dim, pos + 1) + GetValue(dim, pos + 2));
                }
                derivatives[dim] = SpecialSweepMethod(f);
            }
        }

        /// <summary>Calculates function decoded values.</summary>
        /// <remarks>
        /// Calculates function decoded values.
        /// <para />
        /// Function values are stored sequentially in samples table. For a function with multidimensional input
        /// (more than one input variables), the sample values in the first dimension vary fastest,
        /// and the values in the last dimension vary slowest. Order example for size array [4, 4, 4]:
        /// f(0,0,0), f(1,0,0), f(2,0,0), f(3,0,0), f(0,1,0), f(1,1,0), ..., f(3,3,0), f(3,3,1), f(3,3,2), f(3,3,3).
        /// For example in this case f(1,1,0) has position 5.
        /// If the function has multiple output values each value shall occupy bitsPerSample bits and
        /// stored sequentially as well.
        /// </remarks>
        /// <param name="dim">output dimension coordinate (values from [0, ..., outputDimension - 1])</param>
        /// <param name="pos">position in samples table</param>
        /// <returns>function decoded value</returns>
        private double GetValue(int dim, int pos) {
            return Decode(sampleExtractor.Extract(samples, dim + outputDimension * pos), dim);
        }

        /// <summary>Gets a minimal distance between samples of same dimension in samples table for each dimension.</summary>
        /// <returns>for each dimension a minimal distance between samples of same dimension in samples table</returns>
        private int[] GetInputDimensionSteps() {
            int[] steps = new int[size.Length];
            steps[0] = 1;
            for (int i = 1; i < steps.Length; ++i) {
                steps[i] = steps[i - 1] * size[i - 1];
            }
            return steps;
        }

        /// <summary>Decode sampled value.</summary>
        /// <param name="x">sampled value</param>
        /// <param name="dim">output dimension coordinate (values from [0, ..., outputDimension - 1])</param>
        /// <returns>decoded value</returns>
        private double Decode(long x, int dim) {
            int index = dim << 1;
            return decode[index] + (decode[index + 1] - decode[index]) * x / decodeLimit;
        }

        private void SetErrorMessage(String message) {
            errorMessage = message;
            isValidated = true;
        }

        private bool IsInvalid() {
            if (isValidated) {
                return errorMessage != null;
            }
            if (base.GetDomain() == null || base.GetRange() == null || size == null) {
                SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_NOT_NULL_PARAMETERS);
                return true;
            }
            if (order != 1 && order != 3) {
                SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_ORDER);
                return true;
            }
            if (GetDomain().Length == 0 || GetDomain().Length % 2 == 1) {
                SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_DOMAIN);
                return true;
            }
            if (GetRange().Length == 0 || GetRange().Length % 2 == 1) {
                SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_RANGE);
                return true;
            }
            int inputDimension = GetDomain().Length >> 1;
            if (size == null || size.Length != inputDimension) {
                SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_SIZE);
                return true;
            }
            foreach (int s in size) {
                if (s <= 0) {
                    SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_SIZE);
                    return true;
                }
            }
            if (encode.Length != GetDomain().Length) {
                SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_ENCODE);
                return true;
            }
            for (int i = 0; i < encode.Length; i += 2) {
                if (encode[i + 1] < encode[i]) {
                    SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_ENCODE);
                    return true;
                }
            }
            if (decode.Length != GetRange().Length) {
                SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_DECODE);
                return true;
            }
            int samplesMinLength = (JavaUtil.ArraysToEnumerable(size).Aggregate(outputDimension * bitsPerSample, (x, y
                ) => x * y) + 7) / 8;
            if (samples == null || samples.Length < samplesMinLength) {
                SetErrorMessage(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_SAMPLES);
                return true;
            }
            isValidated = true;
            return false;
        }

        /// <summary>Interpolates function by linear interpolation formula using function values at neighbouring points.
        ///     </summary>
        /// <param name="x">input normalized to [0, 1] by neighbouring points</param>
        /// <param name="f0">function value at the left neighbouring point</param>
        /// <param name="f1">function value at the right neighbouring point</param>
        /// <returns>function value obtained by linear interpolation</returns>
        private static double CalculateLinearInterpolationFormula(double x, double f0, double f1) {
            return (1.0 - x) * f0 + x * f1;
        }

        /// <summary>Interpolates function by cubic interpolation formula using function values at neighbouring points.
        ///     </summary>
        /// <param name="x">input normalized to [0, 1] by neighbouring points</param>
        /// <param name="f0">function value at the next to left neighbouring point</param>
        /// <param name="f1">function value at the left neighbouring point</param>
        /// <param name="f2">function value at the right neighbouring point</param>
        /// <param name="f3">function value at the next to right neighbouring point</param>
        /// <returns>function value obtained by cubic interpolation</returns>
        private static double CalculateCubicInterpolationFormula(double x, double f0, double f1, double f2, double
             f3) {
            return f1 + 0.5 * x * (f2 - f0 + x * (2 * f0 - 5 * f1 + 4 * f2 - f3 + x * (3 * (f1 - f2) + f3 - f0)));
        }

        /// <summary>
        /// Interpolates function by cubic spline formula using function and its second derivative values at neighbouring
        /// points.
        /// </summary>
        /// <param name="x">input normalized to [0, 1] by neighbouring points</param>
        /// <param name="f0">function value in the left neighbouring point</param>
        /// <param name="f1">function value in the right neighbouring point</param>
        /// <param name="d0">second derivative value in the left neighbouring point</param>
        /// <param name="d1">second derivative value in the right neighbouring point</param>
        /// <returns>function value interpolated by cubic spline formula</returns>
        private static double CalculateCubicSplineFormula(double x, double f0, double f1, double d0, double d1) {
            double y = 1 - x;
            return f1 * x + f0 * y - x * y * (d0 * (y + 1) + d1 * (x + 1)) / 6;
        }
    }
}

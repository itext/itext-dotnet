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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Pdf.Function {
    /// <summary>The abstract PdfFunction class that represents the Function Dictionary or Stream PDF object.</summary>
    /// <remarks>
    /// The abstract PdfFunction class that represents the Function Dictionary or Stream PDF object.
    /// Holds common properties and methods and a factory method. (see ISO-320001 Chapter 7.10)
    /// </remarks>
    /// <typeparam name="T">
    /// Either a
    /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
    /// or a
    /// <see cref="iText.Kernel.Pdf.PdfStream"/>
    /// </typeparam>
    public abstract class AbstractPdfFunction<T> : PdfObjectWrapper<T>, IPdfFunction
        where T : PdfDictionary {
        private readonly int functionType;

        private double[] domain;

        private double[] range;

        /// <summary>Constructs a PdfFunction from a new PdfObject.</summary>
        /// <param name="pdfObject">The new, empty, object, created in a concrete implementation</param>
        /// <param name="functionType">The function type, can be 0, 2, 3 or 4</param>
        /// <param name="domain">
        /// the valid input domain, input will be clipped to this domain
        /// contains a min max pair per input component
        /// </param>
        /// <param name="range">
        /// the valid output range, oputput will be clipped to this range
        /// contains a min max pair per output component
        /// </param>
        protected internal AbstractPdfFunction(T pdfObject, int functionType, double[] domain, double[] range)
            : base(pdfObject) {
            this.functionType = functionType;
            if (domain != null) {
                this.domain = JavaUtil.ArraysCopyOf(domain, domain.Length);
                pdfObject.Put(PdfName.Domain, new PdfArray(domain));
            }
            if (range != null) {
                this.range = JavaUtil.ArraysCopyOf(range, range.Length);
                pdfObject.Put(PdfName.Range, new PdfArray(range));
            }
            pdfObject.Put(PdfName.FunctionType, new PdfNumber(functionType));
        }

        /// <summary>Constructs a PdfFunction from an existing PdfObject.</summary>
        /// <param name="pdfObject">
        /// Either a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// or a
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// </param>
        protected internal AbstractPdfFunction(T pdfObject)
            : base(pdfObject) {
            PdfNumber functionTypeObj = pdfObject.GetAsNumber(PdfName.FunctionType);
            functionType = functionTypeObj == null ? -1 : functionTypeObj.IntValue();
            PdfArray domainObj = pdfObject.GetAsArray(PdfName.Domain);
            domain = domainObj == null ? null : domainObj.ToDoubleArray();
            PdfArray rangeObj = pdfObject.GetAsArray(PdfName.Range);
            range = rangeObj == null ? null : rangeObj.ToDoubleArray();
        }

        /// <summary>The function type, (see ISO-320001 Table 38).</summary>
        /// <returns>The function type, either 0, 2, 3 or 4</returns>
        public virtual int GetFunctionType() {
            return functionType;
        }

        /// <summary>Chacks wether the output of the function matches in components with the passed by color space.</summary>
        /// <param name="alternateSpace">The color space to verify against</param>
        /// <returns>True when compatible</returns>
        public virtual bool CheckCompatibilityWithColorSpace(PdfColorSpace alternateSpace) {
            return GetOutputSize() == alternateSpace.GetNumberOfComponents();
        }

        /// <summary>The number of input components.</summary>
        /// <returns>The number of input components</returns>
        public virtual int GetInputSize() {
            return GetPdfObject().GetAsArray(PdfName.Domain).Size() / 2;
        }

        /// <summary>The number of output components.</summary>
        /// <returns>The number of output components</returns>
        public virtual int GetOutputSize() {
            return range == null ? 0 : (range.Length / 2);
        }

        /// <summary>The valid input domain, input will be clipped to this domain contains a min max pair per input component.
        ///     </summary>
        /// <remarks>
        /// The valid input domain, input will be clipped to this domain contains a min max pair per input component.
        /// <para />
        /// (see ISO-320001 Table 38)
        /// </remarks>
        /// <returns>the input domain</returns>
        public virtual double[] GetDomain() {
            if (domain == null) {
                return null;
            }
            return JavaUtil.ArraysCopyOf(domain, domain.Length);
        }

        /// <summary>The valid input domain, input will be clipped to this domain contains a min max pair per input component.
        ///     </summary>
        /// <remarks>
        /// The valid input domain, input will be clipped to this domain contains a min max pair per input component.
        /// <para />
        /// (see ISO-320001 Table 38)
        /// </remarks>
        /// <param name="value">the new set of limits</param>
        public virtual void SetDomain(double[] value) {
            domain = JavaUtil.ArraysCopyOf(value, value.Length);
            GetPdfObject().Put(PdfName.Domain, new PdfArray(domain));
        }

        /// <summary>the valid output range, output will be clipped to this range contains a min max pair per output component.
        ///     </summary>
        /// <remarks>
        /// the valid output range, output will be clipped to this range contains a min max pair per output component.
        /// <para />
        /// (see ISO-320001 Table 38)
        /// </remarks>
        /// <returns>the output range</returns>
        public virtual double[] GetRange() {
            if (range != null) {
                return JavaUtil.ArraysCopyOf(range, range.Length);
            }
            return null;
        }

        /// <summary>the valid output range, output will be clipped to this range contains a min max pair per output component.
        ///     </summary>
        /// <remarks>
        /// the valid output range, output will be clipped to this range contains a min max pair per output component.
        /// <para />
        /// (see ISO-320001 Table 38)
        /// </remarks>
        /// <param name="value">the new set of limts</param>
        public virtual void SetRange(double[] value) {
            if (value == null) {
                GetPdfObject().Remove(PdfName.Range);
                return;
            }
            range = JavaUtil.ArraysCopyOf(value, value.Length);
            GetPdfObject().Put(PdfName.Range, new PdfArray(range));
        }

        /// <summary>Performs the calculation in bulk on a set of raw data and returns a new set of raw data.</summary>
        /// <param name="bytes">The uninterpreted set of data to be transformed</param>
        /// <param name="offset">Where to start converting the data</param>
        /// <param name="length">How many of the input bytes should be converted</param>
        /// <param name="wordSizeInputLength">How many bytes represents one input value</param>
        /// <param name="wordSizeOutputLength">How many bytes represents one output value</param>
        /// <returns>the transformed result as a raw byte array</returns>
        public virtual byte[] CalculateFromByteArray(byte[] bytes, int offset, int length, int wordSizeInputLength
            , int wordSizeOutputLength) {
            return CalculateFromByteArray(bytes, offset, length, wordSizeInputLength, wordSizeOutputLength, null, null
                );
        }

        /// <summary>Performs the calculation in bulk on a set of raw data and returns a new set of raw data.</summary>
        /// <param name="bytes">The uninterpreted set of data to be transformed</param>
        /// <param name="offset">Where to start converting the data</param>
        /// <param name="length">How many of the input bytes should be converted</param>
        /// <param name="wordSizeInputLength">How many bytes represents one input value</param>
        /// <param name="wordSizeOutputLength">How many bytes represents one output value</param>
        /// <param name="inputConvertor">a custom input convertor</param>
        /// <param name="outputConvertor">a custom output convertor</param>
        /// <returns>the transformed result as a raw byte array</returns>
        public virtual byte[] CalculateFromByteArray(byte[] bytes, int offset, int length, int wordSizeInputLength
            , int wordSizeOutputLength, BaseInputOutPutConvertors.IInputConversionFunction inputConvertor, BaseInputOutPutConvertors.IOutputConversionFunction
             outputConvertor) {
            int bytesPerInputWord = (int)Math.Ceiling(wordSizeInputLength / 8.0);
            int bytesPerOutputWord = (int)Math.Ceiling(wordSizeOutputLength / 8.0);
            int inputSize = GetInputSize();
            int outputSize = GetOutputSize();
            BaseInputOutPutConvertors.IInputConversionFunction actualInputConvertor = inputConvertor;
            if (actualInputConvertor == null) {
                actualInputConvertor = BaseInputOutPutConvertors.GetInputConvertor(bytesPerInputWord, 1);
            }
            BaseInputOutPutConvertors.IOutputConversionFunction actualOutputConvertor = outputConvertor;
            if (actualOutputConvertor == null) {
                actualOutputConvertor = BaseInputOutPutConvertors.GetOutputConvertor(bytesPerOutputWord, 1.0);
            }
            double[] inValues = actualInputConvertor(bytes, offset, length);
            double[] outValues = new double[inValues.Length / inputSize * outputSize];
            int outIndex = 0;
            for (int i = 0; i < inValues.Length; i += inputSize) {
                double[] singleRes = Calculate(JavaUtil.ArraysCopyOfRange(inValues, i, i + inputSize));
                Array.Copy(singleRes, 0, outValues, outIndex, singleRes.Length);
                outIndex += singleRes.Length;
            }
            return actualOutputConvertor(outValues);
        }

        /// <summary>Clip input values to the allowed domain.</summary>
        /// <remarks>
        /// Clip input values to the allowed domain.
        /// <para />
        /// (see ISO-320001 Table 38)
        /// </remarks>
        /// <param name="input">the input values to be clipped</param>
        /// <returns>the values clipped between the boundaries defined in the domain</returns>
        public virtual double[] ClipInput(double[] input) {
            if (input.Length * 2 != domain.Length) {
                throw new ArgumentException(KernelExceptionMessageConstant.INPUT_NOT_MULTIPLE_OF_DOMAIN_SIZE);
            }
            return Clip(input, domain);
        }

        /// <summary>Clip output values to the allowed range, if there is a range.</summary>
        /// <remarks>
        /// Clip output values to the allowed range, if there is a range.
        /// <para />
        /// (see ISO-320001 Table 38)
        /// </remarks>
        /// <param name="input">the output values to be clipped</param>
        /// <returns>the values clipped between the boundaries defined in the range</returns>
        public virtual double[] ClipOutput(double[] input) {
            if (range == null) {
                return input;
            }
            if (input.Length * 2 != range.Length) {
                throw new ArgumentException(KernelExceptionMessageConstant.INPUT_NOT_MULTIPLE_OF_RANGE_SIZE);
            }
            return Clip(input, range);
        }

        public virtual PdfObject GetAsPdfObject() {
            return base.GetPdfObject();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        protected internal static double[] Clip(double[] values, double[] limits) {
            System.Diagnostics.Debug.Assert((values.Length * 2 == limits.Length));
            double[] result = new double[values.Length];
            int j = 0;
            for (int i = 0; i < values.Length; ++i) {
                double lowerBound = limits[j++];
                double upperBound = limits[j++];
                result[i] = Math.Min(Math.Max(lowerBound, values[i]), upperBound);
            }
            return result;
        }

        protected internal static double[] Normalize(double[] values, double[] limits) {
            System.Diagnostics.Debug.Assert((values.Length * 2 == limits.Length));
            double[] normal = new double[values.Length];
            int j = 0;
            for (int i = 0; i < values.Length; ++i) {
                double lowerBound = limits[j++];
                double upperBound = Math.Max(lowerBound + double.Epsilon, limits[j++]);
                normal[i] = Math.Min(Math.Max(0, (values[i] - lowerBound) / (upperBound - lowerBound)), 1);
            }
            return normal;
        }

        protected internal static double[] ConvertFloatArrayToDoubleArray(float[] array) {
            if (array == null) {
                return null;
            }
            double[] arrayDouble = new double[array.Length];
            for (int i = 0; i < array.Length; i++) {
                arrayDouble[i] = array[i];
            }
            return arrayDouble;
        }

        public abstract double[] Calculate(double[] arg1);
    }
}

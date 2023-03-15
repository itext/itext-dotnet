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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Pdf.Function {
    /// <summary>
    /// This class represents Pdf type 3 function that defines a stitching of the subdomains
    /// of several 1-input functions to produce a single new 1-input function.
    /// </summary>
    /// <remarks>
    /// This class represents Pdf type 3 function that defines a stitching of the subdomains
    /// of several 1-input functions to produce a single new 1-input function.
    /// <para />
    /// For more info see ISO 32000-1, section 7.10.4 "Type 3 (Stitching) Functions".
    /// </remarks>
    public class PdfType3Function : AbstractPdfFunction<PdfDictionary> {
        private static readonly IPdfFunctionFactory DEFAULT_FUNCTION_FACTORY = (pdfObject) => {
            return PdfFunctionFactory.Create(pdfObject);
        }
        ;

        private readonly IPdfFunctionFactory functionFactory;

        private IList<IPdfFunction> functions;

        private double[] bounds;

        private double[] encode;

        /// <summary>Instantiates a new PdfType3Function instance based on passed PdfDictionary instance.</summary>
        /// <param name="dict">the function dictionary</param>
        public PdfType3Function(PdfDictionary dict)
            : this(dict, DEFAULT_FUNCTION_FACTORY) {
        }

        /// <summary>(see ISO-320001 Table 41).</summary>
        /// <param name="domain">
        /// the valid input domain, input will be clipped to this domain
        /// contains a min max pair per input component
        /// </param>
        /// <param name="range">
        /// the valid output range, oputput will be clipped to this range
        /// contains a min max pair per output component
        /// </param>
        /// <param name="functions">The list of functions to stitch</param>
        /// <param name="bounds">
        /// (Required) An array of k − 1 numbers that, in combination with Domain, shall define
        /// the intervals to which each function from the Functions array shall apply.
        /// Bounds elements shall be in order of increasing value, and each value shall be within
        /// the domain defined by Domain.
        /// </param>
        /// <param name="encode">
        /// (Required) An array of 2 × k numbers that, taken in pairs, shall map each subset of the domain
        /// defined by Domain and the Bounds array to the domain of the corresponding function.
        /// </param>
        public PdfType3Function(double[] domain, double[] range, IList<AbstractPdfFunction<PdfDictionary>> functions
            , double[] bounds, double[] encode)
            : base(new PdfDictionary(), PdfFunctionFactory.FUNCTION_TYPE_3, domain, range) {
            functionFactory = DEFAULT_FUNCTION_FACTORY;
            PdfArray funcs = new PdfArray();
            foreach (AbstractPdfFunction<PdfDictionary> func in functions) {
                funcs.Add(func.GetPdfObject());
            }
            base.GetPdfObject().Put(PdfName.Functions, funcs);
            base.GetPdfObject().Put(PdfName.Bounds, new PdfArray(bounds));
            base.GetPdfObject().Put(PdfName.Encode, new PdfArray(encode));
        }

        /// <summary>(see ISO-320001 Table 41).</summary>
        /// <param name="domain">
        /// the valid input domain, input will be clipped to this domain
        /// contains a min max pair per input component
        /// </param>
        /// <param name="range">
        /// the valid output range, oputput will be clipped to this range
        /// contains a min max pair per output component
        /// </param>
        /// <param name="functions">The list of functions to stitch</param>
        /// <param name="bounds">
        /// (Required) An array of k − 1 numbers that, in combination with Domain, shall define
        /// the intervals to which each function from the Functions array shall apply.
        /// Bounds elements shall be in order of increasing value, and each value shall be within
        /// the domain defined by Domain.
        /// </param>
        /// <param name="encode">
        /// (Required) An array of 2 × k numbers that, taken in pairs, shall map each subset of the domain
        /// defined by Domain and the Bounds array to the domain of the corresponding function.
        /// </param>
        public PdfType3Function(float[] domain, float[] range, IList<AbstractPdfFunction<PdfDictionary>> functions
            , float[] bounds, float[] encode)
            : this(ConvertFloatArrayToDoubleArray(domain), ConvertFloatArrayToDoubleArray(range), functions, ConvertFloatArrayToDoubleArray
                (bounds), ConvertFloatArrayToDoubleArray(encode)) {
        }

        internal PdfType3Function(PdfDictionary dict, IPdfFunctionFactory functionFactory)
            : base(dict) {
            this.functionFactory = functionFactory;
            PdfArray functionsArray = dict.GetAsArray(PdfName.Functions);
            functions = JavaCollectionsUtil.UnmodifiableList(CheckAndGetFunctions(functionsArray));
            if (base.GetDomain().Length < 2) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_DOMAIN);
            }
            PdfArray boundsArray = dict.GetAsArray(PdfName.Bounds);
            bounds = CheckAndGetBounds(boundsArray);
            PdfArray encodeArray = dict.GetAsArray(PdfName.Encode);
            encode = CheckAndGetEncode(encodeArray);
        }

        /// <summary>(Required) An array of k 1-input functions that shall make up the stitching function.</summary>
        /// <remarks>
        /// (Required) An array of k 1-input functions that shall make up the stitching function.
        /// The output dimensionality of all functions shall be the same, and compatible with the value
        /// of Range if Range is present.
        /// <para />
        /// (see ISO-320001 Table 41)
        /// </remarks>
        /// <returns>the list of functions</returns>
        public virtual ICollection<IPdfFunction> GetFunctions() {
            return functions;
        }

        /// <summary>(Required) An array of k 1-input functions that shall make up the stitching function.</summary>
        /// <remarks>
        /// (Required) An array of k 1-input functions that shall make up the stitching function.
        /// The output dimensionality of all functions shall be the same, and compatible with the value
        /// of Range if Range is present.
        /// <para />
        /// (see ISO-320001 Table 41)
        /// </remarks>
        /// <param name="value">the list of functions</param>
        public virtual void SetFunctions(IEnumerable<AbstractPdfFunction<PdfDictionary>> value) {
            PdfArray pdfFunctions = new PdfArray();
            foreach (AbstractPdfFunction<PdfDictionary> f in value) {
                pdfFunctions.Add(f.GetPdfObject().GetIndirectReference());
            }
            GetPdfObject().Put(PdfName.Functions, pdfFunctions);
        }

        /// <summary>
        /// An array of k − 1 numbers that, in combination with Domain, shall define
        /// the intervals to which each function from the Functions array shall apply.
        /// </summary>
        /// <remarks>
        /// An array of k − 1 numbers that, in combination with Domain, shall define
        /// the intervals to which each function from the Functions array shall apply.
        /// Bounds elements shall be in order of increasing value, and each value shall be within
        /// the domain defined by Domain.
        /// <para />
        /// (see ISO-320001 Table 41)
        /// </remarks>
        /// <returns>the bounds</returns>
        public virtual double[] GetBounds() {
            return bounds;
        }

        /// <summary>
        /// (Required) An array of k − 1 numbers that, in combination with Domain, shall define
        /// the intervals to which each function from the Functions array shall apply.
        /// </summary>
        /// <remarks>
        /// (Required) An array of k − 1 numbers that, in combination with Domain, shall define
        /// the intervals to which each function from the Functions array shall apply.
        /// Bounds elements shall be in order of increasing value, and each value shall be within
        /// the domain defined by Domain.
        /// <para />
        /// (see ISO-320001 Table 41)
        /// </remarks>
        /// <param name="value">the new set of bounds</param>
        public virtual void SetBounds(double[] value) {
            bounds = JavaUtil.ArraysCopyOf(value, value.Length);
        }

        /// <summary>
        /// An array of 2 × k numbers that, taken in pairs, shall map each subset of the domain defined
        /// by Domain and the Bounds array to the domain of the corresponding function.
        /// </summary>
        /// <remarks>
        /// An array of 2 × k numbers that, taken in pairs, shall map each subset of the domain defined
        /// by Domain and the Bounds array to the domain of the corresponding function.
        /// <para />
        /// (see ISO-320001 Table 41)
        /// </remarks>
        /// <returns>the encode values</returns>
        public virtual double[] GetEncode() {
            return GetPdfObject().GetAsArray(PdfName.Encode).ToDoubleArray();
        }

        /// <summary>
        /// (Required) An array of 2 × k numbers that, taken in pairs, shall map each subset of the domain defined
        /// by Domain and the Bounds array to the domain of the corresponding function.
        /// </summary>
        /// <remarks>
        /// (Required) An array of 2 × k numbers that, taken in pairs, shall map each subset of the domain defined
        /// by Domain and the Bounds array to the domain of the corresponding function.
        /// <para />
        /// (see ISO-320001 Table 41)
        /// </remarks>
        /// <param name="value">the new set of encodings</param>
        public virtual void SetEncode(double[] value) {
            GetPdfObject().Put(PdfName.Encode, new PdfArray(value));
        }

        public override bool CheckCompatibilityWithColorSpace(PdfColorSpace alternateSpace) {
            return false;
        }

        /// <summary>Gets output size of function.</summary>
        /// <remarks>
        /// Gets output size of function.
        /// <para />
        /// If Range field is absent, the output size of functions will be returned.
        /// </remarks>
        /// <returns>output size of function</returns>
        public override int GetOutputSize() {
            return GetRange() == null ? functions[0].GetOutputSize() : GetRange().Length / 2;
        }

        public override double[] Calculate(double[] input) {
            if (input == null || input.Length != 1) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_INPUT_FOR_TYPE_3_FUNCTION);
            }
            double[] clipped = ClipInput(input);
            double x = clipped[0];
            int subdomain = CalculateSubdomain(x);
            double[] subdomainBorders = GetSubdomainBorders(subdomain);
            x = MapValueFromActualRangeToExpected(x, subdomainBorders[0], subdomainBorders[1], encode[subdomain * 2], 
                encode[(subdomain * 2) + 1]);
            double[] output = functions[subdomain].Calculate(new double[] { x });
            return ClipOutput(output);
        }

        private int CalculateSubdomain(double inputValue) {
            if (bounds.Length > 0) {
                if (AreThreeDoubleEqual(bounds[0], GetDomain()[0], inputValue)) {
                    return 0;
                }
                if (AreThreeDoubleEqual(bounds[bounds.Length - 1], GetDomain()[1], inputValue)) {
                    return bounds.Length;
                }
            }
            for (int i = 0; i < bounds.Length; i++) {
                if (inputValue < bounds[i]) {
                    return i;
                }
            }
            return bounds.Length;
        }

        private double[] GetSubdomainBorders(int subdomain) {
            if (bounds.Length == 0) {
                return GetDomain();
            }
            if (subdomain == 0) {
                return new double[] { GetDomain()[0], bounds[0] };
            }
            else {
                if (subdomain == bounds.Length) {
                    return new double[] { bounds[bounds.Length - 1], GetDomain()[1] };
                }
                else {
                    return new double[] { bounds[subdomain - 1], bounds[subdomain] };
                }
            }
        }

        private IList<IPdfFunction> CheckAndGetFunctions(PdfArray functionsArray) {
            if (functionsArray == null || functionsArray.Size() == 0) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_NULL_FUNCTIONS);
            }
            int? tempOutputSize = null;
            if (GetRange() != null) {
                tempOutputSize = GetOutputSize();
            }
            IList<IPdfFunction> tempFunctions = new List<IPdfFunction>();
            foreach (PdfObject funcObj in functionsArray) {
                if (!(funcObj is PdfDictionary)) {
                    continue;
                }
                PdfDictionary funcDict = (PdfDictionary)funcObj;
                IPdfFunction tempFunc = functionFactory(funcDict);
                if (tempOutputSize == null) {
                    tempOutputSize = tempFunc.GetOutputSize();
                }
                if (tempOutputSize != tempFunc.GetOutputSize()) {
                    throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_FUNCTIONS_OUTPUT);
                }
                if (tempFunc.GetInputSize() != 1) {
                    throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_FUNCTIONS_INPUT);
                }
                tempFunctions.Add(tempFunc);
            }
            return tempFunctions;
        }

        private double[] CheckAndGetBounds(PdfArray boundsArray) {
            if (boundsArray == null || boundsArray.Size() != (functions.Count - 1)) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_NULL_BOUNDS);
            }
            double[] bounds = boundsArray.ToDoubleArray();
            bool areBoundsInvalid = false;
            for (int i = 0; i < bounds.Length; i++) {
                areBoundsInvalid |= i == 0 ? bounds[i] < GetDomain()[0] : bounds[i] <= GetDomain()[0];
                areBoundsInvalid |= i == bounds.Length - 1 ? GetDomain()[1] < bounds[i] : GetDomain()[1] <= bounds[i];
                areBoundsInvalid |= (i != 0 && bounds[i] <= bounds[i - 1]);
            }
            if (areBoundsInvalid) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_BOUNDS);
            }
            return bounds;
        }

        private double[] CheckAndGetEncode(PdfArray encodeArray) {
            if (encodeArray == null || encodeArray.Size() < (functions.Count * 2)) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_NULL_ENCODE);
            }
            return encodeArray.ToDoubleArray();
        }

        /// <summary>Maps passed value from actual range to expected range.</summary>
        /// <param name="value">the value to map</param>
        /// <param name="aStart">the start of actual range</param>
        /// <param name="aEnd">the end of actual range</param>
        /// <param name="eStart">the start of expected range</param>
        /// <param name="eEnd">the end of expected range</param>
        /// <returns>the mapped value</returns>
        private static double MapValueFromActualRangeToExpected(double value, double aStart, double aEnd, double eStart
            , double eEnd) {
            // Present ranges [start, end] as [0, ...RangeLength].
            double actualRangeLength = aEnd - aStart;
            if (actualRangeLength == 0) {
                return eStart;
            }
            double expectedRangeLength = eEnd - eStart;
            // New input value = value - actual.start.
            double x = value - aStart;
            double y = (expectedRangeLength / actualRangeLength) * x;
            // Map y from range [0, expectedRangeLength] to [eStart, eEnd].
            return eStart + y;
        }

        private static bool AreThreeDoubleEqual(double first, double second, double third) {
            return JavaUtil.DoubleCompare(first, second) == 0 && JavaUtil.DoubleCompare(second, third) == 0;
        }
    }
}

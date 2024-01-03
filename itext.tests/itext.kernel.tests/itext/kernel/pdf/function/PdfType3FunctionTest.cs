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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Function {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfType3FunctionTest : ExtendedITextTest {
        private const double EPSILON = 10e-6;

        [NUnit.Framework.Test]
        public virtual void ConstructorNullFunctionsTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.Remove(PdfName.Functions);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_NULL_FUNCTIONS, ex.
                Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorZeroSizeOfFunctionsTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.Put(PdfName.Functions, new PdfArray());
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_NULL_FUNCTIONS, ex.
                Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorDifferentOutputSizeOfFunctionsTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.GetAsArray(PdfName.Functions).GetAsDictionary(0).Put(PdfName.Range, new PdfArray(new double[] { 
                -100, 100, -100, 100 }));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_FUNCTIONS_OUTPUT, ex
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorDifferentOutputSizeFuncWithRangeTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.Put(PdfName.Range, new PdfArray(new double[] { -100, 100, -100, 100 }));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_FUNCTIONS_OUTPUT, ex
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidInputSizeOfFuncTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            IPdfFunctionFactory customFactory = (dict) => new PdfType3FunctionTest.CustomPdfFunction((PdfDictionary)dict
                , 2, 1);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func, customFactory
                ));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_FUNCTIONS_INPUT, ex
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorIgnoreNotDictFunctionsTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            type3FuncDict.GetAsArray(PdfName.Functions).Add(new PdfNumber(1));
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            NUnit.Framework.Assert.AreEqual(2, type3Function.GetFunctions().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidFunctionTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.GetAsArray(PdfName.Functions).GetAsDictionary(0).Remove(PdfName.N);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_N, ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorNullBoundsTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.Remove(PdfName.Bounds);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_NULL_BOUNDS, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidSizeOfBoundsTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.Put(PdfName.Bounds, new PdfArray());
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_NULL_BOUNDS, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidBoundsLessThanDomainTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.GetAsArray(PdfName.Bounds).Remove(0);
            type3Func.GetAsArray(PdfName.Bounds).Add(new PdfNumber(-1));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_BOUNDS, ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidBoundsMoreThanDomainTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.GetAsArray(PdfName.Bounds).Remove(0);
            type3Func.GetAsArray(PdfName.Bounds).Add(new PdfNumber(3));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_BOUNDS, ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidBoundsLessThanPreviousTest() {
            PdfDictionary type3Func = new PdfDictionary();
            type3Func.Put(PdfName.FunctionType, new PdfNumber(3));
            PdfArray domain = new PdfArray(new int[] { 0, 1 });
            type3Func.Put(PdfName.Domain, domain);
            PdfArray functions = new PdfArray(PdfFunctionUtil.CreateMinimalPdfType2FunctionDict());
            functions.Add(PdfFunctionUtil.CreateMinimalPdfType2FunctionDict());
            functions.Add(PdfFunctionUtil.CreateMinimalPdfType2FunctionDict());
            type3Func.Put(PdfName.Functions, functions);
            type3Func.Put(PdfName.Bounds, new PdfArray(new double[] { 1, 0.5 }));
            type3Func.Put(PdfName.Encode, new PdfArray(new double[] { 0, 1, 0, 1, 0, 1 }));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_BOUNDS, ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorNullEncodeTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.Remove(PdfName.Encode);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_NULL_ENCODE, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidSizeOfEncodeTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.Put(PdfName.Encode, new PdfArray());
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_NULL_ENCODE, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidDomainTest() {
            PdfDictionary type3Func = CreateMinimalPdfType3FunctionDict();
            type3Func.Put(PdfName.Domain, new PdfArray(new double[] { 1 }));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType3Function(type3Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_3_FUNCTION_DOMAIN, ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetOutputSizeNullRangeTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            IPdfFunctionFactory customFactory = (dict) => new PdfType3FunctionTest.CustomPdfFunction((PdfDictionary)dict
                , 1, 7);
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict, customFactory);
            NUnit.Framework.Assert.AreEqual(7, type3Function.GetOutputSize());
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodeTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            type3FuncDict.Put(PdfName.Encode, new PdfArray(new double[] { 0, 0.37, -1, 0 }));
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            iText.Test.TestUtil.AreEqual(new double[] { 0, 0.37, -1, 0 }, type3Function.GetEncode(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void GetBoundsTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            type3FuncDict.Put(PdfName.Bounds, new PdfArray(new double[] { 0.789 }));
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            iText.Test.TestUtil.AreEqual(new double[] { 0.789 }, type3Function.GetBounds(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateInvalid2NumberInputTest() {
            PdfType3Function type3Func = new PdfType3Function(CreateMinimalPdfType3FunctionDict());
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => type3Func.Calculate(new double[] { 
                0, 1 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_INPUT_FOR_TYPE_3_FUNCTION, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CalculateInvalidNullInputTest() {
            PdfType3Function type3Func = new PdfType3Function(CreateMinimalPdfType3FunctionDict());
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => type3Func.Calculate(null));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_INPUT_FOR_TYPE_3_FUNCTION, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CalculateInputClipTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            double[] output = type3Function.Calculate(new double[] { -5 });
            // input value was clipped to 0 from -5
            iText.Test.TestUtil.AreEqual(new double[] { 0 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateDomainOnePointIntervalTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            type3FuncDict.Put(PdfName.Bounds, new PdfArray());
            type3FuncDict.GetAsArray(PdfName.Functions).Remove(1);
            type3FuncDict.Put(PdfName.Domain, new PdfArray(new double[] { 0.5, 0.5 }));
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            double[] output = type3Function.Calculate(new double[] { 7 });
            iText.Test.TestUtil.AreEqual(new double[] { 0 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateInputClipByFuncTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            type3FuncDict.GetAsArray(PdfName.Functions).GetAsDictionary(0).Put(PdfName.Domain, new PdfArray(new double
                [] { 2, 3 }));
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            double[] output = type3Function.Calculate(new double[] { 0.1 });
            // input value 0.1 was passed to first function with domain [2, 3], so value was clipped to 2 from 0.1
            iText.Test.TestUtil.AreEqual(new double[] { 4 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateInputValueEqualBoundsTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            type3FuncDict.GetAsArray(PdfName.Functions).GetAsDictionary(1).Put(PdfName.C0, new PdfArray(new double[] { 
                -3 }));
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            double[] output = type3Function.Calculate(new double[] { 0.5 });
            // Input value 0.5 was passed to second function.
            // Subdomain is [0.5, 1], encode is [0, 1], so value 0.5 was encoded to 0.
            iText.Test.TestUtil.AreEqual(new double[] { -3 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateInputValueNotEqualBoundsTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            double[] output = type3Function.Calculate(new double[] { 0.53 });
            // Input value 0.53 was passed to second function.
            // Subdomain is [0.5, 1], encode is [0, 1], so value 0.53 was encoded to 0.06.
            iText.Test.TestUtil.AreEqual(new double[] { 0.06 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateInputValueEqualDomainTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            double[] output = type3Function.Calculate(new double[] { 1 });
            iText.Test.TestUtil.AreEqual(new double[] { 1 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateWith3FunctionsTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            PdfDictionary minimalType2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            minimalType2Func.Put(PdfName.N, new PdfNumber(3));
            type3FuncDict.GetAsArray(PdfName.Functions).Add(1, minimalType2Func);
            type3FuncDict.Put(PdfName.Encode, new PdfArray(new double[] { 0, 1, 0, 1, 0, 1 }));
            type3FuncDict.Put(PdfName.Bounds, new PdfArray(new double[] { 0.5, 0.7 }));
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            double[] output = type3Function.Calculate(new double[] { 0.52 });
            // Input value 0.52 was passed to second function.
            // Subdomain is [0.5, 0.7], encode is [0, 1], so value 0.52 was encoded to 0.1.
            iText.Test.TestUtil.AreEqual(new double[] { 0.001 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateReverseEncodingTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            type3FuncDict.Put(PdfName.Encode, new PdfArray(new double[] { 0, 1, 1, 0 }));
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            iText.Test.TestUtil.AreEqual(new double[] { 0, 1, 1, 0 }, type3Function.GetEncode(), EPSILON);
            double[] output = type3Function.Calculate(new double[] { 1 });
            // Input value 1 was passed to second function.
            // Subdomain is [0.5, 1], encode is [1, 0], so value 1 was encoded to 0.
            iText.Test.TestUtil.AreEqual(new double[] { 0 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateOneFunctionTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            type3FuncDict.Put(PdfName.Bounds, new PdfArray());
            type3FuncDict.GetAsArray(PdfName.Functions).Remove(1);
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            double[] output = type3Function.Calculate(new double[] { 0.6 });
            iText.Test.TestUtil.AreEqual(new double[] { 0.36 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateBoundsEqualLeftDomainTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            type3FuncDict.GetAsArray(PdfName.Functions).GetAsDictionary(0).Put(PdfName.C0, new PdfArray(new double[] { 
                -3 }));
            type3FuncDict.GetAsArray(PdfName.Functions).GetAsDictionary(1).Put(PdfName.C1, new PdfArray(new double[] { 
                5 }));
            type3FuncDict.Put(PdfName.Bounds, new PdfArray(new double[] { 0 }));
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            double[] output = type3Function.Calculate(new double[] { 0 });
            // first function was used
            iText.Test.TestUtil.AreEqual(new double[] { -3 }, output, EPSILON);
            output = type3Function.Calculate(new double[] { 0.1 });
            // second function was used
            iText.Test.TestUtil.AreEqual(new double[] { 0.5 }, output, EPSILON);
            output = type3Function.Calculate(new double[] { 1 });
            // second function was used
            iText.Test.TestUtil.AreEqual(new double[] { 5 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateBoundsEqualRightDomainTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            type3FuncDict.GetAsArray(PdfName.Functions).GetAsDictionary(0).Put(PdfName.C1, new PdfArray(new double[] { 
                -3 }));
            type3FuncDict.GetAsArray(PdfName.Functions).GetAsDictionary(1).Put(PdfName.C0, new PdfArray(new double[] { 
                5 }));
            type3FuncDict.Put(PdfName.Bounds, new PdfArray(new double[] { 1 }));
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            double[] output = type3Function.Calculate(new double[] { 0 });
            // first function was used
            iText.Test.TestUtil.AreEqual(new double[] { 0 }, output, EPSILON);
            output = type3Function.Calculate(new double[] { 0.1 });
            // first function was used
            iText.Test.TestUtil.AreEqual(new double[] { -0.03 }, output, EPSILON);
            output = type3Function.Calculate(new double[] { 1 });
            // second function was used
            iText.Test.TestUtil.AreEqual(new double[] { 5 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateBoundsEqualLeftDomainWith3FuncTest() {
            PdfDictionary type3FuncDict = CreateMinimalPdfType3FunctionDict();
            type3FuncDict.GetAsArray(PdfName.Functions).GetAsDictionary(0).Put(PdfName.C0, new PdfArray(new double[] { 
                -3 }));
            type3FuncDict.GetAsArray(PdfName.Functions).GetAsDictionary(1).Put(PdfName.C1, new PdfArray(new double[] { 
                5 }));
            PdfDictionary minimalType2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            minimalType2Func.Put(PdfName.N, new PdfNumber(1));
            minimalType2Func.Put(PdfName.C1, new PdfArray(new double[] { -2 }));
            type3FuncDict.GetAsArray(PdfName.Functions).Add(minimalType2Func);
            type3FuncDict.Put(PdfName.Bounds, new PdfArray(new double[] { 0, 0.5 }));
            type3FuncDict.Put(PdfName.Encode, new PdfArray(new double[] { 0, 1, 0, 1, 0, 1 }));
            PdfType3Function type3Function = new PdfType3Function(type3FuncDict);
            double[] output = type3Function.Calculate(new double[] { 0 });
            // first function was used
            iText.Test.TestUtil.AreEqual(new double[] { -3 }, output, EPSILON);
            output = type3Function.Calculate(new double[] { 0.1 });
            // second function was used
            iText.Test.TestUtil.AreEqual(new double[] { 1 }, output, EPSILON);
            output = type3Function.Calculate(new double[] { 0.6 });
            // third function was used
            iText.Test.TestUtil.AreEqual(new double[] { -0.4 }, output, EPSILON);
            output = type3Function.Calculate(new double[] { 1 });
            // third function was used
            iText.Test.TestUtil.AreEqual(new double[] { -2 }, output, EPSILON);
        }

        private static PdfDictionary CreateMinimalPdfType3FunctionDict() {
            PdfDictionary type3Func = new PdfDictionary();
            type3Func.Put(PdfName.FunctionType, new PdfNumber(3));
            PdfArray domain = new PdfArray(new int[] { 0, 1 });
            type3Func.Put(PdfName.Domain, domain);
            PdfArray functions = new PdfArray(PdfFunctionUtil.CreateMinimalPdfType2FunctionDict());
            PdfDictionary minimalType2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            minimalType2Func.Put(PdfName.N, new PdfNumber(1));
            functions.Add(minimalType2Func);
            type3Func.Put(PdfName.Functions, functions);
            type3Func.Put(PdfName.Bounds, new PdfArray(new double[] { 0.5 }));
            type3Func.Put(PdfName.Encode, new PdfArray(new double[] { 0, 1, 0, 1 }));
            return type3Func;
        }

        private class CustomPdfFunction : AbstractPdfFunction<PdfDictionary> {
            private readonly int inputSize;

            private readonly int outputSize;

            protected internal CustomPdfFunction(PdfDictionary pdfObject, int inputSize, int outputSize)
                : base(pdfObject) {
                this.inputSize = inputSize;
                this.outputSize = outputSize;
            }

            public override int GetInputSize() {
                return inputSize;
            }

            public override int GetOutputSize() {
                return outputSize;
            }

            public override double[] Calculate(double[] input) {
                return new double[0];
            }

            protected internal override bool IsWrappedObjectMustBeIndirect() {
                return false;
            }
        }
    }
}

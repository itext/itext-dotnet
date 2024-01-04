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
    public class PdfType2FunctionTest : ExtendedITextTest {
        private const double EPSILON = 10e-6;

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidObjWithoutNTest() {
            PdfDictionary type2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2Func.Remove(PdfName.N);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType2Function(type2Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_N, ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidObjNNotNumberTest() {
            PdfDictionary type2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2Func.Put(PdfName.N, new PdfString("some text"));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType2Function(type2Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_N, ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidObjWithNonIntegerNTest() {
            PdfDictionary type2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2Func.Put(PdfName.N, new PdfNumber(2.3));
            PdfArray domain = type2Func.GetAsArray(PdfName.Domain);
            domain.Add(0, new PdfNumber(-1));
            domain.Remove(2);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType2Function(type2Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_N_NOT_INTEGER, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidObjWithNegativeNTest() {
            PdfDictionary type2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2Func.Put(PdfName.N, new PdfNumber(-2));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType2Function(type2Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_N_NEGATIVE, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidDomainTest() {
            PdfDictionary type2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2Func.Put(PdfName.Domain, new PdfArray(new double[] { 1 }));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType2Function(type2Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_DOMAIN, ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorCorrectDomainTest() {
            PdfDictionary type2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2Func.Put(PdfName.Domain, new PdfArray(new double[] { 1, 2, 3, 4 }));
            PdfType2Function type2Function = new PdfType2Function(type2Func);
            NUnit.Framework.Assert.AreEqual(2, type2Function.GetInputSize());
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidCDifferentSizeTest() {
            PdfDictionary type2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2Func.Put(PdfName.C0, new PdfArray(new int[] { 1, 2 }));
            type2Func.Put(PdfName.C1, new PdfArray(new int[] { 3 }));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType2Function(type2Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_OUTPUT_SIZE, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorInvalidCAndRangeDifferentSizeTest() {
            PdfDictionary type2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2Func.Put(PdfName.C0, new PdfArray(new int[] { 1, 2 }));
            type2Func.Put(PdfName.Range, new PdfArray(new int[] { 1, 3 }));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfType2Function(type2Func));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_TYPE_2_FUNCTION_OUTPUT_SIZE, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithRangeTest() {
            PdfDictionary type2FuncDict = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2FuncDict.Put(PdfName.Range, new PdfArray(new int[] { 1, 2, 3, 4, 5, 6 }));
            PdfType2Function type2Func = new PdfType2Function(type2FuncDict);
            iText.Test.TestUtil.AreEqual(new double[] { 0, 0, 0 }, type2Func.GetC0(), EPSILON);
            iText.Test.TestUtil.AreEqual(new double[] { 1, 1, 1 }, type2Func.GetC1(), EPSILON);
            NUnit.Framework.Assert.AreEqual(2, type2Func.GetN(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorMinimalTest() {
            PdfDictionary type2FuncDict = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            PdfType2Function type2Func = new PdfType2Function(type2FuncDict);
            iText.Test.TestUtil.AreEqual(new double[] { 0 }, type2Func.GetC0(), EPSILON);
            iText.Test.TestUtil.AreEqual(new double[] { 1 }, type2Func.GetC1(), EPSILON);
            NUnit.Framework.Assert.AreEqual(2, type2Func.GetN(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorFullTest() {
            PdfDictionary type2FuncDict = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2FuncDict.Put(PdfName.C0, new PdfArray(new int[] { 0, 1 }));
            type2FuncDict.Put(PdfName.C1, new PdfArray(new int[] { 1, 2 }));
            PdfType2Function type2Func = new PdfType2Function(type2FuncDict);
            NUnit.Framework.Assert.AreEqual(2, type2Func.GetN(), EPSILON);
            iText.Test.TestUtil.AreEqual(new double[] { 0, 1 }, type2Func.GetC0(), EPSILON);
            iText.Test.TestUtil.AreEqual(new double[] { 1, 2 }, type2Func.GetC1(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateInvalid2NumberInputTest() {
            PdfType2Function type2Func = new PdfType2Function(PdfFunctionUtil.CreateMinimalPdfType2FunctionDict());
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => type2Func.Calculate(new double[] { 
                0, 1 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_INPUT_FOR_TYPE_2_FUNCTION, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CalculateInvalidNullInputTest() {
            PdfType2Function type2Func = new PdfType2Function(PdfFunctionUtil.CreateMinimalPdfType2FunctionDict());
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => type2Func.Calculate(null));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_INPUT_FOR_TYPE_2_FUNCTION, ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CalculateInputClipTest() {
            PdfDictionary type2FuncDict = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2FuncDict.Put(PdfName.Domain, new PdfArray(new int[] { 1, 3 }));
            PdfType2Function type2Function = new PdfType2Function(type2FuncDict);
            double[] output = type2Function.Calculate(new double[] { 8 });
            // input value was clipped to 3 from 8
            iText.Test.TestUtil.AreEqual(new double[] { 9 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateTest() {
            PdfDictionary type2FuncDict = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2FuncDict.Put(PdfName.Domain, new PdfArray(new int[] { 1, 3 }));
            type2FuncDict.Put(PdfName.C0, new PdfArray(new int[] { 0, 1 }));
            type2FuncDict.Put(PdfName.C1, new PdfArray(new int[] { 0, -3 }));
            PdfType2Function type2Function = new PdfType2Function(type2FuncDict);
            double[] output = type2Function.Calculate(new double[] { 2 });
            iText.Test.TestUtil.AreEqual(new double[] { 0, -15 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateWithoutC0Test() {
            PdfDictionary type2FuncDict = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2FuncDict.Put(PdfName.Domain, new PdfArray(new int[] { 1, 3 }));
            type2FuncDict.Put(PdfName.C1, new PdfArray(new int[] { 0, -3 }));
            PdfType2Function type2Function = new PdfType2Function(type2FuncDict);
            double[] output = type2Function.Calculate(new double[] { 2 });
            iText.Test.TestUtil.AreEqual(new double[] { 0, -12 }, output, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateClipOutputTest() {
            PdfDictionary type2FuncDict = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            type2FuncDict.Put(PdfName.Domain, new PdfArray(new int[] { 1, 3 }));
            type2FuncDict.Put(PdfName.Range, new PdfArray(new int[] { -4, -2 }));
            PdfType2Function type2Function = new PdfType2Function(type2FuncDict);
            double[] output = type2Function.Calculate(new double[] { 2 });
            // output value was clipped to -2 from 4
            iText.Test.TestUtil.AreEqual(new double[] { -2 }, output, EPSILON);
        }
    }
}

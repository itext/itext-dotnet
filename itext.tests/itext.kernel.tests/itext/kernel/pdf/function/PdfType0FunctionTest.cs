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
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfType0FunctionTest : ExtendedITextTest {
        protected internal const double DELTA = 1e-12;

        [NUnit.Framework.Test]
        public virtual void TestEncoding() {
            int[][] encode = new int[][] { new int[] { 0, 1 }, new int[] { 0, 10 }, new int[] { 2, 7 }, new int[] { 13
                , 21 } };
            double[] x = new double[] { 0, 0.3, 0.5, 0.9, 1 };
            double[][] expected = new double[][] { x, new double[] { 0, 3, 5, 9, 10 }, new double[] { 2, 3.5, 4.5, 6.5
                , 7 }, new double[] { 13, 15.4, 17, 20.2, 21 } };
            for (int i = 0; i < encode.Length; ++i) {
                for (int j = 0; j < x.Length; ++j) {
                    double actual = PdfType0Function.Encode(x[j], encode[i][0], encode[i][1]);
                    NUnit.Framework.Assert.AreEqual(expected[i][j], actual, DELTA);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestFloor() {
            double[][] normals = new double[][] { new double[] { 0.0, 0.0, 0.0 }, new double[] { 0.5, 0.5, 0.6 }, new 
                double[] { 1.0, 1.0, 1.0 } };
            int[] encode = new int[] { 0, 1, 2, 7, 13, 21 };
            int[][] expected = new int[][] { new int[] { 0, 2, 13 }, new int[] { 0, 4, 17 }, new int[] { 0, 6, 20 } };
            for (int i = 0; i < normals.Length; ++i) {
                int[] actual = PdfType0Function.GetFloor(normals[i], encode);
                NUnit.Framework.Assert.AreEqual(expected[i], actual);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestSamplePositionDim1() {
            for (int size = 2; size < 5; ++size) {
                int[] sizeArr = new int[] { size };
                for (int sample = 0; sample < size; ++sample) {
                    int position = PdfType0Function.GetSamplePosition(new int[] { sample }, sizeArr);
                    NUnit.Framework.Assert.AreEqual(sample, position);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestSamplePositionDim3() {
            int[][] size = new int[][] { new int[] { 2, 2, 2 }, new int[] { 5, 5, 5 }, new int[] { 8, 13, 21 } };
            int[][][] samples = new int[][][] { new int[][] { new int[] { 0, 0, 0 }, new int[] { 1, 1, 1 }, new int[] 
                { 0, 1, 0 }, new int[] { 1, 0, 1 } }, new int[][] { new int[] { 0, 0, 0 }, new int[] { 4, 4, 4 }, new 
                int[] { 2, 3, 4 }, new int[] { 4, 3, 2 } }, new int[][] { new int[] { 0, 0, 0 }, new int[] { 7, 12, 20
                 }, new int[] { 0, 7, 20 }, new int[] { 6, 7, 8 } } };
            int[][] expected = new int[][] { new int[] { 0, 7, 2, 5 }, new int[] { 0, 124, 117, 69 }, new int[] { 0, 2183
                , 2136, 894 } };
            for (int i = 0; i < size.Length; ++i) {
                for (int j = 0; j < samples[i].Length; ++j) {
                    int actual = PdfType0Function.GetSamplePosition(samples[i][j], size[i]);
                    NUnit.Framework.Assert.AreEqual(expected[i][j], actual);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestFloorWeights() {
            double[][] normals = new double[][] { new double[] { 0.0, 0.0, 0.0 }, new double[] { 0.3, 0.5, 0.7 }, new 
                double[] { 1.0, 1.0, 1.0 } };
            int[] encode = new int[] { 0, 1, 2, 7, 13, 21 };
            double[][] expected = new double[][] { new double[] { 0, 0, 0 }, new double[] { 0.3, 0.5, 0.6 }, new double
                [] { 1, 1, 1 } };
            for (int i = 0; i < normals.Length; ++i) {
                double[] actual = PdfType0Function.GetFloorWeights(normals[i], encode);
                iText.Test.TestUtil.AreEqual(expected[i], actual, DELTA);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestFloorWeight() {
            double[] normals = new double[] { 0.0, 0.2, 0.4, 0.6, 0.8, 1.0 };
            int[] encode = new int[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 0, 34 };
            double[] expected = new double[] { 0.0, 0.2, 0.8, 0.0, 0.4, 1.0 };
            for (int i = 0; i < normals.Length; ++i) {
                double actual = PdfType0Function.GetFloorWeight(normals[i], encode[2 * i], encode[2 * i + 1]);
                NUnit.Framework.Assert.AreEqual(expected[i], actual, DELTA);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestSpecialSweepMethod() {
            double[][] rhsVectors = new double[][] { new double[] { 1 }, new double[] { 5, 5 }, new double[] { 5, 6, 5
                 }, new double[] { 6, 12, 18, 19 } };
            double[][] expected = new double[][] { new double[] { 0, 0.25, 0 }, new double[] { 0, 1, 1, 0 }, new double
                [] { 0, 1, 1, 1, 0 }, new double[] { 0, 1, 2, 3, 4, 0 } };
            for (int i = 0; i < rhsVectors.Length; ++i) {
                double[] actual = PdfType0Function.SpecialSweepMethod(rhsVectors[i]);
                iText.Test.TestUtil.AreEqual(expected[i], actual, DELTA);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestNoInputException() {
            AbstractPdfFunction<PdfStream> function = new PdfType0Function(new PdfStream());
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => function.Calculate(new double[
                ] { 0 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_NOT_NULL_PARAMETERS, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestSimpleValidPdfFunction() {
            AbstractPdfFunction<PdfStream> function = GenerateSimplePdfFunction(new byte[] { 0 }, 1);
            NUnit.Framework.Assert.DoesNotThrow(() => function.Calculate(new double[] { 0 }));
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidBitsPerSampleException() {
            PdfType0Function function = GenerateSimplePdfFunction(new byte[] { 0 }, 3);
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => function.Calculate(new double[
                ] { 0 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_BITS_PER_SAMPLE_INVALID_VALUE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidOrderException() {
            PdfType0Function function = GenerateSimplePdfFunction(new byte[] { 0 }, 1);
            function.SetOrder(2);
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => function.Calculate(new double[
                ] { 0 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_ORDER, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidDomainException() {
            PdfType0Function function = GenerateSimplePdfFunction(new byte[] { 0 }, 1);
            function.SetDomain(new double[] { 0, 1, 1 });
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => function.Calculate(new double[
                ] { 0 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_DOMAIN, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidRangeException() {
            PdfType0Function function = GenerateSimplePdfFunction(new byte[] { 0 }, 1);
            function.SetRange(new double[] { 0, 1, 1 });
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => function.Calculate(new double[
                ] { 0 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_RANGE, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidSizeException() {
            PdfType0Function function = GenerateSimplePdfFunction(new byte[] { 0 }, 1);
            function.SetSize(new int[] { 2, 2 });
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => function.Calculate(new double[
                ] { 0 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_SIZE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidEncodeException() {
            PdfType0Function function = GenerateSimplePdfFunction(new byte[] { 0 }, 1);
            function.SetEncode(new int[] { 3, 4 });
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => function.Calculate(new double[
                ] { 0 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_ENCODE, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidDecodeException() {
            PdfType0Function function = GenerateSimplePdfFunction(new byte[] { 0 }, 1);
            function.SetDecode(new double[] { 0 });
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => function.Calculate(new double[
                ] { 0 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_DECODE, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidSamplesException() {
            PdfType0Function function = GenerateSimplePdfFunction(new byte[] { 0x0f, 0x0f }, 4);
            function.SetSize(new int[] { 5 });
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => function.Calculate(new double[
                ] { 0 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_TYPE0_FUNCTION_INVALID_SAMPLES, e.Message
                );
        }

        private PdfType0Function GenerateSimplePdfFunction(byte[] samples, int bitsPerSample) {
            PdfStream stream = new PdfStream(samples);
            stream.Put(PdfName.Domain, new PdfArray(new double[] { 0, 1 }));
            stream.Put(PdfName.Range, new PdfArray(new double[] { 0, 1 }));
            stream.Put(PdfName.Size, new PdfArray(new int[] { 2 }));
            stream.Put(PdfName.BitsPerSample, new PdfNumber(bitsPerSample));
            return new PdfType0Function(stream);
        }
    }
}

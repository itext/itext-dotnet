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
using System.Collections.Generic;
using System.Linq;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Function {
    [NUnit.Framework.Category("IntegrationTest")]
    public abstract class AbstractPdfType0FunctionTest : ExtendedITextTest {
        protected internal const double DELTA = 1e-12;

        private readonly int order;

        protected internal AbstractPdfType0FunctionTest(int order) {
            this.order = order;
        }

        [NUnit.Framework.Test]
        public virtual void TestConstantFunctions() {
            // f(x, y, z) = (1, 2, 3)
            double[] expected = new double[] { -1, 0, 1 };
            PdfStream stream = new PdfStream(new byte[] { 0, 0, 0 });
            stream.Put(PdfName.Domain, new PdfArray(new double[] { 0, 0, 0, 0, 0, 0 }));
            stream.Put(PdfName.Size, new PdfArray(new int[] { 2, 1, 3 }));
            stream.Put(PdfName.Range, new PdfArray(new double[] { expected[0], expected[0], expected[1], expected[1], 
                expected[2], expected[2] }));
            stream.Put(PdfName.BitsPerSample, new PdfNumber(1));
            PdfType0Function pdfFunction = new PdfType0Function(stream);
            double[] actual = pdfFunction.Calculate(new double[] { 0, 10, -99 });
            iText.Test.TestUtil.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void TestLinearFunctions() {
            // f(x) = (x, 3x, 2-x, 2x-1) : [-1, 2] -> [-1,2]x[-3,6]x[0,3]x[-3,3]
            Func<double, IList<double>> function = (x) => JavaUtil.ArraysAsList(x, 3 * x, 2 - x, 2 * x - 1);
            double[] domain = new double[] { -1, 2 };
            int[] size = new int[] { 2 };
            double[] range = new double[] { -1, 2, -3, 6, 0, 3, -3, 3 };
            int bitsPerSample = 1;
            byte[] samples = new byte[] { 0x2d };
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, 
                samples);
            double[] arguments = new double[] { -1.0, -0.67, -0.33, 0.0, 0.33, 0.67, 1.0, 1.33, 1.67, 2.0 };
            foreach (double argument in arguments) {
                IList<double> fRes = function.Invoke(argument);
                IEnumerable<double> stream = fRes;
                double[] expected = stream.Select((x) => x).ToArray();
                double[] actual = pdfFunction.Calculate(new double[] { argument });
                iText.Test.TestUtil.AreEqual(expected, actual, DELTA);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestLinearFunctionsWithEncoding() {
            // f(x) = (x, 3x, 2-x, 2x-1) : [-1, 2] -> [-1,2]x[-3,6]x[0,3]x[-3,3]
            Func<double, IList<double>> function = (x) => JavaUtil.ArraysAsList(x, 3 * x, 2 - x, 2 * x - 1);
            double[] domain = new double[] { -1, 2 };
            int[] size = new int[] { 4 };
            double[] range = new double[] { -1, 2, -3, 6, 0, 3, -3, 3 };
            int[] encode = new int[] { 1, 2 };
            int bitsPerSample = 1;
            byte[] samples = new byte[] { (byte)0xf2, (byte)0xd0 };
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, encode, null, bitsPerSample
                , samples);
            double[] arguments = new double[] { -1.0, -0.67, -0.33, 0.0, 0.33, 0.67, 1.0, 1.33, 1.67, 2.0 };
            foreach (double argument in arguments) {
                double[] expected = function.Invoke(argument).Select((x) => x).ToArray();
                double[] actual = pdfFunction.Calculate(new double[] { argument });
                iText.Test.TestUtil.AreEqual(expected, actual, DELTA);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestLinearFunctionsDim3() {
            // f(x, y, z) = (x+y, x-y, x+y+z) : [0,1]x[0,1]x[0,1] -> [0,2]x[-1,1]x[0,3]
            Func<IList<double>, IList<double>> function = (x) => JavaUtil.ArraysAsList(x[0] + x[1], x[0] - x[1], x[0] 
                + x[1] + x[2]);
            double[] domain = new double[] { 0, 1, 0, 1, 0, 1 };
            int[] size = new int[] { 2, 2, 2 };
            double[] range = new double[] { 0, 2, -1, 1, 0, 3 };
            double[] decode = new double[] { 0, 3, -1, 2, 0, 3 };
            int bitsPerSample = 2;
            byte[] samples = new byte[] { 0x11, (byte)0x94, 0x66, 0x15, (byte)0xa4, (byte)0xa7 };
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, decode, bitsPerSample
                , samples);
            IList<IList<double>> arguments = JavaUtil.ArraysAsList(JavaUtil.ArraysAsList(0.0, 0.0, 0.0), JavaUtil.ArraysAsList
                (1.0, 1.0, 1.0), JavaUtil.ArraysAsList(0.05, 0.95, 0.35), JavaUtil.ArraysAsList(0.15, 0.01, 0.88), JavaUtil.ArraysAsList
                (0.99, 0.99, 0.5), JavaUtil.ArraysAsList(0.98, 0.1, 0.01));
            foreach (IList<double> argument in arguments) {
                double[] expected = function.Invoke(argument).Select((d) => d).ToArray();
                double[] actual = pdfFunction.Calculate(argument.Select((d) => d).ToArray());
                iText.Test.TestUtil.AreEqual(expected, actual, DELTA);
            }
        }

        protected internal virtual void TestPolynomials(double[] expectedDelta) {
            // f(x) = (x^4, 1 - x + x^3, 1 - x^2) : [-1,1] -> [0,1]x[0.5,1.5]x[0,1]
            Func<double, IList<double>> function = (x) => {
                double x2 = x * x;
                return JavaUtil.ArraysAsList(x2 * x2, 1 - x + x * x2, 1 - x2);
            }
            ;
            double[] domain = new double[] { -1, 1 };
            int[] size = new int[] { 5 };
            double[] range = new double[] { 0, 1, 0.5, 1.5, 0, 1 };
            double[] decode = new double[] { 0, 15.9375, 0, 31.875, 0, 63.75 };
            int bitsPerSample = 8;
            byte[] samples = new byte[] { 16, 8, 0, 1, 11, 3, 0, 8, 4, 1, 5, 3, 16, 8, 0 };
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, decode, bitsPerSample
                , samples);
            double[] arguments = new double[] { -1.0, -0.67, -0.33, 0.0, 0.33, 0.67, 1.0 };
            foreach (double argument in arguments) {
                double[] expected = function.Invoke(argument).Select((x) => x).ToArray();
                double[] actual = pdfFunction.Calculate(new double[] { argument });
                for (int i = 0; i < expectedDelta.Length; ++i) {
                    NUnit.Framework.Assert.AreEqual(expected[i], actual[i], expectedDelta[i]);
                }
            }
        }

        protected internal virtual void TestPolynomialsWithEncoding(double[] expectedDelta) {
            // f(x) = (x^4, 1 - x + x^3, 1 - x^2) : [-1,1] -> [0,1]x[0.5,1.5]x[0,1]
            Func<double, IList<double>> function = (x) => {
                double x2 = x * x;
                return JavaUtil.ArraysAsList(x2 * x2, 1 - x + x * x2, 1 - x2);
            }
            ;
            double[] domain = new double[] { -1, 1 };
            int[] size = new int[] { 10 };
            double[] range = new double[] { 0, 1, 0.5, 1.5, 0, 1 };
            int[] encode = new int[] { 3, 7 };
            double[] decode = new double[] { 0, 15.9375, 0, 31.875, 0, 63.75 };
            int bitsPerSample = 8;
            byte[] samples = new byte[] { (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff, (byte
                )0xff, (byte)0xff, (byte)0xff, 16, 8, 0, 1, 11, 3, 0, 8, 4, 1, 5, 3, 16, 8, 0, (byte)0xff, (byte)0xff, 
                (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff };
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, encode, decode, bitsPerSample
                , samples);
            double[] arguments = new double[] { -1.0, -0.67, -0.33, 0.0, 0.33, 0.67, 1.0 };
            foreach (double argument in arguments) {
                double[] expected = function.Invoke(argument).Select((x) => x).ToArray();
                double[] actual = pdfFunction.Calculate(new double[] { argument });
                for (int i = 0; i < expectedDelta.Length; ++i) {
                    NUnit.Framework.Assert.AreEqual(expected[i], actual[i], expectedDelta[i]);
                }
            }
        }

        protected internal virtual void TestPolynomialsDim2(double[] expectedDelta) {
            // f(x, y) = (x^2+y, x+xy); [0, 1]x[0, 1] -> [0, 2]x[0,2]
            Func<IList<double>, IList<double>> function = (x) => JavaUtil.ArraysAsList(x[0] * x[0] + x[1], x[0] + x[0]
                 * x[1]);
            double[] domain = new double[] { 0, 1, 0, 1 };
            int[] size = new int[] { 6, 2 };
            double[] range = new double[] { 0, 2, 0, 2 };
            double[] decode = new double[] { 0, 10.2, 0, 51 };
            int bitsPerSample = 8;
            byte[] samples = new byte[] { 0, 0, 1, 1, 4, 2, 9, 3, 16, 4, 25, 5, 25, 0, 26, 2, 29, 4, 34, 6, 41, 8, 50, 
                10 };
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, decode, bitsPerSample
                , samples);
            IList<IList<double>> arguments = JavaUtil.ArraysAsList(JavaUtil.ArraysAsList(0.0, 0.0), JavaUtil.ArraysAsList
                (1.0, 1.0), JavaUtil.ArraysAsList(0.05, 0.99), JavaUtil.ArraysAsList(0.9, 0.55), JavaUtil.ArraysAsList
                (0.35, 0.11), JavaUtil.ArraysAsList(0.5, 0.99));
            foreach (IList<double> argument in arguments) {
                double[] expected = function.Invoke(argument).Select((d) => d).ToArray();
                double[] actual = pdfFunction.Calculate(argument.Select((d) => d).ToArray());
                for (int i = 0; i < expectedDelta.Length; ++i) {
                    NUnit.Framework.Assert.AreEqual(expected[i], actual[i], expectedDelta[i]);
                }
            }
        }

        protected internal virtual void TestPolynomialsDim2WithEncoding(double[] expectedDelta) {
            // f(x, y) = (x^2+y, x+xy); [0, 1]x[0, 1] -> [0, 2]x[0,2]
            Func<IList<double>, IList<double>> function = (x) => JavaUtil.ArraysAsList(x[0] * x[0] + x[1], x[0] + x[0]
                 * x[1]);
            double[] domain = new double[] { 0, 1, 0, 1 };
            int[] size = new int[] { 10, 5 };
            double[] range = new double[] { 0, 2, 0, 2 };
            int[] encode = new int[] { 2, 7, 3, 4 };
            double[] decode = new double[] { 0, 10.2, 0, 51 };
            int bitsPerSample = 8;
            byte[] samples = new byte[] { 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 8, 0, 9, 0, 0, 1, 1, 1, 2, 1
                , 3, 1, 4, 1, 5, 1, 6, 1, 7, 1, 8, 1, 9, 1, 0, 2, 1, 2, 2, 2, 3, 2, 4, 2, 5, 2, 6, 2, 7, 2, 8, 2, 9, 2
                , 0, 3, 1, 3, 0, 0, 1, 1, 4, 2, 9, 3, 16, 4, 25, 5, 8, 3, 9, 3, 0, 4, 1, 4, 25, 0, 26, 2, 29, 4, 34, 6
                , 41, 8, 50, 10, 8, 4, 9, 4 };
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, encode, decode, bitsPerSample
                , samples);
            IList<IList<double>> arguments = JavaUtil.ArraysAsList(JavaUtil.ArraysAsList(0.0, 0.0), JavaUtil.ArraysAsList
                (1.0, 1.0), JavaUtil.ArraysAsList(0.05, 0.99), JavaUtil.ArraysAsList(0.9, 0.55), JavaUtil.ArraysAsList
                (0.35, 0.11), JavaUtil.ArraysAsList(0.5, 0.99));
            foreach (IList<double> argument in arguments) {
                double[] expected = function.Invoke(argument).Select((d) => d).ToArray();
                double[] actual = pdfFunction.Calculate(argument.Select((d) => d).ToArray());
                for (int i = 0; i < expectedDelta.Length; ++i) {
                    NUnit.Framework.Assert.AreEqual(expected[i], actual[i], expectedDelta[i]);
                }
            }
        }

        protected internal virtual void TestSinus(double delta) {
            // f(x) = sin(x) : [0, 180] -> [0, 1]
            Func<double, double> function = (x) => Math.Sin(MathUtil.ToRadians(x));
            double[] domain = new double[] { 0, 180 };
            int[] size = new int[] { 21 };
            double[] range = new double[] { 0, 1 };
            int bitsPerSample = 32;
            byte[] samples = Generate1Dim32BitSamples(function, size[0], domain, range);
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, 
                samples);
            for (int i = 0; i <= 180; ++i) {
                double expected = function.Invoke((double)i);
                double actual = pdfFunction.Calculate(new double[] { i })[0];
                NUnit.Framework.Assert.AreEqual(expected, actual, delta);
            }
        }

        protected internal virtual void TestExponent(double delta) {
            // f(x) = e^x : [0, 2] -> [1, e^2]
            Func<double, double> function = (val) => Math.Exp(val);
            double[] domain = new double[] { 0, 2 };
            int[] size = new int[] { 9 };
            double[] range = new double[] { 1, function.Invoke(domain[1]) };
            int bitsPerSample = 32;
            byte[] samples = Generate1Dim32BitSamples(function, size[0], domain, range);
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, 
                samples);
            double[] arguments = new double[21];
            for (int i = 0; i < 21; i++) {
                arguments[i] = i / 10;
            }
            //double[] arguments = DoubleStream.iterate(0, x -> x + 0.1).limit(21).toArray();
            foreach (double argument in arguments) {
                double expected = function.Invoke(argument);
                double actual = pdfFunction.Calculate(new double[] { argument })[0];
                NUnit.Framework.Assert.AreEqual(expected, actual, delta);
            }
        }

        protected internal virtual void TestLogarithm(double delta) {
            // f(x) = ln(x) : [1,10] -> [0,ln(10)]
            Func<double, double> function = (val) => Math.Log(val);
            double[] domain = new double[] { 1, 10 };
            int[] size = new int[] { 10 };
            double[] range = new double[] { 0, function.Invoke(domain[1]) };
            int bitsPerSample = 32;
            byte[] samples = Generate1Dim32BitSamples(function, size[0], domain, range);
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, 
                samples);
            double[] arguments = new double[37];
            for (int i = 0; i < 37; i++) {
                arguments[i] = 1 + i / 4;
            }
            //double[] arguments = DoubleStream.iterate(1, x -> x + 0.25).limit(37).toArray();
            foreach (double argument in arguments) {
                double expected = function.Invoke(argument);
                double actual = pdfFunction.Calculate(new double[] { argument })[0];
                NUnit.Framework.Assert.AreEqual(expected, actual, delta);
            }
        }

        protected internal virtual void TestGeneralInterpolation(double delta) {
            // f(x, y) = exp(xy) / (1 + y^2) : [0,1]x[0,1] - > [0.5, e]
            Func<IList<double>, double> function = (x) => Math.Exp(x[0] * x[1]) / (1 + x[1] * x[1]);
            double[] domain = new double[] { 0, 1, 0, 1 };
            int[] size = new int[] { 5, 5 };
            double[] range = new double[] { 0.5, Math.E };
            int bitsPerSample = 32;
            byte[] samples = Generate2Dim32BitSamples(function, size, domain, range);
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, 
                samples);
            for (double x = 0; x < 1.01; x += 0.03) {
                for (double y = 0; y < 1.01; y += 0.03) {
                    double expected = function.Invoke(JavaUtil.ArraysAsList(x, y));
                    double actual = pdfFunction.Calculate(new double[] { x, y })[0];
                    NUnit.Framework.Assert.AreEqual(expected, actual, delta);
                }
            }
        }

        private byte[] Generate1Dim32BitSamples(Func<double, double> function, int size, double[] domain, double[]
             range) {
            byte[] samples = new byte[size << 2];
            int pos = 0;
            for (int i = 0; i < size; ++i) {
                double value = function.Invoke(domain[0] + i * (domain[1] - domain[0]) / (size - 1));
                long sampleValue = (long)MathematicUtil.Round(unchecked((long)(0xffffffffL)) * ((value - range[0]) / (range
                    [1] - range[0])));
                for (int k = 24; k >= 0; k -= 8) {
                    samples[pos++] = (byte)(((sampleValue) >> k) & 0xff);
                }
            }
            return samples;
        }

        private byte[] Generate2Dim32BitSamples(Func<IList<double>, double> function, int[] size, double[] domain, 
            double[] range) {
            byte[] samples = new byte[(size[0] * size[1]) << 2];
            int pos = 0;
            for (int i = 0; i < size[1]; ++i) {
                for (int j = 0; j < size[0]; ++j) {
                    double value = function.Invoke(JavaUtil.ArraysAsList(domain[0] + j * (domain[1] - domain[0]) / (size[0] - 
                        1), domain[0] + i * (domain[1] - domain[0]) / (size[1] - 1)));
                    long sampleValue = (long)MathematicUtil.Round(unchecked((long)(0xffffffffL)) * ((value - range[0]) / (range
                        [1] - range[0])));
                    for (int k = 24; k >= 0; k -= 8) {
                        samples[pos++] = (byte)(((sampleValue) >> k) & 0xff);
                    }
                }
            }
            return samples;
        }
    }
}

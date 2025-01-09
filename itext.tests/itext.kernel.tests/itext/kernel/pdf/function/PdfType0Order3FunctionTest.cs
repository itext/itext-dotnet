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
namespace iText.Kernel.Pdf.Function {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfType0Order3FunctionTest : AbstractPdfType0FunctionTest {
        private const int CUBIC_INTERPOLATION_ORDER = 3;

        public PdfType0Order3FunctionTest()
            : base(CUBIC_INTERPOLATION_ORDER) {
        }

        [NUnit.Framework.Test]
        public virtual void TestPolynomials() {
            TestPolynomials(new double[] { 0.1, 0.07, 0.03 });
        }

        [NUnit.Framework.Test]
        public virtual void TestPolynomialsWithEncoding() {
            TestPolynomialsWithEncoding(new double[] { 0.1, 0.07, 0.03 });
        }

        [NUnit.Framework.Test]
        public virtual void TestPolynomialsDim2() {
            TestPolynomialsDim2(new double[] { 0.007, DELTA });
        }

        [NUnit.Framework.Test]
        public virtual void TestPolynomialsDim2WithEncoding() {
            TestPolynomialsDim2WithEncoding(new double[] { 0.007, DELTA });
        }

        [NUnit.Framework.Test]
        public virtual void TestSinus() {
            TestSinus(1.6e-6);
        }

        [NUnit.Framework.Test]
        public virtual void TestExponent() {
            TestExponent(0.03);
        }

        [NUnit.Framework.Test]
        public virtual void TestLogarithm() {
            TestLogarithm(0.035);
        }

        [NUnit.Framework.Test]
        public virtual void TestGeneralInterpolation() {
            TestGeneralInterpolation(0.01);
        }
    }
}

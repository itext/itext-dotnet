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
namespace iText.Kernel.Pdf.Function {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfType0Order1FunctionTest : AbstractPdfType0FunctionTest {
        private const int LINEAR_INTERPOLATION_ORDER = 1;

        public PdfType0Order1FunctionTest()
            : base(LINEAR_INTERPOLATION_ORDER) {
        }

        [NUnit.Framework.Test]
        public virtual void TestLinerFunctionsWithEncoding() {
            TestLinearFunctionsWithEncoding();
        }

        [NUnit.Framework.Test]
        public virtual void TestPolynomials() {
            TestPolynomials(new double[] { 0.2, 0.14, 0.06 });
        }

        [NUnit.Framework.Test]
        public virtual void TestPolynomialsWithEncoding() {
            TestPolynomialsWithEncoding(new double[] { 0.2, 0.14, 0.06 });
        }

        [NUnit.Framework.Test]
        public virtual void TestPolynomialsDim2() {
            TestPolynomialsDim2(new double[] { 0.01 + DELTA, DELTA });
        }

        [NUnit.Framework.Test]
        public virtual void TestPolynomialsDim2WithEncoding() {
            TestPolynomialsDim2WithEncoding(new double[] { 0.01 + DELTA, DELTA });
        }

        [NUnit.Framework.Test]
        public virtual void TestSinus() {
            TestSinus(3.2e-3);
        }

        [NUnit.Framework.Test]
        public virtual void TestExponent() {
            TestExponent(0.05);
        }

        [NUnit.Framework.Test]
        public virtual void TestLogarithm() {
            TestLogarithm(0.06);
        }

        [NUnit.Framework.Test]
        public virtual void TestGeneralInterpolation() {
            TestGeneralInterpolation(0.015);
        }
    }
}

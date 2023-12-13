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
using iText.Kernel;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class FingerPrintTest : ExtendedITextTest {
        private ProductInfo productInfo;

        private ProductInfo productInfo2;

        [NUnit.Framework.SetUp]
        public virtual void BeforeTest() {
            this.productInfo = new ProductInfo("pdfProduct", 1, 0, 0, true);
            this.productInfo2 = new ProductInfo("pdfProduct2", 1, 0, 0, true);
        }

        [NUnit.Framework.Test]
        public virtual void NormalAddTest() {
            FingerPrint fingerPrint = new FingerPrint();
            NUnit.Framework.Assert.IsTrue(fingerPrint.RegisterProduct(productInfo));
            NUnit.Framework.Assert.IsTrue(fingerPrint.RegisterProduct(productInfo2));
            NUnit.Framework.Assert.AreEqual(2, fingerPrint.GetProducts().Count);
        }

        [NUnit.Framework.Test]
        public virtual void DuplicateTest() {
            FingerPrint fingerPrint = new FingerPrint();
            fingerPrint.RegisterProduct(productInfo);
            NUnit.Framework.Assert.IsFalse(fingerPrint.RegisterProduct(productInfo));
        }
    }
}

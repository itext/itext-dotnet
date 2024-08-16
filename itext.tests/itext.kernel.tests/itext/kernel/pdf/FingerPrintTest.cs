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
using iText.Commons.Actions.Data;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FingerPrintTest : ExtendedITextTest {
        private ProductData productData;

        private ProductData productData2;

        private ProductData duplicateProductData;

        [NUnit.Framework.SetUp]
        public virtual void BeforeTest() {
            this.productData = new ProductData("pdfProduct", "pdfProduct", "7.0.0", 1900, 2000);
            this.productData2 = new ProductData("pdfProduct2", "pdfProduct2", "7.0.0", 1900, 2000);
            this.duplicateProductData = new ProductData("pdfProduct", "pdfProduct", "7.0.0", 1900, 2000);
        }

        [NUnit.Framework.Test]
        public virtual void NormalAddTest() {
            FingerPrint fingerPrint = new FingerPrint();
            NUnit.Framework.Assert.IsTrue(fingerPrint.RegisterProduct(productData));
            NUnit.Framework.Assert.IsTrue(fingerPrint.RegisterProduct(productData2));
            NUnit.Framework.Assert.AreEqual(2, fingerPrint.GetProducts().Count);
        }

        [NUnit.Framework.Test]
        public virtual void DuplicateTest() {
            FingerPrint fingerPrint = new FingerPrint();
            fingerPrint.RegisterProduct(productData);
            NUnit.Framework.Assert.IsFalse(fingerPrint.RegisterProduct(duplicateProductData));
        }

        [NUnit.Framework.Test]
        public virtual void DisableFingerPrintTest() {
            FingerPrint fingerPrint = new FingerPrint();
            fingerPrint.DisableFingerPrint();
            NUnit.Framework.Assert.IsFalse(fingerPrint.IsFingerPrintEnabled());
        }
    }
}

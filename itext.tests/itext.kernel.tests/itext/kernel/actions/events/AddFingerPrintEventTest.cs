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
using iText.Commons.Actions.Data;
using iText.IO.Source;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Actions.Events {
    [NUnit.Framework.Category("UnitTest")]
    public class AddFingerPrintEventTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NullDocumentTest() {
            AddFingerPrintEvent addFingerPrintEvent = new AddFingerPrintEvent(null);
            NUnit.Framework.Assert.DoesNotThrow(() => addFingerPrintEvent.DoAction());
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FINGERPRINT_DISABLED_BUT_NO_REQUIRED_LICENCE)]
        public virtual void DisableFingerPrintAGPLTest() {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    doc.GetFingerPrint().DisableFingerPrint();
                    NUnit.Framework.Assert.DoesNotThrow(() => doc.Close());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void EnabledFingerPrintAGPLTest() {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    NUnit.Framework.Assert.DoesNotThrow(() => doc.Close());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void DisableFingerPrintNoProcessorForProductTest() {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    ProductData productData = new ProductData("public product name", "product name", "1", 2000, 2025);
                    doc.GetFingerPrint().RegisterProduct(productData);
                    NUnit.Framework.Assert.DoesNotThrow(() => doc.Close());
                }
            }
        }
    }
}

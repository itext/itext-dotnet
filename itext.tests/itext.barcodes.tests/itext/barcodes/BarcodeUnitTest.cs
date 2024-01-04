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
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Barcodes {
    [NUnit.Framework.Category("UnitTest")]
    public class BarcodeUnitTest : ExtendedITextTest {
        private const double EPS = 0.0001;

        [NUnit.Framework.Test]
        public virtual void BarcodeMSIGetBarcodeSizeWithChecksumTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            PdfDocument document = new PdfDocument(writer);
            document.AddNewPage();
            Barcode1D barcode = new BarcodeMSI(document);
            document.Close();
            barcode.SetCode("123456789");
            barcode.SetGenerateChecksum(true);
            Rectangle barcodeSize = barcode.GetBarcodeSize();
            NUnit.Framework.Assert.AreEqual(33.656, barcodeSize.GetHeight(), EPS);
            NUnit.Framework.Assert.AreEqual(101.6, barcodeSize.GetWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeMSIGetBarcodeSizeWithoutChecksumTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            PdfDocument document = new PdfDocument(writer);
            document.AddNewPage();
            Barcode1D barcode = new BarcodeMSI(document);
            document.Close();
            barcode.SetCode("123456789");
            barcode.SetGenerateChecksum(false);
            Rectangle barcodeSize = barcode.GetBarcodeSize();
            NUnit.Framework.Assert.AreEqual(33.656, barcodeSize.GetHeight(), EPS);
            NUnit.Framework.Assert.AreEqual(92.0, barcodeSize.GetWidth(), EPS);
        }
    }
}

/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.IO;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Barcodes {
    [NUnit.Framework.Category("UnitTest")]
    public class BarcodeEANUnitTest : ExtendedITextTest {
        public const float EPS = 0.0001f;

        [NUnit.Framework.Test]
        public virtual void CalculateEANParityTest() {
            int expectedParity = BarcodeEAN.CalculateEANParity("1234567890");
            NUnit.Framework.Assert.AreEqual(5, expectedParity);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCEIncorrectTextTest() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("HelloWorld");
            NUnit.Framework.Assert.IsNull(expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCE12DigitsStartNotWith0Or1Test() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("025272730706");
            NUnit.Framework.Assert.IsNull(expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCEFrom3Position00000Test() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("012000005706");
            NUnit.Framework.Assert.AreEqual("01257006", expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCEFrom3Position10000Test() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("012100005706");
            NUnit.Framework.Assert.AreEqual("01257016", expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCEFrom3Position20000Test() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("012200005706");
            NUnit.Framework.Assert.AreEqual("01257026", expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCEFrom3Position000NullTest() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("012000111706");
            NUnit.Framework.Assert.IsNull(expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCEFrom4Position00NullTest() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("012300111706");
            NUnit.Framework.Assert.IsNull(expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCEFrom4Position00000Test() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("012300000706");
            NUnit.Framework.Assert.AreEqual("01237036", expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCEFrom5Position0NullTest() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("012340111706");
            NUnit.Framework.Assert.IsNull(expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCEFrom5Position00000Test() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("012340000006");
            NUnit.Framework.Assert.AreEqual("01234046", expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCE10PositionBiggerThan5NullTest() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("011111111711");
            NUnit.Framework.Assert.IsNull(expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertUPCAtoUPCE10PositionBiggerThan5Test() {
            String expectedUpce = BarcodeEAN.ConvertUPCAtoUPCE("011111000090");
            NUnit.Framework.Assert.AreEqual("01111190", expectedUpce);
        }

        [NUnit.Framework.Test]
        public virtual void GetBarsUPCETest() {
            String expectedBytes = "111212211411132132141111312111111";
            byte[] bytes = BarcodeEAN.GetBarsUPCE("12345678");
            NUnit.Framework.Assert.AreEqual(33, bytes.Length);
            for (int i = 0; i < expectedBytes.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expectedBytes[i] - '0', bytes[i]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetBarsSupplemental2Test() {
            String expectedBytes = "1121222113211";
            byte[] bytes = BarcodeEAN.GetBarsSupplemental2("10");
            NUnit.Framework.Assert.AreEqual(13, bytes.Length);
            for (int i = 0; i < expectedBytes.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expectedBytes[i] - '0', bytes[i]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetBarcodeSizeUPCATest() {
            Rectangle expectedRectangle = new Rectangle(84.895996f, 33.656f);
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            Barcode1D barcode = new BarcodeEAN(document);
            barcode.SetCodeType(BarcodeEAN.UPCA);
            barcode.SetCode("9781935182610");
            Rectangle barcodeSize = barcode.GetBarcodeSize();
            NUnit.Framework.Assert.AreEqual(expectedRectangle.GetWidth(), barcodeSize.GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(expectedRectangle.GetHeight(), barcodeSize.GetHeight(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void GetBarcodeSizeUPCETest() {
            Rectangle expectedRectangle = new Rectangle(49.696f, 33.656f);
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            Barcode1D barcode = new BarcodeEAN(document);
            barcode.SetCodeType(BarcodeEAN.UPCE);
            barcode.SetCode("9781935182610");
            Rectangle barcodeSize = barcode.GetBarcodeSize();
            NUnit.Framework.Assert.AreEqual(expectedRectangle.GetWidth(), barcodeSize.GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(expectedRectangle.GetHeight(), barcodeSize.GetHeight(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void GetBarcodeSizeSUPP2Test() {
            Rectangle expectedRectangle = new Rectangle(16, 33.656f);
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            Barcode1D barcode = new BarcodeEAN(document);
            barcode.SetCodeType(BarcodeEAN.SUPP2);
            barcode.SetCode("03456781");
            Rectangle barcodeSize = barcode.GetBarcodeSize();
            NUnit.Framework.Assert.AreEqual(expectedRectangle.GetWidth(), barcodeSize.GetWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(expectedRectangle.GetHeight(), barcodeSize.GetHeight(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void GetBarcodeSizeIncorrectTypeTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            Barcode1D barcode = new BarcodeEAN(document);
            barcode.SetCode("9781935182610");
            // Set incorrect type
            barcode.SetCodeType(1234);
            // We do expect an exception here
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => barcode.GetBarcodeSize());
            NUnit.Framework.Assert.AreEqual("Invalid code type", e.Message);
        }
    }
}

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
using iText.Barcodes.Qrcode;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Barcodes {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BarcodeQRCodeTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/barcodes/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/barcodes/BarcodeQRCode/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode01Test() {
            String filename = "barcodeQRCode01.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            IDictionary<EncodeHintType, Object> hints = new Dictionary<EncodeHintType, Object>();
            hints.Put(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.L);
            BarcodeQRCode barcode = new BarcodeQRCode("some specific text 239214 hello world");
            barcode.PlaceBarcode(canvas, ColorConstants.GRAY, 12);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode02Test() {
            String filename = "barcodeQRCode02.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            IDictionary<EncodeHintType, Object> hints = new Dictionary<EncodeHintType, Object>();
            hints.Put(EncodeHintType.CHARACTER_SET, "UTF-8");
            BarcodeQRCode barcode1 = new BarcodeQRCode("дима", hints);
            barcode1.PlaceBarcode(canvas, ColorConstants.GRAY, 12);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeVersioningTest() {
            String filename = "barcodeQRCodeVersioning.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            for (int i = -9; i < 42; i += 10) {
                PdfPage page1 = document.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                IDictionary<EncodeHintType, Object> hints = new Dictionary<EncodeHintType, Object>();
                hints.Put(EncodeHintType.CHARACTER_SET, "UTF-8");
                hints.Put(EncodeHintType.MIN_VERSION_NR, i);
                BarcodeQRCode barcode1 = new BarcodeQRCode("дима", hints);
                barcode1.PlaceBarcode(canvas, ColorConstants.GRAY, 3);
            }
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }
    }
}

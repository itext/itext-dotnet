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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Barcodes {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BarcodeMSITest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/barcodes/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/barcodes/BarcodeMSI/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode01Test() {
            String filename = "barcodeMSI_01.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Barcode1D barcode = new BarcodeMSI(document);
            barcode.SetCode("123456789");
            barcode.SetGenerateChecksum(true);
            barcode.SetTextAlignment(Barcode1D.ALIGN_LEFT);
            barcode.PlaceBarcode(canvas, ColorConstants.BLACK, ColorConstants.WHITE);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff01_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode02Test() {
            String filename = "barcodeMSI_02.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfReader reader = new PdfReader(sourceFolder + "DocumentWithTrueTypeFont1.pdf");
            PdfDocument document = new PdfDocument(reader, writer);
            PdfCanvas canvas = new PdfCanvas(document.GetLastPage());
            Barcode1D barcode = new BarcodeMSI(document);
            barcode.SetCode("9781935182610");
            barcode.SetTextAlignment(Barcode1D.ALIGN_LEFT);
            barcode.PlaceBarcode(canvas, ColorConstants.BLACK, ColorConstants.WHITE);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff02_"));
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeAlignRightTest() {
            String filename = "barcodeMSI_AlignRight.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Barcode1D barcode = new BarcodeMSI(document);
            barcode.SetCode("123456789");
            barcode.SetGenerateChecksum(true);
            barcode.SetTextAlignment(Barcode1D.ALIGN_RIGHT);
            barcode.PlaceBarcode(canvas, ColorConstants.BLACK, ColorConstants.RED);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff01_"));
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeAlignCenterTest() {
            String filename = "barcodeMSI_AlignCenter.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Barcode1D barcode = new BarcodeMSI(document);
            barcode.SetCode("123456789");
            barcode.SetGenerateChecksum(true);
            barcode.SetTextAlignment(Barcode1D.ALIGN_CENTER);
            barcode.PlaceBarcode(canvas, ColorConstants.BLACK, ColorConstants.RED);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff01_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode03Test() {
            byte[] expected = new byte[] { 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 
                0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1 };
            byte[] barcodeBytes = BarcodeMSI.GetBarsMSI("1234");
            bool isEqual = JavaUtil.ArraysEquals(expected, barcodeBytes);
            NUnit.Framework.Assert.IsTrue(isEqual);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode04Test() {
            String code = "0987654321";
            int expectedChecksum = 7;
            int checksum = BarcodeMSI.GetChecksum(code);
            NUnit.Framework.Assert.AreEqual(checksum, expectedChecksum);
        }
    }
}

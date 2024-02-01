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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Barcodes {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BarcodeDataMatrixTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/barcodes/BarcodeDataMatrix/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/barcodes/";

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
            String filename = "barcodeDataMatrix.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            BarcodeDataMatrix barcode = new BarcodeDataMatrix();
            barcode.SetCode("AAAAAAAAAA;BBBBAAAA3;00028;BBBAA05;AAAA;AAAAAA;1234567;AQWXSZ;JEAN;;;;7894561;AQWXSZ;GEO;;;;1;1;1;1;0;0;1;0;1;0;0;0;1;0;1;0;0;0;0;0;0;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1"
                );
            barcode.PlaceBarcode(canvas, ColorConstants.GREEN, 5);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode02Test() {
            String filename = "barcodeDataMatrix2.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            BarcodeDataMatrix barcode2 = new BarcodeDataMatrix("дима", "UTF-8");
            barcode2.PlaceBarcode(canvas, ColorConstants.GREEN, 10);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode03Test() {
            String filename = "barcodeDataMatrix3.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            BarcodeDataMatrix barcode3 = new BarcodeDataMatrix();
            barcode3.SetWidth(36);
            barcode3.SetHeight(12);
            barcode3.SetCode("AbcdFFghijklmnopqrstuWXSQ");
            barcode3.PlaceBarcode(canvas, ColorConstants.BLACK, 10);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode04Test() {
            String filename = "barcodeDataMatrix4.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            BarcodeDataMatrix barcode3 = new BarcodeDataMatrix();
            barcode3.SetWidth(36);
            barcode3.SetHeight(12);
            barcode3.SetCode("01AbcdefgAbcdefg123451231231234");
            barcode3.PlaceBarcode(canvas, ColorConstants.BLACK, 10);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode05Test() {
            String filename = "barcodeDataMatrix5.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            BarcodeDataMatrix barcode3 = new BarcodeDataMatrix();
            barcode3.SetWidth(40);
            barcode3.SetHeight(40);
            barcode3.SetCode("aaabbbcccdddAAABBBAAABBaaabbbcccdddaaa");
            barcode3.PlaceBarcode(canvas, ColorConstants.BLACK, 10);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode06Test() {
            String filename = "barcodeDataMatrix6.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            BarcodeDataMatrix barcode3 = new BarcodeDataMatrix();
            barcode3.SetWidth(36);
            barcode3.SetHeight(12);
            barcode3.SetCode(">>>\r>>>THIS VERY TEXT>>\r>");
            barcode3.PlaceBarcode(canvas, ColorConstants.BLACK, 10);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode07Test() {
            BarcodeDataMatrix bc = new BarcodeDataMatrix();
            bc.SetOptions(BarcodeDataMatrix.DM_AUTO);
            bc.SetWidth(10);
            bc.SetHeight(10);
            String aCode = "aBCdeFG12";
            int result = bc.SetCode(aCode);
            NUnit.Framework.Assert.AreEqual(result, BarcodeDataMatrix.DM_ERROR_TEXT_TOO_BIG);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode08Test() {
            BarcodeDataMatrix barcodeDataMatrix = new BarcodeDataMatrix();
            barcodeDataMatrix.SetWidth(18);
            barcodeDataMatrix.SetHeight(18);
            int result = barcodeDataMatrix.SetCode("AbcdFFghijklmnopqrstuWXSQ");
            NUnit.Framework.Assert.AreEqual(BarcodeDataMatrix.DM_ERROR_TEXT_TOO_BIG, result);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode09Test() {
            BarcodeDataMatrix barcodeDataMatrix = new BarcodeDataMatrix();
            barcodeDataMatrix.SetWidth(17);
            barcodeDataMatrix.SetHeight(17);
            int result = barcodeDataMatrix.SetCode("AbcdFFghijklmnopqrstuWXSQ");
            NUnit.Framework.Assert.AreEqual(BarcodeDataMatrix.DM_ERROR_INVALID_SQUARE, result);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode10Test() {
            BarcodeDataMatrix barcodeDataMatrix = new BarcodeDataMatrix();
            barcodeDataMatrix.SetWidth(26);
            barcodeDataMatrix.SetHeight(12);
            int result = barcodeDataMatrix.SetCode("AbcdFFghijklmnopqrstuWXSQ");
            NUnit.Framework.Assert.AreEqual(BarcodeDataMatrix.DM_ERROR_TEXT_TOO_BIG, result);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode11Test() {
            BarcodeDataMatrix barcodeDataMatrix = new BarcodeDataMatrix();
            barcodeDataMatrix.SetWidth(18);
            barcodeDataMatrix.SetHeight(18);
            byte[] str = "AbcdFFghijklmnop".GetBytes();
            int result = barcodeDataMatrix.SetCode(str, 0, str.Length);
            NUnit.Framework.Assert.AreEqual(BarcodeDataMatrix.DM_NO_ERROR, result);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode12Test() {
            BarcodeDataMatrix barcodeDataMatrix = new BarcodeDataMatrix();
            barcodeDataMatrix.SetWidth(18);
            barcodeDataMatrix.SetHeight(18);
            byte[] str = "AbcdFFghijklmnop".GetBytes();
            Exception e = NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => barcodeDataMatrix.SetCode
                (str, -1, str.Length));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode13Test() {
            BarcodeDataMatrix barcodeDataMatrix = new BarcodeDataMatrix();
            barcodeDataMatrix.SetWidth(18);
            barcodeDataMatrix.SetHeight(18);
            byte[] str = "AbcdFFghijklmnop".GetBytes();
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => barcodeDataMatrix.SetCode(str, 0, str
                .Length + 1));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode14Test() {
            BarcodeDataMatrix barcodeDataMatrix = new BarcodeDataMatrix();
            barcodeDataMatrix.SetWidth(18);
            barcodeDataMatrix.SetHeight(18);
            byte[] str = "AbcdFFghijklmnop".GetBytes();
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => barcodeDataMatrix.SetCode(str, 0, -1)
                );
        }

        [NUnit.Framework.Test]
        public virtual void Barcode15Test() {
            BarcodeDataMatrix barcodeDataMatrix = new BarcodeDataMatrix();
            barcodeDataMatrix.SetWidth(18);
            barcodeDataMatrix.SetHeight(18);
            byte[] str = "AbcdFFghijklmnop".GetBytes();
            int result = barcodeDataMatrix.SetCode(str, str.Length, 0);
            NUnit.Framework.Assert.AreEqual(BarcodeDataMatrix.DM_NO_ERROR, result);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode16Test() {
            String filename = "barcode16Test.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            BarcodeDataMatrix barcode = new BarcodeDataMatrix();
            barcode.SetCode("999999DILLERT XANG LIMITON 18               000");
            canvas.ConcatMatrix(1, 0, 0, 1, 100, 600);
            barcode.PlaceBarcode(canvas, ColorConstants.BLACK, 3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }
    }
}

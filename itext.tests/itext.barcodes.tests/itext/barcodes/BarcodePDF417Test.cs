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
using System;
using System.IO;
using iText.Barcodes.Exceptions;
using iText.IO.Codec;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Barcodes {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BarcodePDF417Test : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/barcodes/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/barcodes/BarcodePDF417/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode01Test() {
            String filename = "barcode417_01.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(text);
            barcode.PlaceBarcode(canvas, null);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode02Test() {
            String filename = "barcode417_02.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "DocumentWithTrueTypeFont1.pdf");
            PdfDocument document = new PdfDocument(reader, writer);
            PdfCanvas canvas = new PdfCanvas(document.GetLastPage());
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(text);
            barcode.PlaceBarcode(canvas, null);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void MacroPDF417Test01() {
            String filename = "barcode417Macro_01.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
            PdfDocument pdfDocument = new PdfDocument(writer);
            PdfCanvas pdfCanvas = new PdfCanvas(pdfDocument.AddNewPage());
            pdfCanvas.AddXObjectWithTransformationMatrix(CreateMacroBarcodePart(pdfDocument, "This is PDF417 segment 0"
                , 1, 1, 0), 1, 0, 0, 1, 36, 791);
            pdfCanvas.AddXObjectWithTransformationMatrix(CreateMacroBarcodePart(pdfDocument, "This is PDF417 segment 1"
                , 1, 1, 1), 1, 0, 0, 1, 36, 676);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417AspectRatioTest() {
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(text);
            barcode.SetAspectRatio(10);
            barcode.PlaceBarcode(canvas, null);
            document.Close();
            NUnit.Framework.Assert.AreEqual(10, barcode.GetAspectRatio(), 0);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417DefaultParamsTest() {
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetAspectRatio(10);
            barcode.SetCode(text);
            barcode.SetDefaultParameters();
            barcode.PlaceBarcode(canvas, null);
            document.Close();
            NUnit.Framework.Assert.AreEqual(0.5, barcode.GetAspectRatio(), 0);
        }

        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        [NUnit.Framework.Test]
        public virtual void Barcode417XObjectTest() {
            String filename = "barcode417XObjectTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(text);
            PdfFormXObject xObject = barcode.CreateFormXObject(document);
            canvas.AddXObjectAt(xObject, 10, 650);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER));
        }

        // Android-Conversion-Skip-Block-End
        [NUnit.Framework.Test]
        public virtual void Barcode417YHeightTest() {
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(text);
            barcode.SetYHeight(15);
            barcode.PlaceBarcode(canvas, null);
            document.Close();
            NUnit.Framework.Assert.AreEqual(15, barcode.GetYHeight(), 0);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417CodeReuseTest() {
            String filename = "barcode417CodeReuseTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(text);
            barcode.PlaceBarcode(canvas, ColorConstants.BLUE);
            byte[] baos = barcode.GetCode();
            BarcodePDF417 barcode2 = new BarcodePDF417();
            barcode2.SetCode(baos);
            canvas = new PdfCanvas(document.AddNewPage());
            barcode2.PlaceBarcode(canvas, ColorConstants.CYAN);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417NumbersTest() {
            String filename = "barcode417NumbersTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            String numbers = "1234567890";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(numbers);
            barcode.PlaceBarcode(canvas, null);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417ByteLessThanSixSizeNumbersTest() {
            String filename = "barcode417ByteLessThanSixSizeNumbersTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            byte[] numbers = new byte[] { 0, 10 };
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(numbers);
            barcode.PlaceBarcode(canvas, ColorConstants.BLUE);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417ByteMoreThanSixSizeNumbersTest() {
            String filename = "barcode417ByteMoreThanSixSizeNumbersTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            byte[] numbers = new byte[] { 0, 10, 11, 12, 13, 30, 50, 70 };
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(numbers);
            barcode.PlaceBarcode(canvas, ColorConstants.BLUE);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417CodeRowsWithBarcodeGenerationTest() {
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCodeRows(150);
            barcode.PlaceBarcode(canvas, null);
            NUnit.Framework.Assert.AreEqual(8, barcode.GetCodeRows());
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417CodeColumnsWithBarcodeGenerationTest() {
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCodeColumns(150);
            barcode.PlaceBarcode(canvas, null);
            NUnit.Framework.Assert.AreEqual(1, barcode.GetCodeColumns());
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417CodeWordsWithBarcodeGenerationTest() {
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetLenCodewords(150);
            barcode.PlaceBarcode(canvas, null);
            NUnit.Framework.Assert.AreEqual(8, barcode.GetLenCodewords());
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417ErrorLevelWithBarcodeGenerationTest() {
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetErrorLevel(3);
            barcode.PlaceBarcode(canvas, null);
            NUnit.Framework.Assert.AreEqual(2, barcode.GetErrorLevel());
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417OptionsWithBarcodeGenerationTest() {
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetOptions(63);
            barcode.PlaceBarcode(canvas, null);
            NUnit.Framework.Assert.AreEqual(63, barcode.GetOptions());
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417OptionsWithBarcodeGenerationInvalidSizeTest() {
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetOptions(64);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => barcode.PlaceBarcode(canvas, null));
            NUnit.Framework.Assert.AreEqual("Invalid codeword size.", e.Message);
            NUnit.Framework.Assert.AreEqual(64, barcode.GetOptions());
        }

        [NUnit.Framework.Test]
        public virtual void LenCodewordsIsNotEnoughTest() {
            BarcodePDF417 barcodePDF417 = new BarcodePDF417();
            barcodePDF417.SetOptions(BarcodePDF417.PDF417_USE_RAW_CODEWORDS);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => barcodePDF417.PaintCode());
            NUnit.Framework.Assert.AreEqual(BarcodesExceptionMessageConstant.INVALID_CODEWORD_SIZE, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LenCodewordsIsTooSmallTest() {
            BarcodePDF417 barcodePDF417 = new BarcodePDF417();
            barcodePDF417.SetOptions(BarcodePDF417.PDF417_USE_RAW_CODEWORDS);
            // lenCodeWords should be bigger than 1
            barcodePDF417.SetLenCodewords(0);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => barcodePDF417.PaintCode());
            NUnit.Framework.Assert.AreEqual(BarcodesExceptionMessageConstant.INVALID_CODEWORD_SIZE, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LenCodewordsMoreThanMaxDataCodewordsTest() {
            BarcodePDF417 barcodePDF417 = new BarcodePDF417();
            barcodePDF417.SetOptions(BarcodePDF417.PDF417_USE_RAW_CODEWORDS);
            // lenCodeWords should be smaller than MAX_DATA_CODEWORDS
            barcodePDF417.SetLenCodewords(927);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => barcodePDF417.PaintCode());
            NUnit.Framework.Assert.AreEqual(BarcodesExceptionMessageConstant.INVALID_CODEWORD_SIZE, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CcittImageFromBarcodeTest() {
            String filename = "ccittImage01.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(text);
            barcode.PaintCode();
            byte[] g4 = CCITTG4Encoder.Compress(barcode.GetOutBits(), barcode.GetBitColumns(), barcode.GetCodeRows());
            ImageData img = ImageDataFactory.Create(barcode.GetBitColumns(), barcode.GetCodeRows(), false, RawImageData
                .CCITTG4, 0, g4, null);
            canvas.AddImageAt(img, 100, 100, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER, "diff_"));
        }

        private PdfFormXObject CreateMacroBarcodePart(PdfDocument document, String text, float mh, float mw, int segmentId
            ) {
            BarcodePDF417 pf = new BarcodePDF417();
            // MacroPDF417 setup
            pf.SetOptions(BarcodePDF417.PDF417_USE_MACRO);
            pf.SetMacroFileId("12");
            pf.SetMacroSegmentCount(2);
            pf.SetMacroSegmentId(segmentId);
            pf.SetCode(text);
            return pf.CreateFormXObject(ColorConstants.BLACK, mw, mh, document);
        }
    }
}

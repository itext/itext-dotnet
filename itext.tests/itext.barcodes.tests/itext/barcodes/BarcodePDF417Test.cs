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
using iText.Kernel;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Barcodes {
    public class BarcodePDF417Test : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/barcodes/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/barcodes/BarcodePDF417/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode01Test() {
            String filename = "barcode417_01.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(text);
            barcode.PlaceBarcode(canvas, null);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode02Test() {
            String filename = "barcode417_02.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfReader reader = new PdfReader(sourceFolder + "DocumentWithTrueTypeFont1.pdf");
            PdfDocument document = new PdfDocument(reader, writer);
            PdfCanvas canvas = new PdfCanvas(document.GetLastPage());
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(text);
            barcode.PlaceBarcode(canvas, null);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void MacroPDF417Test01() {
            String filename = "barcode417Macro_01.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfDocument pdfDocument = new PdfDocument(writer);
            PdfCanvas pdfCanvas = new PdfCanvas(pdfDocument.AddNewPage());
            pdfCanvas.AddXObject(CreateMacroBarcodePart(pdfDocument, "This is PDF417 segment 0", 1, 1, 0), 1, 0, 0, 1, 
                36, 791);
            pdfCanvas.AddXObject(CreateMacroBarcodePart(pdfDocument, "This is PDF417 segment 1", 1, 1, 1), 1, 0, 0, 1, 
                36, 676);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
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

        [NUnit.Framework.Test]
        public virtual void Barcode417XObjectTest() {
            String filename = "barcode417XObjectTest.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(text);
            PdfFormXObject xObject = barcode.CreateFormXObject(document);
            canvas.AddXObject(xObject, 10, 650);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }

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
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417NumbersTest() {
            String filename = "barcode417NumbersTest.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            String numbers = "1234567890";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(numbers);
            barcode.PlaceBarcode(canvas, null);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417ByteLessThanSixSizeNumbersTest() {
            String filename = "barcode417ByteLessThanSixSizeNumbersTest.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            byte[] numbers = new byte[] { 0, 10 };
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(numbers);
            barcode.PlaceBarcode(canvas, ColorConstants.BLUE);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417ByteMoreThanSixSizeNumbersTest() {
            String filename = "barcode417ByteMoreThanSixSizeNumbersTest.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            byte[] numbers = new byte[] { 0, 10, 11, 12, 13, 30, 50, 70 };
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(numbers);
            barcode.PlaceBarcode(canvas, ColorConstants.BLUE);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
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

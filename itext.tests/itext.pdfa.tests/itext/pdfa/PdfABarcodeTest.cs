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
using System.IO;
using iText.Barcodes;
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfABarcodeTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfABarcodeTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfABarcodeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeMSITest() {
            String outPdf = destinationFolder + "barcodeMSITest.pdf";
            String cmpPdf = cmpFolder + "cmp_barcodeMSITest.pdf";
            Document doc = CreatePdfATaggedDocument(outPdf);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            font.SetSubset(true);
            BarcodeMSI codeMSI = new BarcodeMSI(doc.GetPdfDocument(), font);
            FillBarcode1D(codeMSI, "1234567");
            PdfFormXObject barcode = codeMSI.CreateFormXObject(doc.GetPdfDocument());
            Image img = new Image(barcode).SetMargins(0, 0, 0, 0);
            img.GetAccessibilityProperties().SetAlternateDescription("hello world!");
            doc.Add(img);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeInter25Test() {
            String outPdf = destinationFolder + "barcodeInter25Test.pdf";
            String cmpPdf = cmpFolder + "cmp_barcodeInter25Test.pdf";
            Document doc = CreatePdfATaggedDocument(outPdf);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            font.SetSubset(true);
            BarcodeInter25 codeInter25 = new BarcodeInter25(doc.GetPdfDocument(), font);
            FillBarcode1D(codeInter25, "1234567");
            PdfFormXObject barcode = codeInter25.CreateFormXObject(doc.GetPdfDocument());
            Image img = new Image(barcode).SetMargins(0, 0, 0, 0);
            img.GetAccessibilityProperties().SetAlternateDescription("hello world!");
            doc.Add(img);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeEANTest() {
            String outPdf = destinationFolder + "barcodeEANTest.pdf";
            String cmpPdf = cmpFolder + "cmp_barcodeEANTest.pdf";
            Document doc = CreatePdfATaggedDocument(outPdf);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            font.SetSubset(true);
            BarcodeEAN codeEAN = new BarcodeEAN(doc.GetPdfDocument(), font);
            FillBarcode1D(codeEAN, "9781935182610");
            PdfFormXObject barcode = codeEAN.CreateFormXObject(doc.GetPdfDocument());
            Image img = new Image(barcode).SetMargins(0, 0, 0, 0);
            img.GetAccessibilityProperties().SetAlternateDescription("hello world!");
            doc.Add(img);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeCodabarTest() {
            String outPdf = destinationFolder + "barcodeCodabarTest.pdf";
            String cmpPdf = cmpFolder + "cmp_barcodeCodabarTest.pdf";
            Document doc = CreatePdfATaggedDocument(outPdf);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            font.SetSubset(true);
            BarcodeCodabar codeCodabar = new BarcodeCodabar(doc.GetPdfDocument(), font);
            FillBarcode1D(codeCodabar, "A123A");
            PdfFormXObject barcode = codeCodabar.CreateFormXObject(doc.GetPdfDocument());
            Image img = new Image(barcode).SetMargins(0, 0, 0, 0);
            img.GetAccessibilityProperties().SetAlternateDescription("hello world!");
            doc.Add(img);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode39Test() {
            String outPdf = destinationFolder + "barcode39Test.pdf";
            String cmpPdf = cmpFolder + "cmp_barcode39Test.pdf";
            Document doc = CreatePdfATaggedDocument(outPdf);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            font.SetSubset(true);
            Barcode39 code39 = new Barcode39(doc.GetPdfDocument(), font);
            FillBarcode1D(code39, "1234567");
            PdfFormXObject barcode = code39.CreateFormXObject(doc.GetPdfDocument());
            Image img = new Image(barcode).SetMargins(0, 0, 0, 0);
            img.GetAccessibilityProperties().SetAlternateDescription("hello world!");
            doc.Add(img);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode128Test() {
            String outPdf = destinationFolder + "barcode128Test.pdf";
            String cmpPdf = cmpFolder + "cmp_barcode128Test.pdf";
            Document doc = CreatePdfATaggedDocument(outPdf);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            font.SetSubset(true);
            Barcode128 code128 = new Barcode128(doc.GetPdfDocument(), font);
            FillBarcode1D(code128, "1234567");
            PdfFormXObject barcode = code128.CreateFormXObject(doc.GetPdfDocument());
            Image img = new Image(barcode).SetMargins(0, 0, 0, 0);
            img.GetAccessibilityProperties().SetAlternateDescription("hello world!");
            doc.Add(img);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        private void FillBarcode1D(Barcode1D barcode1D, String code) {
            barcode1D.SetCode(code);
            barcode1D.SetCodeType(Barcode128.CODE128);
            barcode1D.SetSize(10);
            barcode1D.SetBaseline(barcode1D.GetSize());
            barcode1D.SetGenerateChecksum(true);
            barcode1D.SetX(1);
            barcode1D.SetN(5);
            barcode1D.SetBarHeight(20);
            barcode1D.SetChecksumText(false);
        }

        private Document CreatePdfATaggedDocument(String outPdf) {
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfDocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            Document doc = new Document(pdfDocument);
            pdfDocument.SetTagged();
            return doc;
        }

        private void CompareResult(String outFile, String cmpFile) {
            String differences = new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_");
            if (differences != null) {
                NUnit.Framework.Assert.Fail(differences);
            }
        }
    }
}

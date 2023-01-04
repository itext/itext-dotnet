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
using iText.Barcodes;
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
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
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

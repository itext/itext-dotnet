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

        [NUnit.Framework.Test]
        public virtual void Barcode01Test() {
            String filename = "barcodeMSI_01.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
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
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
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
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
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
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
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

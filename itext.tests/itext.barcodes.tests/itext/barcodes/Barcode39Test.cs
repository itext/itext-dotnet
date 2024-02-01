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
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Barcodes {
    [NUnit.Framework.Category("IntegrationTest")]
    public class Barcode39Test : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/barcodes/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/barcodes/Barcode39/";

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
            String filename = "barcode39_01.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Barcode1D barcode = new Barcode39(document);
            barcode.SetCode("9781935182610");
            barcode.SetTextAlignment(Barcode1D.ALIGN_LEFT);
            barcode.PlaceBarcode(canvas, ColorConstants.BLACK, ColorConstants.BLACK);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode02Test() {
            String filename = "barcode39_02.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "DocumentWithTrueTypeFont1.pdf"), CompareTool
                .CreateTestPdfWriter(destinationFolder + filename));
            PdfCanvas canvas = new PdfCanvas(document.GetLastPage());
            Barcode1D barcode = new Barcode39(document);
            barcode.SetCode("9781935182610");
            barcode.SetTextAlignment(Barcode1D.ALIGN_LEFT);
            barcode.PlaceBarcode(canvas, ColorConstants.BLACK, ColorConstants.BLACK);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Barcode03Test() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Barcode39 barcode = new Barcode39(document);
            try {
                Barcode39.GetBarsCode39("9781935*182610");
                NUnit.Framework.Assert.Fail("IllegalArgumentException expected");
            }
            catch (ArgumentException) {
            }
        }
    }
}

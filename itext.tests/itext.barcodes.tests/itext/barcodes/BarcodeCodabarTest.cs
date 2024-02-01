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
using iText.Barcodes.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Barcodes {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BarcodeCodabarTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/barcodes/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/barcodes/Codabar/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode01Test() {
            String filename = "codabar.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + filename);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            BarcodeCodabar codabar = new BarcodeCodabar(document);
            codabar.SetCode("A123A");
            codabar.SetStartStopText(true);
            codabar.PlaceBarcode(canvas, null, null);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeHasNoAbcdAsStartCharacterTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            BarcodeCodabar codabar = new BarcodeCodabar(pdfDocument);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => BarcodeCodabar.GetBarsCodabar
                ("qbcd"));
            NUnit.Framework.Assert.AreEqual(BarcodesExceptionMessageConstant.CODABAR_MUST_HAVE_ONE_ABCD_AS_START_STOP_CHARACTER
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeHasNoAbcdAsStopCharacterTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            BarcodeCodabar codabar = new BarcodeCodabar(pdfDocument);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => BarcodeCodabar.GetBarsCodabar
                ("abcf"));
            NUnit.Framework.Assert.AreEqual(BarcodesExceptionMessageConstant.CODABAR_MUST_HAVE_ONE_ABCD_AS_START_STOP_CHARACTER
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeHasNoAbcdAsStartAndStopCharacterTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            BarcodeCodabar codabar = new BarcodeCodabar(pdfDocument);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => BarcodeCodabar.GetBarsCodabar
                ("qbcq"));
            NUnit.Framework.Assert.AreEqual(BarcodesExceptionMessageConstant.CODABAR_MUST_HAVE_ONE_ABCD_AS_START_STOP_CHARACTER
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeHasNoStartAndStopCharacterTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            BarcodeCodabar codabar = new BarcodeCodabar(pdfDocument);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => BarcodeCodabar.GetBarsCodabar
                (""));
            NUnit.Framework.Assert.AreEqual(BarcodesExceptionMessageConstant.CODABAR_MUST_HAVE_AT_LEAST_START_AND_STOP_CHARACTER
                , exception.Message);
        }
    }
}

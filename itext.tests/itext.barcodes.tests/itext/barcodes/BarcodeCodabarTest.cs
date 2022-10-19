/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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

        [NUnit.Framework.Test]
        public virtual void Barcode01Test() {
            String filename = "codabar.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
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
            NUnit.Framework.Assert.AreEqual(BarcodeExceptionMessageConstant.CODABAR_MUST_HAVE_ONE_ABCD_AS_START_STOP_CHARACTER
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeHasNoAbcdAsStopCharacterTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            BarcodeCodabar codabar = new BarcodeCodabar(pdfDocument);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => BarcodeCodabar.GetBarsCodabar
                ("abcf"));
            NUnit.Framework.Assert.AreEqual(BarcodeExceptionMessageConstant.CODABAR_MUST_HAVE_ONE_ABCD_AS_START_STOP_CHARACTER
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeHasNoAbcdAsStartAndStopCharacterTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            BarcodeCodabar codabar = new BarcodeCodabar(pdfDocument);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => BarcodeCodabar.GetBarsCodabar
                ("qbcq"));
            NUnit.Framework.Assert.AreEqual(BarcodeExceptionMessageConstant.CODABAR_MUST_HAVE_ONE_ABCD_AS_START_STOP_CHARACTER
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BarcodeHasNoStartAndStopCharacterTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            BarcodeCodabar codabar = new BarcodeCodabar(pdfDocument);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => BarcodeCodabar.GetBarsCodabar
                (""));
            NUnit.Framework.Assert.AreEqual(BarcodeExceptionMessageConstant.CODABAR_MUST_HAVE_AT_LEAST_START_AND_STOP_CHARACTER
                , exception.Message);
        }
    }
}

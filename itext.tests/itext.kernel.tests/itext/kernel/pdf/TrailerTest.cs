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
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Test;

namespace iText.Kernel.Pdf {
    /// <author>Michael Demey</author>
    public class TrailerTest : ExtendedITextTest {
        private ProductInfo productInfo;

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/TrailerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.SetUp]
        public virtual void BeforeTest() {
            this.productInfo = new ProductInfo("pdfProduct", 1, 0, 0, true);
        }

        [NUnit.Framework.Test]
        public virtual void TrailerFingerprintTest() {
            FileStream fos = new FileStream(destinationFolder + "output.pdf", FileMode.Create);
            PdfDocument pdf = new PdfDocument(new PdfWriter(fos));
            pdf.RegisterProduct(this.productInfo);
            PdfPage page = pdf.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(), 12f).ShowText("Hello World").EndText();
            pdf.Close();
            NUnit.Framework.Assert.IsTrue(DoesTrailerContainFingerprint(new FileInfo(destinationFolder + "output.pdf")
                , productInfo.ToString()));
        }

        private bool DoesTrailerContainFingerprint(FileInfo file, String fingerPrint) {
            FileStream raf = FileUtil.GetRandomAccessFile(file);
            // put the pointer at the end of the file
            raf.Seek(raf.Length);
            // look for startxref
            String startxref = "startxref";
            String templine = "";
            while (!templine.Contains(startxref)) {
                templine = (char)raf.ReadByte() + templine;
                raf.Seek(raf.Position - 2);
            }
            // look for fingerprint
            char read = ' ';
            templine = "";
            while (read != '%') {
                read = (char)raf.ReadByte();
                templine = read + templine;
                raf.Seek(raf.Position - 2);
            }
            bool output = templine.Contains(fingerPrint);
            raf.Dispose();
            return output;
        }
    }
}

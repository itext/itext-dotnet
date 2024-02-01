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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Actions.Data;
using iText.Commons.Utils;
using iText.Kernel.Actions.Data;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TrailerTest : ExtendedITextTest {
        private ProductData productData;

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/TrailerTest/";

        private static readonly byte[] USERPASS = "user".GetBytes();

        private static readonly byte[] OWNERPASS = "owner".GetBytes();

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.SetUp]
        public virtual void BeforeTest() {
            this.productData = new ProductData("pdfProduct", "pdfProduct", "1.0.0", 1900, 2000);
        }

        [NUnit.Framework.Test]
        public virtual void TrailerFingerprintTest() {
            PdfDocument pdf = new PdfDocument(new PdfWriter(destinationFolder + "output.pdf"));
            pdf.RegisterProduct(this.productData);
            PdfPage page = pdf.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(), 12f).ShowText("Hello World").EndText();
            pdf.Close();
            NUnit.Framework.Assert.IsTrue(DoesTrailerContainFingerprint(new FileInfo(destinationFolder + "output.pdf")
                , MessageFormatUtil.Format("%iText-{0}-{1}\n", productData.GetProductName(), productData.GetVersion())
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ExistingTrailerValuesTest() {
            MemoryStream baos = new MemoryStream();
            PdfName expectedKey = new PdfName("Custom");
            PdfName expectedValue = new PdfName("Value");
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                pdfDocument.GetTrailer().Put(expectedKey, expectedValue);
            }
            using (PdfDocument stampingDocument = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())), new 
                PdfWriter(new MemoryStream()))) {
                PdfDictionary trailer = stampingDocument.GetTrailer();
                bool keyPresent = trailer.ContainsKey(expectedKey);
                PdfName actualValue = trailer.GetAsName(expectedKey);
                stampingDocument.Close();
                NUnit.Framework.Assert.IsTrue(keyPresent);
                NUnit.Framework.Assert.AreEqual(expectedValue, actualValue);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ExistingTrailerValuesTestWithEncryption() {
            MemoryStream baos = new MemoryStream();
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetStandardEncryption(USERPASS, OWNERPASS, EncryptionConstants.ALLOW_PRINTING, EncryptionConstants
                .ENCRYPTION_AES_128);
            PdfName expectedKey = new PdfName("Custom");
            PdfName expectedValue = new PdfName("Value");
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos, writerProperties))) {
                pdfDocument.GetTrailer().Put(expectedKey, expectedValue);
            }
            ReaderProperties readerProperties = new ReaderProperties().SetPassword(OWNERPASS);
            using (PdfDocument stampingDocument = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray()), readerProperties
                ), new PdfWriter(new MemoryStream()))) {
                PdfDictionary trailer = stampingDocument.GetTrailer();
                bool keyPresent = trailer.ContainsKey(expectedKey);
                PdfName actualValue = trailer.GetAsName(expectedKey);
                stampingDocument.Close();
                NUnit.Framework.Assert.IsTrue(keyPresent);
                NUnit.Framework.Assert.AreEqual(expectedValue, actualValue);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ExistingTrailerValuesWithStandardizedNameTest() {
            MemoryStream baos = new MemoryStream();
            Dictionary<PdfName, PdfName> standardizedNames = new Dictionary<PdfName, PdfName>();
            //some standardized names to put in the trailer, but they may not be removed
            standardizedNames.Put(PdfName.Color, new PdfName("brown"));
            standardizedNames.Put(PdfName.BaseFont, new PdfName("CustomFont"));
            standardizedNames.Put(PdfName.Pdf_Version_1_6, new PdfName("1.6"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                foreach (KeyValuePair<PdfName, PdfName> entry in standardizedNames) {
                    PdfName pdfName = entry.Key;
                    PdfName s = entry.Value;
                    pdfDocument.GetTrailer().Put(pdfName, s);
                }
            }
            using (PdfDocument stampingDocument = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())), new 
                PdfWriter(new MemoryStream()))) {
                PdfDictionary trailer = stampingDocument.GetTrailer();
                foreach (KeyValuePair<PdfName, PdfName> entry in standardizedNames) {
                    PdfName pdfName = entry.Key;
                    PdfName pdfName2 = entry.Value;
                    bool keyPresent = trailer.ContainsKey(pdfName);
                    PdfName actualValue = trailer.GetAsName(pdfName);
                    NUnit.Framework.Assert.IsTrue(keyPresent);
                    NUnit.Framework.Assert.AreEqual(pdfName2, actualValue);
                }
                stampingDocument.Close();
            }
        }

        private bool DoesTrailerContainFingerprint(FileInfo file, String fingerPrint) {
            using (FileStream raf = FileUtil.GetRandomAccessFile(file)) {
                // put the pointer at the end of the file
                raf.Seek(raf.Length);
                // look for coreProductData
                String coreProductData = "%iText-Core-" + ITextCoreProductData.GetInstance().GetVersion();
                String templine = "";
                while (!templine.Contains(coreProductData)) {
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
                return templine.Contains(fingerPrint);
            }
        }
    }
}

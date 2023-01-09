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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Actions.Data;
using iText.Commons.Utils;
using iText.Kernel.Actions.Data;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Test;

namespace iText.Kernel.Pdf {
    /// <author>Michael Demey</author>
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
            FileStream fos = new FileStream(destinationFolder + "output.pdf", FileMode.Create);
            PdfDocument pdf = new PdfDocument(new PdfWriter(fos));
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

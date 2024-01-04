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
using iText.Kernel.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfEncryptorTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfEncryptorTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfEncryptorTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptFileTest() {
            String outFileName = DESTINATION_FOLDER + "encryptFileTest.pdf";
            String initialFileName = SOURCE_FOLDER + "initial.pdf";
            PdfEncryptor encryptor = new PdfEncryptor();
            EncryptionProperties encryptionProperties = new EncryptionProperties();
            encryptionProperties.SetStandardEncryption(new byte[16], new byte[16], 0, 0);
            encryptor.SetEncryptionProperties(encryptionProperties);
            using (PdfReader initialFile = new PdfReader(initialFileName)) {
                using (FileStream outputStream = new FileStream(outFileName, FileMode.Create)) {
                    encryptor.Encrypt(initialFile, outputStream);
                }
            }
            ReaderProperties readerProperties = new ReaderProperties();
            readerProperties.SetPassword(new byte[16]);
            PdfReader outFile = new PdfReader(outFileName, readerProperties);
            PdfDocument doc = new PdfDocument(outFile);
            doc.Close();
            NUnit.Framework.Assert.IsTrue(outFile.IsEncrypted());
        }
    }
}

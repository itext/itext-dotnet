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
using iText.IO.Source;
using iText.Kernel.Font;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Crypto {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class UnencryptedWrapperTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto/UnencryptedWrapperTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/UnencryptedWrapperTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleWrapperDocumentTest() {
            CreateWrapper("customEncryptedDocument.pdf", "simpleUnencryptedWrapper.pdf", "iText");
        }

        [NUnit.Framework.Test]
        public virtual void ExtractCustomEncryptedDocumentTest() {
            ExtractEncrypted("customEncryptedDocument.pdf", "simpleUnencryptedWrapper.pdf", null);
        }

        [NUnit.Framework.Test]
        public virtual void CreateWrapperForStandardEncryptedTest() {
            CreateWrapper("standardEncryptedDocument.pdf", "standardUnencryptedWrapper.pdf", "Standard");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ExtractStandardEncryptedDocumentTest() {
            ExtractEncrypted("standardEncryptedDocument.pdf", "standardUnencryptedWrapper.pdf", "World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ));
        }

        private void CreateWrapper(String encryptedName, String wrapperName, String cryptoFilter) {
            String inPath = sourceFolder + "cmp_" + encryptedName;
            String cmpPath = sourceFolder + "cmp_" + wrapperName;
            String outPath = destinationFolder + wrapperName;
            String diff = "diff_" + wrapperName + "_";
            PdfDocument document = new PdfDocument(new PdfWriter(outPath, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            PdfFileSpec fs = PdfEncryptedPayloadFileSpecFactory.Create(document, inPath, new PdfEncryptedPayload(cryptoFilter
                ));
            document.SetEncryptedPayload(fs);
            PdfFont font = PdfFontFactory.CreateFont();
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(font, 30).ShowText("Hi! I'm wrapper document."
                ).EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        private void ExtractEncrypted(String encryptedName, String wrapperName, byte[] password) {
            String inPath = sourceFolder + "cmp_" + wrapperName;
            String cmpPath = sourceFolder + "cmp_" + encryptedName;
            String outPath = destinationFolder + encryptedName;
            String diff = "diff_" + encryptedName + "_";
            PdfDocument document = new PdfDocument(new PdfReader(inPath));
            PdfEncryptedPayloadDocument encryptedDocument = document.GetEncryptedPayloadDocument();
            byte[] encryptedDocumentBytes = encryptedDocument.GetDocumentBytes();
            FileStream fos = new FileStream(outPath, FileMode.Create);
            fos.Write(encryptedDocumentBytes);
            fos.Dispose();
            document.Close();
            PdfEncryptedPayload ep = encryptedDocument.GetEncryptedPayload();
            NUnit.Framework.Assert.AreEqual(PdfEncryptedPayloadFileSpecFactory.GenerateFileDisplay(ep), encryptedDocument
                .GetName());
            if (password != null) {
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                    , password, password));
            }
            else {
                RandomAccessFileOrArray raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource
                    (cmpPath));
                byte[] cmpBytes = new byte[(int)raf.Length()];
                raf.ReadFully(cmpBytes);
                raf.Close();
                NUnit.Framework.Assert.AreEqual(cmpBytes, encryptedDocumentBytes);
            }
        }
    }
}

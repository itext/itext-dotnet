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

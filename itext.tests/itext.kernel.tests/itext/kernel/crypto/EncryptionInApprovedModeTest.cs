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
using NUnit.Framework;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Crypto {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class EncryptionInApprovedModeTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto" + "/EncryptionInApprovedModeTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto" + "/EncryptionInApprovedModeTest/";

        /// <summary>User password.</summary>
        public static byte[] USER = "Hello".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);

        /// <summary>Owner password.</summary>
        public static byte[] OWNER = "World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            NUnit.Framework.Assume.That(FACTORY.IsInApprovedOnlyMode());
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT)]
        public virtual void CheckMD5LogMessageWhileReadingPdfTest() {
            String fileName = "checkMD5LogMessageWhileReadingPdf.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + fileName))) {
            }
        }

        // this test checks log message
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT)]
        public virtual void CheckMD5LogMessageWhileCreatingPdfTest() {
            String fileName = "checkMD5LogMessageWhileCreatingPdf.pdf";
            using (PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + fileName, new WriterProperties
                ().SetStandardEncryption(USER, OWNER, EncryptionConstants.ALLOW_SCREENREADERS, EncryptionConstants.ENCRYPTION_AES_256
                ).AddXmpMetadata()))) {
            }
        }

        // this test checks log message
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Count = 3)]
        public virtual void CheckMD5LogMessageForEachPdfTest() {
            String fileName = "checkMD5LogMessageForEachPdf.pdf";
            for (int i = 0; i < 3; ++i) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + fileName, new WriterProperties
                    ().SetStandardEncryption(USER, OWNER, EncryptionConstants.ALLOW_SCREENREADERS, EncryptionConstants.ENCRYPTION_AES_256
                    ).AddXmpMetadata()))) {
                }
            }
        }
        // this test checks log message
    }
}

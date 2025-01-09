/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Crypto;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Crypto.Securityhandler {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class PubKeySecurityHandlerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ComputeGlobalKeyDecryptTest() {
            PubKeySecurityHandler securityHandler = new PubKeySecurityHandlerTest.TestSecurityHandler();
            NUnit.Framework.Assert.AreEqual(20, securityHandler.ComputeGlobalKey("SHA1", false).Length);
        }

        private class TestSecurityHandler : PubKeySecurityHandler {
            public override OutputStreamEncryption GetEncryptionStream(Stream os) {
                throw new NotSupportedException();
            }

            public override IDecryptor GetDecryptor() {
                throw new NotSupportedException();
            }

            protected internal override void SetPubSecSpecificHandlerDicEntries(PdfDictionary encryptionDictionary, bool
                 encryptMetadata, bool embeddedFilesOnly) {
                throw new NotSupportedException();
            }

            protected internal override String GetDigestAlgorithm() {
                throw new NotSupportedException();
            }

            protected internal override void InitKey(byte[] globalKey, int keyLength) {
                throw new NotSupportedException();
            }
        }
    }
}

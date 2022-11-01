using System;
using System.IO;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Crypto.Securityhandler {
    [NUnit.Framework.Category("UnitTest")]
    public class PubKeySecurityHandlerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
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

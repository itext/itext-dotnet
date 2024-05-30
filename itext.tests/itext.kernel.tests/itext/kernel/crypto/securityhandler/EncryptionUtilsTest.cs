using System;
using iText.Test;

namespace iText.Kernel.Crypto.Securityhandler {
    [NUnit.Framework.Category("UnitTest")]
    public class EncryptionUtilsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void FetchEnvelopedDataThrows() {
            NUnit.Framework.Assert.Catch(typeof(Exception), () => EncryptionUtils.FetchEnvelopedData(null, null, null)
                );
        }
    }
}

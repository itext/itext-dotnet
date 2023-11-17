using System.Collections.Generic;
using System.IO;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Commons.Bouncycastle {
    /// <summary>
    /// This class contains util methods, which use bouncy-castle objects.
    /// </summary>
    public interface IBouncyCastleUtil {
        /// <summary>
        /// Read <see cref="IX509Certificate"/> objects encoded in PKCS7 format
        /// </summary>
        /// <param name="data"><see cref="Stream"/> containing PKCS7 encoded certificates</param>
        /// <returns><see cref="List<IX509Certificate>"/> of certificates</returns>
        List<IX509Certificate> ReadPkcs7Certs(Stream data);
    }
}
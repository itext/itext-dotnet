using System;
using iText.Commons.Bouncycastle;

namespace iText.Bouncycastle {
    public class BouncyCastleTestConstantsFactory : IBouncyCastleTestConstantsFactory {
        public virtual String GetCertificateInfoTestConst() {
            return "corrupted stream - out of bounds length found: 8 >= 6";
        }
    }
}

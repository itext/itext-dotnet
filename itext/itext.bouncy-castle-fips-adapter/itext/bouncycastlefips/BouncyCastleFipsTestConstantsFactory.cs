using System;
using iText.Commons.Bouncycastle;

namespace iText.Bouncycastlefips {
    public class BouncyCastleFipsTestConstantsFactory : IBouncyCastleTestConstantsFactory {
        public virtual String GetCertificateInfoTestConst() {
            return "DEF length 8 object truncated by 4";
        }
    }
}

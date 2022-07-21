using System;

namespace iText.Commons.Bouncycastle.Asn1 {
    public interface IASN1Encoding {
        String GetDer();

        String GetBer();
    }
}

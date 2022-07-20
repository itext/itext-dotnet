using System;

namespace iText.Commons.Bouncycastle.Asn1 {
    public interface IASN1Primitive : IASN1Encodable {
        byte[] GetEncoded();

        byte[] GetEncoded(String encoding);
    }
}

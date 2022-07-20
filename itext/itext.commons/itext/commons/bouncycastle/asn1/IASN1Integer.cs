using Org.BouncyCastle.Math;

namespace iText.Commons.Bouncycastle.Asn1 {
    public interface IASN1Integer : IASN1Primitive {
        BigInteger GetValue();
    }
}

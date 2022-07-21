using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Asn1 {
    public interface IASN1Integer : IASN1Primitive {
        IBigInteger GetValue();
    }
}

using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Asn1 {
    public interface IASN1EncodableVector {
        void Add(IASN1Primitive primitive);

        void Add(IAttribute attribute);

        void Add(IAlgorithmIdentifier element);
    }
}

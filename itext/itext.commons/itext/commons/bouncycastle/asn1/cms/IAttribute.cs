using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Cms {
    public interface IAttribute : IASN1Encodable {
        IASN1Set GetAttrValues();
    }
}

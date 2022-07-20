using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Cms {
    public interface IAttributeTable {
        IAttribute Get(IASN1ObjectIdentifier oid);
    }
}

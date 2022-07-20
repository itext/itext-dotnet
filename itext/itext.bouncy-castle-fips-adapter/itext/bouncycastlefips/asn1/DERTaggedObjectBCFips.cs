using  Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class DERTaggedObjectBCFips : ASN1TaggedObjectBCFips, IDERTaggedObject {
        public DERTaggedObjectBCFips(DerTaggedObject derTaggedObject)
            : base(derTaggedObject) {
        }

        public DERTaggedObjectBCFips(int i, Asn1Encodable encodable)
            : base(new DerTaggedObject(i, encodable)) {
        }

        public DERTaggedObjectBCFips(bool b, int i, Asn1Encodable encodable)
            : base(new DerTaggedObject(b, i, encodable)) {
        }

        public virtual DerTaggedObject GetDERTaggedObject() {
            return (DerTaggedObject)GetEncodable();
        }
    }
}

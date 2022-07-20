using  Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class DERTaggedObjectBC : ASN1TaggedObjectBC, IDERTaggedObject {
        public DERTaggedObjectBC(DerTaggedObject derTaggedObject)
            : base(derTaggedObject) {
        }

        public DERTaggedObjectBC(int i, Asn1Encodable encodable)
            : base(new DerTaggedObject(i, encodable)) {
        }

        public DERTaggedObjectBC(bool b, int i, Asn1Encodable encodable)
            : base(new DerTaggedObject(b, i, encodable)) {
        }

        public virtual DerTaggedObject GetDERTaggedObject() {
            return (DerTaggedObject)GetEncodable();
        }
    }
}

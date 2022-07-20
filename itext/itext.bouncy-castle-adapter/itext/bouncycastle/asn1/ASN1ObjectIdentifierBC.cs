using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    public class ASN1ObjectIdentifierBC : ASN1PrimitiveBC, IASN1ObjectIdentifier {
        public ASN1ObjectIdentifierBC(String identifier)
            : base(new DerObjectIdentifier(identifier)) {
        }

        public ASN1ObjectIdentifierBC(DerObjectIdentifier identifier)
            : base(identifier) {
        }

        public virtual DerObjectIdentifier GetASN1ObjectIdentifier() {
            return (DerObjectIdentifier)GetPrimitive();
        }

        public virtual String GetId() {
            return GetASN1ObjectIdentifier().Id;
        }
    }
}

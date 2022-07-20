using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    public class ASN1ObjectIdentifierBCFips : ASN1PrimitiveBCFips, IASN1ObjectIdentifier {
        public ASN1ObjectIdentifierBCFips(String identifier)
            : base(new DerObjectIdentifier(identifier)) {
        }

        public ASN1ObjectIdentifierBCFips(DerObjectIdentifier identifier)
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

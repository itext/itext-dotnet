using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerObjectIdentifier"/>.
    /// </summary>
    public class ASN1ObjectIdentifierBC : ASN1PrimitiveBC, IASN1ObjectIdentifier {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerObjectIdentifier"/>.
        /// </summary>
        /// <param name="identifier">
        /// string to create
        /// <see cref="Org.BouncyCastle.Asn1.DerObjectIdentifier"/>
        /// </param>
        public ASN1ObjectIdentifierBC(String identifier)
            : base(new DerObjectIdentifier(identifier)) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerObjectIdentifier"/>.
        /// </summary>
        /// <param name="identifier">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerObjectIdentifier"/>
        /// to be wrapped
        /// </param>
        public ASN1ObjectIdentifierBC(DerObjectIdentifier identifier)
            : base(identifier) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerObjectIdentifier"/>.
        /// </returns>
        public virtual DerObjectIdentifier GetASN1ObjectIdentifier() {
            return (DerObjectIdentifier)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetId() {
            return GetASN1ObjectIdentifier().Id;
        }
    }
}

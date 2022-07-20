using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerObjectIdentifier"/>.
    /// </summary>
    public class ASN1ObjectIdentifierBCFips : ASN1PrimitiveBCFips, IASN1ObjectIdentifier {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerObjectIdentifier"/>.
        /// </summary>
        /// <param name="identifier">
        /// string to create
        /// <see cref="Org.BouncyCastle.Asn1.DerObjectIdentifier"/>
        /// </param>
        public ASN1ObjectIdentifierBCFips(String identifier)
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
        public ASN1ObjectIdentifierBCFips(DerObjectIdentifier identifier)
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

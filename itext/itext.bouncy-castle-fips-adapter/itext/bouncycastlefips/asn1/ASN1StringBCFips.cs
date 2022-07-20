using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerStringBase"/>.
    /// </summary>
    public class ASN1StringBCFips : IASN1String {
        private readonly DerStringBase asn1String;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerStringBase"/>.
        /// </summary>
        /// <param name="asn1String">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerStringBase"/>
        /// to be wrapped
        /// </param>
        public ASN1StringBCFips(DerStringBase asn1String) {
            this.asn1String = asn1String;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerStringBase"/>.
        /// </returns>
        public virtual DerStringBase GetAsn1String() {
            return asn1String;
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetString() {
            return asn1String.GetString();
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Asn1.ASN1StringBCFips that = (iText.Bouncycastlefips.Asn1.ASN1StringBCFips)o;
            return Object.Equals(asn1String, that.asn1String);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(asn1String);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return asn1String.ToString();
        }
    }
}

using System;
using Org.BouncyCastle.Asn1.Utilities;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Util;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1.Util {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Utilities.Asn1Dump"/>.
    /// </summary>
    public class ASN1DumpBC : IASN1Dump {
        private static readonly iText.Bouncycastle.Asn1.Util.ASN1DumpBC INSTANCE = new iText.Bouncycastle.Asn1.Util.ASN1DumpBC
            (null);

        private readonly Asn1Dump asn1Dump;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Utilities.Asn1Dump"/>.
        /// </summary>
        /// <param name="asn1Dump">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Utilities.Asn1Dump"/>
        /// to be wrapped
        /// </param>
        public ASN1DumpBC(Asn1Dump asn1Dump) {
            this.asn1Dump = asn1Dump;
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="ASN1DumpBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Asn1.Util.ASN1DumpBC GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Utilities.Asn1Dump"/>.
        /// </returns>
        public virtual Asn1Dump GetAsn1Dump() {
            return asn1Dump;
        }

        /// <summary><inheritDoc/></summary>
        public virtual String DumpAsString(Object obj, bool b) {
            if (obj is ASN1EncodableBC) {
                obj = ((ASN1EncodableBC)obj).GetEncodable();
            }
            return Asn1Dump.DumpAsString(obj, b);
        }

        /// <summary><inheritDoc/></summary>
        public virtual String DumpAsString(Object obj) {
            if (obj is ASN1EncodableBC) {
                obj = ((ASN1EncodableBC)obj).GetEncodable();
            }
            return Asn1Dump.DumpAsString(obj);
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
            iText.Bouncycastle.Asn1.Util.ASN1DumpBC that = (iText.Bouncycastle.Asn1.Util.ASN1DumpBC)o;
            return Object.Equals(asn1Dump, that.asn1Dump);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(asn1Dump);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return asn1Dump.ToString();
        }
    }
}

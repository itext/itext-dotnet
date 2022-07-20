using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.KeyUsage"/>.
    /// </summary>
    public class KeyUsageBC : ASN1EncodableBC, IKeyUsage {
        private static readonly iText.Bouncycastle.Asn1.X509.KeyUsageBC INSTANCE = new iText.Bouncycastle.Asn1.X509.KeyUsageBC
            (null);

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.KeyUsage"/>.
        /// </summary>
        /// <param name="keyUsage">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.KeyUsage"/>
        /// to be wrapped
        /// </param>
        public KeyUsageBC(KeyUsage keyUsage)
            : base(keyUsage) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="KeyUsageBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Asn1.X509.KeyUsageBC GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.KeyUsage"/>.
        /// </returns>
        public virtual KeyUsage GetKeyUsage() {
            return (KeyUsage)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetDigitalSignature() {
            return KeyUsage.DigitalSignature;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetNonRepudiation() {
            return KeyUsage.NonRepudiation;
        }
    }
}

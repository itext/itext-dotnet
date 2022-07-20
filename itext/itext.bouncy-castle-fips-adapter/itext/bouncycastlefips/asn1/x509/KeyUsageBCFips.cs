using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.KeyUsage"/>.
    /// </summary>
    public class KeyUsageBCFips : ASN1EncodableBCFips, IKeyUsage {
        private static readonly iText.Bouncycastlefips.Asn1.X509.KeyUsageBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.X509.KeyUsageBCFips
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
        public KeyUsageBCFips(KeyUsage keyUsage)
            : base(keyUsage) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="KeyUsageBCFips"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastlefips.Asn1.X509.KeyUsageBCFips GetInstance() {
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

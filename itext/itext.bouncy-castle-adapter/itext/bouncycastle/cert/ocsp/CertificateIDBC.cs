using System;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>.
    /// </summary>
    public class CertificateIDBC : ICertificateID {
        private static readonly iText.Bouncycastle.Cert.Ocsp.CertificateIDBC INSTANCE = new iText.Bouncycastle.Cert.Ocsp.CertificateIDBC
            (null);

        private static readonly string HASH_SHA1 = CertificateID.HashSha1;

        private readonly CertID certificateID;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>.
        /// </summary>
        /// <param name="certificateID">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>
        /// to be wrapped
        /// </param>
        public CertificateIDBC(CertID certificateID) {
            this.certificateID = certificateID;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>.
        /// </summary>
        /// <param name="hashAlgorithm">
        /// hash algorithm to create
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>
        /// </param>
        /// <param name="issuerCert">
        /// X509Certificate wrapper to create
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>
        /// </param>
        /// <param name="serialNumber">
        /// serial number to create
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>
        /// </param>
        public CertificateIDBC(string hashAlgorithm, IX509Certificate issuerCert, IBigInteger serialNumber) {
            AlgorithmIdentifier hashAlgId = new AlgorithmIdentifier(new DerObjectIdentifier(hashAlgorithm), DerNull.Instance);

            X509Name issuerName = PrincipalUtilities.GetSubjectX509Principal(((X509CertificateBC)issuerCert).GetCertificate());
            byte[] issuerNameHash = DigestUtilities.CalculateDigest(hashAlgorithm, issuerName.GetEncoded());

            AsymmetricKeyParameter issuerKey = ((X509CertificateBC)issuerCert).GetCertificate().GetPublicKey();
            SubjectPublicKeyInfo info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(issuerKey);
            byte[] issuerKeyHash = DigestUtilities.CalculateDigest(hashAlgorithm, info.PublicKeyData.GetBytes());

            this.certificateID = new CertID(hashAlgId, new DerOctetString(issuerNameHash),
                new DerOctetString(issuerKeyHash), new DerInteger(((BigIntegerBC)serialNumber).GetBigInteger()));
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="CertificateIDBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Cert.Ocsp.CertificateIDBC GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertID"/>.
        /// </returns>
        public virtual CertID GetCertificateID() {
            return certificateID;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetHashAlgOID() {
            return new ASN1ObjectIdentifierBC(certificateID.HashAlgorithm.Algorithm.Id);
        }

        /// <summary><inheritDoc/></summary>
        public virtual string GetHashSha1() {
            return HASH_SHA1;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool MatchesIssuer(IX509Certificate issuerCert) {
            return new CertificateIDBC(certificateID.HashAlgorithm.Algorithm.Id, issuerCert, new BigIntegerBC(
                certificateID.SerialNumber.Value)).GetCertificateID().Equals(certificateID);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBigInteger GetSerialNumber() {
            return new BigIntegerBC(certificateID.SerialNumber.Value);
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
            iText.Bouncycastle.Cert.Ocsp.CertificateIDBC that = (iText.Bouncycastle.Cert.Ocsp.CertificateIDBC)o;
            return Object.Equals(certificateID, that.certificateID);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateID);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return certificateID.ToString();
        }
    }
}

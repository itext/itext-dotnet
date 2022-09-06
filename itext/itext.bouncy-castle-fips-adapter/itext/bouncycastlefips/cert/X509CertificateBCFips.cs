using System;
using System.Collections;
using System.Collections.Generic;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X500;
using iText.Bouncycastlefips.Crypto;
using iText.Bouncycastlefips.Math;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using Org.BouncyCastle.Cert;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Operators;

namespace iText.Bouncycastlefips.Cert {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.X509Certificate"/>.
    /// </summary>
    public class X509CertificateBCFips : IX509Certificate {
        private readonly X509Certificate certificate;

        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.Cert.X509Certificate"/>.
        /// </summary>
        /// <param name="certificate">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.X509Certificate"/> to be wrapped
        /// </param>
        public X509CertificateBCFips(X509Certificate certificate) {
            this.certificate = certificate;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="Org.BouncyCastle.Cert.X509Certificate"/>.
        /// </returns>
        public virtual X509Certificate GetCertificate() {
            return certificate;
        }

        /// <summary><inheritDoc/></summary>
        public IX500Name GetIssuerDN() {
            return new X500NameBCFips(certificate.IssuerDN);
        }

        /// <summary><inheritDoc/></summary>
        public IBigInteger GetSerialNumber() {
            return new BigIntegerBCFips(certificate.SerialNumber);
        }

        /// <summary><inheritDoc/></summary>
        public IPublicKey GetPublicKey() {
            return new PublicKeyBCFips(certificate.GetPublicKey());
        }
        
        /// <summary><inheritDoc/></summary>
        public byte[] GetEncoded() {
            return certificate.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GetTbsCertificate() {
            return certificate.GetTbsCertificate();
        }
        
        /// <summary><inheritDoc/></summary>
        public IASN1OctetString GetExtensionValue(string oid) {
            return new ASN1OctetStringBCFips((Asn1OctetString) Asn1OctetString.FromByteArray(
                certificate.GetExtensionValue(new DerObjectIdentifier(oid))));
        }

        /// <summary><inheritDoc/></summary>
        public void Verify(IPublicKey issuerPublicKey) {
            PkixVerifierFactoryProvider factoryProvider = new PkixVerifierFactoryProvider(
                ((PublicKeyBCFips) issuerPublicKey).GetPublicKey());
            certificate.Verify(factoryProvider);
        }
        
        /// <summary><inheritDoc/></summary>
        public ISet<string> GetCriticalExtensionOids() {
            ISet<string> set = new HashSet<string>();
            foreach (string oid in certificate.GetCriticalExtensionOids()) {
                set.Add(oid);
            }
            return set;
        }

        /// <summary><inheritDoc/></summary>
        public void CheckValidity(DateTime time) {
            certificate.CheckValidity(time);
        }

        /// <summary><inheritDoc/></summary>
        public IX500Name GetSubjectDN() {
            return new X500NameBCFips(certificate.SubjectDN);
        }

        /// <summary><inheritDoc/></summary>
        public string GetEndDateTime() {
            return certificate.ToAsn1Structure().EndDate.GetTime();
        }

        /// <summary><inheritDoc/></summary>
        public DateTime GetNotBefore() {
            return certificate.NotBefore;
        }

        public IList GetExtendedKeyUsage()
        {
            return certificate.GetExtendedKeyUsage();
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cert.X509CertificateBCFips that = (iText.Bouncycastlefips.Cert.X509CertificateBCFips
                )o;
            return Object.Equals(certificate, that.certificate);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificate);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return certificate.ToString();
        }
    }
}

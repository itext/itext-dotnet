using System;
using System.Collections;
using System.Collections.Generic;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X500;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.Math;
using iText.Bouncycastle.Security;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.Cert {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.X509.X509Certificate"/>.
    /// </summary>
    public class X509CertificateBC : IX509Certificate {
        private readonly X509Certificate certificate;

        /// <summary>
        /// Creates new wrapper instance for <see cref="Org.BouncyCastle.X509.X509Certificate"/>.
        /// </summary>
        /// <param name="certificate">
        /// 
        /// <see cref="Org.BouncyCastle.X509.X509Certificate"/> to be wrapped
        /// </param>
        public X509CertificateBC(X509Certificate certificate) {
            this.certificate = certificate;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped <see cref="Org.BouncyCastle.X509.X509Certificate"/>.
        /// </returns>
        public virtual X509Certificate GetCertificate() {
            return certificate;
        }
        
        /// <summary><inheritDoc/></summary>
        public IX500Name GetIssuerDN() {
            return new X500NameBC(certificate.IssuerDN);
        }

        /// <summary><inheritDoc/></summary>
        public IBigInteger GetSerialNumber() {
            return new BigIntegerBC(certificate.SerialNumber);
        }

        /// <summary><inheritDoc/></summary>
        public IPublicKey GetPublicKey() {
            return new PublicKeyBC(certificate.GetPublicKey());
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
            return new ASN1OctetStringBC(certificate.GetExtensionValue(oid));
        }

        /// <summary><inheritDoc/></summary>
        public void Verify(IPublicKey issuerPublicKey) {
            try {
                certificate.Verify(((PublicKeyBC)issuerPublicKey).GetPublicKey());
            }
            catch (GeneralSecurityException e) {
                throw new GeneralSecurityExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public ISet<string> GetCriticalExtensionOids() {
            if (certificate.GetCriticalExtensionOids() == null) {
                return null;
            }
            ISet<string> set = new HashSet<string>();
            foreach (string oid in certificate.GetCriticalExtensionOids()) {
                set.Add(oid);
            }
            return set;
        }

        /// <summary><inheritDoc/></summary>
        public void CheckValidity(DateTime time) {
            try {
                certificate.CheckValidity(time);
            } catch (CertificateExpiredException e) {
                throw new CertificateExpiredExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public IX500Name GetSubjectDN() {
            return new X500NameBC(certificate.SubjectDN);
        }

        /// <summary><inheritDoc/></summary>
        public string GetEndDateTime() {
            return certificate.CertificateStructure.EndDate.GetTime();
        }

        /// <summary><inheritDoc/></summary>
        public DateTime GetNotBefore() {
            return certificate.NotBefore;
        }

        /// <summary><inheritDoc/></summary>
        public IList GetExtendedKeyUsage() {
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
            iText.Bouncycastle.Cert.X509CertificateBC that = (iText.Bouncycastle.Cert.X509CertificateBC)o;
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

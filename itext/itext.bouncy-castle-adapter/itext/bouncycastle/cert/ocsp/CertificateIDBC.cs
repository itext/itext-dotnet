using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Bouncycastle.Cert;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    public class CertificateIDBC : ICertificateID {
        private static readonly iText.Bouncycastle.Cert.Ocsp.CertificateIDBC INSTANCE = new iText.Bouncycastle.Cert.Ocsp.CertificateIDBC
            (null);

        private static readonly AlgorithmIdentifierBC HASH_SHA1 = new AlgorithmIdentifierBC(Org.BouncyCastle.Ocsp.CertificateID.HashSha1
            );

        private readonly CertificateID certificateID;

        public CertificateIDBC(CertificateID certificateID) {
            this.certificateID = certificateID;
        }

        public CertificateIDBC(IDigestCalculator digestCalculator, IX509CertificateHolder certificateHolder, BigInteger
             bigInteger) {
            try {
                this.certificateID = new CertificateID(((DigestCalculatorBC)digestCalculator).GetDigestCalculator(), ((X509CertificateHolderBC
                    )certificateHolder).GetCertificateHolder(), bigInteger);
            }
            catch (OcspException e) {
                throw new OCSPExceptionBC(e);
            }
        }

        public static iText.Bouncycastle.Cert.Ocsp.CertificateIDBC GetInstance() {
            return INSTANCE;
        }

        public virtual CertificateID GetCertificateID() {
            return certificateID;
        }

        public virtual IASN1ObjectIdentifier GetHashAlgOID() {
            return new ASN1ObjectIdentifierBC(certificateID.HashAlgOid);
        }

        public virtual IAlgorithmIdentifier GetHashSha1() {
            return HASH_SHA1;
        }

        public virtual bool MatchesIssuer(IX509CertificateHolder certificateHolder, IDigestCalculatorProvider provider
            ) {
            try {
                return certificateID.MatchesIssuer(((X509CertificateHolderBC)certificateHolder).GetCertificateHolder(), ((
                    DigestCalculatorProviderBC)provider).GetCalculatorProvider());
            }
            catch (OcspException e) {
                throw new OCSPExceptionBC(e);
            }
        }

        public virtual BigInteger GetSerialNumber() {
            return certificateID.SerialNumber;
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateID);
        }

        public override String ToString() {
            return certificateID.ToString();
        }
    }
}

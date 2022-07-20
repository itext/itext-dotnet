using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Bouncycastlefips.Cert;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class CertificateIDBCFips : ICertificateID {
        private static readonly iText.Bouncycastlefips.Cert.Ocsp.CertificateIDBCFips INSTANCE = new iText.Bouncycastlefips.Cert.Ocsp.CertificateIDBCFips
            (null);

        private static readonly AlgorithmIdentifierBCFips HASH_SHA1 = new AlgorithmIdentifierBCFips(Org.BouncyCastle.Ocsp.CertificateID.HashSha1
            );

        private readonly CertificateID certificateID;

        public CertificateIDBCFips(CertificateID certificateID) {
            this.certificateID = certificateID;
        }

        public CertificateIDBCFips(IDigestCalculator digestCalculator, IX509CertificateHolder certificateHolder, BigInteger
             bigInteger) {
            try {
                this.certificateID = new CertificateID(((DigestCalculatorBCFips)digestCalculator).GetDigestCalculator(), (
                    (X509CertificateHolderBCFips)certificateHolder).GetCertificateHolder(), bigInteger);
            }
            catch (OcspException e) {
                throw new OCSPExceptionBCFips(e);
            }
        }

        public static iText.Bouncycastlefips.Cert.Ocsp.CertificateIDBCFips GetInstance() {
            return INSTANCE;
        }

        public virtual CertificateID GetCertificateID() {
            return certificateID;
        }

        public virtual IASN1ObjectIdentifier GetHashAlgOID() {
            return new ASN1ObjectIdentifierBCFips(certificateID.HashAlgOid);
        }

        public virtual IAlgorithmIdentifier GetHashSha1() {
            return HASH_SHA1;
        }

        public virtual bool MatchesIssuer(IX509CertificateHolder certificateHolder, IDigestCalculatorProvider provider
            ) {
            try {
                return certificateID.MatchesIssuer(((X509CertificateHolderBCFips)certificateHolder).GetCertificateHolder()
                    , ((DigestCalculatorProviderBCFips)provider).GetCalculatorProvider());
            }
            catch (OcspException e) {
                throw new OCSPExceptionBCFips(e);
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
            iText.Bouncycastlefips.Cert.Ocsp.CertificateIDBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.CertificateIDBCFips
                )o;
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

using System;
using System.Security.Cryptography;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    public class CertIDBCFips : ICertID {
        private static readonly CertIDBCFips INSTANCE = new CertIDBCFips(null);

        private static readonly AlgorithmIdentifierBCFips HASH_SHA1 = new AlgorithmIdentifierBCFips(
            new AlgorithmIdentifier(new DerObjectIdentifier(HashAlgorithmName.SHA1.Name), DerNull.Instance));

        private readonly CertID certificateID;

        public CertIDBCFips(CertID certificateID) {
            this.certificateID = certificateID;
        }

        public static CertIDBCFips GetInstance() {
            return INSTANCE;
        }

        public virtual CertID GetCertID() {
            return certificateID;
        }

        public virtual IASN1ObjectIdentifier GetHashAlgOID() {
            return new ASN1ObjectIdentifierBCFips(certificateID.HashAlgorithm.Algorithm.Id);
        }

        public virtual IAlgorithmIdentifier GetHashSha1() {
            return HASH_SHA1;
        }

        public virtual IASN1Integer GetSerialNumber() {
            return new ASN1IntegerBCFips(certificateID.SerialNumber);
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            CertIDBCFips that = (CertIDBCFips)o;
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

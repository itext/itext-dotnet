using System;
using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    public class SubjectPublicKeyInfoBC : ASN1EncodableBC, ISubjectPublicKeyInfo {
        public SubjectPublicKeyInfoBC(SubjectPublicKeyInfo subjectPublicKeyInfo)
            : base(subjectPublicKeyInfo) {
        }

        public SubjectPublicKeyInfoBC(Object obj)
            : base(SubjectPublicKeyInfo.GetInstance(obj)) {
        }

        public virtual SubjectPublicKeyInfo GetSubjectPublicKeyInfo() {
            return (SubjectPublicKeyInfo)GetEncodable();
        }

        public virtual IAlgorithmIdentifier GetAlgorithm() {
            return new AlgorithmIdentifierBC(GetSubjectPublicKeyInfo().AlgorithmID);
        }
    }
}

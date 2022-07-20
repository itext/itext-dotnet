using System;
using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    public class SubjectPublicKeyInfoBCFips : ASN1EncodableBCFips, ISubjectPublicKeyInfo {
        public SubjectPublicKeyInfoBCFips(SubjectPublicKeyInfo subjectPublicKeyInfo)
            : base(subjectPublicKeyInfo) {
        }

        public SubjectPublicKeyInfoBCFips(Object obj)
            : base(SubjectPublicKeyInfo.GetInstance(obj)) {
        }

        public virtual SubjectPublicKeyInfo GetSubjectPublicKeyInfo() {
            return (SubjectPublicKeyInfo)GetEncodable();
        }

        public virtual IAlgorithmIdentifier GetAlgorithm() {
            return new AlgorithmIdentifierBCFips(GetSubjectPublicKeyInfo().AlgorithmID);
        }
    }
}

using System;
using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo"/>.
    /// </summary>
    public class SubjectPublicKeyInfoBC : ASN1EncodableBC, ISubjectPublicKeyInfo {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo"/>.
        /// </summary>
        /// <param name="subjectPublicKeyInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo"/>
        /// to be wrapped
        /// </param>
        public SubjectPublicKeyInfoBC(SubjectPublicKeyInfo subjectPublicKeyInfo)
            : base(subjectPublicKeyInfo) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo"/>.
        /// </summary>
        /// <param name="obj">
        /// to get
        /// <see cref="Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo"/>
        /// instance to be wrapped
        /// </param>
        public SubjectPublicKeyInfoBC(Object obj)
            : base(SubjectPublicKeyInfo.GetInstance(obj)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo"/>.
        /// </returns>
        public virtual SubjectPublicKeyInfo GetSubjectPublicKeyInfo() {
            return (SubjectPublicKeyInfo)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier GetAlgorithm() {
            return new AlgorithmIdentifierBC(GetSubjectPublicKeyInfo().AlgorithmID);
        }
    }
}

using System;
using Org.BouncyCastle.Cert;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert {
    public class X509ExtensionUtilsBCFips : IX509ExtensionUtils {
        private readonly X509ExtensionUtils extensionUtils;

        public X509ExtensionUtilsBCFips(X509ExtensionUtils extensionUtils) {
            this.extensionUtils = extensionUtils;
        }

        public X509ExtensionUtilsBCFips(IDigestCalculator digestCalculator)
            : this(new X509ExtensionUtils(((DigestCalculatorBCFips)digestCalculator).GetDigestCalculator())) {
        }

        public virtual X509ExtensionUtils GetExtensionUtils() {
            return extensionUtils;
        }

        public virtual IAuthorityKeyIdentifier CreateAuthorityKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo) {
            return new AuthorityKeyIdentifierBCFips(extensionUtils.CreateAuthorityKeyIdentifier(((SubjectPublicKeyInfoBCFips
                )publicKeyInfo).GetSubjectPublicKeyInfo()));
        }

        public virtual ISubjectKeyIdentifier CreateSubjectKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo) {
            return new SubjectKeyIdentifierBCFips(extensionUtils.CreateSubjectKeyIdentifier(((SubjectPublicKeyInfoBCFips
                )publicKeyInfo).GetSubjectPublicKeyInfo()));
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cert.X509ExtensionUtilsBCFips that = (iText.Bouncycastlefips.Cert.X509ExtensionUtilsBCFips
                )o;
            return Object.Equals(extensionUtils, that.extensionUtils);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(extensionUtils);
        }

        public override String ToString() {
            return extensionUtils.ToString();
        }
    }
}

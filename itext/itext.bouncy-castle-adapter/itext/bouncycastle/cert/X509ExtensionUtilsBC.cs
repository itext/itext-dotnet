using System;
using Org.BouncyCastle.Cert;
using iText.Bouncycastle.Asn1.X509;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert {
    public class X509ExtensionUtilsBC : IX509ExtensionUtils {
        private readonly X509ExtensionUtils extensionUtils;

        public X509ExtensionUtilsBC(X509ExtensionUtils extensionUtils) {
            this.extensionUtils = extensionUtils;
        }

        public X509ExtensionUtilsBC(IDigestCalculator digestCalculator)
            : this(new X509ExtensionUtils(((DigestCalculatorBC)digestCalculator).GetDigestCalculator())) {
        }

        public virtual X509ExtensionUtils GetExtensionUtils() {
            return extensionUtils;
        }

        public virtual IAuthorityKeyIdentifier CreateAuthorityKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo) {
            return new AuthorityKeyIdentifierBC(extensionUtils.CreateAuthorityKeyIdentifier(((SubjectPublicKeyInfoBC)publicKeyInfo
                ).GetSubjectPublicKeyInfo()));
        }

        public virtual ISubjectKeyIdentifier CreateSubjectKeyIdentifier(ISubjectPublicKeyInfo publicKeyInfo) {
            return new SubjectKeyIdentifierBC(extensionUtils.CreateSubjectKeyIdentifier(((SubjectPublicKeyInfoBC)publicKeyInfo
                ).GetSubjectPublicKeyInfo()));
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.X509ExtensionUtilsBC that = (iText.Bouncycastle.Cert.X509ExtensionUtilsBC)o;
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

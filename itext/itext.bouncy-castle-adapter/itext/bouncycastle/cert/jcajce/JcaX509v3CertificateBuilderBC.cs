using System;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Cert.Jcajce;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X500;
using iText.Bouncycastle.Cert;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Jcajce;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Jcajce {
    public class JcaX509v3CertificateBuilderBC : IJcaX509v3CertificateBuilder {
        private readonly JcaX509v3CertificateBuilder certificateBuilder;

        public JcaX509v3CertificateBuilderBC(JcaX509v3CertificateBuilder certificateBuilder) {
            this.certificateBuilder = certificateBuilder;
        }

        public JcaX509v3CertificateBuilderBC(X509Certificate signingCert, BigInteger certSerialNumber, DateTime startDate
            , DateTime endDate, IX500Name subjectDnName, AsymmetricKeyParameter publicKey)
            : this(new JcaX509v3CertificateBuilder(signingCert, certSerialNumber, startDate, endDate, ((X500NameBC)subjectDnName
                ).GetX500Name(), publicKey)) {
        }

        public virtual JcaX509v3CertificateBuilder GetCertificateBuilder() {
            return certificateBuilder;
        }

        public virtual IX509CertificateHolder Build(IContentSigner contentSigner) {
            return new X509CertificateHolderBC(certificateBuilder.Build(((ContentSignerBC)contentSigner).GetContentSigner
                ()));
        }

        public virtual IJcaX509v3CertificateBuilder AddExtension(IASN1ObjectIdentifier extensionOID, bool critical
            , IASN1Encodable extensionValue) {
            try {
                certificateBuilder.AddExtension(((ASN1ObjectIdentifierBC)extensionOID).GetASN1ObjectIdentifier(), critical
                    , ((ASN1EncodableBC)extensionValue).GetEncodable());
                return this;
            }
            catch (CertIOException e) {
                throw new CertIOExceptionBC(e);
            }
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Jcajce.JcaX509v3CertificateBuilderBC that = (iText.Bouncycastle.Cert.Jcajce.JcaX509v3CertificateBuilderBC
                )o;
            return Object.Equals(certificateBuilder, that.certificateBuilder);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateBuilder);
        }

        public override String ToString() {
            return certificateBuilder.ToString();
        }
    }
}

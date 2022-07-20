using System;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Cert.Ocsp;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Bouncycastlefips.Cert;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    public class BasicOCSPRespBuilderBCFips : IBasicOCSPRespBuilder {
        private readonly BasicOCSPRespBuilder basicOCSPRespBuilder;

        public BasicOCSPRespBuilderBCFips(BasicOCSPRespBuilder basicOCSPRespBuilder) {
            this.basicOCSPRespBuilder = basicOCSPRespBuilder;
        }

        public BasicOCSPRespBuilderBCFips(IRespID respID)
            : this(new BasicOCSPRespBuilder(((RespIDBCFips)respID).GetRespID())) {
        }

        public virtual BasicOCSPRespBuilder GetBasicOCSPRespBuilder() {
            return basicOCSPRespBuilder;
        }

        public virtual IBasicOCSPRespBuilder SetResponseExtensions(IExtensions extensions) {
            basicOCSPRespBuilder.SetResponseExtensions(((ExtensionsBCFips)extensions).GetExtensions());
            return this;
        }

        public virtual IBasicOCSPRespBuilder AddResponse(ICertificateID certID, ICertificateStatus certificateStatus
            , DateTime time, DateTime time1, IExtensions extensions) {
            basicOCSPRespBuilder.AddResponse(((CertificateIDBCFips)certID).GetCertificateID(), ((CertificateStatusBCFips
                )certificateStatus).GetCertificateStatus(), time, time1, ((ExtensionsBCFips)extensions).GetExtensions(
                ));
            return this;
        }

        public virtual IBasicOCSPResp Build(IContentSigner signer, IX509CertificateHolder[] chain, DateTime time) {
            try {
                X509CertificateHolder[] certificateHolders = new X509CertificateHolder[chain.Length];
                for (int i = 0; i < chain.Length; ++i) {
                    certificateHolders[i] = ((X509CertificateHolderBCFips)chain[i]).GetCertificateHolder();
                }
                return new BasicOCSPRespBCFips(basicOCSPRespBuilder.Build(((ContentSignerBCFips)signer).GetContentSigner()
                    , certificateHolders, time));
            }
            catch (OcspException e) {
                throw new OCSPExceptionBCFips(e);
            }
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cert.Ocsp.BasicOCSPRespBuilderBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.BasicOCSPRespBuilderBCFips
                )o;
            return Object.Equals(basicOCSPRespBuilder, that.basicOCSPRespBuilder);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(basicOCSPRespBuilder);
        }

        public override String ToString() {
            return basicOCSPRespBuilder.ToString();
        }
    }
}

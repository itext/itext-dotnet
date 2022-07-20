using System;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Cert.Ocsp;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastle.Asn1.X509;
using iText.Bouncycastle.Cert;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.Ocsp.BasicOCSPRespBuilder"/>.
    /// </summary>
    public class BasicOCSPRespBuilderBC : IBasicOCSPRespBuilder {
        private readonly BasicOCSPRespBuilder basicOCSPRespBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.BasicOCSPRespBuilder"/>.
        /// </summary>
        /// <param name="basicOCSPRespBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.BasicOCSPRespBuilder"/>
        /// to be wrapped
        /// </param>
        public BasicOCSPRespBuilderBC(BasicOCSPRespBuilder basicOCSPRespBuilder) {
            this.basicOCSPRespBuilder = basicOCSPRespBuilder;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.BasicOCSPRespBuilder"/>.
        /// </summary>
        /// <param name="respID">
        /// RespID wrapper to create
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.BasicOCSPRespBuilder"/>
        /// to be wrapped
        /// </param>
        public BasicOCSPRespBuilderBC(IRespID respID)
            : this(new BasicOCSPRespBuilder(((RespIDBC)respID).GetRespID())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.BasicOCSPRespBuilder"/>.
        /// </returns>
        public virtual BasicOCSPRespBuilder GetBasicOCSPRespBuilder() {
            return basicOCSPRespBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOCSPRespBuilder SetResponseExtensions(IExtensions extensions) {
            basicOCSPRespBuilder.SetResponseExtensions(((ExtensionsBC)extensions).GetExtensions());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOCSPRespBuilder AddResponse(ICertificateID certID, ICertificateStatus certificateStatus
            , DateTime time, DateTime time1, IExtensions extensions) {
            basicOCSPRespBuilder.AddResponse(((CertificateIDBC)certID).GetCertificateID(), ((CertificateStatusBC)certificateStatus
                ).GetCertificateStatus(), time, time1, ((ExtensionsBC)extensions).GetExtensions());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOCSPResp Build(IContentSigner signer, IX509CertificateHolder[] chain, DateTime time) {
            try {
                X509CertificateHolder[] certificateHolders = new X509CertificateHolder[chain.Length];
                for (int i = 0; i < chain.Length; ++i) {
                    certificateHolders[i] = ((X509CertificateHolderBC)chain[i]).GetCertificateHolder();
                }
                return new BasicOCSPRespBC(basicOCSPRespBuilder.Build(((ContentSignerBC)signer).GetContentSigner(), certificateHolders
                    , time));
            }
            catch (OcspException e) {
                throw new OCSPExceptionBC(e);
            }
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Ocsp.BasicOCSPRespBuilderBC that = (iText.Bouncycastle.Cert.Ocsp.BasicOCSPRespBuilderBC
                )o;
            return Object.Equals(basicOCSPRespBuilder, that.basicOCSPRespBuilder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(basicOCSPRespBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return basicOCSPRespBuilder.ToString();
        }
    }
}

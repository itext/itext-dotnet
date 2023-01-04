/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.Bouncycastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastle.Asn1.X509;
using iText.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="BasicOcspRespGenerator"/>.
    /// </summary>
    public class BasicOCSPRespBuilderBC : IBasicOCSPRespBuilder {
        private readonly BasicOcspRespGenerator basicOCSPRespBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="BasicOcspRespGenerator"/>.
        /// </summary>
        /// <param name="basicOCSPRespBuilder">
        /// 
        /// <see cref="BasicOcspRespGenerator"/>
        /// to be wrapped
        /// </param>
        public BasicOCSPRespBuilderBC(BasicOcspRespGenerator basicOCSPRespBuilder) {
            this.basicOCSPRespBuilder = basicOCSPRespBuilder;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="BasicOcspRespGenerator"/>.
        /// </summary>
        /// <param name="respID">
        /// RespID wrapper to create
        /// <see cref="BasicOcspRespGenerator"/>
        /// to be wrapped
        /// </param>
        public BasicOCSPRespBuilderBC(IRespID respID)
            : this(new BasicOcspRespGenerator(((RespIDBC)respID).GetRespID())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="BasicOcspRespGenerator"/>.
        /// </returns>
        public virtual BasicOcspRespGenerator GetBasicOCSPRespBuilder() {
            return basicOCSPRespBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOCSPRespBuilder SetResponseExtensions(IExtensions extensions) {
            basicOCSPRespBuilder.SetResponseExtensions(((ExtensionsBC)extensions).GetX509Extensions());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOCSPRespBuilder AddResponse(ICertificateID certID, ICertificateStatus certificateStatus
            , DateTime time, DateTime time1, IExtensions extensions) {
            CertificateStatus status = certificateStatus is IUnknownStatus ? new UnknownStatus()
                : certificateStatus is IRevokedStatus ? new RevokedStatus(
                    RevokedInfo.GetInstance(((RevokedStatusBC)certificateStatus).GetRevokedStatus().Status))
                : CertificateStatus.Good;
            basicOCSPRespBuilder.AddResponse(new CertificateID(((CertificateIDBC)certID).GetCertificateID()), 
                status, time, time1, ((ExtensionsBC)extensions).GetX509Extensions());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public IBasicOCSPResponse Build(IContentSigner signer, IX509Certificate[] chain, DateTime time) {
            try {
                X509Certificate[] certificates = new X509Certificate[chain.Length];
                for (int i = 0; i < chain.Length; ++i) {
                    certificates[i] = ((X509CertificateBC)chain[i]).GetCertificate();
                }
                BasicOcspResp resp = basicOCSPRespBuilder.Generate(
                    ((ContentSignerBC)signer).GetContentSigner(), certificates, time);
                return new BasicOCSPResponseBC(BasicOcspResponse.GetInstance(
                   new Asn1InputStream(resp.GetEncoded()).ReadObject()));
            } catch (OcspException e) {
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

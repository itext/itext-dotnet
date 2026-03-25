/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
    Authors: Apryse Software.

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
using iText.Bouncycastlefips.Asn1.Ocsp;
using Org.BouncyCastle.Cert;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for generator for basic OCSP response objects.
    /// </summary>
    public class BasicOcspRespGeneratorBCFips : IBasicOcspRespGenerator {
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
        public BasicOcspRespGeneratorBCFips(BasicOcspRespGenerator basicOCSPRespBuilder) {
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
        public BasicOcspRespGeneratorBCFips(IRespID respID)
            : this(new BasicOcspRespGenerator(((RespIDBCFips)respID).GetRespID())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="BasicOcspRespGenerator"/>.
        /// </returns>
        public virtual BasicOcspRespGenerator GetBasicOcspRespGenerator() {
            return basicOCSPRespBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOcspRespGenerator SetResponseExtensions(IX509Extensions extensions) {
            basicOCSPRespBuilder.SetResponseExtensions(((X509ExtensionsBCFips)extensions).GetX509Extensions());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOcspRespGenerator AddResponse(ICertID certID, ICertStatus certificateStatus, 
            DateTime time, DateTime time1, IX509Extensions extensions) {
            CertificateStatus status = certificateStatus is IUnknownCertStatus ? new UnknownStatus()
                : certificateStatus is IRevokedCertStatus ? new RevokedStatus(
                    RevokedInfo.GetInstance(((RevokedStatusBCFips)certificateStatus).GetRevokedStatus().Status))
                : CertificateStatus.Good;
            basicOCSPRespBuilder.AddResponse(new CertificateID(((CertIDBCFips)certID).GetCertificateID()), 
                status, time, time1, ((X509ExtensionsBCFips)extensions).GetX509Extensions());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOcspResponse Build(IContentSigner signer, IX509Certificate[] wrappersChain,
            DateTime producedAt) {
            try {
                X509Certificate[] certificates = new X509Certificate[wrappersChain.Length];
                for (int i = 0; i < wrappersChain.Length; ++i) {
                    certificates[i] = ((X509CertificateBCFips)wrappersChain[i]).GetCertificate();
                }
                BasicOcspResp resp = basicOCSPRespBuilder.Generate(
                    ((ContentSignerBCFips)signer).GetContentSigner(), certificates, producedAt);
                return new BasicOcspResponseBCFips(BasicOcspResponse.GetInstance(
                    new Asn1InputStream(resp.GetEncoded()).ReadObject()));
            } catch (OcspException e) {
                throw new OcspExceptionBCFips(e);
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
            BasicOcspRespGeneratorBCFips that = (BasicOcspRespGeneratorBCFips)o;
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

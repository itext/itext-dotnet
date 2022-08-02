/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.Ocsp.BasicOCSPRespBuilder"/>.
    /// </summary>
    public class BasicOCSPRespBuilderBCFips : IBasicOCSPRespBuilder {
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
        public BasicOCSPRespBuilderBCFips(BasicOCSPRespBuilder basicOCSPRespBuilder) {
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
        public BasicOCSPRespBuilderBCFips(IRespID respID)
            : this(new BasicOCSPRespBuilder(((RespIDBCFips)respID).GetRespID())) {
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
            basicOCSPRespBuilder.SetResponseExtensions(((ExtensionsBCFips)extensions).GetExtensions());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOCSPRespBuilder AddResponse(ICertificateID certID, ICertificateStatus certificateStatus
            , DateTime time, DateTime time1, IExtensions extensions) {
            basicOCSPRespBuilder.AddResponse(((CertificateIDBCFips)certID).GetCertificateID(), ((CertificateStatusBCFips
                )certificateStatus).GetCertificateStatus(), time, time1, ((ExtensionsBCFips)extensions).GetExtensions(
                ));
            return this;
        }

        /// <summary><inheritDoc/></summary>
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

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
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

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
using Org.BouncyCastle.Cert.Ocsp;
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.Ocsp.OCSPReqBuilder"/>.
    /// </summary>
    public class OCSPReqBuilderBCFips : IOCSPReqBuilder {
        private readonly OCSPReqBuilder reqBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.OCSPReqBuilder"/>.
        /// </summary>
        /// <param name="reqBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.OCSPReqBuilder"/>
        /// to be wrapped
        /// </param>
        public OCSPReqBuilderBCFips(OCSPReqBuilder reqBuilder) {
            this.reqBuilder = reqBuilder;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.OCSPReqBuilder"/>.
        /// </returns>
        public virtual OCSPReqBuilder GetReqBuilder() {
            return reqBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPReqBuilder SetRequestExtensions(IExtensions extensions) {
            reqBuilder.SetRequestExtensions(((ExtensionsBCFips)extensions).GetExtensions());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPReqBuilder AddRequest(ICertificateID certificateID) {
            reqBuilder.AddRequest(((CertificateIDBCFips)certificateID).GetCertificateID());
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPReq Build() {
            try {
                return new OCSPReqBCFips(reqBuilder.Build());
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
            iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBuilderBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBuilderBCFips
                )o;
            return Object.Equals(reqBuilder, that.reqBuilder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(reqBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return reqBuilder.ToString();
        }
    }
}

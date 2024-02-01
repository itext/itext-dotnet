/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Ocsp;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.SingleResponse"/>.
    /// </summary>
    public class SingleResponseBC : ISingleResponse {
        private readonly SingleResponse singleResp;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.SingleResponse"/>.
        /// </summary>
        /// <param name="singleResp">
        /// 
        /// <see cref="SingleResponse"/>
        /// to be wrapped
        /// </param>
        public SingleResponseBC(SingleResponse singleResp) {
            this.singleResp = singleResp;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.SingleResponse"/>.
        /// </summary>
        /// <param name="basicResp">
        /// 
        /// <see cref="IBasicOcspResponse"/>
        /// wrapper to get
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.SingleResponse"/>
        /// </param>
        public SingleResponseBC(IBasicOcspResponse basicResp)
            : this(SingleResponse.GetInstance(((BasicOcspResponseBC)basicResp)
                .GetBasicOcspResponse().TbsResponseData.Responses[0])) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.SingleResponse"/>.
        /// </returns>
        public virtual SingleResponse GetSingleResponse() {
            return singleResp;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICertID GetCertID() {
            return new CertIDBC(singleResp.CertId);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICertStatus GetCertStatus() {
            return new CertStatusBC(singleResp.CertStatus);
        }

        /// <summary><inheritDoc/></summary>
        public virtual DateTime GetNextUpdate() {
            return singleResp.NextUpdate.ToDateTime();
        }

        /// <summary><inheritDoc/></summary>
        public virtual DateTime GetThisUpdate() {
            return singleResp.ThisUpdate.ToDateTime();
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Cert.Ocsp.SingleResponseBC that = (iText.Bouncycastle.Cert.Ocsp.SingleResponseBC)o;
            return Object.Equals(singleResp, that.singleResp);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(singleResp);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return singleResp.ToString();
        }
    }
}

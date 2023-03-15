/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Bouncycastle.Cert.Ocsp;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for revoked
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertStatus"/>.
    /// </summary>
    public class RevokedStatusBC : CertificateStatusBC, IRevokedStatus {
        /// <summary>
        /// Creates new wrapper instance for revoked
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertStatus"/>.
        /// </summary>
        /// <param name="certificateStatus">
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertStatus"/>
        /// to be wrapped
        /// </param>
        public RevokedStatusBC(CertStatus certificateStatus)
            : base(certificateStatus != null && certificateStatus.TagNo == 1 ? certificateStatus : null) {
        }

        /// <summary>
        /// Creates new wrapper instance for revoked
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertStatus"/>.
        /// </summary>
        /// <param name="date">date to create RevokedInfo</param>
        /// <param name="i">CrlReason int value</param>
        public RevokedStatusBC(DateTime date, int i) 
            : base(new CertStatus(new RevokedInfo(new DerGeneralizedTime(date), new CrlReason(i)))) {
        }
        
        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.CertStatus"/>.
        /// </returns>
        public virtual CertStatus GetRevokedStatus() {
            return base.GetCertificateStatus();
        }
    }
}

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
using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastle.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus"/>.
    /// </summary>
    public class OCSPResponseStatusBC : ASN1EncodableBC, IOCSPResponseStatus {
        private static readonly iText.Bouncycastle.Asn1.Ocsp.OCSPResponseStatusBC INSTANCE = new iText.Bouncycastle.Asn1.Ocsp.OCSPResponseStatusBC
            (null);

        private const int SUCCESSFUL = Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus"/>.
        /// </summary>
        /// <param name="ocspResponseStatus">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus"/>
        /// to be wrapped
        /// </param>
        public OCSPResponseStatusBC(OcspResponseStatus ocspResponseStatus)
            : base(ocspResponseStatus) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="OCSPResponseStatusBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Asn1.Ocsp.OCSPResponseStatusBC GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus"/>.
        /// </returns>
        public virtual OcspResponseStatus GetOcspResponseStatus() {
            return (OcspResponseStatus)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetSuccessful() {
            return SUCCESSFUL;
        }
    }
}

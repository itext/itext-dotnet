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
using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastle.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
    /// </summary>
    public class OCSPResponseBC : ASN1EncodableBC, IOCSPResponse {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
        /// </summary>
        /// <param name="ocspResponse">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>
        /// to be wrapped
        /// </param>
        public OCSPResponseBC(OcspResponse ocspResponse)
            : base(ocspResponse) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
        /// </summary>
        /// <param name="respStatus">OCSPResponseStatus wrapper</param>
        /// <param name="responseBytes">ResponseBytes wrapper</param>
        public OCSPResponseBC(IOCSPResponseStatus respStatus, IResponseBytes responseBytes)
            : base(new OcspResponse(((OCSPResponseStatusBC)respStatus).GetOcspResponseStatus(), ((ResponseBytesBC)responseBytes
                ).GetResponseBytes())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
        /// </returns>
        public virtual OcspResponse GetOcspResponse() {
            return (OcspResponse)GetEncodable();
        }
    }
}

/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>.
    /// </summary>
    public class ResponseBytesBCFips : Asn1EncodableBCFips, IResponseBytes {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>.
        /// </summary>
        /// <param name="responseBytes">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>
        /// to be wrapped
        /// </param>
        public ResponseBytesBCFips(ResponseBytes responseBytes)
            : base(responseBytes) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>.
        /// </summary>
        /// <param name="asn1ObjectIdentifier">ASN1ObjectIdentifier wrapper</param>
        /// <param name="derOctetString">DEROctetString wrapper</param>
        public ResponseBytesBCFips(IDerObjectIdentifier asn1ObjectIdentifier, IDerOctetString derOctetString)
            : base(new ResponseBytes(((DerObjectIdentifierBCFips)asn1ObjectIdentifier).GetDerObjectIdentifier(), ((DerOctetStringBCFips
                )derOctetString).GetDerOctetString())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponseBytes"/>.
        /// </returns>
        public virtual ResponseBytes GetResponseBytes() {
            return (ResponseBytes)GetEncodable();
        }
    }
}

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

using System.IO;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
    /// </summary>
    public class OCSPResponseBCFips : ASN1EncodableBCFips, IOCSPResponse {
        
        private const int SUCCESSFUL = OcspResponseStatus.Successful;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
        /// </summary>
        /// <param name="ocspResponse">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>
        /// to be wrapped
        /// </param>
        public OCSPResponseBCFips(OcspResponse ocspResponse)
            : base(ocspResponse) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
        /// </summary>
        /// <param name="respStatus">OCSPResponseStatus wrapper</param>
        /// <param name="responseBytes">ResponseBytes wrapper</param>
        public OCSPResponseBCFips(IOCSPResponseStatus respStatus, IResponseBytes responseBytes)
            : base(new OcspResponse(((OCSPResponseStatusBCFips)respStatus).GetOcspResponseStatus(), ((ResponseBytesBCFips
                )responseBytes).GetResponseBytes())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspResponse"/>.
        /// </returns>
        public virtual OcspResponse GetOcspResponse() {
            return (OcspResponse)GetEncodable();
        }

        public byte[] GetEncoded()
        {
            return GetOcspResponse().GetEncoded();
        }

        public int GetStatus()
        {
            return GetOcspResponse().ResponseStatus.Value.IntValue;
        }

        public object GetResponseObject()
        {
            ResponseBytes rb = this.GetOcspResponse().ResponseBytes;

            if (rb == null)
                return null;

            if (rb.ResponseType.Equals(OcspObjectIdentifiers.PkixOcspBasic))
            {
                MemoryStream input = new MemoryStream(rb.Response.GetOctets(), false);
                Asn1InputStream asn1 = new Asn1InputStream(input, rb.Response.GetOctets().Length);
                Asn1Object result = asn1.ReadObject();
                return BasicOcspResponse.GetInstance(result);
            }

            return rb.Response;
        }

        public int GetSuccessful()
        {
            return SUCCESSFUL;
        }
    }
}

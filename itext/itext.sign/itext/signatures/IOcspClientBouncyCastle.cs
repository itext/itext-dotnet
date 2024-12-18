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
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures {
    /// <summary>Interface for the Online Certificate Status Protocol (OCSP) Client.</summary>
    /// <remarks>
    /// Interface for the Online Certificate Status Protocol (OCSP) Client.
    /// With a method returning parsed IBasicOCSPResp instead of encoded response.
    /// </remarks>
    public interface IOcspClientBouncyCastle : IOcspClient {
        /// <summary>Gets OCSP response.</summary>
        /// <remarks>
        /// Gets OCSP response.
        /// <para />
        /// If required,
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// can be checked using
        /// <see cref="iText.Signatures.Validation.OCSPValidator"/>
        /// class.
        /// </remarks>
        /// <param name="checkCert">the certificate to check</param>
        /// <param name="rootCert">parent certificate</param>
        /// <param name="url">to get the verification</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// an OCSP response wrapper
        /// </returns>
        IBasicOcspResponse GetBasicOCSPResp(IX509Certificate checkCert, IX509Certificate rootCert, String url);
    }
}

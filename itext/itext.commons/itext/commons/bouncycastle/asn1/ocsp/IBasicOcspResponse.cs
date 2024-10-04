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
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;

namespace iText.Commons.Bouncycastle.Asn1.Ocsp {
    /// <summary>
    /// This interface represents the wrapper for BasicOCSPResponse that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IBasicOcspResponse : IAsn1Encodable {
        /// <summary>
        /// Gets TbsResponseData for the wrapped BasicOCSPResponse object
        /// and calls actual
        /// <c>getProducedAt</c>
        /// method, then gets DateTime.
        /// </summary>
        /// <returns>produced at date.</returns>
        DateTime GetProducedAtDate();

        /// <summary>
        /// Verifies given certificate for the wrapped BasicOCSPResponse object.
        /// </summary>
        /// <returns>boolean value.</returns>
        bool Verify(IX509Certificate cert);
        
        /// <summary>
        /// Gets actual
        /// <c>Certs</c>
        /// field for the wrapped BasicOCSPResponse object.
        /// </summary>
        /// <returns>list of wrapped certificates.</returns>
        IEnumerable<IX509Certificate> GetCerts();

        /// <summary>
        /// Gets actual
        /// <c>Certs</c>
        /// field for the wrapped BasicOCSPResponse object.
        /// </summary>
        /// <returns>list of wrapped certificates.</returns>
        IX509Certificate[] GetOcspCerts();

        /// <summary>
        /// Calls actual
        /// <c>GetEncoded</c>
        /// method for the wrapped BasicOCSPResponse object.
        /// </summary>
        /// <returns>the default encoding for the wrapped object.</returns>
        byte[] GetEncoded();

        /// <summary>
        /// Calls actual
        /// <c>GetResponses</c>
        /// method for the wrapped BasicOCSPResponse object.
        /// </summary>
        /// <returns>wrapped SingleResp list.</returns>
        ISingleResponse[] GetResponses();

        /// <summary>
        /// Calls actual
        /// <c>GetProducedAt</c>
        /// method for the wrapped BasicOCSPResponse object.
        /// </summary>
        /// <returns>date BasicOCSPResponse was produced at.</returns>
        DateTime GetProducedAt();

        /// <summary>
        /// Calls actual
        /// <c>GetExtensionParsedValue</c>
        /// method for the wrapped BasicOCSPResponse object.
        /// </summary>
        /// <returns>Parsed extension value.</returns>
        IAsn1Encodable GetExtensionParsedValue(IDerObjectIdentifier getIdPkixOcspArchiveCutoff);

        IRespID GetResponderId();
    }
}

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
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Crypto;

namespace iText.Commons.Bouncycastle.Cert {
    /// <summary>
    /// This interface represents the wrapper for X509Crl that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IX509Crl {
        /// <summary>
        /// Calls actual
        /// <c>IsRevoked</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <param name="cert">x509 certificate wrapper</param>
        /// <returns>boolean value.</returns>
        bool IsRevoked(IX509Certificate cert);

        /// <summary>
        /// Calls actual
        /// <c>GetIssuerDN</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <returns>X500Name wrapper.</returns>
        IX500Name GetIssuerDN();
        
        /// <summary>
        /// Calls actual
        /// <c>GetNextUpdate</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <returns>DateTime value of the next update.</returns>
        DateTime GetNextUpdate();
        
        /// <summary>
        /// Calls actual
        /// <c>Verify</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <param name="publicKey">public key to verify</param>
        void Verify(IPublicKey publicKey);

        /// <summary>
        /// Calls actual
        /// <c>GetEncoded</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <returns>encoded array</returns>
        byte[] GetEncoded();

        /// <summary>
        /// Calls actual
        /// <c>GetExtensionValue</c>
        /// method for the wrapped X509Crl object.
        /// </summary>
        /// <returns>the DER-encoded octet string of the extension value.</returns>
        IAsn1OctetString GetExtensionValue(string oid);
    }
}

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
using System.Collections;
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Cert {
    /// <summary>
    /// This interface represents the wrapper for X509Certificate that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IX509Certificate {
        /// <summary>
        /// Calls actual
        /// <c>GetIssuerDN</c>
        /// method for the wrapped X509Certificate object.
        /// </summary>
        /// <returns>Issuer DN wrapper .</returns>
        IX500Name GetIssuerDN();
        
        /// <summary>
        /// Calls actual
        /// <c>GetSerialNumber</c>
        /// method for the wrapped X509Certificate object.
        /// </summary>
        /// <returns>Serial number.</returns>
        IBigInteger GetSerialNumber();
        
        /// <summary>
        /// Calls actual
        /// <c>GetPublicKey</c>
        /// method for the wrapped X509Certificate object.
        /// </summary>
        /// <returns>Public key wrapper.</returns>
        IPublicKey GetPublicKey();
        
        /// <summary>
        /// Calls actual
        /// <c>GetEncoded</c>
        /// method for the wrapped X509Certificate object.
        /// </summary>
        /// <returns>byte array.</returns>
        byte[] GetEncoded();
        
        /// <summary>
        /// Calls actual
        /// <c>GetTbsCertificate</c>
        /// method for the wrapped X509Certificate object.
        /// </summary>
        /// <returns>byte array.</returns>
        byte[] GetTbsCertificate();

        /// <summary>
        /// Calls actual
        /// <c>GetExtensionValue</c>
        /// method for the wrapped X509Certificate object.
        /// </summary>
        /// <param name="oid">string oid value</param>
        /// <returns>extension value wrapper.</returns>
        IAsn1OctetString GetExtensionValue(string oid);

        /// <summary>
        /// Calls actual
        /// <c>Verify</c>
        /// method for the wrapped X509Certificate object.
        /// </summary>
        /// <param name="issuerPublicKey">public key wrapper</param>
        void Verify(IPublicKey issuerPublicKey);
        
        /// <summary>
        /// Calls actual
        /// <c>GetCriticalExtensionOids</c>
        /// method for the wrapped X509Certificate object.
        /// </summary>
        /// <returns>critical extension oids set.</returns>
        ISet<string> GetCriticalExtensionOids();

        /// <summary>
        /// Calls actual
        /// <c>CheckValidity</c>
        /// method for the wrapped X509Certificate object.
        /// </summary>
        /// <param name="time">DateTime object</param>
        void CheckValidity(DateTime time);

        /// <summary>
        /// Returns actual
        /// <c>SubjectDN</c>
        /// property for the wrapped X509Certificate object.
        /// </summary>
        /// <returns>X500Name wrapper.</returns>
        IX500Name GetSubjectDN();

        /// <summary>
        /// Returns actual
        /// <c>EndDate</c>
        /// property of the certificate structure
        /// for the wrapped X509Certificate object.
        /// </summary>
        /// <returns>String time value.</returns>
        string GetEndDateTime();

        /// <summary>
        /// Returns actual
        /// <c>NotBefore</c>
        /// property for the wrapped X509Certificate object.
        /// </summary>
        /// <returns>DateTime value.</returns>
        DateTime GetNotBefore();

        /// <summary>
        /// Calls actual
        /// <c>GetExtendedKeyUsage</c>
        /// method for the wrapped X509Certificate object.
        /// </summary>
        /// <returns>List of object identifiers represented as Strings.</returns>
        IList GetExtendedKeyUsage();
    }
}

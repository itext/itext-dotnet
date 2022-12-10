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
namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for X509Extensions that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IExtensions : IASN1Encodable {
        /// <summary>
        /// Gets
        /// <c>cRLDistributionPoints</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.cRLDistributionPoints wrapper.</returns>
        IASN1ObjectIdentifier GetCRlDistributionPoints();

        /// <summary>
        /// Gets
        /// <c>authorityInfoAccess</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.authorityInfoAccess wrapper.</returns>
        IASN1ObjectIdentifier GetAuthorityInfoAccess();

        /// <summary>
        /// Gets
        /// <c>basicConstraints</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.basicConstraints wrapper.</returns>
        IASN1ObjectIdentifier GetBasicConstraints();

        /// <summary>
        /// Gets
        /// <c>keyUsage</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.keyUsage wrapper.</returns>
        IASN1ObjectIdentifier GetKeyUsage();

        /// <summary>
        /// Gets
        /// <c>extendedKeyUsage</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.extendedKeyUsage wrapper.</returns>
        IASN1ObjectIdentifier GetExtendedKeyUsage();

        /// <summary>
        /// Gets
        /// <c>authorityKeyIdentifier</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.authorityKeyIdentifier wrapper.</returns>
        IASN1ObjectIdentifier GetAuthorityKeyIdentifier();

        /// <summary>
        /// Gets
        /// <c>subjectKeyIdentifier</c>
        /// constant for the wrapped Extension.
        /// </summary>
        /// <returns>Extension.subjectKeyIdentifier wrapper.</returns>
        IASN1ObjectIdentifier GetSubjectKeyIdentifier();
    }
}

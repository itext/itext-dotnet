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
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X500;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for TBSCertificate that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ITbsCertificateStructure : IAsn1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getSubjectPublicKeyInfo</c>
        /// method for the wrapped TBSCertificate object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="ISubjectPublicKeyInfo"/>
        /// wrapped SubjectPublicKeyInfo.
        /// </returns>
        ISubjectPublicKeyInfo GetSubjectPublicKeyInfo();

        /// <summary>
        /// Calls actual
        /// <c>getIssuer</c>
        /// method for the wrapped TBSCertificate object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X500.IX500Name"/>
        /// wrapped X500Name.
        /// </returns>
        IX500Name GetIssuer();

        /// <summary>
        /// Calls actual
        /// <c>getSerialNumber</c>
        /// method for the wrapped TBSCertificate object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IDerInteger"/>
        /// wrapped ASN1Integer.
        /// </returns>
        IDerInteger GetSerialNumber();
    }
}

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
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for DistributionPointName that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IDistributionPointName : IAsn1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getType</c>
        /// method for the wrapped DistributionPointName object.
        /// </summary>
        /// <returns>type value.</returns>
        int GetType();

        /// <summary>
        /// Calls actual
        /// <c>getName</c>
        /// method for the wrapped DistributionPointName object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Encodable"/>
        /// ASN1Encodable wrapper.
        /// </returns>
        IAsn1Encodable GetName();

        /// <summary>
        /// Gets
        /// <c>FULL_NAME</c>
        /// constant for the wrapped DistributionPointName.
        /// </summary>
        /// <returns>DistributionPointName.FULL_NAME value.</returns>
        int GetFullName();
    }
}

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

namespace iText.Commons.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// This interface represents the wrapper for IssuingDistributionPoint that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IIssuingDistributionPoint : IAsn1Encodable {
        /// <summary>
        /// Calls actual
        /// <c>getDistributionPoint</c>
        /// method for the wrapped IssuingDistributionPoint object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IDistributionPointName"/>
        /// wrapped distribution point name.
        /// </returns>
        IDistributionPointName GetDistributionPoint();

        /// <summary>
        /// Calls actual
        /// <c>onlyContainsUserCerts</c>
        /// method for the wrapped IssuingDistributionPoint object.
        /// </summary>
        /// <returns>true if onlyContainsUserCerts was set, false otherwise.</returns>
        bool OnlyContainsUserCerts();

        /// <summary>
        /// Calls actual
        /// <c>onlyContainsCACerts</c>
        /// method for the wrapped IssuingDistributionPoint object.
        /// </summary>
        /// <returns>true if onlyContainsCACerts was set, false otherwise.</returns>
        bool OnlyContainsCACerts();

        /// <summary>
        /// Calls actual
        /// <c>isIndirectCRL</c>
        /// method for the wrapped IssuingDistributionPoint object.
        /// </summary>
        /// <returns>boolean value identifying if CRL is indirect.</returns>
        bool IsIndirectCRL();

        /// <summary>
        /// Calls actual
        /// <c>onlyContainsAttributeCerts</c>
        /// method for the wrapped IssuingDistributionPoint object.
        /// </summary>
        /// <returns>true if onlyContainsAttributeCerts was set, false otherwise.</returns>
        bool OnlyContainsAttributeCerts();

        /// <summary>
        /// Calls actual
        /// <c>getOnlySomeReasons</c>
        /// method for the wrapped IssuingDistributionPoint object.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IReasonFlags"/>
        /// wrapped reason flags.
        /// </returns>
        IReasonFlags GetOnlySomeReasons();
    }
}

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
using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.IssuingDistributionPoint"/>.
    /// </summary>
    public class IssuingDistributionPointBC : Asn1EncodableBC, IIssuingDistributionPoint {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.IssuingDistributionPoint"/>.
        /// </summary>
        /// <param name="issuingDistPoint">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.IssuingDistributionPoint"/>
        /// to be wrapped
        /// </param>
        public IssuingDistributionPointBC(IssuingDistributionPoint issuingDistPoint)
            : base(issuingDistPoint) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.IssuingDistributionPoint"/>.
        /// </returns>
        public virtual IssuingDistributionPoint GetIssuingDistributionPoint() {
            return (IssuingDistributionPoint)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual IDistributionPointName GetDistributionPoint() {
            return new DistributionPointNameBC(GetIssuingDistributionPoint().DistributionPoint);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual bool OnlyContainsUserCerts() {
            return GetIssuingDistributionPoint().OnlyContainsUserCerts;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual bool OnlyContainsCACerts() {
            return GetIssuingDistributionPoint().OnlyContainsCACerts;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual bool IsIndirectCRL() {
            return GetIssuingDistributionPoint().IsIndirectCrl;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual bool OnlyContainsAttributeCerts() {
            return GetIssuingDistributionPoint().OnlyContainsAttributeCerts;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual IReasonFlags GetOnlySomeReasons() {
            return new ReasonFlagsBC(GetIssuingDistributionPoint().OnlySomeReasons);
        }
    }
}

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
using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.CrlDistPoint"/>.
    /// </summary>
    public class CrlDistPointBCFips : Asn1EncodableBCFips, ICrlDistPoint {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.CrlDistPoint"/>.
        /// </summary>
        /// <param name="crlDistPoint">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.CrlDistPoint"/>
        /// to be wrapped
        /// </param>
        public CrlDistPointBCFips(CrlDistPoint crlDistPoint)
            : base(crlDistPoint) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.CrlDistPoint"/>.
        /// </returns>
        public virtual CrlDistPoint GetCrlDistPoint() {
            return (CrlDistPoint)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDistributionPoint[] GetDistributionPoints() {
            DistributionPoint[] distributionPoints = GetCrlDistPoint().GetDistributionPoints();
            IDistributionPoint[] distributionPointsBC = new IDistributionPoint[distributionPoints.Length];
            for (int i = 0; i < distributionPoints.Length; ++i) {
                distributionPointsBC[i] = new DistributionPointBCFips(distributionPoints[i]);
            }
            return distributionPointsBC;
        }
    }
}

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
using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPointName"/>.
    /// </summary>
    public class DistributionPointNameBC : Asn1EncodableBC, IDistributionPointName {
        private static readonly iText.Bouncycastle.Asn1.X509.DistributionPointNameBC INSTANCE = new iText.Bouncycastle.Asn1.X509.DistributionPointNameBC
            (null);

        private const int FULL_NAME = DistributionPointName.FullName;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPointName"/>.
        /// </summary>
        /// <param name="distributionPointName">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPointName"/>
        /// to be wrapped
        /// </param>
        public DistributionPointNameBC(DistributionPointName distributionPointName)
            : base(distributionPointName) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="DistributionPointNameBC"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastle.Asn1.X509.DistributionPointNameBC GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.DistributionPointName"/>.
        /// </returns>
        public virtual DistributionPointName GetDistributionPointName() {
            return (DistributionPointName)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetType() {
            return GetDistributionPointName().PointType;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Encodable GetName() {
            return new Asn1EncodableBC(GetDistributionPointName().Name);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetFullName() {
            return FULL_NAME;
        }
    }
}

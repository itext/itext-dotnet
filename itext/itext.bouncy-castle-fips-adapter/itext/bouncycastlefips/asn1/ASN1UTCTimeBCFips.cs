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
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerUtcTime"/>.
    /// </summary>
    public class ASN1UTCTimeBCFips : ASN1PrimitiveBCFips, IASN1UTCTime {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerUtcTime"/>.
        /// </summary>
        /// <param name="asn1UTCTime">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerUtcTime"/>
        /// to be wrapped
        /// </param>
        public ASN1UTCTimeBCFips(DerUtcTime asn1UTCTime)
            : base(asn1UTCTime) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerUtcTime"/>.
        /// </returns>
        public virtual DerUtcTime GetASN1UTCTime() {
            return (DerUtcTime)GetEncodable();
        }
    }
}

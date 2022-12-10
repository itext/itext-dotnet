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
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Math;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X500;

namespace iText.Bouncycastle.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>.
    /// </summary>
    public class IssuerAndSerialNumberBC : ASN1EncodableBC, IIssuerAndSerialNumber {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>.
        /// </summary>
        /// <param name="issuerAndSerialNumber">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>
        /// to be wrapped
        /// </param>
        public IssuerAndSerialNumberBC(IssuerAndSerialNumber issuerAndSerialNumber)
            : base(issuerAndSerialNumber) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>.
        /// </summary>
        /// <param name="issuer">
        /// X500Name wrapper to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>
        /// </param>
        /// <param name="value">
        /// BigInteger to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>
        /// </param>
        public IssuerAndSerialNumberBC(IX500Name issuer, BigInteger value)
            : base(new IssuerAndSerialNumber(((X500NameBC)issuer).GetX500Name(), value)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber"/>.
        /// </returns>
        public virtual IssuerAndSerialNumber GetIssuerAndSerialNumber() {
            return (IssuerAndSerialNumber)GetEncodable();
        }
    }
}

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
using Org.BouncyCastle.Asn1.Esf;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.Esf {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Esf.OtherHashAlgAndValue"/>.
    /// </summary>
    public class OtherHashAlgAndValueBC : ASN1EncodableBC, IOtherHashAlgAndValue {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.OtherHashAlgAndValue"/>.
        /// </summary>
        /// <param name="otherHashAlgAndValue">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Esf.OtherHashAlgAndValue"/>
        /// to be wrapped
        /// </param>
        public OtherHashAlgAndValueBC(OtherHashAlgAndValue otherHashAlgAndValue)
            : base(otherHashAlgAndValue) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Esf.OtherHashAlgAndValue"/>.
        /// </summary>
        /// <param name="algorithmIdentifier">AlgorithmIdentifier wrapper</param>
        /// <param name="octetString">ASN1OctetString wrapper</param>
        public OtherHashAlgAndValueBC(IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString octetString)
            : this(new OtherHashAlgAndValue(((AlgorithmIdentifierBC)algorithmIdentifier).GetAlgorithmIdentifier(), ((ASN1OctetStringBC
                )octetString).GetASN1OctetString())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Esf.OtherHashAlgAndValue"/>.
        /// </returns>
        public virtual OtherHashAlgAndValue GetOtherHashAlgAndValue() {
            return (OtherHashAlgAndValue)GetEncodable();
        }
    }
}

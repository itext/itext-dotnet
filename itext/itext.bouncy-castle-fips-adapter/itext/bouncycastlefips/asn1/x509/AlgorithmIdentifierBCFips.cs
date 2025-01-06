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
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier"/>.
    /// </summary>
    public class AlgorithmIdentifierBCFips : Asn1EncodableBCFips, IAlgorithmIdentifier {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier"/>.
        /// </summary>
        /// <param name="algorithmIdentifier">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier"/>
        /// to be wrapped
        /// </param>
        public AlgorithmIdentifierBCFips(AlgorithmIdentifier algorithmIdentifier)
            : base(algorithmIdentifier) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier"/>.
        /// </returns>
        public virtual AlgorithmIdentifier GetAlgorithmIdentifier() {
            return (AlgorithmIdentifier)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier GetAlgorithm() {
            return new DerObjectIdentifierBCFips(GetAlgorithmIdentifier().Algorithm);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Encodable GetParameters() {
            return new Asn1EncodableBCFips(GetAlgorithmIdentifier().Parameters);
        }
    }
}

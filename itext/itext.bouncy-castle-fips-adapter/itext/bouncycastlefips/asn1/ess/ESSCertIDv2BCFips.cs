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
using Org.BouncyCastle.Asn1.Ess;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.Ess;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastlefips.Asn1.Ess {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ess.EssCertIDv2"/>.
    /// </summary>
    public class ESSCertIDv2BCFips : Asn1EncodableBCFips, IEssCertIDv2 {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ess.EssCertIDv2"/>.
        /// </summary>
        /// <param name="essCertIDv2">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ess.EssCertIDv2"/>
        /// to be wrapped
        /// </param>
        public ESSCertIDv2BCFips(EssCertIDv2 essCertIDv2)
            : base(essCertIDv2) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ess.EssCertIDv2"/>.
        /// </returns>
        public virtual EssCertIDv2 GetEssCertIDv2() {
            return (EssCertIDv2)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier GetHashAlgorithm() {
            return new AlgorithmIdentifierBCFips(GetEssCertIDv2().HashAlgorithm);
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetCertHash() {
            return GetEssCertIDv2().GetCertHash();
        }
    }
}

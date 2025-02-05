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
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Math;

namespace iText.Commons.Bouncycastle.Asn1.Pkcs {
    /// <summary>Wrapper interface for BouncyCastle's representation of RSASSA-PSS parameters in ASN.1.</summary>
    public interface IRsassaPssParameters : IAsn1Encodable {
        /// <summary>
        /// Return the
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IAlgorithmIdentifier"/>
        /// describing the digest algorithm to be used in the signature.
        /// </summary>
        /// <returns>
        /// an
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IAlgorithmIdentifier"/>
        /// </returns>
        IAlgorithmIdentifier GetHashAlgorithm();

        /// <summary>
        /// Return the
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IAlgorithmIdentifier"/>
        /// describing the mask generation function to be used in the signature.
        /// </summary>
        /// <returns>
        /// an
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IAlgorithmIdentifier"/>
        /// </returns>
        IAlgorithmIdentifier GetMaskGenAlgorithm();

        /// <summary>Return the salt length parameter.</summary>
        /// <remarks>
        /// Return the salt length parameter. This is a
        /// <see cref="iText.Commons.Bouncycastle.Math.IASN1Integer"/>
        /// for API consistency reasons, but typical
        /// values will be small.
        /// </remarks>
        IDerInteger GetSaltLength();

        /// <summary>Return the trailer field parameter.</summary>
        /// <remarks>
        /// Return the trailer field parameter. This is a
        /// <see cref="iText.Commons.Bouncycastle.Math.IASN1Integer"/>
        /// for API consistency reasons, but typical
        /// values will be small.
        /// </remarks>
        IDerInteger GetTrailerField();
    }
}

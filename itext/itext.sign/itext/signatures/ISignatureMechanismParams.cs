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
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Signatures {
    /// <summary>Interface to encode the parameters to a signature algorithm for inclusion in a signature object.</summary>
    /// <remarks>
    /// Interface to encode the parameters to a signature algorithm for inclusion in a signature object.
    /// See
    /// <see cref="RSASSAPSSMechanismParams"/>
    /// for an example.
    /// </remarks>
    public interface ISignatureMechanismParams {
        /// <summary>
        /// Represent the parameters as an
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IASN1Encodable"/>
        /// for inclusion in a signature object.
        /// </summary>
        /// <returns>
        /// an
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IASN1Encodable"/>
        /// object
        /// </returns>
        IASN1Encodable ToEncodable();
    }
}

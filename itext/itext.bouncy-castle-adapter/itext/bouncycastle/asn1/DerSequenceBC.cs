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
using  Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>.
    /// </summary>
    public class DerSequenceBC : Asn1SequenceBC, IDerSequence {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>.
        /// </summary>
        /// <param name="derSequence">
        /// 
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>
        /// to be wrapped
        /// </param>
        public DerSequenceBC(DerSequence derSequence)
            : base(derSequence) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>.
        /// </summary>
        /// <param name="vector">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>
        /// to create
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>
        /// </param>
        public DerSequenceBC(Asn1EncodableVector vector)
            : base(new DerSequence(vector)) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>.
        /// </summary>
        /// <param name="encodable">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>
        /// to create
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>
        /// </param>
        public DerSequenceBC(Asn1Encodable encodable)
            : base(new DerSequence(encodable)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref=" Org.BouncyCastle.Asn1.DerSequence"/>.
        /// </returns>
        public virtual DerSequence GetDerSequence() {
            return (DerSequence)GetEncodable();
        }
    }
}

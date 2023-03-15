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
using iText.Bouncycastle.Math;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Math;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>.
    /// </summary>
    public class ASN1IntegerBC : ASN1PrimitiveBC, IASN1Integer {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>.
        /// </summary>
        /// <param name="i">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>
        /// to be wrapped
        /// </param>
        public ASN1IntegerBC(DerInteger i)
            : base(i) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>.
        /// </summary>
        /// <param name="i">
        /// int value to create
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>
        /// to be wrapped
        /// </param>
        public ASN1IntegerBC(int i)
            : base(new DerInteger(i)) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>.
        /// </summary>
        /// <param name="i">
        /// BigInteger value to create
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>
        /// to be wrapped
        /// </param>
        public ASN1IntegerBC(IBigInteger i)
            : base(new DerInteger(((BigIntegerBC) i).GetBigInteger())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerInteger"/>.
        /// </returns>
        public virtual DerInteger GetASN1Integer() {
            return (DerInteger)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBigInteger GetValue() {
            return new BigIntegerBC(GetASN1Integer().Value);
        }

        public int GetIntValue()
        {
            return GetASN1Integer().Value.IntValue;
        }
    }
}

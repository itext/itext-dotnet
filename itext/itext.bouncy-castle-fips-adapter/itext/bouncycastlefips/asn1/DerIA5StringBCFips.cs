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
using System;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>.
    /// </summary>
    public class DerIA5StringBCFips : Asn1ObjectBCFips, IDerIA5String {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>.
        /// </summary>
        /// <param name="deria5String">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>
        /// to be wrapped
        /// </param>
        public DerIA5StringBCFips(DerIA5String deria5String)
            : base(deria5String) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>.
        /// </summary>
        /// <param name="str">
        /// string to create
        /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>
        /// to be wrapped
        /// </param>
        public DerIA5StringBCFips(String str)
            : this(new DerIA5String(str)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.DerIA5String"/>.
        /// </returns>
        public virtual DerIA5String GetDerIA5String() {
            return (DerIA5String)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetString() {
            return GetDerIA5String().GetString();
        }
    }
}

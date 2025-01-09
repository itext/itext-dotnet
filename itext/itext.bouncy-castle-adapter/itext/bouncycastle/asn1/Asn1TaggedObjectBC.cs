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
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>.
    /// </summary>
    public class Asn1TaggedObjectBC : Asn1ObjectBC, IAsn1TaggedObject {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>.
        /// </summary>
        /// <param name="taggedObject">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>
        /// to be wrapped
        /// </param>
        public Asn1TaggedObjectBC(Asn1TaggedObject taggedObject)
            : base(taggedObject) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>.
        /// </returns>
        public virtual Asn1TaggedObject GetAsn1TaggedObject() {
            return (Asn1TaggedObject)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Object GetObject() {
            return new Asn1ObjectBC(GetAsn1TaggedObject().GetObject());
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetTagNo() {
            return GetAsn1TaggedObject().TagNo;
        }
    }
}

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
    /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>.
    /// </summary>
    public class Asn1OctetStringBC : Asn1ObjectBC, IAsn1OctetString {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>.
        /// </summary>
        /// <param name="string">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>
        /// to be wrapped
        /// </param>
        public Asn1OctetStringBC(Asn1OctetString @string)
            : base(@string) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>.
        /// </summary>
        /// <param name="taggedObject">
        /// ASN1TaggedObject wrapper to create
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>
        /// </param>
        /// <param name="b">
        /// boolean to create
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>
        /// </param>
        public Asn1OctetStringBC(IAsn1TaggedObject taggedObject, bool b)
            : base(Asn1OctetString.GetInstance(((Asn1TaggedObjectBC)taggedObject).GetAsn1TaggedObject(), b)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1OctetString"/>.
        /// </returns>
        public virtual Asn1OctetString GetAsn1OctetString() {
            return (Asn1OctetString)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetOctets() {
            return GetAsn1OctetString().GetOctets();
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GetDerEncoded() {
            return !IsNull() ? GetAsn1OctetString().GetDerEncoded() : null;
        }
    }
}

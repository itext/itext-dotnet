/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref=" Org.BouncyCastle.Asn1.DerTaggedObject"/>.
    /// </summary>
    public class DERTaggedObjectBCFips : ASN1TaggedObjectBCFips, IDERTaggedObject {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref=" Org.BouncyCastle.Asn1.DerTaggedObject"/>.
        /// </summary>
        /// <param name="derTaggedObject">
        /// 
        /// <see cref=" Org.BouncyCastle.Asn1.DerTaggedObject"/>
        /// to be wrapped
        /// </param>
        public DERTaggedObjectBCFips(DerTaggedObject derTaggedObject)
            : base(derTaggedObject) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref=" Org.BouncyCastle.Asn1.DerTaggedObject"/>.
        /// </summary>
        /// <param name="i">
        /// int value to create
        /// <see cref=" Org.BouncyCastle.Asn1.DerTaggedObject"/>
        /// to be wrapped
        /// </param>
        /// <param name="encodable">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>
        /// to create
        /// <see cref=" Org.BouncyCastle.Asn1.DerTaggedObject"/>
        /// to be wrapped
        /// </param>
        public DERTaggedObjectBCFips(int i, Asn1Encodable encodable)
            : base(new DerTaggedObject(i, encodable)) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref=" Org.BouncyCastle.Asn1.DerTaggedObject"/>.
        /// </summary>
        /// <param name="b">
        /// boolean to create
        /// <see cref=" Org.BouncyCastle.Asn1.DerTaggedObject"/>
        /// to be wrapped
        /// </param>
        /// <param name="i">
        /// int value to create
        /// <see cref=" Org.BouncyCastle.Asn1.DerTaggedObject"/>
        /// to be wrapped
        /// </param>
        /// <param name="encodable">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Encodable"/>
        /// to create
        /// <see cref=" Org.BouncyCastle.Asn1.DerTaggedObject"/>
        /// to be wrapped
        /// </param>
        public DERTaggedObjectBCFips(bool b, int i, Asn1Encodable encodable)
            : base(new DerTaggedObject(b, i, encodable)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref=" Org.BouncyCastle.Asn1.DerTaggedObject"/>.
        /// </returns>
        public virtual DerTaggedObject GetDERTaggedObject() {
            return (DerTaggedObject)GetEncodable();
        }
    }
}

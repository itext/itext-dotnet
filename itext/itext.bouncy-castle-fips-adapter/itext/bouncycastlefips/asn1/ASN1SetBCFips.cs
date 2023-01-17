/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Collections;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1Set"/>.
    /// </summary>
    public class ASN1SetBCFips : ASN1PrimitiveBCFips, IASN1Set {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Set"/>.
        /// </summary>
        /// <param name="set">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Set"/>
        /// to be wrapped
        /// </param>
        public ASN1SetBCFips(Asn1Set set)
            : base(set) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Set"/>.
        /// </summary>
        /// <param name="taggedObject">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1TaggedObject"/>
        /// to create
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Set"/>
        /// to be wrapped
        /// </param>
        /// <param name="b">
        /// boolean to create
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Set"/>
        /// to be wrapped
        /// </param>
        public ASN1SetBCFips(Asn1TaggedObject taggedObject, bool b)
            : base(Asn1Set.GetInstance(taggedObject, b)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Set"/>.
        /// </returns>
        public virtual Asn1Set GetASN1Set() {
            return (Asn1Set)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IEnumerator GetObjects() {
            return GetASN1Set().GetEnumerator();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Size() {
            return GetASN1Set().Count;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Encodable GetObjectAt(int index) {
            return new ASN1EncodableBCFips(GetASN1Set()[index]);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Encodable[] ToArray() {
            Asn1Encodable[] encodables = GetASN1Set().ToArray();
            ASN1EncodableBCFips[] encodablesBCFips = new ASN1EncodableBCFips[encodables.Length];
            for (int i = 0; i < encodables.Length; ++i) {
                encodablesBCFips[i] = new ASN1EncodableBCFips(encodables[i]);
            }
            return encodablesBCFips;
        }
    }
}

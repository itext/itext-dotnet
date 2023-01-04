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
using System;
using System.Collections;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
    /// </summary>
    public class ASN1SequenceBC : ASN1PrimitiveBC, IASN1Sequence {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
        /// </summary>
        /// <param name="sequence">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>
        /// to be wrapped
        /// </param>
        public ASN1SequenceBC(Asn1Sequence sequence)
            : base(sequence) {
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
        /// </summary>
        /// <param name="obj">
        /// to get
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>
        /// instance to be wrapped
        /// </param>
        public ASN1SequenceBC(Object obj)
            : base(Asn1Sequence.GetInstance(obj)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
        /// </returns>
        public virtual Asn1Sequence GetASN1Sequence() {
            return (Asn1Sequence)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Encodable GetObjectAt(int i) {
            return new ASN1EncodableBC(GetASN1Sequence()[i]);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IEnumerator GetObjects() {
            return GetASN1Sequence().GetEnumerator();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Size() {
            return GetASN1Sequence().Count;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Encodable[] ToArray() {
            Asn1Encodable[] encodables = GetASN1Sequence().ToArray();
            ASN1EncodableBC[] encodablesBC = new ASN1EncodableBC[encodables.Length];
            for (int i = 0; i < encodables.Length; ++i) {
                encodablesBC[i] = new ASN1EncodableBC(encodables[i]);
            }
            return encodablesBC;
        }
    }
}

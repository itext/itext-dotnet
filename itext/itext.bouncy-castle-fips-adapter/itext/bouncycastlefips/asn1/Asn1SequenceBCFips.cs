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
using System;
using System.Collections;
using Org.BouncyCastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Bouncycastlefips.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
    /// </summary>
    public class Asn1SequenceBCFips : Asn1ObjectBCFips, IAsn1Sequence {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
        /// </summary>
        /// <param name="sequence">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>
        /// to be wrapped
        /// </param>
        public Asn1SequenceBCFips(Asn1Sequence sequence)
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
        public Asn1SequenceBCFips(Object obj)
            : base(Asn1Sequence.GetInstance(obj)) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Sequence"/>.
        /// </returns>
        public virtual Asn1Sequence GetAsn1Sequence() {
            return (Asn1Sequence)GetPrimitive();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Encodable GetObjectAt(int i) {
            return new Asn1EncodableBCFips(GetAsn1Sequence()[i]);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IEnumerator GetObjects() {
            return GetAsn1Sequence().GetEnumerator();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Size() {
            return GetAsn1Sequence().Count;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Encodable[] ToArray() {
            Asn1Sequence encodables = GetAsn1Sequence();
            Asn1EncodableBCFips[] encodablesBCFips = new Asn1EncodableBCFips[encodables.Count];
            for (int i = 0; i < encodables.Count; ++i) {
                encodablesBCFips[i] = new Asn1EncodableBCFips(encodables[i]);
            }
            return encodablesBCFips;
        }
    }
}

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
using iText.Bouncycastle.Asn1.Cms;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>.
    /// </summary>
    public class Asn1EncodableVectorBC : IAsn1EncodableVector {
        private readonly Asn1EncodableVector encodableVector;

        /// <summary>
        /// Creates new wrapper instance for new
        /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>
        /// object.
        /// </summary>
        public Asn1EncodableVectorBC() {
            encodableVector = new Asn1EncodableVector();
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>.
        /// </summary>
        /// <param name="encodableVector">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>
        /// to be wrapped
        /// </param>
        public Asn1EncodableVectorBC(Asn1EncodableVector encodableVector) {
            this.encodableVector = encodableVector;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Asn1EncodableVector"/>.
        /// </returns>
        public virtual Asn1EncodableVector GetEncodableVector() {
            return encodableVector;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Add(IAsn1Object primitive) {
            Asn1ObjectBC primitiveBC = (Asn1ObjectBC)primitive;
            encodableVector.Add(primitiveBC.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Add(IAttribute attribute) {
            AttributeBC attributeBC = (AttributeBC)attribute;
            encodableVector.Add(attributeBC.GetAttribute());
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Add(IAlgorithmIdentifier element) {
            AlgorithmIdentifierBC elementBc = (AlgorithmIdentifierBC)element;
            encodableVector.Add(elementBc.GetAlgorithmIdentifier());
        }

        /// <summary><inheritDoc/></summary>
        public virtual void AddOptional(IAsn1Object primitive) {
            if (primitive != null) {
                Add(primitive);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual void AddOptional(IAttribute attribute) {
            if (attribute != null) {
                Add(attribute);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual void AddOptional(IAlgorithmIdentifier element) {
            if (element != null) {
                Add(element);
            }
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            Asn1EncodableVectorBC that = (Asn1EncodableVectorBC)o;
            return Object.Equals(encodableVector, that.encodableVector);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(encodableVector);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return encodableVector.ToString();
        }
    }
}

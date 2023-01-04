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
using Org.BouncyCastle.Asn1;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1.Cms {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Cms.AttributeTable"/>.
    /// </summary>
    public class AttributeTableBCFips : IAttributeTable {
        private readonly Org.BouncyCastle.Asn1.Cms.AttributeTable attributeTable;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.AttributeTable"/>.
        /// </summary>
        /// <param name="attributeTable">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Cms.AttributeTable"/>
        /// to be wrapped
        /// </param>
        public AttributeTableBCFips(Org.BouncyCastle.Asn1.Cms.AttributeTable attributeTable) {
            this.attributeTable = attributeTable;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Cms.AttributeTable"/>.
        /// </summary>
        /// <param name="set">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Asn1Set"/>
        /// to create
        /// <see cref="Org.BouncyCastle.Asn1.Cms.AttributeTable"/>
        /// </param>
        public AttributeTableBCFips(Asn1Set set) {
            attributeTable = new Org.BouncyCastle.Asn1.Cms.AttributeTable(set);
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Cms.AttributeTable"/>.
        /// </returns>
        public virtual Org.BouncyCastle.Asn1.Cms.AttributeTable GetAttributeTable() {
            return attributeTable;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAttribute Get(IASN1ObjectIdentifier oid) {
            ASN1ObjectIdentifierBCFips asn1ObjectIdentifier = (ASN1ObjectIdentifierBCFips)oid;
            return new AttributeBCFips(attributeTable[asn1ObjectIdentifier.GetASN1ObjectIdentifier()]);
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
            iText.Bouncycastlefips.Asn1.Cms.AttributeTableBCFips that = (iText.Bouncycastlefips.Asn1.Cms.AttributeTableBCFips
                )o;
            return Object.Equals(attributeTable, that.attributeTable);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(attributeTable);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return attributeTable.ToString();
        }
    }
}

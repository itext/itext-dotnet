using System;
using Org.BouncyCastle.Asn1;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1.Cms {
    public class AttributeTableBC : IAttributeTable {
        private readonly Org.BouncyCastle.Asn1.Cms.AttributeTable attributeTable;

        public AttributeTableBC(Org.BouncyCastle.Asn1.Cms.AttributeTable attributeTable) {
            this.attributeTable = attributeTable;
        }

        public AttributeTableBC(Asn1Set unat) {
            attributeTable = new Org.BouncyCastle.Asn1.Cms.AttributeTable(unat);
        }

        public virtual Org.BouncyCastle.Asn1.Cms.AttributeTable GetAttributeTable() {
            return attributeTable;
        }

        public virtual IAttribute Get(IASN1ObjectIdentifier oid) {
            ASN1ObjectIdentifierBC asn1ObjectIdentifier = (ASN1ObjectIdentifierBC)oid;
            return new AttributeBC(attributeTable[asn1ObjectIdentifier.GetASN1ObjectIdentifier()]);
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Asn1.Cms.AttributeTableBC that = (iText.Bouncycastle.Asn1.Cms.AttributeTableBC)o;
            return Object.Equals(attributeTable, that.attributeTable);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(attributeTable);
        }

        public override String ToString() {
            return attributeTable.ToString();
        }
    }
}

using System;
using Org.BouncyCastle.Asn1;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1.Cms {
    public class AttributeTableBCFips : IAttributeTable {
        private readonly Org.BouncyCastle.Asn1.Cms.AttributeTable attributeTable;

        public AttributeTableBCFips(Org.BouncyCastle.Asn1.Cms.AttributeTable attributeTable) {
            this.attributeTable = attributeTable;
        }

        public AttributeTableBCFips(Asn1Set unat) {
            attributeTable = new Org.BouncyCastle.Asn1.Cms.AttributeTable(unat);
        }

        public virtual Org.BouncyCastle.Asn1.Cms.AttributeTable GetAttributeTable() {
            return attributeTable;
        }

        public virtual IAttribute Get(IASN1ObjectIdentifier oid) {
            ASN1ObjectIdentifierBCFips asn1ObjectIdentifier = (ASN1ObjectIdentifierBCFips)oid;
            return new AttributeBCFips(attributeTable[asn1ObjectIdentifier.GetASN1ObjectIdentifier()]);
        }

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

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(attributeTable);
        }

        public override String ToString() {
            return attributeTable.ToString();
        }
    }
}

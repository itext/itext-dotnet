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

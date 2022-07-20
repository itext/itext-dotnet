using System;
using Org.BouncyCastle.Cert.Ocsp;
using iText.Bouncycastlefips.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cert.Ocsp.RespID"/>.
    /// </summary>
    public class RespIDBCFips : IRespID {
        private readonly RespID respID;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.RespID"/>.
        /// </summary>
        /// <param name="respID">
        /// 
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.RespID"/>
        /// to be wrapped
        /// </param>
        public RespIDBCFips(RespID respID) {
            this.respID = respID;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.RespID"/>.
        /// </summary>
        /// <param name="x500Name">
        /// X500Name wrapper to create
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.RespID"/>
        /// </param>
        public RespIDBCFips(IX500Name x500Name)
            : this(new RespID(((X500NameBCFips)x500Name).GetX500Name())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cert.Ocsp.RespID"/>.
        /// </returns>
        public virtual RespID GetRespID() {
            return respID;
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
            iText.Bouncycastlefips.Cert.Ocsp.RespIDBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.RespIDBCFips)o;
            return Object.Equals(respID, that.respID);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(respID);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return respID.ToString();
        }
    }
}

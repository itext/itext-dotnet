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
using Org.BouncyCastle.Ocsp;
using iText.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.RespID"/>.
    /// </summary>
    public class RespIDBC : IRespID {
        private readonly RespID respID;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.RespID"/>.
        /// </summary>
        /// <param name="respID">
        /// 
        /// <see cref="Org.BouncyCastle.Ocsp.RespID"/>
        /// to be wrapped
        /// </param>
        public RespIDBC(RespID respID) {
            this.respID = respID;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.RespID"/>.
        /// </summary>
        /// <param name="x500Name">
        /// X500Name wrapper to create
        /// <see cref="Org.BouncyCastle.Ocsp.RespID"/>
        /// </param>
        public RespIDBC(IX500Name x500Name)
            : this(new RespID(((X500NameBC)x500Name).GetX500Name())) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Ocsp.RespID"/>.
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
            iText.Bouncycastle.Cert.Ocsp.RespIDBC that = (iText.Bouncycastle.Cert.Ocsp.RespIDBC)o;
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

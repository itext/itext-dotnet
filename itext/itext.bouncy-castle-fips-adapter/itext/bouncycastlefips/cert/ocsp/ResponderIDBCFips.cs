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
using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastlefips.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert.Ocsp;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponderID"/>.
    /// </summary>
    public class ResponderIDBCFips : IResponderID {
        private readonly ResponderID responderID;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponderID"/>.
        /// </summary>
        /// <param name="responderID">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponderID"/>
        /// to be wrapped
        /// </param>
        public ResponderIDBCFips(ResponderID responderID) {
            this.responderID = responderID;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX500Name GetName() {
            return new X500NameBCFips(responderID.Name);
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.ResponderID"/>.
        /// </returns>
        public virtual ResponderID GetResponderID() {
            return responderID;
        }
    }
}

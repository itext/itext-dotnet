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
using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspObjectIdentifiers"/>.
    /// </summary>
    public class OCSPObjectIdentifiersBCFips : IOCSPObjectIdentifiers {
        private static readonly iText.Bouncycastlefips.Asn1.Ocsp.OCSPObjectIdentifiersBCFips INSTANCE = new iText.Bouncycastlefips.Asn1.Ocsp.OCSPObjectIdentifiersBCFips
            (null);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_BASIC = new ASN1ObjectIdentifierBCFips(OcspObjectIdentifiers
            .PkixOcspBasic);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_NONCE = new ASN1ObjectIdentifierBCFips(OcspObjectIdentifiers
            .PkixOcspNonce);

        private static readonly IASN1ObjectIdentifier ID_PKIX_OCSP_NOCHECK = new ASN1ObjectIdentifierBCFips(OcspObjectIdentifiers
            .PkixOcspNocheck);

        private readonly OcspObjectIdentifiers ocspObjectIdentifiers;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspObjectIdentifiers"/>.
        /// </summary>
        /// <param name="ocspObjectIdentifiers">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspObjectIdentifiers"/>
        /// to be wrapped
        /// </param>
        public OCSPObjectIdentifiersBCFips(OcspObjectIdentifiers ocspObjectIdentifiers) {
            this.ocspObjectIdentifiers = ocspObjectIdentifiers;
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="OCSPObjectIdentifiersBCFips"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastlefips.Asn1.Ocsp.OCSPObjectIdentifiersBCFips GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspObjectIdentifiers"/>.
        /// </returns>
        public virtual OcspObjectIdentifiers GetOcspObjectIdentifiers() {
            return ocspObjectIdentifiers;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetIdPkixOcspBasic() {
            return ID_PKIX_OCSP_BASIC;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetIdPkixOcspNonce() {
            return ID_PKIX_OCSP_NONCE;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier GetIdPkixOcspNoCheck() {
            return ID_PKIX_OCSP_NOCHECK;
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
            iText.Bouncycastlefips.Asn1.Ocsp.OCSPObjectIdentifiersBCFips that = (iText.Bouncycastlefips.Asn1.Ocsp.OCSPObjectIdentifiersBCFips
                )o;
            return Object.Equals(ocspObjectIdentifiers, that.ocspObjectIdentifiers);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspObjectIdentifiers);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return ocspObjectIdentifiers.ToString();
        }
    }
}

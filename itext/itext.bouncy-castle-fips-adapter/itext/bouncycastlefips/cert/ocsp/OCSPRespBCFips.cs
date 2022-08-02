/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using iText.Bouncycastlefips.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.OcspResp"/>.
    /// </summary>
    public class OCSPRespBCFips : IOCSPResp {
        private static readonly iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBCFips INSTANCE = new iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBCFips
            ((OcspResp)null);

        private const int SUCCESSFUL = Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful;

        private readonly OcspResp ocspResp;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.OcspResp"/>.
        /// </summary>
        /// <param name="ocspResp">
        /// 
        /// <see cref="Org.BouncyCastle.Ocsp.OcspResp"/>
        /// to be wrapped
        /// </param>
        public OCSPRespBCFips(OcspResp ocspResp) {
            this.ocspResp = ocspResp;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.OcspResp"/>.
        /// </summary>
        /// <param name="ocspResponse">OCSPResponse wrapper</param>
        public OCSPRespBCFips(IOCSPResponse ocspResponse)
            : this(new OcspResp(((OCSPResponseBCFips)ocspResponse).GetOcspResponse())) {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="OCSPRespBCFips"/>
        /// instance.
        /// </returns>
        public static iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBCFips GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Ocsp.OcspResp"/>.
        /// </returns>
        public virtual OcspResp GetOcspResp() {
            return ocspResp;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return ocspResp.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetStatus() {
            return ocspResp.Status;
        }

        /// <summary><inheritDoc/></summary>
        public virtual Object GetResponseObject() {
            try {
                return ocspResp.GetResponseObject();
            }
            catch (OcspException e) {
                throw new OCSPExceptionBCFips(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetSuccessful() {
            return SUCCESSFUL;
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
            iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.OCSPRespBCFips)o;
            return Object.Equals(ocspResp, that.ocspResp);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspResp);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return ocspResp.ToString();
        }
    }
}

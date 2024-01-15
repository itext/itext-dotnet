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
using System.Collections;
using System.Collections.Generic;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>.
    /// </summary>
    public class OcspReqBC : IOcspRequest {
        private readonly OcspReq ocspReq;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>.
        /// </summary>
        /// <param name="ocspReq">
        /// 
        /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>
        /// to be wrapped
        /// </param>
        public OcspReqBC(OcspReq ocspReq) {
            this.ocspReq = ocspReq;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>.
        /// </summary>
        /// <param name="certId">
        /// CertID wrapper
        /// </param>
        /// <param name="documentId">
        /// byte array
        /// </param>
        public OcspReqBC(ICertID certId, byte[] documentId) {
            OcspReqGenerator gen = new OcspReqGenerator();
            gen.AddRequest(new CertificateID(((CertIDBC) certId).GetCertID()));

            // create details for nonce extension
            IDictionary<DerObjectIdentifier, X509Extension> extensions = new Dictionary<DerObjectIdentifier,
                X509Extension>();
            DerOctetString derOctetString = new DerOctetString(new DerOctetString(documentId).GetEncoded());
            extensions[OcspObjectIdentifiers.PkixOcspNonce] = new X509Extension(false, derOctetString);

            gen.SetRequestExtensions(new X509Extensions(extensions));
            ocspReq = gen.Generate();
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>.
        /// </returns>
        public virtual OcspReq GetOcspReq() {
            return ocspReq;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return ocspReq.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public IReq[] GetRequestList() {
            Req[] reqs = ocspReq.GetRequestList();
            IReq[] wrappedReqs = new IReq[reqs.Length];
            for (int i = 0; i < reqs.Length; ++i) {
                wrappedReqs[i] = new ReqBC(reqs[i]);
            }
            return wrappedReqs;
        }

        /// <summary><inheritDoc/></summary>
        public IX509Extension GetExtension(IDerObjectIdentifier objectIdentifier) {
            return new X509ExtensionBC(ocspReq.RequestExtensions.GetExtension(
                ((DerObjectIdentifierBC) objectIdentifier).GetDerObjectIdentifier()));
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
            iText.Bouncycastle.Cert.Ocsp.OcspReqBC ocspReqBC = (iText.Bouncycastle.Cert.Ocsp.OcspReqBC)o;
            return Object.Equals(ocspReq, ocspReqBC.ocspReq);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspReq);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return ocspReq.ToString();
        }
    }
}

/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>.
    /// </summary>
    public class OCSPReqBCFips : IOCSPReq {
        private readonly OcspRequest ocspReq;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspRequest"/>.
        /// </summary>
        /// <param name="ocspReq">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspRequest"/>
        /// to be wrapped
        /// </param>
        public OCSPReqBCFips(OcspRequest ocspReq) {
            this.ocspReq = ocspReq;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspRequest"/>.
        /// </summary>
        /// <param name="certId">
        /// CertID wrapper
        /// </param>
        /// <param name="documentId">
        /// byte array
        /// </param>
        public OCSPReqBCFips(ICertificateID certId, byte[] documentId) {
            // create details for nonce extension
            IDictionary extensions = new Hashtable();
            
            extensions[OcspObjectIdentifiers.PkixOcspNonce] = new X509Extension(false, new DerOctetString(
                new DerOctetString(documentId).GetEncoded()));
            X509Extensions x509Extensions = new X509Extensions(extensions);

            Asn1EncodableVector vector = new Asn1EncodableVector();
            vector.Add(new Request(((CertificateIDBCFips) certId).GetCertificateID(), (X509Extensions)null));
            Asn1Sequence requestsList = new DerSequence(vector);
            TbsRequest request = new TbsRequest(null, requestsList, x509Extensions);
            
            ocspReq = new OcspRequest(request, null);
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.OcspRequest"/>.
        /// </returns>
        public virtual OcspRequest GetOcspReq() {
            return ocspReq;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return ocspReq.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public IReq[] GetRequestList() {
            Asn1Sequence reqs = ocspReq.TbsRequest.RequestList;
            IReq[] wrappedReqs = new IReq[reqs.Count];
            for (int i = 0; i < reqs.Count; ++i) {
                wrappedReqs[i] = new ReqBCFips(Request.GetInstance(reqs[i]));
            }
            return wrappedReqs;
        }

        /// <summary><inheritDoc/></summary>
        public IExtension GetExtension(IASN1ObjectIdentifier objectIdentifier) {
            return new ExtensionBCFips(ocspReq.TbsRequest.RequestExtensions.GetExtension(
                ((ASN1ObjectIdentifierBCFips) objectIdentifier).GetASN1ObjectIdentifier()));
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
            iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBCFips)o;
            return Object.Equals(ocspReq, that.ocspReq);
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

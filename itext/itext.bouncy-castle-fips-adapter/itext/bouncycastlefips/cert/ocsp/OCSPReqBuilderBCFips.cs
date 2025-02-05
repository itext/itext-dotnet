/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Bouncycastlefips.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for OCSPReqBuilder"/>.
    /// </summary>
    public class OCSPReqBuilderBCFips : IOcspReqGenerator {
        private IList list = new ArrayList();
        private GeneralName requestorName = null;
        private X509Extensions requestExtensions;

        /// <summary>
        /// Creates new wrapper instance for OCSPReqBuilder.
        /// </summary>
        public OCSPReqBuilderBCFips() {
        }

        /// <summary>
        /// Creates new wrapper instance for OCSPReqBuilder.
        /// </summary>
        /// <param name="list">Requests array</param>
        /// <param name="requestorName">requestor name</param>
        /// <param name="requestExtensions">X509 extensions</param>
        public OCSPReqBuilderBCFips(IList list, GeneralName requestorName, X509Extensions requestExtensions) {
            this.list = list;
            this.requestorName = requestorName;
            this.requestExtensions = requestExtensions;
        }

        /// <summary>Gets list of added requests.</summary>
        /// <returns>Requests list.</returns>
        public virtual IList GetList() {
            return list;
        }

        /// <summary>Gets requestorName field.</summary>
        /// <returns>Requestor name.</returns>
        public virtual GeneralName GetRequestorName() {
            return requestorName;
        }
        
        /// <summary>Gets requestExtensions field.</summary>
        /// <returns>Request Extensions.</returns>
        public virtual X509Extensions GetRequestExtensions() {
            return requestExtensions;
        }
        
        /// <summary><inheritDoc/></summary>
        public virtual IOcspReqGenerator SetRequestExtensions(IX509Extensions extensions) {
            requestExtensions = ((X509ExtensionsBCFips)extensions).GetX509Extensions();
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspReqGenerator AddRequest(ICertID certificateID) {
            list.Add(new Request(((CertIDBCFips)certificateID).GetCertificateID(), null));
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspRequest Build() {
            Asn1EncodableVector requests = new Asn1EncodableVector();
            foreach (Request reqObj in list) {
                try {
                    requests.Add(reqObj);
                } catch (Exception e) {
                    throw new OcspExceptionBCFips("exception creating Request");
                }
            }
            TbsRequest tbsReq = new TbsRequest(requestorName, new DerSequence(requests), requestExtensions);
            return new OcspRequestBCFips(new OcspRequest(tbsReq, null));
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
            iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBuilderBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.OCSPReqBuilderBCFips
                )o;
            return Object.Equals(list, that.list) &&
                   Object.Equals(requestorName, that.requestorName) &&
                   Object.Equals(requestExtensions, that.requestExtensions);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode<object>(list, requestorName, requestExtensions);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return list + " " + requestorName + " " + requestExtensions;
        }
    }
}

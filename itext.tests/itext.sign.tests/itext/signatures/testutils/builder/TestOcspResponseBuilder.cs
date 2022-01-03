/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Security.Certificates;

namespace iText.Signatures.Testutils.Builder {
    public class TestOcspResponseBuilder {
        private const String SIGN_ALG = "SHA256withRSA";

        private BasicOcspRespGenerator responseBuilder;

        private X509Certificate issuerCert;
        private ICipherParameters issuerPrivateKey;

        private CertificateStatus certificateStatus;

        private DateTime thisUpdate = DateTimeUtil.GetCurrentTime();

        private DateTime nextUpdate = DateTimeUtil.GetCurrentTime();

        public TestOcspResponseBuilder(X509Certificate issuerCert, ICipherParameters issuerPrivateKey,
            CertificateStatus certificateStatus)
        {
            this.issuerCert = issuerCert;
            this.issuerPrivateKey = issuerPrivateKey;
            this.certificateStatus = certificateStatus;
            X509Name subjectDN = issuerCert.SubjectDN;
            thisUpdate = thisUpdate.AddDays(-1);
            nextUpdate = nextUpdate.AddDays(30);
            responseBuilder = new BasicOcspRespGenerator(new RespID(subjectDN));
        }

        public TestOcspResponseBuilder(X509Certificate issuerCert, ICipherParameters issuerPrivateKey)
            : this(issuerCert, issuerPrivateKey, CertificateStatus.Good)
        {
        }

        public X509Certificate GetIssuerCert() {
            return issuerCert;
        }

        public virtual void SetCertificateStatus(CertificateStatus certificateStatus) {
            this.certificateStatus = certificateStatus;
        }

        public virtual void SetThisUpdate(DateTime thisUpdate) {
            this.thisUpdate = thisUpdate;
        }

        public virtual void SetNextUpdate(DateTime nextUpdate) {
            this.nextUpdate = nextUpdate;
        }

        public virtual byte[] MakeOcspResponse(byte[] requestBytes) {
            BasicOcspResp ocspResponse = MakeOcspResponseObject(requestBytes);
            return ocspResponse.GetEncoded();
        }
        
        public virtual BasicOcspResp MakeOcspResponseObject(byte[] requestBytes) {
            OcspReq ocspRequest = new OcspReq(requestBytes);
            Req[] requestList = ocspRequest.GetRequestList();

            X509Extension extNonce = ocspRequest.RequestExtensions.GetExtension(OcspObjectIdentifiers.PkixOcspNonce);
            if (extNonce != null) {
                // TODO ensure
                X509Extensions responseExtensions = new X509Extensions(new Dictionary<DerObjectIdentifier, X509Extension>() { { OcspObjectIdentifiers.PkixOcspNonce, extNonce }});
                responseBuilder.SetResponseExtensions(responseExtensions);
            }

            foreach (Req req in requestList) {
                responseBuilder.AddResponse(req.GetCertID(), certificateStatus, thisUpdate.ToUniversalTime(), nextUpdate.ToUniversalTime(), null);
            }
            DateTime time = DateTimeUtil.GetCurrentUtcTime();
            return responseBuilder.Generate(new Asn1SignatureFactory(SIGN_ALG, (AsymmetricKeyParameter)issuerPrivateKey), new X509Certificate[] { issuerCert }, time);
        }
    }
}

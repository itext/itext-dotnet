/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;

namespace iText.Signatures.Testutils.Client {
    public class TestOcspClient : IOcspClient {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private readonly IDictionary<String, TestOcspResponseBuilder> issuerIdToResponseBuilder = new LinkedDictionary
            <String, TestOcspResponseBuilder>();

        public virtual TestOcspClient AddBuilderForCertIssuer(IX509Certificate cert, IPrivateKey privateKey) {
            issuerIdToResponseBuilder.Put(cert.GetSerialNumber().ToString(16), new TestOcspResponseBuilder(cert, privateKey
                ));
            return this;
        }

        public virtual TestOcspClient AddBuilderForCertIssuer(IX509Certificate cert, TestOcspResponseBuilder builder
            ) {
            issuerIdToResponseBuilder.Put(cert.GetSerialNumber().ToString(16), builder);
            return this;
        }

        public virtual byte[] GetEncoded(IX509Certificate checkCert, IX509Certificate issuerCert, String url) {
            byte[] bytes = null;
            try {
                ICertificateID id = SignTestPortUtil.GenerateCertificateId(issuerCert, checkCert.GetSerialNumber(), BOUNCY_CASTLE_FACTORY
                    .CreateCertificateID().GetHashSha1());
                TestOcspResponseBuilder builder = issuerIdToResponseBuilder.Get(issuerCert.GetSerialNumber().ToString(16));
                if (builder == null) {
                    throw new ArgumentException("This TestOcspClient instance is not capable of providing OCSP response for the given issuerCert:"
                         + issuerCert.GetSubjectDN().ToString());
                }
                bytes = builder.MakeOcspResponse(SignTestPortUtil.GenerateOcspRequestWithNonce(id).GetEncoded());
            }
            catch (Exception ignored) {
                if (ignored is Exception) {
                    throw (Exception)ignored;
                }
            }
            return bytes;
        }
    }
}

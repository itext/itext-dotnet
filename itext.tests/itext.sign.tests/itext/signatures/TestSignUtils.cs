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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Pdf;

namespace iText.Signatures {
    public sealed class TestSignUtils {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        public static void AssertDssDict(Stream inputStream, IDictionary<String, int?> expectedNumberOfCrls, IDictionary
            <String, int?> expectedNumberOfOcsp) {
            using (PdfDocument outDocument = new PdfDocument(new PdfReader(inputStream))) {
                PdfDictionary dss = outDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
                PdfArray crls = dss.GetAsArray(PdfName.CRLs) == null ? new PdfArray() : dss.GetAsArray(PdfName.CRLs);
                IDictionary<String, int?> realNumberOfCrls = CreateCrlMap(crls);
                PdfArray ocsps = dss.GetAsArray(PdfName.OCSPs) == null ? new PdfArray() : dss.GetAsArray(PdfName.OCSPs);
                IDictionary<String, int?> realNumberOfOcsp = CreateOcspMap(ocsps);
                NUnit.Framework.Assert.IsTrue(MapUtil.Equals(expectedNumberOfCrls, realNumberOfCrls), "CRLs entry in DSS dictionary isn't correct"
                    );
                NUnit.Framework.Assert.IsTrue(MapUtil.Equals(expectedNumberOfOcsp, realNumberOfOcsp), "OCSPs entry in DSS dictionary isn't correct"
                    );
            }
        }

        public static void BasicCheckSignedDoc(String filePath, String signatureName) {
            using (Stream inputStream = FileUtil.GetInputStreamForFile(filePath)) {
                BasicCheckSignedDoc(inputStream, signatureName);
            }
        }

        public static void BasicCheckSignedDoc(Stream inputStream, String signatureName) {
            using (PdfDocument outDocument = new PdfDocument(new PdfReader(inputStream))) {
                SignatureUtil sigUtil = new SignatureUtil(outDocument);
                PdfPKCS7 signatureData = sigUtil.ReadSignatureData(signatureName);
                NUnit.Framework.Assert.IsTrue(signatureData.VerifySignatureIntegrityAndAuthenticity());
            }
        }

        public static void SignedDocumentContainsCerts(Stream inputStream, IList<IX509Certificate> expectedCertificates
            ) {
            using (PdfDocument outDocument = new PdfDocument(new PdfReader(inputStream))) {
                SignatureUtil sigUtil = new SignatureUtil(outDocument);
                IList<IX509Certificate> actualCertificates = JavaUtil.ArraysAsList(sigUtil.ReadSignatureData("Signature1")
                    .GetCertificates());
                // Searching for every certificate we expect should be in the resulting document.
                NUnit.Framework.Assert.AreEqual(expectedCertificates.Count, actualCertificates.Count);
                foreach (IX509Certificate expectedCert in expectedCertificates) {
                    NUnit.Framework.Assert.IsTrue(actualCertificates.Any((cert) => ((IX509Certificate)cert).GetSubjectDN().Equals
                        (expectedCert.GetSubjectDN())));
                }
            }
        }

        private static IDictionary<String, int?> CreateCrlMap(PdfArray crls) {
            IDictionary<String, int?> realNumberOfCrls = new Dictionary<String, int?>();
            foreach (PdfObject crl in crls) {
                PdfStream crlStream = (PdfStream)crl;
                byte[] crlBytes = crlStream.GetBytes(true);
                IX509Crl crlObj = (IX509Crl)SignUtils.ParseCrlFromStream(new MemoryStream(crlBytes));
                String x500Principal = crlObj.GetIssuerDN().ToString();
                int? currentAmount = realNumberOfCrls.Get(x500Principal) == null ? 0 : realNumberOfCrls.Get(x500Principal);
                realNumberOfCrls.Put(x500Principal, currentAmount + 1);
            }
            return realNumberOfCrls;
        }

        private static IDictionary<String, int?> CreateOcspMap(PdfArray ocsps) {
            IDictionary<String, int?> realNumberOfOcsp = new Dictionary<String, int?>();
            foreach (PdfObject ocsp in ocsps) {
                PdfStream ocspStream = (PdfStream)ocsp;
                byte[] ocspBytes = ocspStream.GetBytes(true);
                IOcspResponse ocspResp = FACTORY.CreateOCSPResponse(ocspBytes);
                IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(ocspResp.GetResponseObject());
                IEnumerable<IX509Certificate> certs = SignUtils.GetCertsFromOcspResponse(basicOCSPResp);
                foreach (IX509Certificate cert in certs) {
                    String x500Principal = cert.GetSubjectDN().ToString();
                    int? currentAmount = realNumberOfOcsp.Get(x500Principal) == null ? 0 : realNumberOfOcsp.Get(x500Principal);
                    realNumberOfOcsp.Put(x500Principal, currentAmount + 1);
                }
            }
            return realNumberOfOcsp;
        }
    }
}

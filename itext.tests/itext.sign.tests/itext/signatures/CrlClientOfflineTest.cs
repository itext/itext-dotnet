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
using System.Collections.Generic;
using Org.BouncyCastle.X509;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures {
    [NUnit.Framework.Category("UnitTest")]
    public class CrlClientOfflineTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/CrlClientOfflineTest/";

        private static readonly char[] PASSWORD = "password".ToCharArray();

        private const String CRL_DISTRIBUTION_POINT = "http://www.example.com/";

        private static X509Certificate checkCert;

        private static ICollection<byte[]> listOfByteArrays;

        [NUnit.Framework.Test]
        public virtual void CheckUnknownPdfExceptionWhenCrlIsNull() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => listOfByteArrays = new CrlClientOffline
                ((X509Crl)null).GetEncoded(null, ""));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNKNOWN_PDF_EXCEPTION, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlEmptyByteArrayRealArgsTest() {
            ValidateCrlBytes(null, checkCert, CRL_DISTRIBUTION_POINT);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlEmptyByteArrayWithoutArgsTest() {
            ValidateCrlBytes(null, null, "");
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlEmptyByteArrayUrlIsEmptyTest() {
            ValidateCrlBytes(null, checkCert, "");
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlEmptyByteArrayNonExistingUrlTest() {
            ValidateCrlBytes(null, checkCert, "http://nonexistingurl.com");
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlEmptyByteArrayCertIsNullNonExistingUrlTest() {
            ValidateCrlBytes(null, null, "http://nonexistingurl.com");
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlEmptyByteArrayCertIsNullUrlIsRealTest() {
            ValidateCrlBytes(null, null, CRL_DISTRIBUTION_POINT);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlObjectRealArgsTest() {
            String fileName = SOURCE_FOLDER + "pdfWithCrl.pdf";
            byte[] testBytes = ObtainCrlFromPdf(fileName);
            ValidateCrlBytes(testBytes, checkCert, CRL_DISTRIBUTION_POINT);
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlObjectWithoutCertAndUrlTest() {
            String fileName = SOURCE_FOLDER + "pdfWithCrl.pdf";
            byte[] testBytes = ObtainCrlFromPdf(fileName);
            ValidateCrlBytes(testBytes, null, "");
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlObjectUrlIsEmptyTest() {
            String fileName = SOURCE_FOLDER + "pdfWithCrl.pdf";
            byte[] testBytes = ObtainCrlFromPdf(fileName);
            ValidateCrlBytes(testBytes, checkCert, "");
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlObjectNonExistingUrlTest() {
            String fileName = SOURCE_FOLDER + "pdfWithCrl.pdf";
            byte[] testBytes = ObtainCrlFromPdf(fileName);
            ValidateCrlBytes(testBytes, checkCert, "http://nonexistingurl.com");
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlObjectCertIsNullNonExistingUrlTest() {
            String fileName = SOURCE_FOLDER + "pdfWithCrl.pdf";
            byte[] testBytes = ObtainCrlFromPdf(fileName);
            ValidateCrlBytes(testBytes, null, "http://nonexistingurl.com");
        }

        [NUnit.Framework.Test]
        public virtual void GetEncodedFromCrlObjectCertIsNullUrlIsRealTest() {
            String fileName = SOURCE_FOLDER + "pdfWithCrl.pdf";
            byte[] testBytes = ObtainCrlFromPdf(fileName);
            ValidateCrlBytes(testBytes, null, CRL_DISTRIBUTION_POINT);
        }

        //Get CRL from PDF. We expect the PDF to contain an array of CRLs from which we only take the first
        private static byte[] ObtainCrlFromPdf(String fileName) {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(fileName));
            PdfDictionary pdfDictionary = pdfDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            PdfArray crlArray = pdfDictionary.GetAsArray(PdfName.CRLs);
            PdfStream stream = crlArray.GetAsStream(0);
            return stream.GetBytes();
        }

        private static ICollection<byte[]> ValidateCrlBytes(byte[] testBytes, X509Certificate checkCert, String crlDistPoint
            ) {
            CrlClientOffline crlClientOffline = new CrlClientOffline(testBytes);
            checkCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(SOURCE_FOLDER + "crlDistPoint.p12", PASSWORD)
                [0];
            listOfByteArrays = crlClientOffline.GetEncoded(checkCert, crlDistPoint);
            //These checks are enough, because there is exactly one element in the collection,
            //and these are the same test bytes 
            NUnit.Framework.Assert.AreEqual(1, listOfByteArrays.Count);
            NUnit.Framework.Assert.IsTrue(listOfByteArrays.Contains(testBytes));
            return listOfByteArrays;
        }
    }
}

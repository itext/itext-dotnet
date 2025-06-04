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
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("UnitTest")]
    public class XmlCertificateRetrieverTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/XmlCertificateRetrieverTest/";

        [NUnit.Framework.Test]
        public virtual void ReadSingleCertificateTest() {
            String xmlPath = SOURCE_FOLDER + "certificate.xml";
            String certPath = SOURCE_FOLDER + "certificate.pem";
            IX509Certificate actualCertificate = XmlCertificateRetriever.GetCertificates(xmlPath)[0];
            IX509Certificate expectedCertificate = PemFileHelper.ReadFirstChain(certPath)[0];
            NUnit.Framework.Assert.AreEqual(expectedCertificate, actualCertificate);
        }

        [NUnit.Framework.Test]
        public virtual void ReadLotlCertificatesTest() {
            String xmlPath = SOURCE_FOLDER + "eu-lotl.xml";
            IList<IX509Certificate> certificateList = XmlCertificateRetriever.GetCertificates(xmlPath);
            NUnit.Framework.Assert.AreEqual(142, certificateList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ReadPivotCertificatesTest() {
            String xmlPath = SOURCE_FOLDER + "eu-lotl-pivot-282.xml";
            IList<IX509Certificate> certificateList = XmlCertificateRetriever.GetCertificates(xmlPath);
            NUnit.Framework.Assert.AreEqual(126, certificateList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ReadAustriaCertificatesTest() {
            String xmlPath = SOURCE_FOLDER + "ttlAustria.xml";
            IList<IX509Certificate> certificateList = XmlCertificateRetriever.GetCertificates(xmlPath);
            NUnit.Framework.Assert.AreEqual(103, certificateList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyXmlTest() {
            String xmlPath = SOURCE_FOLDER + "emptyXml.xml";
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => XmlCertificateRetriever.GetCertificates
                (xmlPath));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignExceptionMessageConstant.FAILED_TO_READ_CERTIFICATE_BYTES_FROM_XML
                , xmlPath), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidCertificateTest() {
            String xmlPath = SOURCE_FOLDER + "invalidCertificate.xml";
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => XmlCertificateRetriever.GetCertificates
                (xmlPath));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.FAILED_TO_RETRIEVE_CERTIFICATE, exception.Message
                );
        }
    }
}

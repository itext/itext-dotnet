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
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("UnitTest")]
    public class XmlCertificateRetrieverTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation" + "/XmlCertificateRetrieverTest/";

        [NUnit.Framework.Test]
        public virtual void ReadSingleCertificateTest() {
            String xmlPath = SOURCE_FOLDER + "certificate.xml";
            String certPath = SOURCE_FOLDER + "certificate.pem";
            IX509Certificate actualCertificate = new XmlCertificateRetriever(new XmlDefaultCertificateHandler()).GetCertificates
                (iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(xmlPath)))[0];
            IX509Certificate expectedCertificate = PemFileHelper.ReadFirstChain(certPath)[0];
            NUnit.Framework.Assert.AreEqual(expectedCertificate, actualCertificate);
        }

        [NUnit.Framework.Test]
        public virtual void ReadLotlCertificatesTest() {
            String xmlPath = SOURCE_FOLDER + "eu-lotl.xml";
            IList<IX509Certificate> certificateList = new XmlCertificateRetriever(new XmlDefaultCertificateHandler()).
                GetCertificates(iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(xmlPath)));
            NUnit.Framework.Assert.AreEqual(142, certificateList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ReadPivotCertificatesTest() {
            String xmlPath = SOURCE_FOLDER + "eu-lotl-pivot-282.xml";
            XmlCertificateRetriever xmlCertificateRetriever = new XmlCertificateRetriever(new XmlDefaultCertificateHandler
                ());
            IList<IX509Certificate> certificateList = xmlCertificateRetriever.GetCertificates(iText.Commons.Utils.FileUtil.GetInputStreamForFile
                (System.IO.Path.Combine(xmlPath)));
            NUnit.Framework.Assert.AreEqual(126, certificateList.Count);
            IServiceContext context = xmlCertificateRetriever.GetServiceContexts()[0];
            NUnit.Framework.Assert.IsNotNull(context);
            NUnit.Framework.Assert.IsTrue(context is SimpleServiceContext);
        }

        [NUnit.Framework.Test]
        public virtual void ReadAustriaCertificatesTest() {
            String xmlPath = SOURCE_FOLDER + "austriaTrustedList.xml";
            IList<IX509Certificate> certificateList = new XmlCertificateRetriever(new XmlDefaultCertificateHandler()).
                GetCertificates(iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(xmlPath)));
            NUnit.Framework.Assert.AreEqual(104, certificateList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ReadBulgariaCertificatesTest() {
            String xmlPath = SOURCE_FOLDER + "BulgariaTrustedList.xml";
            XmlCertificateRetriever xmlCertificateRetriever = new XmlCertificateRetriever(new XmlCountryCertificateHandler
                ());
            IList<IX509Certificate> certificateList = xmlCertificateRetriever.GetCertificates(iText.Commons.Utils.FileUtil.GetInputStreamForFile
                (System.IO.Path.Combine(xmlPath)));
            NUnit.Framework.Assert.AreEqual(104, certificateList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ReadCzechiaCertificatesTest() {
            String xmlPath = SOURCE_FOLDER + "CzechiaTrustedList.txt";
            XmlCertificateRetriever xmlCertificateRetriever = new XmlCertificateRetriever(new XmlCountryCertificateHandler
                ());
            IList<IX509Certificate> certificateList = xmlCertificateRetriever.GetCertificates(iText.Commons.Utils.FileUtil.GetInputStreamForFile
                (System.IO.Path.Combine(xmlPath)));
            NUnit.Framework.Assert.AreEqual(441, certificateList.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ReadCyrpusCertificateContextTest() {
            String xmlPath = SOURCE_FOLDER + "cyprusTrustedList.xml";
            XmlCertificateRetriever xmlCertificateRetriever = new XmlCertificateRetriever(new XmlCountryCertificateHandler
                ());
            IList<IX509Certificate> certificateList = xmlCertificateRetriever.GetCertificates(iText.Commons.Utils.FileUtil.GetInputStreamForFile
                (System.IO.Path.Combine(xmlPath)));
            NUnit.Framework.Assert.AreEqual(8, certificateList.Count);
            CountryServiceContext serviceContext = (CountryServiceContext)xmlCertificateRetriever.GetServiceContexts()
                [0];
            NUnit.Framework.Assert.AreEqual(2, serviceContext.GetServiceStatusInfosSize());
            NUnit.Framework.Assert.AreEqual("http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/withdrawn", serviceContext
                .GetCurrentStatusInfo().GetServiceStatus());
            NUnit.Framework.Assert.AreEqual("http://uri.etsi.org/TrstSvc/Svctype/CA/QC", serviceContext.GetServiceType
                ());
            NUnit.Framework.Assert.AreEqual(iText.Commons.Utils.DateTimeUtil.CreateDateTime(2021, 12, 16, 6, 0, 18), serviceContext
                .GetCurrentStatusInfo().GetServiceStatusStartingTime());
        }

        [NUnit.Framework.Test]
        public virtual void ReadEstoniaCertificateContextTest() {
            String xmlPath = SOURCE_FOLDER + "estoniaTrustedList.xml";
            XmlCertificateRetriever xmlCertificateRetriever = new XmlCertificateRetriever(new XmlCountryCertificateHandler
                ());
            IList<IX509Certificate> certificateList = xmlCertificateRetriever.GetCertificates(iText.Commons.Utils.FileUtil.GetInputStreamForFile
                (System.IO.Path.Combine(xmlPath)));
            NUnit.Framework.Assert.AreEqual(64, certificateList.Count);
            CountryServiceContext serviceContext = (CountryServiceContext)xmlCertificateRetriever.GetServiceContexts()
                [0];
            NUnit.Framework.Assert.AreEqual(3, serviceContext.GetServiceStatusInfosSize());
            NUnit.Framework.Assert.AreEqual("http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/withdrawn", serviceContext
                .GetCurrentStatusInfo().GetServiceStatus());
            NUnit.Framework.Assert.AreEqual("http://uri.etsi.org/TrstSvc/Svctype/CA/QC", serviceContext.GetServiceType
                ());
            NUnit.Framework.Assert.AreEqual(iText.Commons.Utils.DateTimeUtil.CreateDateTime(2017, 6, 30, 22, 0), serviceContext
                .GetCurrentStatusInfo().GetServiceStatusStartingTime());
            DateTime previousStatusTime = iText.Commons.Utils.DateTimeUtil.CreateDateTime(2016, 6, 30, 22, 0);
            String previousStatus = serviceContext.GetServiceStatusByDate(previousStatusTime);
            NUnit.Framework.Assert.AreEqual("http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/undersupervision", previousStatus
                );
        }

        [NUnit.Framework.Test]
        public virtual void ReadHungaryCertificateContextTest() {
            String xmlPath = SOURCE_FOLDER + "HungaryTrustedList.xml";
            XmlCertificateRetriever xmlCertificateRetriever = new XmlCertificateRetriever(new XmlCountryCertificateHandler
                ());
            IList<IX509Certificate> certificateList = xmlCertificateRetriever.GetCertificates(iText.Commons.Utils.FileUtil.GetInputStreamForFile
                (System.IO.Path.Combine(xmlPath)));
            NUnit.Framework.Assert.AreEqual(346, certificateList.Count);
            CountryServiceContext serviceContext = (CountryServiceContext)xmlCertificateRetriever.GetServiceContexts()
                [0];
            NUnit.Framework.Assert.AreEqual(3, serviceContext.GetServiceStatusInfosSize());
            NUnit.Framework.Assert.AreEqual("http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/withdrawn", serviceContext
                .GetCurrentStatusInfo().GetServiceStatus());
            NUnit.Framework.Assert.AreEqual("http://uri.etsi.org/TrstSvc/Svctype/CA/QC", serviceContext.GetServiceType
                ());
            NUnit.Framework.Assert.AreEqual(iText.Commons.Utils.DateTimeUtil.CreateDateTime(2016, 6, 30, 22, 0), serviceContext
                .GetCurrentStatusInfo().GetServiceStatusStartingTime());
        }

        [NUnit.Framework.Test]
        public virtual void EmptyXmlTest() {
            String xmlPath = SOURCE_FOLDER + "emptyXml.xml";
            XmlCertificateRetriever certificateRetriever = new XmlCertificateRetriever(new XmlDefaultCertificateHandler
                ());
            //No checking for message as it is different for C# and java because of differences in library
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => certificateRetriever.GetCertificates(iText.Commons.Utils.FileUtil.GetInputStreamForFile
                (System.IO.Path.Combine(xmlPath))));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidCertificateTest() {
            String xmlPath = SOURCE_FOLDER + "invalidCertificate.xml";
            XmlCertificateRetriever certificateRetriever = new XmlCertificateRetriever(new XmlDefaultCertificateHandler
                ());
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => certificateRetriever.GetCertificates
                (iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(xmlPath))));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.FAILED_TO_RETRIEVE_CERTIFICATE, exception.Message
                );
        }
    }
}

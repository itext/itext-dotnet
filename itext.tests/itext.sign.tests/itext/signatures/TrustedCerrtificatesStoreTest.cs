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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Signatures.Validation;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class TrustedCerrtificatesStoreTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] KEY_PASSWORD = "testpassphrase".ToCharArray();

        private static IX509Certificate crlCert;

        private static IX509Certificate crlRootCert;

        private static IX509Certificate intermediateCert;

        private static IX509Certificate ocspCert;

        private static IX509Certificate rootCert;

        private static IX509Certificate signCert;

        private static IX509Certificate tsaCert;

        private static IX509Certificate tsaRootCert;

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpOnce() {
            crlCert = (IX509Certificate)PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "crlCert.pem")[0];
            crlRootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "crlRoot.pem")[0];
            intermediateCert = (IX509Certificate)PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "intermediate.pem")[0];
            ocspCert = (IX509Certificate)PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "ocspCert.pem")[0];
            rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "root.pem")[0];
            signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "sign.pem")[0];
            tsaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "tsaCert.pem")[0];
            tsaRootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "tsaRoot.pem")[0];
        }

        [NUnit.Framework.Test]
        public virtual void TestIsCertificateGenerallyTrusted() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            NUnit.Framework.Assert.IsFalse(sut.IsCertificateGenerallyTrusted(rootCert));
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            NUnit.Framework.Assert.IsTrue(sut.IsCertificateGenerallyTrusted(rootCert));
        }

        [NUnit.Framework.Test]
        public virtual void TestIsCertificateTrustedForCA() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            NUnit.Framework.Assert.IsFalse(sut.IsCertificateTrustedForCA(rootCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            NUnit.Framework.Assert.IsTrue(sut.IsCertificateTrustedForCA(rootCert));
        }

        [NUnit.Framework.Test]
        public virtual void TestIsCertificateTrustedForTimestamp() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            NUnit.Framework.Assert.IsFalse(sut.IsCertificateTrustedForTimestamp(rootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            NUnit.Framework.Assert.IsTrue(sut.IsCertificateTrustedForTimestamp(rootCert));
        }

        [NUnit.Framework.Test]
        public virtual void TestIsCertificateTrustedForOcsp() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            NUnit.Framework.Assert.IsFalse(sut.IsCertificateTrustedForOcsp(rootCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            NUnit.Framework.Assert.IsTrue(sut.IsCertificateTrustedForOcsp(rootCert));
        }

        [NUnit.Framework.Test]
        public virtual void TestIsCertificateTrustedForCrl() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            NUnit.Framework.Assert.IsFalse(sut.IsCertificateTrustedForCrl(rootCert));
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            NUnit.Framework.Assert.IsTrue(sut.IsCertificateTrustedForCrl(rootCert));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetKnownCertificates() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(crlCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(ocspCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaCert));
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaRootCert));
            NUnit.Framework.Assert.AreEqual(1, sut.GetKnownCertificates(crlCert.GetSubjectDN().ToString()).Count);
            NUnit.Framework.Assert.AreEqual(1, sut.GetKnownCertificates(rootCert.GetSubjectDN().ToString()).Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestGetAllTrustedCertificates() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaRootCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaRootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaCert));
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaRootCert));
            //duplicates should be removed
            NUnit.Framework.Assert.AreEqual(3, sut.GetAllTrustedCertificates().Count);
            NUnit.Framework.Assert.IsTrue(sut.GetAllTrustedCertificates().Contains(tsaRootCert));
            NUnit.Framework.Assert.IsTrue(sut.GetAllTrustedCertificates().Contains(rootCert));
            NUnit.Framework.Assert.IsTrue(sut.GetAllTrustedCertificates().Contains(tsaCert));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetAllTrustedCertificatesByName() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaRootCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaRootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaCert));
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaRootCert));
            //duplicates should be removed
            NUnit.Framework.Assert.AreEqual(1, sut.GetAllTrustedCertificates(tsaRootCert.GetSubjectDN().ToString()).Count
                );
            NUnit.Framework.Assert.IsTrue(sut.GetAllTrustedCertificates(tsaRootCert.GetSubjectDN().ToString()).Contains
                (tsaRootCert));
            NUnit.Framework.Assert.IsTrue(sut.GetAllTrustedCertificates(rootCert.GetSubjectDN().ToString()).Contains(rootCert
                ));
            NUnit.Framework.Assert.IsTrue(sut.GetAllTrustedCertificates(tsaCert.GetSubjectDN().ToString()).Contains(tsaCert
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetGenerallyTrustedCertificates() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(signCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(ocspCert));
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(crlCert));
            String name = signCert.GetSubjectDN().ToString();
            NUnit.Framework.Assert.AreEqual(1, sut.GetGenerallyTrustedCertificates(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForCA(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForCrl(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForOcsp(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForTimestamp(name).Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestGetCertificatesTrustedForCA() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(signCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(ocspCert));
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(crlCert));
            String name = rootCert.GetSubjectDN().ToString();
            NUnit.Framework.Assert.AreEqual(0, sut.GetGenerallyTrustedCertificates(name).Count);
            NUnit.Framework.Assert.AreEqual(1, sut.GetCertificatesTrustedForCA(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForCrl(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForOcsp(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForTimestamp(name).Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestGetCertificatesTrustedForTimeStamp() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(signCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(ocspCert));
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(crlCert));
            String name = tsaCert.GetSubjectDN().ToString();
            NUnit.Framework.Assert.AreEqual(0, sut.GetGenerallyTrustedCertificates(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForCA(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForCrl(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForOcsp(name).Count);
            NUnit.Framework.Assert.AreEqual(1, sut.GetCertificatesTrustedForTimestamp(name).Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestGetCertificatesTrustedForOcsp() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(signCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(ocspCert));
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(crlCert));
            String name = ocspCert.GetSubjectDN().ToString();
            NUnit.Framework.Assert.AreEqual(0, sut.GetGenerallyTrustedCertificates(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForCA(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForCrl(name).Count);
            NUnit.Framework.Assert.AreEqual(1, sut.GetCertificatesTrustedForOcsp(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForTimestamp(name).Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestGetCertificatesTrustedForCrl() {
            TrustedCertificatesStore sut = new TrustedCertificatesStore();
            sut.AddGenerallyTrustedCertificates(JavaCollectionsUtil.SingletonList(signCert));
            sut.AddCATrustedCertificates(JavaCollectionsUtil.SingletonList(rootCert));
            sut.AddTimestampTrustedCertificates(JavaCollectionsUtil.SingletonList(tsaCert));
            sut.AddOcspTrustedCertificates(JavaCollectionsUtil.SingletonList(ocspCert));
            sut.AddCrlTrustedCertificates(JavaCollectionsUtil.SingletonList(crlCert));
            String name = crlCert.GetSubjectDN().ToString();
            NUnit.Framework.Assert.AreEqual(0, sut.GetGenerallyTrustedCertificates(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForCA(name).Count);
            NUnit.Framework.Assert.AreEqual(1, sut.GetCertificatesTrustedForCrl(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForOcsp(name).Count);
            NUnit.Framework.Assert.AreEqual(0, sut.GetCertificatesTrustedForTimestamp(name).Count);
        }
    }
}

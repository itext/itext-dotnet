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
using System.Linq;
using iText.Commons.Actions;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Cms;
using iText.Signatures.Testutils;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Events;
using iText.Test;

namespace iText.Signatures.Validation.Report.Pades {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class PAdESLevelReportGeneratorTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private PAdESLevelReportGenerator sut;

        private EventManager eventManager;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            ValidatorChainBuilder builder = new ValidatorChainBuilder();
            sut = new PAdESLevelReportGenerator();
            builder.WithPAdESLevelReportGenerator(sut);
            eventManager = builder.GetEventManager();
        }

        [NUnit.Framework.Test]
        public virtual void TestB_BHappyPath() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_B_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_B, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_B, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestB_THappyPath() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_T_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            IX509Certificate[] chain = PemFileHelper.ReadFirstChain(certsSrc + "signCertRsa01.pem");
            @event = new RevocationNotFromDssEvent((IX509Certificate)chain[0]);
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestB_LTHappyPath() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.L_T_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LT, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LT, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestB_LTAHappyPath() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestB_LTADSSMissingPath() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            IX509Certificate[] chain = PemFileHelper.ReadFirstChain(certsSrc + "signCertRsa01.pem");
            @event = new RevocationNotFromDssEvent((IX509Certificate)chain[0]);
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_LT).
                Any((nc) => nc.Contains(AbstractPadesLevelRequirements.REVOCATION_DATA_FOR_THESE_CERTIFICATES_IS_MISSING
                )));
        }

        [NUnit.Framework.Test]
        public virtual void TestCMSContainsSigningDate() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.WITH_CMS_SIGNING_TIME_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((nc) => AbstractPadesLevelRequirements.CLAIMED_TIME_OF_SIGNING_SHALL_NOT_BE_INCLUDED_IN_THE_CMS.Equals
                (nc)));
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestSingleSignatureMissingCMSCerts() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(CMSTestHelper.SERIALIZED_B64_MISSING_CERTIFICATES
                ));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((nc) => AbstractPadesLevelRequirements.SIGNED_DATA_CERTIFICATES_MUST_BE_INCLUDED.Equals(nc)));
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestCMSContainsMissingContentType() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.CMS_MISSING_CONTENT_TYPE_B64)
                );
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((nc) => AbstractPadesLevelRequirements.CMS_CONTENT_TYPE_MUST_BE_ID_DATA.Equals(nc)));
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestCMSContainsWrongContentType() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.WRONG_CONTENT_TYPE));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((nc) => AbstractPadesLevelRequirements.CMS_CONTENT_TYPE_MUST_BE_ID_DATA.Equals(nc)));
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestCMSMissingMessageDigest() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.MISSING_MESSAGE_DIGEST));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((nc) => AbstractPadesLevelRequirements.CMS_MESSAGE_DIGEST_IS_MISSING.Equals(nc)));
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestCMSCommitmentTypeAndDictReasonArePresent() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.CONTAINING_COMMITMENT_INDICATION
                ));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            signatureDict.Put(PdfName.Reason, new PdfString("Reason"));
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((nc) => AbstractPadesLevelRequirements.COMMITMENT_TYPE_AND_REASON_SHALL_NOT_BE_USED_TOGETHER.Equals(nc
                )));
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestOnlyDictReasonIsPresent() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            signatureDict.Put(PdfName.Reason, new PdfString("Reason"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestCMSCommitmentTypeIsPresent() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.CONTAINING_COMMITMENT_INDICATION
                ));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestCMSMissingSigningCertificate() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.MISSING_SIGNER_CERT_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((nc) => AbstractPadesLevelRequirements.SIGNED_DATA_CERTIFICATES_MUST_INCLUDE_SIGNING_CERTIFICATE.Equals
                (nc)));
        }

        [NUnit.Framework.Test]
        public virtual void TestMissingClaimedTimeOfSinging() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((nc) => AbstractPadesLevelRequirements.DICTIONARY_ENTRY_M_IS_MISSING.Equals(nc)));
        }

        [NUnit.Framework.Test]
        public virtual void TestCMSContainsSigningTime() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.WITH_SIGNING_TIME_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((nc) => AbstractPadesLevelRequirements.CLAIMED_TIME_OF_SIGNING_SHALL_NOT_BE_INCLUDED_IN_THE_CMS.Equals
                (nc)));
        }

        [NUnit.Framework.Test]
        public virtual void TestDictionaryContainsSignerCert() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            signatureDict.Put(PdfName.Cert, new PdfString(""));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((nc) => AbstractPadesLevelRequirements.CERT_ENTRY_IS_ADDED_TO_THE_SIGNATURE_DICTIONARY.Equals(nc)));
        }

        [NUnit.Framework.Test]
        public virtual void TestB_TTrustedTimeIsMissing() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.NO_TIMESTAMP_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_B, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_T).Any
                ((nc) => AbstractPadesLevelRequirements.THERE_MUST_BE_A_SIGNATURE_OR_DOCUMENT_TIMESTAMP_AVAILABLE.Equals
                (nc)));
        }

        [NUnit.Framework.Test]
        public virtual void TestCMSB_TPoEFromSignature() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LT, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestCMSPoEFromDocTimeStamp() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.NO_TIMESTAMP_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidSignature() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_B_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationFailureEvent(true, "test");
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.INDETERMINATE, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.INDETERMINATE, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void TestB_LTA_DSSMissing_Certs() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            IX509Certificate[] chain = PemFileHelper.ReadFirstChain(certsSrc + "signCertRsa01.pem");
            @event = new CertificateIssuerExternalRetrievalEvent((IX509Certificate)chain[0]);
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_LT).
                Any((nc) => nc.Contains(AbstractPadesLevelRequirements.ISSUER_FOR_THESE_CERTIFICATES_IS_MISSING)));
        }

        [NUnit.Framework.Test]
        public virtual void TestB_LTA_DSSMissing_CertsTest2() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            IX509Certificate[] chain = PemFileHelper.ReadFirstChain(certsSrc + "signCertRsa01.pem");
            @event = new CertificateIssuerRetrievedOutsideDSSEvent((IX509Certificate)chain[0]);
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetWarnings().Get(PAdESLevel.B_LT).Any((nc
                ) => nc.Contains(AbstractPadesLevelRequirements.ISSUER_FOR_THESE_CERTIFICATES_IS_NOT_IN_DSS)));
        }

        [NUnit.Framework.Test]
        public virtual void TestB_DSSMissingRevData() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            IX509Certificate[] chain = PemFileHelper.ReadFirstChain(certsSrc + "signCertRsa01.pem");
            @event = new RevocationNotFromDssEvent((IX509Certificate)chain[0]);
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_LT).
                Any((nc) => nc.Contains(AbstractPadesLevelRequirements.REVOCATION_DATA_FOR_THESE_CERTIFICATES_IS_MISSING
                )));
        }

        [NUnit.Framework.Test]
        public virtual void TestB_DSSMissingTimestampedRevData() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            IX509Certificate[] chain = PemFileHelper.ReadFirstChain(certsSrc + "signCertRsa01.pem");
            @event = new DssNotTimestampedEvent((IX509Certificate)chain[0]);
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LT, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LT, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_LTA)
                .Any((nc) => nc.Contains(AbstractPadesLevelRequirements.REVOCATION_DATA_FOR_THESE_CERTIFICATES_NOT_TIMESTAMPED
                )));
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidAlgorithmUsed() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_B_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            eventManager.OnEvent(new AlgorithmUsageEvent("MD5", "1.2.840.113549.2.5", "HASH x"));
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((nc) => nc.Contains(AbstractPadesLevelRequirements.A_FORBIDDEN_HASH_OR_SIGNING_ALGORITHM_WAS_USED) &&
                 nc.Contains("1.2.840.113549.2.5")));
        }

        [NUnit.Framework.Test]
        public virtual void TestTimestampWrongSubFilter() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            timestampDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LT, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LT, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_LTA)
                .Any((nc) => nc.Contains(DocumentTimestampRequirements.SUBFILTER_NOT_ETSI_RFC3161)));
        }

        [NUnit.Framework.Test]
        public virtual void TestAlgorithmReportingPositive() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            eventManager.OnEvent(new AlgorithmUsageEvent("SHA-512", OID.SHA_512, SignatureValidator.VALIDATING_SIGNATURE_NAME
                ));
            eventManager.OnEvent(new AlgorithmUsageEvent("SHA-256", OID.SHA_256, SignatureValidator.VALIDATING_SIGNATURE_NAME
                ));
            eventManager.OnEvent(new AlgorithmUsageEvent("RSA", OID.RSA, SignatureValidator.VALIDATING_SIGNATURE_NAME)
                );
            eventManager.OnEvent(new AlgorithmUsageEvent("ECDSA", OID.ECDSA, SignatureValidator.VALIDATING_SIGNATURE_NAME
                ));
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsFalse(report.GetSignatureReport("test").GetWarnings().Get(PAdESLevel.B_B).Any((m) =>
                 m.Contains(AbstractPadesLevelRequirements.A_DISCOURAGED_HASH_OR_SIGNING_ALGORITHM_WAS_USED)));
        }

        [NUnit.Framework.Test]
        public virtual void TestAlgorithmReportingDiscouraged() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            eventManager.OnEvent(new AlgorithmUsageEvent("SHA-1", "1.3.14.3.2.26", SignatureValidator.VALIDATING_SIGNATURE_NAME
                ));
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetWarnings().Get(PAdESLevel.B_B).Any((m) =>
                 m.Contains(AbstractPadesLevelRequirements.A_DISCOURAGED_HASH_OR_SIGNING_ALGORITHM_WAS_USED) && m.Contains
                ("SHA-1")));
        }

        [NUnit.Framework.Test]
        public virtual void TestAlgorithmReportingForbidden() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            eventManager.OnEvent(new AlgorithmUsageEvent("MD5", OID.MD5, SignatureValidator.VALIDATING_SIGNATURE_NAME)
                );
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.NONE, report.GetDocumentLevel());
            NUnit.Framework.Assert.IsTrue(report.GetSignatureReport("test").GetNonConformaties().Get(PAdESLevel.B_B).Any
                ((m) => m.Contains(AbstractPadesLevelRequirements.A_FORBIDDEN_HASH_OR_SIGNING_ALGORITHM_WAS_USED) && m
                .Contains(OID.MD5)));
        }

        [NUnit.Framework.Test]
        public virtual void SignatureValidationSuccessEventMisfiresTest() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void SignatureValidationFailureEventMisfiresTest() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            eventManager.OnEvent(new SignatureValidationFailureEvent(true, "test"));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void CertificateIssuerRetrievedOutsideDSSEventMisfiresTest() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            eventManager.OnEvent(new CertificateIssuerRetrievedOutsideDSSEvent(new X509MockCertificate()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void CertificateIssuerExternalRetrievalEventMisfiresTest() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            eventManager.OnEvent(new CertificateIssuerExternalRetrievalEvent(new X509MockCertificate()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void RevocationNotFromDssEventMisfiresTest() {
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_LTA_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.LTA_1_TS_B64));
            PdfSignature timestampDict = GetTimestampPdfDictionary(contents);
            eventManager.OnEvent(new ProofOfExistenceFoundEvent(timestampDict, "timestampSig1"));
            eventManager.OnEvent(new SignatureValidationSuccessEvent());
            eventManager.OnEvent(new DSSProcessedEvent(new PdfDictionary()));
            eventManager.OnEvent(new RevocationNotFromDssEvent(new X509MockCertificate()));
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LTA, report.GetDocumentLevel());
        }

        [NUnit.Framework.Test]
        public virtual void AlgorithmUsageEventMisfiresTest() {
            eventManager.OnEvent(new AlgorithmUsageEvent("MD5", OID.MD5, "Test"));
            PdfDictionary signatureDict = new PdfDictionary();
            PdfString contents = new PdfString(Convert.FromBase64String(PAdESLevelHelper.B_B_1_B64));
            contents.SetHexWriting(true);
            signatureDict.Put(PdfName.Contents, contents);
            signatureDict.Put(PdfName.Filter, PdfName.Sig);
            signatureDict.Put(PdfName.SubFilter, PdfName.ETSI_CAdES_DETACHED);
            signatureDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            signatureDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            PdfSignature sig = new PdfSignature(signatureDict);
            IValidationEvent @event = new StartSignatureValidationEvent(sig, "test", new DateTime());
            eventManager.OnEvent(@event);
            @event = new SignatureValidationSuccessEvent();
            eventManager.OnEvent(@event);
            DocumentPAdESLevelReport report = sut.GetReport();
            System.Console.Out.WriteLine(report);
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_B, report.GetSignatureReport("test").GetLevel());
            NUnit.Framework.Assert.AreEqual(PAdESLevel.B_B, report.GetDocumentLevel());
        }

        private static PdfSignature GetTimestampPdfDictionary(PdfString contents) {
            PdfDictionary timestampDict = new PdfDictionary();
            timestampDict.Put(PdfName.Contents, contents);
            timestampDict.Put(PdfName.Filter, PdfName.Sig);
            timestampDict.Put(PdfName.SubFilter, PdfName.ETSI_RFC3161);
            timestampDict.Put(PdfName.ByteRange, new PdfString("1 2 3 4"));
            timestampDict.Put(PdfName.M, new PdfString("D:20231204144752+01'00'"));
            return new PdfSignature(timestampDict);
        }
    }
}

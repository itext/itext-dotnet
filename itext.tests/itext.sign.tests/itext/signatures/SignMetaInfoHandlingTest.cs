/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Actions.Events;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;
using iText.Signatures.Cms;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SignMetaInfoHandlingTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/SignMetaInfoHandlingTest/";

        public static readonly String CERTS = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        public static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static readonly SignMetaInfoHandlingTest.TestConfigurationEvent CONFIGURATION_ACCESS = new SignMetaInfoHandlingTest.TestConfigurationEvent
            ();

        private static SignMetaInfoHandlingTest.StoreEventsHandler handler;

        private static readonly String srcFile = SOURCE_FOLDER + "helloWorldDoc.pdf";

        private static readonly String signCertFileName = CERTS + "signCertRsa01.pem";

        private static readonly String tsaCertFileName = CERTS + "tsCertRsa.pem";

        private static readonly String caCertFileName = CERTS + "rootRsa.pem";

        private static IX509Certificate[] signRsaChain;

        private static IPrivateKey signRsaPrivateKey;

        private static IX509Certificate[] tsaChain;

        private static IPrivateKey tsaPrivateKey;

        private static IX509Certificate caCert;

        private static IPrivateKey caPrivateKey;

        static SignMetaInfoHandlingTest() {
            try {
                signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
                signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
                tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
                tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
                caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
                caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, PASSWORD);
            }
            catch (Exception) {
            }
        }

        // Ignore exception.
        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUpHandler() {
            handler = new SignMetaInfoHandlingTest.StoreEventsHandler(UnknownContext.PERMISSIVE);
            EventManager.GetInstance().Register(handler);
        }

        [NUnit.Framework.TearDown]
        public virtual void ResetHandler() {
            EventManager.GetInstance().Unregister(handler);
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithSignMetaInfoTest() {
            ByteArrayOutputStream @out = new ByteArrayOutputStream();
            SequenceId docSequenceId;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(@out), new DocumentProperties().SetEventCountingMetaInfo
                (new SignMetaInfo()))) {
                docSequenceId = pdfDocument.GetDocumentIdWrapper();
            }
            IList<AbstractProductProcessITextEvent> confirmedEvents = CONFIGURATION_ACCESS.GetPublicEvents(docSequenceId
                );
            // No confirmed events.
            NUnit.Framework.Assert.AreEqual(0, confirmedEvents.Count);
            IList<AbstractContextBasedITextEvent> events = handler.GetEvents();
            NUnit.Framework.Assert.AreEqual(0, events.Count);
        }

        [NUnit.Framework.Test]
        public virtual void SignWithBaselineLTProfileEventHandlingTest() {
            ByteArrayOutputStream @out = new ByteArrayOutputStream();
            SignerProperties signerProperties = new SignerProperties();
            PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFile)), @out
                );
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient);
            padesSigner.SetStampingProperties(new StampingProperties().UseAppendMode());
            padesSigner.SignWithBaselineLTProfile(signerProperties, signRsaChain, signRsaPrivateKey, testTsa);
            IList<AbstractContextBasedITextEvent> events = handler.GetEvents();
            NUnit.Framework.Assert.AreEqual(2, events.Count);
            NUnit.Framework.Assert.IsTrue(events[0] is ITextCoreProductEvent);
            ITextCoreProductEvent iTextCoreProductEvent = (ITextCoreProductEvent)events[0];
            NUnit.Framework.Assert.AreEqual(ITextCoreProductEvent.PROCESS_PDF, iTextCoreProductEvent.GetEventType());
            // Only first iTextCoreProductEvent is confirmed.
            NUnit.Framework.Assert.IsTrue(events[1] is ConfirmEvent);
            ConfirmEvent confirmEvent = (ConfirmEvent)events[1];
            NUnit.Framework.Assert.AreEqual(iTextCoreProductEvent, confirmEvent.GetConfirmedEvent());
        }

        [NUnit.Framework.Test]
        public virtual void SignWithBaselineLTAProfileEventHandlingTest() {
            ByteArrayOutputStream @out = new ByteArrayOutputStream();
            SignerProperties signerProperties = new SignerProperties();
            PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFile)), @out
                );
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient).SetTimestampSignatureName("timestampSig1");
            padesSigner.SignWithBaselineLTAProfile(signerProperties, signRsaChain, signRsaPrivateKey, testTsa);
            IList<AbstractContextBasedITextEvent> events = handler.GetEvents();
            NUnit.Framework.Assert.AreEqual(2, events.Count);
            NUnit.Framework.Assert.IsTrue(events[0] is ITextCoreProductEvent);
            ITextCoreProductEvent iTextCoreProductEvent = (ITextCoreProductEvent)events[0];
            NUnit.Framework.Assert.AreEqual(ITextCoreProductEvent.PROCESS_PDF, iTextCoreProductEvent.GetEventType());
            // Only first iTextCoreProductEvent is confirmed.
            NUnit.Framework.Assert.IsTrue(events[1] is ConfirmEvent);
            ConfirmEvent confirmEvent = (ConfirmEvent)events[1];
            NUnit.Framework.Assert.AreEqual(iTextCoreProductEvent, confirmEvent.GetConfirmedEvent());
        }

        [NUnit.Framework.Test]
        public virtual void SignCMSContainerWithBaselineLTProfileEventHandlingTest() {
            ByteArrayOutputStream @out = new ByteArrayOutputStream();
            PadesTwoPhaseSigningHelper twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            crlClient.AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            ocspClient.AddBuilderForCertIssuer(caCert, caPrivateKey);
            twoPhaseSigningHelper.SetCrlClient(crlClient).SetOcspClient(ocspClient).SetTSAClient(testTsa);
            twoPhaseSigningHelper.SetStampingProperties(new StampingProperties().UseAppendMode());
            using (MemoryStream preparedDoc = new MemoryStream()) {
                CMSContainer container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(signRsaChain, DigestAlgorithms
                    .SHA512, new PdfReader(srcFile), preparedDoc, new SignerProperties());
                IExternalSignature externalSignature = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA512);
                twoPhaseSigningHelper.SignCMSContainerWithBaselineLTProfile(externalSignature, new PdfReader(new MemoryStream
                    (preparedDoc.ToArray())), @out, "Signature1", container);
            }
            IList<AbstractContextBasedITextEvent> events = handler.GetEvents();
            NUnit.Framework.Assert.AreEqual(4, events.Count);
            NUnit.Framework.Assert.IsTrue(events[0] is ITextCoreProductEvent);
            ITextCoreProductEvent iTextCoreProductEvent = (ITextCoreProductEvent)events[0];
            NUnit.Framework.Assert.AreEqual(ITextCoreProductEvent.PROCESS_PDF, iTextCoreProductEvent.GetEventType());
            // First iTextCoreProductEvent is confirmed (coming from createCMSContainerWithoutSignature).
            NUnit.Framework.Assert.IsTrue(events[1] is ConfirmEvent);
            ConfirmEvent confirmEvent = (ConfirmEvent)events[1];
            NUnit.Framework.Assert.AreEqual(iTextCoreProductEvent, confirmEvent.GetConfirmedEvent());
            NUnit.Framework.Assert.IsTrue(events[2] is ITextCoreProductEvent);
            iTextCoreProductEvent = (ITextCoreProductEvent)events[2];
            NUnit.Framework.Assert.AreEqual(ITextCoreProductEvent.PROCESS_PDF, iTextCoreProductEvent.GetEventType());
            // Second iTextCoreProductEvent is confirmed (coming from signCMSContainerWithBaselineLTProfile).
            NUnit.Framework.Assert.IsTrue(events[3] is ConfirmEvent);
            confirmEvent = (ConfirmEvent)events[3];
            NUnit.Framework.Assert.AreEqual(iTextCoreProductEvent, confirmEvent.GetConfirmedEvent());
        }

        [NUnit.Framework.Test]
        public virtual void SignCMSContainerWithBaselineLTAProfileEventHandlingTest() {
            ByteArrayOutputStream @out = new ByteArrayOutputStream();
            PadesTwoPhaseSigningHelper twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            crlClient.AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            ocspClient.AddBuilderForCertIssuer(caCert, caPrivateKey);
            twoPhaseSigningHelper.SetCrlClient(crlClient).SetOcspClient(ocspClient).SetTSAClient(testTsa).SetTimestampSignatureName
                ("timestampSig1");
            using (MemoryStream preparedDoc = new MemoryStream()) {
                CMSContainer container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(signRsaChain, DigestAlgorithms
                    .SHA512, new PdfReader(srcFile), preparedDoc, new SignerProperties());
                IExternalSignature externalSignature = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA512);
                twoPhaseSigningHelper.SignCMSContainerWithBaselineLTAProfile(externalSignature, new PdfReader(new MemoryStream
                    (preparedDoc.ToArray())), @out, "Signature1", container);
            }
            IList<AbstractContextBasedITextEvent> events = handler.GetEvents();
            NUnit.Framework.Assert.AreEqual(4, events.Count);
            NUnit.Framework.Assert.IsTrue(events[0] is ITextCoreProductEvent);
            ITextCoreProductEvent iTextCoreProductEvent = (ITextCoreProductEvent)events[0];
            NUnit.Framework.Assert.AreEqual(ITextCoreProductEvent.PROCESS_PDF, iTextCoreProductEvent.GetEventType());
            // First iTextCoreProductEvent is confirmed (coming from createCMSContainerWithoutSignature).
            NUnit.Framework.Assert.IsTrue(events[1] is ConfirmEvent);
            ConfirmEvent confirmEvent = (ConfirmEvent)events[1];
            NUnit.Framework.Assert.AreEqual(iTextCoreProductEvent, confirmEvent.GetConfirmedEvent());
            NUnit.Framework.Assert.IsTrue(events[2] is ITextCoreProductEvent);
            iTextCoreProductEvent = (ITextCoreProductEvent)events[2];
            NUnit.Framework.Assert.AreEqual(ITextCoreProductEvent.PROCESS_PDF, iTextCoreProductEvent.GetEventType());
            // Second iTextCoreProductEvent is confirmed (coming from signCMSContainerWithBaselineLTAProfile).
            NUnit.Framework.Assert.IsTrue(events[3] is ConfirmEvent);
            confirmEvent = (ConfirmEvent)events[3];
            NUnit.Framework.Assert.AreEqual(iTextCoreProductEvent, confirmEvent.GetConfirmedEvent());
        }

        [NUnit.Framework.Test]
        public virtual void PassSignMetaInfoThroughStampingPropertiesTest() {
            ByteArrayOutputStream @out = new ByteArrayOutputStream();
            SignerProperties signerProperties = new SignerProperties();
            PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFile)), @out
                );
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient);
            padesSigner.SetStampingProperties((StampingProperties)new StampingProperties().UseAppendMode().SetEventCountingMetaInfo
                (new SignMetaInfo()));
            padesSigner.SignWithBaselineLTProfile(signerProperties, signRsaChain, signRsaPrivateKey, testTsa);
            IList<AbstractContextBasedITextEvent> events = handler.GetEvents();
            NUnit.Framework.Assert.AreEqual(0, events.Count);
        }

        private class TestConfigurationEvent : AbstractITextConfigurationEvent {
            protected override void DoAction() {
                throw new InvalidOperationException();
            }

            public virtual IList<AbstractProductProcessITextEvent> GetPublicEvents(SequenceId sequenceId) {
                return base.GetEvents(sequenceId);
            }
        }

        private class StoreEventsHandler : AbstractContextBasedEventHandler {
            private readonly IList<AbstractContextBasedITextEvent> events = new List<AbstractContextBasedITextEvent>();

            protected internal StoreEventsHandler(IContext onUnknownContext)
                : base(onUnknownContext) {
            }

            public virtual IList<AbstractContextBasedITextEvent> GetEvents() {
                return events;
            }

            protected override void OnAcceptedEvent(AbstractContextBasedITextEvent @event) {
                events.Add(@event);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Kernel.Actions.Events;
using iText.Kernel.Pdf;
using iText.Signatures.Validation.V1.Context;
using iText.Test;

namespace iText.Signatures.Validation.V1 {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ValidationMetaInfoEventsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/v1/ValidationMetaInfoEventsTest/";

        private static ValidationMetaInfoEventsTest.StoreEventsHandler handler;

        private readonly ValidatorChainBuilder builder = new ValidatorChainBuilder();

        private readonly ValidationContext validationContext = new ValidationContext(ValidatorContext.DOCUMENT_REVISIONS_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUpHandler() {
            handler = new ValidationMetaInfoEventsTest.StoreEventsHandler(UnknownContext.PERMISSIVE);
            EventManager.GetInstance().Register(handler);
        }

        [NUnit.Framework.TearDown]
        public virtual void ResetHandler() {
            EventManager.GetInstance().Unregister(handler);
        }

        [NUnit.Framework.Test]
        public virtual void DocumentRevisionsValidatorSingleEventTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocument.pdf"
                ))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.ValidateAllDocumentRevisions(validationContext, document);
            }
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
        public virtual void DocumentRevisionsValidatorZeroEventsTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocument.pdf"
                ), new DocumentProperties().SetEventCountingMetaInfo(new ValidationMetaInfo()))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetEventCountingMetaInfo(new ValidationMetaInfo());
                validator.ValidateAllDocumentRevisions(validationContext, document);
            }
            IList<AbstractContextBasedITextEvent> events = handler.GetEvents();
            NUnit.Framework.Assert.AreEqual(0, events.Count);
        }

        [NUnit.Framework.Test]
        public virtual void SignatureValidatorSingleEventTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocument.pdf"
                ))) {
                SignatureValidator validator = builder.BuildSignatureValidator();
                validator.ValidateSignatures(document);
            }
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
        public virtual void SignatureValidatorZeroEventsTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocument.pdf"
                ), new DocumentProperties().SetEventCountingMetaInfo(new ValidationMetaInfo()))) {
                SignatureValidator validator = builder.BuildSignatureValidator();
                validator.SetEventCountingMetaInfo(new ValidationMetaInfo());
                validator.ValidateSignatures(document);
            }
            IList<AbstractContextBasedITextEvent> events = handler.GetEvents();
            NUnit.Framework.Assert.AreEqual(0, events.Count);
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

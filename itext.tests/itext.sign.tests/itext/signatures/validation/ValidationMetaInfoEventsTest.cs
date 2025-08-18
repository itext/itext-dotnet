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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Kernel.Actions.Events;
using iText.Kernel.Pdf;
using iText.Signatures.Validation.Context;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ValidationMetaInfoEventsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/ValidationMetaInfoEventsTest/";

        private readonly ValidatorChainBuilder builder = new ValidatorChainBuilder();

        private readonly ValidationContext validationContext = new ValidationContext(ValidatorContext.DOCUMENT_REVISIONS_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void DocumentRevisionsValidatorSingleEventTest() {
            ValidationMetaInfoEventsTest.StoreEventsHandler handler = new ValidationMetaInfoEventsTest.StoreEventsHandler
                (UnknownContext.PERMISSIVE);
            EventManager.GetInstance().Register(handler);
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
            EventManager.GetInstance().Unregister(handler);
        }

        [NUnit.Framework.Test]
        public virtual void DocumentRevisionsValidatorZeroEventsTest() {
            ValidationMetaInfoEventsTest.StoreEventsHandler handler = new ValidationMetaInfoEventsTest.StoreEventsHandler
                (UnknownContext.PERMISSIVE);
            EventManager.GetInstance().Register(handler);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocument.pdf"
                ), new DocumentProperties().SetEventCountingMetaInfo(new ValidationMetaInfo()))) {
                DocumentRevisionsValidator validator = builder.BuildDocumentRevisionsValidator();
                validator.SetEventCountingMetaInfo(new ValidationMetaInfo());
                validator.ValidateAllDocumentRevisions(validationContext, document);
            }
            IList<AbstractContextBasedITextEvent> events = handler.GetEvents();
            NUnit.Framework.Assert.AreEqual(0, events.Count);
            EventManager.GetInstance().Unregister(handler);
        }

        [NUnit.Framework.Test]
        public virtual void SignatureValidatorSingleEventTest() {
            ValidationMetaInfoEventsTest.StoreEventsHandler handler = new ValidationMetaInfoEventsTest.StoreEventsHandler
                (UnknownContext.PERMISSIVE);
            EventManager.GetInstance().Register(handler);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocument.pdf"
                ))) {
                SignatureValidator validator = builder.BuildSignatureValidator(document);
                validator.ValidateSignatures();
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
            ValidationMetaInfoEventsTest.StoreEventsHandler handler = new ValidationMetaInfoEventsTest.StoreEventsHandler
                (UnknownContext.PERMISSIVE);
            EventManager.GetInstance().Register(handler);
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocument.pdf"
                ), new DocumentProperties().SetEventCountingMetaInfo(new ValidationMetaInfo()))) {
                SignatureValidator validator = builder.BuildSignatureValidator(document);
                validator.SetEventCountingMetaInfo(new ValidationMetaInfo());
                validator.ValidateSignatures();
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

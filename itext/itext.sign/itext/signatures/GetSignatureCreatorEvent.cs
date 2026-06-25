/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Actions.Producer;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Kernel.Actions.Data;
using iText.Kernel.Pdf;

namespace iText.Signatures {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class is responsible for providing signature creator for PdfSignaer class.</summary>
    internal sealed class GetSignatureCreatorEvent : AbstractITextConfigurationEvent {
        private readonly WeakReference document;

        private String signatureCreator;

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates a new instance of the GetSignatureCreatorEvent.</summary>
        /// <param name="document">document in which the signature creator is required</param>
        internal GetSignatureCreatorEvent(PdfDocument document)
            : base() {
            this.document = new WeakReference(document);
        }
//\endcond

        /// <summary>
        /// Provides signature creator string, which can be accessed via
        /// <see cref="GetSignatureCreator()"/>.
        /// </summary>
        protected internal override void DoAction() {
            PdfDocument pdfDocument = (PdfDocument)document.Target;
            if (pdfDocument == null) {
                return;
            }
            IList<AbstractProductProcessITextEvent> events = GetEvents(pdfDocument.GetDocumentIdWrapper());
            if (events == null || events.IsEmpty()) {
                signatureCreator = "";
                return;
            }
            String coreProductName = ITextCoreProductData.GetInstance().GetProductName();
            AbstractProductProcessITextEvent coreEvent = null;
            foreach (AbstractProductProcessITextEvent @event in events) {
                if (coreProductName.Equals(@event.GetProductName())) {
                    coreEvent = @event;
                    break;
                }
            }
            if (coreEvent == null) {
                signatureCreator = ProducerBuilder.ModifyProducer(JavaCollectionsUtil.SingletonList(events[0]), null);
            }
            else {
                signatureCreator = ProducerBuilder.ModifyProducer(JavaCollectionsUtil.SingletonList(coreEvent), null);
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets signature creator.</summary>
        /// <returns>String with a signature creator</returns>
        internal String GetSignatureCreator() {
            return signatureCreator;
        }
//\endcond
    }
//\endcond
}

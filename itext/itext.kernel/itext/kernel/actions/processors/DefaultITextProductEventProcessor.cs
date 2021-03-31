/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.Kernel;
using iText.Kernel.Actions.Events;
using iText.Kernel.Actions.Session;
using iText.Kernel.Pdf;

namespace iText.Kernel.Actions.Processors {
    /// <summary>Defines a default strategy of product event processing.</summary>
    public class DefaultITextProductEventProcessor : ITextProductEventProcessor {
        // TODO: DEVSIX-5054 should be removed when new producer line building logic is implemented
        private const String OLD_MECHANISM_PRODUCER_LINE_WAS_SET = "old-mechanism-producer-line-was-set";

        private readonly String productName;

        /// <summary>Creates an instance of product event processor.</summary>
        /// <param name="productName">is a product name</param>
        public DefaultITextProductEventProcessor(String productName) {
            if (productName == null) {
                throw new ArgumentException(PdfException.ProductNameCannotBeNull);
            }
            this.productName = productName;
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="event">to handle</param>
        public virtual void OnEvent(AbstractITextProductEvent @event) {
        }

        // TODO: DEVSIX-4964 provide appropriate logic if any
        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual String GetProductName() {
            return productName;
        }

        /// <summary>Collects info about products involved into document processing.</summary>
        /// <param name="closingSession">is a closing session</param>
        public virtual void AggregationOnClose(ClosingSession closingSession) {
            if (closingSession.GetProducer() == null) {
                closingSession.SetProducer(new List<String>());
            }
            closingSession.GetProducer().Add(GetProducer());
        }

        /// <summary>Updates meta info of the document.</summary>
        /// <param name="closingSession">is a closing session</param>
        public virtual void CompletionOnClose(ClosingSession closingSession) {
            if (closingSession.GetProducer() != null) {
                IList<String> lines = closingSession.GetProducer();
                UpdateProducerLine(closingSession.GetDocument(), lines);
                closingSession.SetProducer(null);
            }
            //TODO: DEVSIX-5054 code below should be removed when new producer line building logic is implemented
            if (closingSession.GetProperty(OLD_MECHANISM_PRODUCER_LINE_WAS_SET) == null) {
                if (closingSession.GetDocument() != null) {
                    closingSession.GetDocument().UpdateProducerInInfoDictionary();
                }
                closingSession.SetProperty(OLD_MECHANISM_PRODUCER_LINE_WAS_SET, true);
            }
        }

        /// <summary>Gets a label which defines a product.</summary>
        /// <returns>a product label</returns>
        protected internal virtual String GetProducer() {
            // TODO: DEVSIX-5054: probably productName + "(AGPL)"
            return productName;
        }

        private static void UpdateProducerLine(PdfDocument document, IList<String> elements) {
        }
        // TODO: DEVSIX-5054
    }
}

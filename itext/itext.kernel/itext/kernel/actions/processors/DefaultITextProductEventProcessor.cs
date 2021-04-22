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
using iText.Kernel;
using iText.Kernel.Actions.Events;
using iText.Kernel.Actions.Session;

namespace iText.Kernel.Actions.Processors {
    /// <summary>Defines a default strategy of product event processing.</summary>
    public class DefaultITextProductEventProcessor : ITextProductEventProcessor {
        // TODO: DEVSIX-5323 should be removed when new producer line building logic is implemented
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

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual String GetUsageType() {
            return "AGPL";
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual String GetProducer() {
            return "iText\u00ae ${usedProducts:P V (T 'version')} \u00a9${copyrightSince}-${copyrightTo} iText Group NV";
        }

        /// <summary>Collects info about products involved into document processing.</summary>
        /// <param name="closingSession">is a closing session</param>
        public virtual void AggregationOnClose(ClosingSession closingSession) {
        }

        /// <summary>Updates meta info of the document.</summary>
        /// <param name="closingSession">is a closing session</param>
        public virtual void CompletionOnClose(ClosingSession closingSession) {
            // TODO DEVSIX-5323 remove the logic when all tests are ready
            if (!Toggle.NEW_PRODUCER_LINE) {
                if (closingSession.GetProperty(OLD_MECHANISM_PRODUCER_LINE_WAS_SET) == null) {
                    if (closingSession.GetDocument() != null) {
                        closingSession.GetDocument().UpdateProducerInInfoDictionary();
                    }
                    closingSession.SetProperty(OLD_MECHANISM_PRODUCER_LINE_WAS_SET, true);
                }
            }
        }
    }
}

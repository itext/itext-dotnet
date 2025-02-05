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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Processors;
using iText.Commons.Utils;
using iText.Kernel.Actions.Data;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;

namespace iText.Kernel.Actions.Events {
    /// <summary>This class is responsible for adding a fingerprint.</summary>
    public sealed class AddFingerPrintEvent : AbstractITextConfigurationEvent {
        private readonly WeakReference document;

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Actions.Events.AddFingerPrintEvent
            ));

        private const String AGPL_MODE = "AGPL";

        /// <summary>Creates a new instance of the AddFingerPrintEvent.</summary>
        /// <param name="document">document in which the fingerprint will be added</param>
        public AddFingerPrintEvent(PdfDocument document)
            : base() {
            this.document = new WeakReference(document);
        }

        /// <summary>Adds fingerprint to the document.</summary>
        protected internal override void DoAction() {
            PdfDocument pdfDocument = (PdfDocument)document.Target;
            if (pdfDocument == null) {
                return;
            }
            FingerPrint fingerPrint = pdfDocument.GetFingerPrint();
            ICollection<ProductData> products = fingerPrint.GetProducts();
            //if fingerprint is disabled and all licence types isn't AGPL then no actions required
            if (!fingerPrint.IsFingerPrintEnabled()) {
                bool nonAGPLMode = true;
                foreach (ProductData productData in products) {
                    ITextProductEventProcessor processor = GetActiveProcessor(productData.GetProductName());
                    if (processor == null) {
                        continue;
                    }
                    if (AGPL_MODE.Equals(processor.GetUsageType())) {
                        nonAGPLMode = false;
                        break;
                    }
                }
                if (nonAGPLMode) {
                    return;
                }
                LOGGER.LogWarning(KernelLogMessageConstant.FINGERPRINT_DISABLED_BUT_NO_REQUIRED_LICENCE);
            }
            PdfWriter writer = pdfDocument.GetWriter();
            if (products.IsEmpty()) {
                writer.WriteString(MessageFormatUtil.Format("%iText-{0}-no-registered-products\n", ITextCoreProductData.GetInstance
                    ().GetVersion()));
                return;
            }
            foreach (ProductData productData in products) {
                writer.WriteString(MessageFormatUtil.Format("%iText-{0}-{1}\n", productData.GetPublicProductName(), productData
                    .GetVersion()));
            }
        }
    }
}

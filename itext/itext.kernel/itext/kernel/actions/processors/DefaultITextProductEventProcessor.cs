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
using iText.Kernel.Actions;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Actions.Processors {
    /// <summary>Defines a default strategy of product event processing.</summary>
    public class DefaultITextProductEventProcessor : ITextProductEventProcessor {
        private readonly String productName;

        /// <summary>Creates an instance of product event processor.</summary>
        /// <param name="productName">is a product name</param>
        public DefaultITextProductEventProcessor(String productName) {
            if (productName == null) {
                throw new ArgumentException(KernelExceptionMessageConstant.PRODUCT_NAME_CAN_NOT_BE_NULL);
            }
            this.productName = productName;
        }

        public virtual void OnEvent(AbstractProductProcessITextEvent @event) {
        }

        // TODO: DEVSIX-5341 provide appropriate logic if any
        public virtual String GetProductName() {
            return productName;
        }

        public virtual String GetUsageType() {
            return "AGPL";
        }

        public virtual String GetProducer() {
            return "iText\u00ae ${usedProducts:P V (T 'version')} \u00a9${copyrightSince}-${copyrightTo} iText Group NV";
        }
    }
}

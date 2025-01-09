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
using iText.Commons.Actions;
using iText.Commons.Exceptions;

namespace iText.Commons.Actions.Processors {
    /// <summary>Abstract class with some standard functionality for product event processing.</summary>
    public abstract class AbstractITextProductEventProcessor : ITextProductEventProcessor {
        private readonly String productName;

        /// <summary>Creates a new instance of an abstract processor for the provided product.</summary>
        /// <param name="productName">the product which will be handled by this processor</param>
        public AbstractITextProductEventProcessor(String productName) {
            if (productName == null) {
                throw new ArgumentException(CommonsExceptionMessageConstant.PRODUCT_NAME_CAN_NOT_BE_NULL);
            }
            this.productName = productName;
        }

        public abstract void OnEvent(AbstractProductProcessITextEvent @event);

        public abstract String GetUsageType();

        public virtual String GetProducer() {
            return "iText\u00ae ${usedProducts:P V (T 'version')} \u00a9${copyrightSince}-${copyrightTo} Apryse Group NV";
        }

        public virtual String GetProductName() {
            return productName;
        }
    }
}

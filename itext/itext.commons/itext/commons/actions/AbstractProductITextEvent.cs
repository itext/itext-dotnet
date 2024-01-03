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
using iText.Commons.Actions.Data;

namespace iText.Commons.Actions {
    /// <summary>
    /// Abstract class which defines general product events by encapsulating
    /// <see cref="iText.Commons.Actions.Data.ProductData"/>
    /// of the product which generated event.
    /// </summary>
    /// <remarks>
    /// Abstract class which defines general product events by encapsulating
    /// <see cref="iText.Commons.Actions.Data.ProductData"/>
    /// of the product which generated event. Only for internal usage.
    /// </remarks>
    public abstract class AbstractProductITextEvent : AbstractITextEvent {
        private readonly ProductData productData;

        /// <summary>
        /// Creates instance of abstract product iText event based
        /// on passed product data.
        /// </summary>
        /// <remarks>
        /// Creates instance of abstract product iText event based
        /// on passed product data. Only for internal usage.
        /// </remarks>
        /// <param name="productData">is a description of the product which has generated an event</param>
        protected internal AbstractProductITextEvent(ProductData productData)
            : base() {
            if (productData == null) {
                // IllegalStateException is thrown because AbstractProductITextEvent for internal usage
                throw new InvalidOperationException("ProductData shouldn't be null.");
            }
            this.productData = productData;
        }

        /// <summary>Gets a product data which generated the event.</summary>
        /// <returns>information about the product</returns>
        public virtual ProductData GetProductData() {
            return productData;
        }

        /// <summary>Gets a name of product which generated the event.</summary>
        /// <returns>product name</returns>
        public virtual String GetProductName() {
            return GetProductData().GetProductName();
        }
    }
}

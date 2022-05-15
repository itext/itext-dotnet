/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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

namespace iText.Commons.Actions.Processors {
    /// <summary>
    /// Factory class to construct
    /// <see cref="ITextProductEventProcessor"/>
    /// instance under AGPL license.
    /// </summary>
    public class UnderAgplProductProcessorFactory : IProductProcessorFactory {
        /// <summary>Creates under AGPL product processor using a product name.</summary>
        /// <param name="productName">the product which will be handled by this processor</param>
        /// <returns>
        /// current
        /// <see cref="ITextProductEventProcessor"/>
        /// instance
        /// </returns>
        public virtual ITextProductEventProcessor CreateProcessor(String productName) {
            return new UnderAgplITextProductEventProcessor(productName);
        }
    }
}

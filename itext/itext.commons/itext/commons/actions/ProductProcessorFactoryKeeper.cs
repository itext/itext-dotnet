/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Commons.Actions.Processors;

namespace iText.Commons.Actions {
    /// <summary>Helper class which allow to change used product processor factory instance.</summary>
    internal sealed class ProductProcessorFactoryKeeper {
        private static readonly IProductProcessorFactory DEFAULT_FACTORY = new DefaultProductProcessorFactory();

        private static IProductProcessorFactory productProcessorFactory = DEFAULT_FACTORY;

        private ProductProcessorFactoryKeeper() {
        }

        // do nothing
        /// <summary>Sets product processor factory instance.</summary>
        /// <param name="productProcessorFactory">the instance to be set</param>
        internal static void SetProductProcessorFactory(IProductProcessorFactory productProcessorFactory) {
            iText.Commons.Actions.ProductProcessorFactoryKeeper.productProcessorFactory = productProcessorFactory;
        }

        /// <summary>Restores default factory.</summary>
        internal static void RestoreDefaultProductProcessorFactory() {
            iText.Commons.Actions.ProductProcessorFactoryKeeper.productProcessorFactory = DEFAULT_FACTORY;
        }

        /// <summary>Gets reporting product processor factory instance.</summary>
        /// <returns>the product processor factory instance</returns>
        internal static IProductProcessorFactory GetProductProcessorFactory() {
            return productProcessorFactory;
        }
    }
}

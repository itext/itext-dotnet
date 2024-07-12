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
using iText.Commons.Actions;
using iText.Commons.Actions.Data;

namespace iText.Kernel.Actions.Data {
    /// <summary>
    /// Stores an instance of
    /// <see cref="iText.Commons.Actions.Data.ProductData"/>
    /// related to iText core module.
    /// </summary>
    public sealed class ITextCoreProductData {
        private const String CORE_PUBLIC_PRODUCT_NAME = "Core";

        private const String CORE_VERSION = "8.0.6-SNAPSHOT";

        private const int CORE_COPYRIGHT_SINCE = 2000;

        private const int CORE_COPYRIGHT_TO = 2024;

        private static readonly ProductData ITEXT_PRODUCT_DATA = new ProductData(CORE_PUBLIC_PRODUCT_NAME, ProductNameConstant
            .ITEXT_CORE, CORE_VERSION, CORE_COPYRIGHT_SINCE, CORE_COPYRIGHT_TO);

        private ITextCoreProductData() {
        }

        // Empty constructor.
        /// <summary>
        /// Getter for an instance of
        /// <see cref="iText.Commons.Actions.Data.ProductData"/>
        /// related to iText core module.
        /// </summary>
        /// <returns>iText core product description</returns>
        public static ProductData GetInstance() {
            return ITEXT_PRODUCT_DATA;
        }
    }
}

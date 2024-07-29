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

namespace iText.Commons.Actions.Data {
    /// <summary>
    /// Stores an instance of
    /// <see cref="ProductData"/>
    /// related to iText commons module.
    /// </summary>
    public sealed class CommonsProductData {
//\cond DO_NOT_DOCUMENT
        internal const String COMMONS_PUBLIC_PRODUCT_NAME = "Commons";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String COMMONS_PRODUCT_NAME = "commons";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String COMMONS_VERSION = "9.0.0-SNAPSHOT";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String MINIMAL_COMPATIBLE_LICENSEKEY_VERSION = "4.1.0";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const int COMMONS_COPYRIGHT_SINCE = 2000;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const int COMMONS_COPYRIGHT_TO = 2024;
//\endcond

        private static readonly ProductData COMMONS_PRODUCT_DATA = new ProductData(COMMONS_PUBLIC_PRODUCT_NAME, COMMONS_PRODUCT_NAME
            , COMMONS_VERSION, MINIMAL_COMPATIBLE_LICENSEKEY_VERSION, COMMONS_COPYRIGHT_SINCE, COMMONS_COPYRIGHT_TO
            );

        private CommonsProductData() {
        }

        // Empty constructor for util class
        /// <summary>
        /// Getter for an instance of
        /// <see cref="ProductData"/>
        /// related to iText commons module.
        /// </summary>
        /// <returns>iText commons product description</returns>
        public static ProductData GetInstance() {
            return COMMONS_PRODUCT_DATA;
        }
    }
}

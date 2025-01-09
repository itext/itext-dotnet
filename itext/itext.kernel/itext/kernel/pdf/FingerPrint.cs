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
using System.Collections.Generic;
using iText.Commons.Actions.Data;
using iText.Commons.Utils;

namespace iText.Kernel.Pdf {
    /// <summary>Data container for debugging information.</summary>
    /// <remarks>
    /// Data container for debugging information. This class keeps a record of every registered product that
    /// was involved in the creation of a certain PDF file.
    /// </remarks>
    public class FingerPrint {
        private ICollection<ProductData> productDataSet;

        private bool fingerPrintEnabled = true;

        /// <summary>Default constructor.</summary>
        /// <remarks>Default constructor. Initializes the productDataSet.</remarks>
        public FingerPrint() {
            this.productDataSet = new LinkedHashSet<ProductData>();
        }

        /// <summary>This method is used to disable iText fingerprint.</summary>
        /// <remarks>
        /// This method is used to disable iText fingerprint.
        /// IText fingerPrint can only be disabled if all products are in non AGPL mode.
        /// </remarks>
        public virtual void DisableFingerPrint() {
            fingerPrintEnabled = false;
        }

        /// <summary>This method is used to check iText fingerprint state.</summary>
        /// <returns>true if fingerprint will be added to the document</returns>
        public virtual bool IsFingerPrintEnabled() {
            return fingerPrintEnabled;
        }

        /// <summary>Registers a product to be added to the fingerprint or other debugging info.</summary>
        /// <param name="productData">ProductData to be added</param>
        /// <returns>true if the fingerprint did not already contain the specified element</returns>
        public virtual bool RegisterProduct(ProductData productData) {
            int initialSize = productDataSet.Count;
            productDataSet.Add(productData);
            return initialSize != productDataSet.Count;
        }

        /// <summary>Returns the registered products.</summary>
        /// <returns>registered products.</returns>
        public virtual ICollection<ProductData> GetProducts() {
            return JavaCollectionsUtil.UnmodifiableSet(new LinkedHashSet<ProductData>(this.productDataSet));
        }
    }
}

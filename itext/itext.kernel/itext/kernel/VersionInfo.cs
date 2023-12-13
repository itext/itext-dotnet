/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.Kernel {
    public class VersionInfo {
        private readonly String productName;

        private readonly String releaseNumber;

        private readonly String producerLine;

        private readonly String licenseKey;

        public VersionInfo(String productName, String releaseNumber, String producerLine, String licenseKey) {
            this.productName = productName;
            this.releaseNumber = releaseNumber;
            this.producerLine = producerLine;
            this.licenseKey = licenseKey;
        }

        /// <summary>Gets the product name.</summary>
        /// <remarks>
        /// Gets the product name.
        /// iText Group NV requests that you retain the iText producer line
        /// in every PDF that is created or manipulated using iText.
        /// </remarks>
        /// <returns>the product name</returns>
        public virtual String GetProduct() {
            return productName;
        }

        /// <summary>Gets the release number.</summary>
        /// <remarks>
        /// Gets the release number.
        /// iText Group NV requests that you retain the iText producer line
        /// in every PDF that is created or manipulated using iText.
        /// </remarks>
        /// <returns>the release number</returns>
        public virtual String GetRelease() {
            return releaseNumber;
        }

        /// <summary>Returns the iText version as shown in the producer line.</summary>
        /// <remarks>
        /// Returns the iText version as shown in the producer line.
        /// iText is a product developed by iText Group NV.
        /// iText Group requests that you retain the iText producer line
        /// in every PDF that is created or manipulated using iText.
        /// </remarks>
        /// <returns>iText version</returns>
        public virtual String GetVersion() {
            return producerLine;
        }

        /// <summary>Returns a license key if one was provided, or null if not.</summary>
        /// <returns>a license key.</returns>
        public virtual String GetKey() {
            return licenseKey;
        }
    }
}

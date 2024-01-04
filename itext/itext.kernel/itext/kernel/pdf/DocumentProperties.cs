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
using iText.Commons.Actions.Contexts;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// Class with additional properties for
    /// <see cref="PdfDocument"/>
    /// processing.
    /// </summary>
    /// <remarks>
    /// Class with additional properties for
    /// <see cref="PdfDocument"/>
    /// processing.
    /// Needs to be passed at document initialization.
    /// </remarks>
    public class DocumentProperties {
        protected internal IMetaInfo metaInfo = null;

        /// <summary>Default constructor, use provided setters for configuration options.</summary>
        public DocumentProperties() {
        }

        /// <summary>Creates a copy of class instance.</summary>
        /// <param name="other">the base for new class instance</param>
        public DocumentProperties(iText.Kernel.Pdf.DocumentProperties other) {
            this.metaInfo = other.metaInfo;
        }

        /// <summary>Sets document meta info.</summary>
        /// <param name="metaInfo">meta info to set</param>
        /// <returns>
        /// this
        /// <see cref="DocumentProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.DocumentProperties SetEventCountingMetaInfo(IMetaInfo metaInfo) {
            this.metaInfo = metaInfo;
            return this;
        }
    }
}

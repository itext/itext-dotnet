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
namespace iText.Kernel.Pdf {
    /// <summary>
    /// Class with additional properties for
    /// <see cref="PdfDocument"/>
    /// processing
    /// in stamping mode.
    /// </summary>
    /// <remarks>
    /// Class with additional properties for
    /// <see cref="PdfDocument"/>
    /// processing
    /// in stamping mode. Needs to be passed at document initialization.
    /// <para />
    /// See
    /// <see cref="PageFlushingHelper"/>
    /// documentation to find more information about modes of document processing.
    /// </remarks>
    public class StampingProperties : DocumentProperties {
        protected internal bool appendMode = false;

        protected internal bool preserveEncryption = false;

        protected internal bool disableMac = false;

        /// <summary>Default constructor, use provided setters for configuration options.</summary>
        public StampingProperties() {
        }

        // Do nothing
        /// <summary>Creates a copy of class instance.</summary>
        /// <param name="other">the base for new class instance</param>
        public StampingProperties(iText.Kernel.Pdf.StampingProperties other)
            : base(other) {
            this.appendMode = other.appendMode;
            this.preserveEncryption = other.preserveEncryption;
            this.disableMac = other.disableMac;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates a copy of
        /// <see cref="DocumentProperties"/>
        /// instance.
        /// </summary>
        /// <param name="documentProperties">the base for new class instance</param>
        internal StampingProperties(DocumentProperties documentProperties)
            : base(documentProperties) {
            this.dependencies = documentProperties.dependencies;
        }
//\endcond

        /// <summary>Defines if the document will be edited in append mode.</summary>
        /// <returns>
        /// this
        /// <see cref="StampingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.StampingProperties UseAppendMode() {
            appendMode = true;
            return this;
        }

        /// <summary>Defines if the encryption of the original document (if it was encrypted) will be preserved.</summary>
        /// <remarks>
        /// Defines if the encryption of the original document (if it was encrypted) will be preserved.
        /// By default, the resultant document doesn't preserve the original encryption.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="StampingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.StampingProperties PreserveEncryption() {
            this.preserveEncryption = true;
            return this;
        }

        /// <summary>Disables MAC token in the output PDF-2.0 document.</summary>
        /// <remarks>
        /// Disables MAC token in the output PDF-2.0 document.
        /// By default, MAC token will be embedded.
        /// This property does not remove MAC token from existing document in append mode because it removes MAC protection
        /// from all previous revisions also.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="StampingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.StampingProperties DisableMac() {
            this.disableMac = true;
            return this;
        }
    }
}

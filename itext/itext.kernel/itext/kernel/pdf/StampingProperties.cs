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
namespace iText.Kernel.Pdf {
    public class StampingProperties : DocumentProperties {
        protected internal bool appendMode = false;

        protected internal bool preserveEncryption = false;

        public StampingProperties() {
        }

        public StampingProperties(iText.Kernel.Pdf.StampingProperties other)
            : base(other) {
            this.appendMode = other.appendMode;
            this.preserveEncryption = other.preserveEncryption;
        }

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
    }
}

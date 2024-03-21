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
using System.Collections.Generic;

namespace iText.Kernel.Pdf {
    /// <summary>Class which stores information about single PDF document revision.</summary>
    public class DocumentRevision {
        private readonly long eofOffset;

        private readonly ICollection<PdfIndirectReference> modifiedObjects;

        /// <summary>
        /// Creates
        /// <see cref="DocumentRevision"/>
        /// from end-of-file byte position and a set of indirect references which were
        /// modified in this document revision.
        /// </summary>
        /// <param name="eofOffset">end-of-file byte position</param>
        /// <param name="modifiedObjects">
        /// 
        /// <see cref="Java.Util.Set{E}"/>
        /// of
        /// <see cref="PdfIndirectReference"/>
        /// objects which were modified
        /// </param>
        public DocumentRevision(long eofOffset, ICollection<PdfIndirectReference> modifiedObjects) {
            this.eofOffset = eofOffset;
            this.modifiedObjects = modifiedObjects;
        }

        /// <summary>Gets end-of-file byte position.</summary>
        /// <returns>end-of-file byte position</returns>
        public virtual long GetEofOffset() {
            return eofOffset;
        }

        /// <summary>Gets objects which were modified in this document revision.</summary>
        /// <returns>
        /// 
        /// <see cref="Java.Util.Set{E}"/>
        /// of
        /// <see cref="PdfIndirectReference"/>
        /// objects which were modified
        /// </returns>
        public virtual ICollection<PdfIndirectReference> GetModifiedObjects() {
            return modifiedObjects;
        }
    }
}

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

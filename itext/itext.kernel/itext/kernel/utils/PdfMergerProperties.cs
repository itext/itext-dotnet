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
namespace iText.Kernel.Utils {
    /// <summary>
    /// Class with additional properties for
    /// <see cref="PdfMerger"/>
    /// processing.
    /// </summary>
    /// <remarks>
    /// Class with additional properties for
    /// <see cref="PdfMerger"/>
    /// processing.
    /// Needs to be passed at merger initialization.
    /// </remarks>
    public class PdfMergerProperties {
        private bool closeSrcDocuments;

        private bool mergeTags;

        private bool mergeOutlines;

        private bool mergeScripts;

        /// <summary>Default constructor, use provided setters for configuration options.</summary>
        public PdfMergerProperties() {
            closeSrcDocuments = false;
            mergeTags = true;
            mergeOutlines = true;
            mergeScripts = false;
        }

        /// <summary>check if source documents should be close after merging</summary>
        /// <returns>true if they should, false otherwise</returns>
        public virtual bool IsCloseSrcDocuments() {
            return closeSrcDocuments;
        }

        /// <summary>check if tags should be merged</summary>
        /// <returns>true if they should, false otherwise</returns>
        public virtual bool IsMergeTags() {
            return mergeTags;
        }

        /// <summary>check if outlines should be merged</summary>
        /// <returns>true if they should, false otherwise</returns>
        public virtual bool IsMergeOutlines() {
            return mergeOutlines;
        }

        /// <summary>check if ECMA scripts (which are executed at document opening) should be merged</summary>
        /// <returns>true if they should, false otherwise</returns>
        public virtual bool IsMergeScripts() {
            return mergeScripts;
        }

        /// <summary>close source documents after merging</summary>
        /// <param name="closeSrcDocuments">true to close, false otherwise</param>
        /// <returns><c>PdfMergerProperties</c> instance</returns>
        public virtual iText.Kernel.Utils.PdfMergerProperties SetCloseSrcDocuments(bool closeSrcDocuments) {
            this.closeSrcDocuments = closeSrcDocuments;
            return this;
        }

        /// <summary>merge documents tags</summary>
        /// <param name="mergeTags">true to merge, false otherwise</param>
        /// <returns><c>PdfMergerProperties</c> instance</returns>
        public virtual iText.Kernel.Utils.PdfMergerProperties SetMergeTags(bool mergeTags) {
            this.mergeTags = mergeTags;
            return this;
        }

        /// <summary>merge documents outlines</summary>
        /// <param name="mergeOutlines">true to merge, false otherwise</param>
        /// <returns><c>PdfMergerProperties</c> instance</returns>
        public virtual iText.Kernel.Utils.PdfMergerProperties SetMergeOutlines(bool mergeOutlines) {
            this.mergeOutlines = mergeOutlines;
            return this;
        }

        /// <summary>merge documents ECMA scripts</summary>
        /// <param name="mergeNames">true to merge, false otherwise</param>
        /// <returns><c>PdfMergerProperties</c> instance</returns>
        public virtual iText.Kernel.Utils.PdfMergerProperties SetMergeScripts(bool mergeNames) {
            this.mergeScripts = mergeNames;
            return this;
        }
    }
}

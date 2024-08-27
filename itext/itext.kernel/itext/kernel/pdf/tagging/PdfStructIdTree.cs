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
using iText.Kernel.Pdf;
using iText.Kernel.Validation.Context;

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>Models the tree of structure element IDs.</summary>
    /// <remarks>
    /// Models the tree of structure element IDs.
    /// This is an optional feature of tagged PDF documents.
    /// </remarks>
    public class PdfStructIdTree : GenericNameTree {
//\cond DO_NOT_DOCUMENT
        internal PdfStructIdTree(PdfDocument pdfDoc)
            : base(pdfDoc) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Parse a structure element ID tree into its in-memory representation.</summary>
        /// <param name="pdfDoc">
        /// the associated
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// </param>
        /// <param name="dict">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// from which to parse the tree
        /// </param>
        /// <returns>
        /// the parsed
        /// <see cref="PdfStructIdTree"/>
        /// </returns>
        internal static iText.Kernel.Pdf.Tagging.PdfStructIdTree ReadFromDictionary(PdfDocument pdfDoc, PdfDictionary
             dict) {
            iText.Kernel.Pdf.Tagging.PdfStructIdTree structIdTree = new iText.Kernel.Pdf.Tagging.PdfStructIdTree(pdfDoc
                );
            structIdTree.SetItems(GenericNameTree.ReadTree(dict));
            return structIdTree;
        }
//\endcond

        /// <summary>Retrieve a structure element by ID, if it has one.</summary>
        /// <param name="id">the ID of the structure element to retrieve</param>
        /// <returns>the structure element with the given ID if one exists, or null otherwise.</returns>
        public virtual PdfStructElem GetStructElemById(PdfString id) {
            PdfObject rawObj = this.GetItems().Get(id);
            if (rawObj is PdfIndirectReference) {
                rawObj = ((PdfIndirectReference)rawObj).GetRefersTo();
            }
            if (rawObj is PdfDictionary) {
                return new PdfStructElem((PdfDictionary)rawObj);
            }
            return null;
        }

        /// <summary>Retrieve a structure element by ID, if it has one.</summary>
        /// <param name="id">the ID of the structure element to retrieve</param>
        /// <returns>the structure element with the given ID if one exists, or null otherwise.</returns>
        public virtual PdfStructElem GetStructElemById(byte[] id) {
            return this.GetStructElemById(new PdfString(id));
        }

        public override void AddEntry(PdfString key, PdfObject value) {
            base.AddEntry(key, value, (pdfDoc) => pdfDoc.CheckIsoConformance(new DuplicateIdEntryValidationContext(key
                )));
        }
    }
}

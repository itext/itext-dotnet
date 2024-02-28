using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Pdfua.Checkers.Utils.Tables {
    /// <summary>The result matrix to validate PDF UA1 tables with based on the TagTreeStructure of the document.</summary>
    internal class StructTreeResultMatrix : AbstractResultMatrix<PdfStructElem> {
        /// <summary>
        /// Creates a new
        /// <see cref="StructTreeResultMatrix"/>
        /// instance.
        /// </summary>
        public StructTreeResultMatrix(PdfStructElem elem)
            : base(new TableStructElementIterator(elem)) {
        }

        /// <summary><inheritDoc/></summary>
        internal override IList<byte[]> GetHeaders(PdfStructElem cell) {
            PdfObject @object = cell.GetAttributes(false);
            PdfArray pdfArr = null;
            if (@object is PdfArray) {
                PdfArray array = (PdfArray)@object;
                foreach (PdfObject pdfObject in array) {
                    if (pdfObject is PdfDictionary) {
                        pdfArr = ((PdfDictionary)pdfObject).GetAsArray(PdfName.Headers);
                    }
                }
            }
            else {
                if (@object is PdfDictionary) {
                    pdfArr = ((PdfDictionary)@object).GetAsArray(PdfName.Headers);
                }
            }
            if (pdfArr == null) {
                return null;
            }
            IList<byte[]> list = new List<byte[]>();
            foreach (PdfObject pdfObject in pdfArr) {
                PdfString str = (PdfString)pdfObject;
                list.Add(str.GetValueBytes());
            }
            return list;
        }

        /// <summary><inheritDoc/></summary>
        internal override String GetScope(PdfStructElem cell) {
            PdfObject @object = cell.GetAttributes(false);
            if (@object is PdfArray) {
                PdfArray array = (PdfArray)@object;
                foreach (PdfObject pdfObject in array) {
                    if (pdfObject is PdfDictionary) {
                        PdfName f = ((PdfDictionary)pdfObject).GetAsName(PdfName.Scope);
                        if (f != null) {
                            return f.GetValue();
                        }
                    }
                }
            }
            else {
                if (@object is PdfDictionary) {
                    PdfName f = ((PdfDictionary)@object).GetAsName(PdfName.Scope);
                    if (f != null) {
                        return f.GetValue();
                    }
                }
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        internal override byte[] GetElementId(PdfStructElem cell) {
            if (cell == null) {
                return null;
            }
            if (cell.GetStructureElementId() == null) {
                return null;
            }
            return cell.GetStructureElementId().GetValueBytes();
        }

        /// <summary><inheritDoc/></summary>
        internal override String GetRole(PdfStructElem cell) {
            PdfName role = cell.GetRole();
            if (role != null) {
                return role.GetValue();
            }
            return null;
        }
    }
}

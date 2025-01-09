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
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf {
    public abstract class PdfObjectWrapper<T>
        where T : PdfObject {
        private T pdfObject = null;

        protected internal PdfObjectWrapper(T pdfObject) {
            this.pdfObject = pdfObject;
            if (IsWrappedObjectMustBeIndirect()) {
                MarkObjectAsIndirect(this.pdfObject);
            }
        }

        public virtual T GetPdfObject() {
            return pdfObject;
        }

        /// <summary>Marks object behind wrapper to be saved as indirect.</summary>
        /// <param name="document">a document the indirect reference belongs to.</param>
        /// <param name="reference">a reference which will be assigned for the object behind wrapper.</param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.PdfObjectWrapper<T> MakeIndirect(PdfDocument document, PdfIndirectReference
             reference) {
            GetPdfObject().MakeIndirect(document, reference);
            return this;
        }

        /// <summary>Marks object behind wrapper to be saved as indirect.</summary>
        /// <param name="document">a document the indirect reference will belong to.</param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.Pdf.PdfObjectWrapper<T> MakeIndirect(PdfDocument document) {
            return MakeIndirect(document, null);
        }

        public virtual iText.Kernel.Pdf.PdfObjectWrapper<T> SetModified() {
            pdfObject.SetModified();
            return this;
        }

        public virtual void Flush() {
            pdfObject.Flush();
        }

        public virtual bool IsFlushed() {
            return pdfObject.IsFlushed();
        }

        /// <summary>
        /// Defines if the object behind this wrapper must be an indirect object in the
        /// resultant document.
        /// </summary>
        /// <remarks>
        /// Defines if the object behind this wrapper must be an indirect object in the
        /// resultant document.
        /// <br /><br />
        /// If this method returns <i>true</i> it doesn't necessarily mean that object
        /// must be in the indirect state at any moment, but rather defines that
        /// when the object will be written to the document it will be transformed into
        /// indirect object if it's not indirect yet.
        /// <br /><br />
        /// Return value of this method shouldn't depend on any logic, it should return
        /// always <i>true</i> or <i>false</i>.
        /// </remarks>
        /// <returns>
        /// <i>true</i> if in the resultant document the object behind the wrapper
        /// must be indirect, otherwise <i>false</i>.
        /// </returns>
        protected internal abstract bool IsWrappedObjectMustBeIndirect();

        protected internal virtual void SetPdfObject(T pdfObject) {
            this.pdfObject = pdfObject;
        }

        protected internal virtual void SetForbidRelease() {
            if (pdfObject != null) {
                pdfObject.SetState(PdfObject.FORBID_RELEASE);
            }
        }

        protected internal virtual void UnsetForbidRelease() {
            if (pdfObject != null) {
                pdfObject.ClearState(PdfObject.FORBID_RELEASE);
            }
        }

        protected internal virtual void EnsureUnderlyingObjectHasIndirectReference() {
            if (GetPdfObject().GetIndirectReference() == null) {
                throw new PdfException(KernelExceptionMessageConstant.TO_FLUSH_THIS_WRAPPER_UNDERLYING_OBJECT_MUST_BE_ADDED_TO_DOCUMENT
                    );
            }
        }

        protected internal static void MarkObjectAsIndirect(PdfObject pdfObject) {
            if (pdfObject.GetIndirectReference() == null) {
                pdfObject.SetState(PdfObject.MUST_BE_INDIRECT);
            }
        }

        /// <summary>
        /// Some wrappers use object's indirect reference to obtain the
        /// <c>PdfDocument</c>
        /// to which the object belongs to.
        /// </summary>
        /// <remarks>
        /// Some wrappers use object's indirect reference to obtain the
        /// <c>PdfDocument</c>
        /// to which the object belongs to. For this matter, for these wrappers it is implicitly defined
        /// that they work with indirect objects only. Commonly these wrappers have two constructors: one with
        /// <c>PdfDocument</c>
        /// as parameter to create a new object, and the other one which
        /// wraps around the given
        /// <c>PdfObject</c>
        /// . This method should be used in the second
        /// type of constructors to ensure that wrapper will able to obtain the
        /// <c>PdfDocument</c>
        /// instance.
        /// </remarks>
        /// <param name="object">
        /// the
        /// <c>PdfObject</c>
        /// to be checked if it is indirect.
        /// </param>
        protected internal static void EnsureObjectIsAddedToDocument(PdfObject @object) {
            if (@object.GetIndirectReference() == null) {
                throw new PdfException(KernelExceptionMessageConstant.OBJECT_MUST_BE_INDIRECT_TO_WORK_WITH_THIS_WRAPPER);
            }
        }
    }
}

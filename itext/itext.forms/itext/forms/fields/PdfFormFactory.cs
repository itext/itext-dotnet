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
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms.Fields {
    /// <summary>Default factory for form related instances creation.</summary>
    public class PdfFormFactory {
        /// <summary>
        /// Create
        /// <see cref="PdfFormFactory"/>
        /// instance.
        /// </summary>
        public PdfFormFactory() {
        }

        // Empty constructor.
        /// <summary>
        /// Create a minimal, empty
        /// <see cref="PdfFormField"/>.
        /// </summary>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFormField"/>
        /// </returns>
        public virtual PdfFormField CreateFormField(PdfDocument document) {
            return new PdfFormField(document);
        }

        /// <summary>
        /// Creates a form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Creates a form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfFormField"/>
        /// </returns>
        public virtual PdfFormField CreateFormField(PdfDictionary dictionary) {
            return new PdfFormField(dictionary);
        }

        /// <summary>
        /// Creates a form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </summary>
        /// <param name="widget">
        /// The widget which will be a kid of the
        /// <see cref="PdfFormField"/>.
        /// </param>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFormField"/>
        /// </returns>
        public virtual PdfFormField CreateFormField(PdfWidgetAnnotation widget, PdfDocument document) {
            return new PdfFormField(widget, document);
        }

        /// <summary>
        /// Create a minimal, empty
        /// <see cref="PdfTextFormField"/>.
        /// </summary>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public virtual PdfTextFormField CreateTextFormField(PdfDocument document) {
            return new PdfTextFormField(document);
        }

        /// <summary>
        /// Creates a text form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Creates a text form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public virtual PdfTextFormField CreateTextFormField(PdfDictionary dictionary) {
            return new PdfTextFormField(dictionary);
        }

        /// <summary>
        /// Creates a text form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </summary>
        /// <param name="widget">
        /// The widget which will be a kid of the
        /// <see cref="PdfTextFormField"/>.
        /// </param>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public virtual PdfTextFormField CreateTextFormField(PdfWidgetAnnotation widget, PdfDocument document) {
            return new PdfTextFormField(widget, document);
        }

        /// <summary>
        /// Create a minimal, empty
        /// <see cref="PdfButtonFormField"/>.
        /// </summary>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public virtual PdfButtonFormField CreateButtonFormField(PdfDocument document) {
            return new PdfButtonFormField(document);
        }

        /// <summary>
        /// Creates a button form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Creates a button form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public virtual PdfButtonFormField CreateButtonFormField(PdfDictionary dictionary) {
            return new PdfButtonFormField(dictionary);
        }

        /// <summary>
        /// Creates a button form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </summary>
        /// <param name="widget">
        /// The widget which will be a kid of the
        /// <see cref="PdfButtonFormField"/>.
        /// </param>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public virtual PdfButtonFormField CreateButtonFormField(PdfWidgetAnnotation widget, PdfDocument document) {
            return new PdfButtonFormField(widget, document);
        }

        /// <summary>
        /// Create a minimal, empty
        /// <see cref="PdfChoiceFormField"/>.
        /// </summary>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual PdfChoiceFormField CreateChoiceFormField(PdfDocument document) {
            return new PdfChoiceFormField(document);
        }

        /// <summary>
        /// Creates a choice form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Creates a choice form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual PdfChoiceFormField CreateChoiceFormField(PdfDictionary dictionary) {
            return new PdfChoiceFormField(dictionary);
        }

        /// <summary>
        /// Creates a choice form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </summary>
        /// <param name="widget">
        /// The widget which will be a kid of the
        /// <see cref="PdfChoiceFormField"/>.
        /// </param>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public virtual PdfChoiceFormField CreateChoiceFormField(PdfWidgetAnnotation widget, PdfDocument document) {
            return new PdfChoiceFormField(widget, document);
        }

        /// <summary>
        /// Create a minimal, empty
        /// <see cref="PdfSignatureFormField"/>.
        /// </summary>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfSignatureFormField"/>
        /// </returns>
        public virtual PdfSignatureFormField CreateSignatureFormField(PdfDocument document) {
            return new PdfSignatureFormField(document);
        }

        /// <summary>
        /// Creates a signature form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Creates a signature form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfSignatureFormField"/>
        /// </returns>
        public virtual PdfSignatureFormField CreateSignatureFormField(PdfDictionary dictionary) {
            return new PdfSignatureFormField(dictionary);
        }

        /// <summary>
        /// Creates a signature form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </summary>
        /// <param name="widget">
        /// The widget which will be a kid of the
        /// <see cref="PdfSignatureFormField"/>.
        /// </param>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfSignatureFormField"/>
        /// </returns>
        public virtual PdfSignatureFormField CreateSignatureFormField(PdfWidgetAnnotation widget, PdfDocument document
            ) {
            return new PdfSignatureFormField(widget, document);
        }

        /// <summary>
        /// Creates a form field annotation as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Creates a form field annotation as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfFormAnnotation"/>
        /// </returns>
        public virtual PdfFormAnnotation CreateFormAnnotation(PdfDictionary dictionary) {
            return new PdfFormAnnotation(dictionary);
        }

        /// <summary>
        /// Creates a form field annotation as a wrapper of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </summary>
        /// <param name="widget">
        /// The widget which will be a kid of the
        /// <see cref="PdfFormField"/>
        /// </param>
        /// <param name="document">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfFormAnnotation"/>
        /// </returns>
        public virtual PdfFormAnnotation CreateFormAnnotation(PdfWidgetAnnotation widget, PdfDocument document) {
            return new PdfFormAnnotation(widget, document);
        }

        /// <summary>Retrieves AcroForm from the document.</summary>
        /// <remarks>
        /// Retrieves AcroForm from the document. If there is no AcroForm in the
        /// document Catalog and createIfNotExist flag is true then the AcroForm
        /// dictionary will be created and added to the document.
        /// </remarks>
        /// <param name="document">
        /// the document to retrieve the
        /// <see cref="iText.Forms.PdfAcroForm"/>
        /// from
        /// </param>
        /// <param name="createIfNotExist">
        /// when <c>true</c>, this method will create a
        /// <see cref="iText.Forms.PdfAcroForm"/>
        /// if none exists for this document
        /// </param>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument">document</see>
        /// 's AcroForm, or a new one provided that <c>createIfNotExist</c>
        /// parameter is <c>true</c>, otherwise <c>null</c>.
        /// </returns>
        public virtual PdfAcroForm GetAcroForm(PdfDocument document, bool createIfNotExist) {
            return PdfAcroForm.GetAcroForm(document, createIfNotExist);
        }
    }
}

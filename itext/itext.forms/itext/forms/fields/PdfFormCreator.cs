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
    /// <summary>Creator which shall be used in order to create all form related instances.</summary>
    /// <remarks>
    /// Creator which shall be used in order to create all form related instances. By default
    /// <see cref="PdfFormFactory"/>
    /// is used.
    /// </remarks>
    public sealed class PdfFormCreator {
        private static PdfFormFactory factory = new PdfFormFactory();

        private PdfFormCreator() {
        }

        /// <summary>
        /// Set
        /// <see cref="PdfFormFactory"/>
        /// to be used for form related instances creation.
        /// </summary>
        /// <param name="factory">
        /// 
        /// <see cref="PdfFormFactory"/>
        /// to set
        /// </param>
        public static void SetFactory(PdfFormFactory factory) {
            iText.Forms.Fields.PdfFormCreator.factory = factory;
        }

        /// <summary>
        /// Create a minimal, empty
        /// <see cref="PdfFormField"/>
        /// using provided factory.
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
        public static PdfFormField CreateFormField(PdfDocument document) {
            return factory.CreateFormField(document);
        }

        /// <summary>
        /// Creates a form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// </summary>
        /// <remarks>
        /// Creates a form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfFormField"/>
        /// </returns>
        public static PdfFormField CreateFormField(PdfDictionary dictionary) {
            return factory.CreateFormField(dictionary);
        }

        /// <summary>
        /// Creates a form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// using provided factory.
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
        public static PdfFormField CreateFormField(PdfWidgetAnnotation widget, PdfDocument document) {
            return factory.CreateFormField(widget, document);
        }

        /// <summary>
        /// Create a minimal, empty
        /// <see cref="PdfTextFormField"/>
        /// using provided factory.
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
        public static PdfTextFormField CreateTextFormField(PdfDocument document) {
            return factory.CreateTextFormField(document);
        }

        /// <summary>
        /// Creates a text form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// </summary>
        /// <remarks>
        /// Creates a text form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfTextFormField"/>
        /// </returns>
        public static PdfTextFormField CreateTextFormField(PdfDictionary dictionary) {
            return factory.CreateTextFormField(dictionary);
        }

        /// <summary>
        /// Creates a text form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// using provided factory.
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
        public static PdfTextFormField CreateTextFormField(PdfWidgetAnnotation widget, PdfDocument document) {
            return factory.CreateTextFormField(widget, document);
        }

        /// <summary>
        /// Create a minimal, empty
        /// <see cref="PdfButtonFormField"/>
        /// using provided factory.
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
        public static PdfButtonFormField CreateButtonFormField(PdfDocument document) {
            return factory.CreateButtonFormField(document);
        }

        /// <summary>
        /// Creates a button form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// </summary>
        /// <remarks>
        /// Creates a button form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public static PdfButtonFormField CreateButtonFormField(PdfDictionary dictionary) {
            return factory.CreateButtonFormField(dictionary);
        }

        /// <summary>
        /// Creates a button form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// using provided factory.
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
        public static PdfButtonFormField CreateButtonFormField(PdfWidgetAnnotation widget, PdfDocument document) {
            return factory.CreateButtonFormField(widget, document);
        }

        /// <summary>
        /// Create a minimal, empty
        /// <see cref="PdfChoiceFormField"/>
        /// using provided factory.
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
        public static PdfChoiceFormField CreateChoiceFormField(PdfDocument document) {
            return factory.CreateChoiceFormField(document);
        }

        /// <summary>
        /// Creates a choice form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// </summary>
        /// <remarks>
        /// Creates a choice form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfChoiceFormField"/>
        /// </returns>
        public static PdfChoiceFormField CreateChoiceFormField(PdfDictionary dictionary) {
            return factory.CreateChoiceFormField(dictionary);
        }

        /// <summary>
        /// Creates a choice form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// using provided factory.
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
        public static PdfChoiceFormField CreateChoiceFormField(PdfWidgetAnnotation widget, PdfDocument document) {
            return factory.CreateChoiceFormField(widget, document);
        }

        /// <summary>
        /// Create a minimal, empty
        /// <see cref="PdfSignatureFormField"/>
        /// using provided factory.
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
        public static PdfSignatureFormField CreateSignatureFormField(PdfDocument document) {
            return factory.CreateSignatureFormField(document);
        }

        /// <summary>
        /// Creates a signature form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// </summary>
        /// <remarks>
        /// Creates a signature form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfSignatureFormField"/>
        /// </returns>
        public static PdfSignatureFormField CreateSignatureFormField(PdfDictionary dictionary) {
            return factory.CreateSignatureFormField(dictionary);
        }

        /// <summary>
        /// Creates a signature form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// using provided factory.
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
        public static PdfSignatureFormField CreateSignatureFormField(PdfWidgetAnnotation widget, PdfDocument document
            ) {
            return factory.CreateSignatureFormField(widget, document);
        }

        /// <summary>
        /// Creates a form field annotation as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// </summary>
        /// <remarks>
        /// Creates a form field annotation as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// using provided factory.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="dictionary">the dictionary to be wrapped, must have an indirect reference.</param>
        /// <returns>
        /// created
        /// <see cref="PdfFormAnnotation"/>
        /// </returns>
        public static PdfFormAnnotation CreateFormAnnotation(PdfDictionary dictionary) {
            return factory.CreateFormAnnotation(dictionary);
        }

        /// <summary>
        /// Creates a form field annotation as a wrapper of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// using provided factory.
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
        public static PdfFormAnnotation CreateFormAnnotation(PdfWidgetAnnotation widget, PdfDocument document) {
            return factory.CreateFormAnnotation(widget, document);
        }

        /// <summary>Retrieves AcroForm from the document using provided factory.</summary>
        /// <remarks>
        /// Retrieves AcroForm from the document using provided factory. If there is no AcroForm in the
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
        public static PdfAcroForm GetAcroForm(PdfDocument document, bool createIfNotExist) {
            return factory.GetAcroForm(document, createIfNotExist);
        }
    }
}

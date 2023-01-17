/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System;
using iText.Forms.Exceptions;
using iText.IO.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Xobject;

namespace iText.Forms.Fields {
    /// <summary>Builder for choice form field.</summary>
    public class ChoiceFormFieldBuilder : TerminalFormFieldBuilder<iText.Forms.Fields.ChoiceFormFieldBuilder> {
        private PdfArray options = null;

        /// <summary>
        /// Creates builder for
        /// <see cref="PdfChoiceFormField"/>
        /// creation.
        /// </summary>
        /// <param name="document">document to be used for form field creation</param>
        /// <param name="formFieldName">name of the form field</param>
        public ChoiceFormFieldBuilder(PdfDocument document, String formFieldName)
            : base(document, formFieldName) {
        }

        /// <summary>Gets options for choice form field.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of choice form field options
        /// </returns>
        public virtual PdfArray GetOptions() {
            return options;
        }

        /// <summary>Sets options for choice form field.</summary>
        /// <param name="options">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of choice form field options
        /// </param>
        /// <returns>this builder</returns>
        public virtual iText.Forms.Fields.ChoiceFormFieldBuilder SetOptions(PdfArray options) {
            this.options = options;
            return this;
        }

        /// <summary>Sets options for choice form field.</summary>
        /// <param name="options">
        /// array of
        /// <see cref="System.String"/>
        /// options
        /// </param>
        /// <returns>this builder</returns>
        public virtual iText.Forms.Fields.ChoiceFormFieldBuilder SetOptions(String[] options) {
            return SetOptions(ProcessOptions(options));
        }

        /// <summary>Sets options for choice form field.</summary>
        /// <param name="options">
        /// two-dimensional array of
        /// <see cref="System.String"/>
        /// options. Every inner array shall have two elements.
        /// </param>
        /// <returns>this builder</returns>
        public virtual iText.Forms.Fields.ChoiceFormFieldBuilder SetOptions(String[][] options) {
            return SetOptions(ProcessOptions(options));
        }

        /// <summary>Creates list form field based on provided parameters.</summary>
        /// <returns>
        /// new
        /// <see cref="PdfChoiceFormField"/>
        /// instance
        /// </returns>
        public virtual PdfChoiceFormField CreateList() {
            return CreateChoice(0);
        }

        /// <summary>Creates combobox form field base on provided parameters.</summary>
        /// <returns>
        /// new
        /// <see cref="PdfChoiceFormField"/>
        /// instance
        /// </returns>
        public virtual PdfChoiceFormField CreateComboBox() {
            return CreateChoice(PdfChoiceFormField.FF_COMBO);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override iText.Forms.Fields.ChoiceFormFieldBuilder GetThis() {
            return this;
        }

        private PdfChoiceFormField CreateChoice(int flags) {
            PdfChoiceFormField field;
            PdfWidgetAnnotation annotation = null;
            if (GetWidgetRectangle() == null) {
                field = new PdfChoiceFormField(GetDocument());
            }
            else {
                annotation = new PdfWidgetAnnotation(GetWidgetRectangle());
                if (null != GetConformanceLevel()) {
                    annotation.SetFlag(PdfAnnotation.PRINT);
                }
                field = new PdfChoiceFormField(annotation, GetDocument());
            }
            field.pdfAConformanceLevel = GetConformanceLevel();
            field.UpdateFontAndFontSize(GetDocument().GetDefaultFont(), PdfFormField.DEFAULT_FONT_SIZE);
            field.SetFieldFlags(flags);
            field.SetFieldName(GetFormFieldName());
            if (options == null) {
                field.Put(PdfName.Opt, new PdfArray());
                field.SetListSelected(new String[0], false);
            }
            else {
                field.Put(PdfName.Opt, options);
                field.SetListSelected(new String[0], false);
                String optionsArrayString = "";
                if ((flags & PdfChoiceFormField.FF_COMBO) == 0) {
                    optionsArrayString = PdfFormField.OptionsArrayToString(options);
                }
                if (annotation != null) {
                    PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, GetWidgetRectangle().GetWidth(), GetWidgetRectangle
                        ().GetHeight()));
                    field.DrawChoiceAppearance(GetWidgetRectangle(), field.fontSize, optionsArrayString, xObject, 0);
                    annotation.SetNormalAppearance(xObject.GetPdfObject());
                    SetPageToField(field);
                }
            }
            return field;
        }

        /// <summary>
        /// Convert
        /// <see cref="System.String"/>
        /// multidimensional array of combo box or list options to
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>.
        /// </summary>
        /// <param name="options">Two-dimensional array of options.</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// that contains all the options.
        /// </returns>
        private static PdfArray ProcessOptions(String[][] options) {
            PdfArray array = new PdfArray();
            foreach (String[] option in options) {
                if (option.Length != 2) {
                    throw new ArgumentException(FormsExceptionMessageConstant.INNER_ARRAY_SHALL_HAVE_TWO_ELEMENTS);
                }
                PdfArray subArray = new PdfArray(new PdfString(option[0], PdfEncodings.UNICODE_BIG));
                subArray.Add(new PdfString(option[1], PdfEncodings.UNICODE_BIG));
                array.Add(subArray);
            }
            return array;
        }

        /// <summary>
        /// Convert
        /// <see cref="System.String"/>
        /// array of combo box or list options to
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>.
        /// </summary>
        /// <param name="options">array of options.</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// that contains all the options.
        /// </returns>
        private static PdfArray ProcessOptions(String[] options) {
            PdfArray array = new PdfArray();
            foreach (String option in options) {
                array.Add(new PdfString(option, PdfEncodings.UNICODE_BIG));
            }
            return array;
        }
    }
}

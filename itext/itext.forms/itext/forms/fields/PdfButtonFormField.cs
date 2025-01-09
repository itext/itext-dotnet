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
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms.Logs;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Xobject;

namespace iText.Forms.Fields {
    /// <summary>An interactive control on the screen that raises events and/or can retain data.</summary>
    public class PdfButtonFormField : PdfFormField {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Forms.Fields.PdfButtonFormField
            ));

        /// <summary>If true, clicking the selected button deselects it, leaving no button selected.</summary>
        /// <remarks>
        /// If true, clicking the selected button deselects it, leaving no button selected.
        /// If false, exactly one radio button shall be selected at all times.
        /// </remarks>
        public static readonly int FF_NO_TOGGLE_TO_OFF = MakeFieldFlag(15);

        /// <summary>If true, the field is a set of radio buttons.</summary>
        /// <remarks>
        /// If true, the field is a set of radio buttons.
        /// If false, the field is a check box.
        /// This flag should be set only if the
        /// <see cref="FF_PUSH_BUTTON"/>
        /// flag is set to false.
        /// </remarks>
        public static readonly int FF_RADIO = MakeFieldFlag(16);

        /// <summary>If true, the field is a push button that does not retain a permanent value.</summary>
        public static readonly int FF_PUSH_BUTTON = MakeFieldFlag(17);

        /// <summary>
        /// If true, a group of radio buttons within a radio button field,
        /// that use the same value for the on state will turn on and off in unison.
        /// </summary>
        /// <remarks>
        /// If true, a group of radio buttons within a radio button field,
        /// that use the same value for the on state will turn on and off in unison.
        /// That is if one is checked, they are all checked.
        /// If false, the buttons are mutually exclusive.
        /// </remarks>
        public static readonly int FF_RADIOS_IN_UNISON = MakeFieldFlag(26);

        /// <summary>
        /// Creates a minimal
        /// <see cref="PdfButtonFormField"/>.
        /// </summary>
        /// <param name="pdfDocument">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        protected internal PdfButtonFormField(PdfDocument pdfDocument)
            : base(pdfDocument) {
        }

        /// <summary>
        /// Creates a button form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </summary>
        /// <param name="widget">
        /// The widget which will be a kid of the
        /// <see cref="PdfButtonFormField"/>.
        /// </param>
        /// <param name="pdfDocument">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        protected internal PdfButtonFormField(PdfWidgetAnnotation widget, PdfDocument pdfDocument)
            : base(widget, pdfDocument) {
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
        /// <param name="pdfObject">the dictionary to be wrapped, must have an indirect reference.</param>
        protected internal PdfButtonFormField(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Returns <c>Btn</c>, the form type for choice form fields.</summary>
        /// <returns>
        /// the form type, as a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// </returns>
        public override PdfName GetFormType() {
            return PdfName.Btn;
        }

        /// <summary>
        /// If true, the field is a set of radio buttons; if false, the field is a
        /// check box.
        /// </summary>
        /// <remarks>
        /// If true, the field is a set of radio buttons; if false, the field is a
        /// check box. This flag only works if the Pushbutton flag is set to false.
        /// </remarks>
        /// <returns>whether the field is currently radio buttons or a checkbox</returns>
        public virtual bool IsRadio() {
            return GetFieldFlag(FF_RADIO);
        }

        /// <summary>
        /// If true, the field is a set of radio buttons; if false, the field is a
        /// check box.
        /// </summary>
        /// <remarks>
        /// If true, the field is a set of radio buttons; if false, the field is a
        /// check box. This flag should be set only if the Pushbutton flag is set to false.
        /// </remarks>
        /// <param name="radio">whether the field should be radio buttons or a checkbox</param>
        /// <returns>
        /// current
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfButtonFormField SetRadio(bool radio) {
            return (iText.Forms.Fields.PdfButtonFormField)SetFieldFlag(FF_RADIO, radio);
        }

        /// <summary>
        /// If true, clicking the selected button deselects it, leaving no button
        /// selected.
        /// </summary>
        /// <remarks>
        /// If true, clicking the selected button deselects it, leaving no button
        /// selected. If false, exactly one radio button shall be selected at all
        /// times. Only valid for radio buttons.
        /// </remarks>
        /// <returns>whether a radio button currently allows to choose no options</returns>
        public virtual bool IsToggleOff() {
            return !GetFieldFlag(FF_NO_TOGGLE_TO_OFF);
        }

        /// <summary>If true, clicking the selected button deselects it, leaving no button selected.</summary>
        /// <remarks>
        /// If true, clicking the selected button deselects it, leaving no button selected.
        /// If false, exactly one radio button shall be selected at all times.
        /// </remarks>
        /// <param name="toggleOff">whether a radio button may allow to choose no options</param>
        /// <returns>
        /// current
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfButtonFormField SetToggleOff(bool toggleOff) {
            return (iText.Forms.Fields.PdfButtonFormField)SetFieldFlag(FF_NO_TOGGLE_TO_OFF, !toggleOff);
        }

        /// <summary>If true, the field is a pushbutton that does not retain a permanent value.</summary>
        /// <returns>whether or not the field is currently a pushbutton</returns>
        public virtual bool IsPushButton() {
            return GetFieldFlag(FF_PUSH_BUTTON);
        }

        /// <summary>If true, the field is a pushbutton that does not retain a permanent value.</summary>
        /// <param name="pushButton">whether or not to set the field to a pushbutton</param>
        /// <returns>
        /// current
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfButtonFormField SetPushButton(bool pushButton) {
            return (iText.Forms.Fields.PdfButtonFormField)SetFieldFlag(FF_PUSH_BUTTON, pushButton);
        }

        /// <summary>
        /// If true, a group of radio buttons within a radio button field that use
        /// the same value for the on state will turn on and off in unison;
        /// that is if one is checked, they are all checked.
        /// </summary>
        /// <remarks>
        /// If true, a group of radio buttons within a radio button field that use
        /// the same value for the on state will turn on and off in unison;
        /// that is if one is checked, they are all checked.
        /// If false, the buttons are mutually exclusive
        /// </remarks>
        /// <returns>whether or not buttons are turned off in unison</returns>
        public virtual bool IsRadiosInUnison() {
            return GetFieldFlag(FF_RADIOS_IN_UNISON);
        }

        /// <summary>
        /// If true, a group of radio buttons within a radio button field that use
        /// the same value for the on state will turn on and off in unison; that is
        /// if one is checked, they are all checked.
        /// </summary>
        /// <remarks>
        /// If true, a group of radio buttons within a radio button field that use
        /// the same value for the on state will turn on and off in unison; that is
        /// if one is checked, they are all checked.
        /// If false, the buttons are mutually exclusive
        /// </remarks>
        /// <param name="radiosInUnison">whether or not buttons should turn off in unison</param>
        /// <returns>
        /// current
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfButtonFormField SetRadiosInUnison(bool radiosInUnison) {
            return (iText.Forms.Fields.PdfButtonFormField)SetFieldFlag(FF_RADIOS_IN_UNISON, radiosInUnison);
        }

        /// <summary>Set image to be used as a background content in a push button.</summary>
        /// <param name="image">path to the image to be used.</param>
        /// <returns>
        /// this
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfButtonFormField SetImage(String image) {
            Stream @is = FileUtil.GetInputStreamForFile(image);
            String str = Convert.ToBase64String(StreamUtil.InputStreamToArray(@is));
            return (iText.Forms.Fields.PdfButtonFormField)SetValue(str);
        }

        /// <summary>
        /// Set image to be used as a background content in a push button as
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>.
        /// </summary>
        /// <param name="form">
        /// 
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// to be used as an image
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfButtonFormField"/>
        /// </returns>
        public virtual iText.Forms.Fields.PdfButtonFormField SetImageAsForm(PdfFormXObject form) {
            this.form = form;
            RegenerateField();
            return this;
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="kid">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override PdfFormField AddKid(AbstractPdfFormField kid) {
            if (IsRadio() && kid is PdfFormAnnotation) {
                PdfFormAnnotation kidAsFormAnnotation = (PdfFormAnnotation)kid;
                // annotation will always be an object because of the assert in getWidget
                PdfWidgetAnnotation annotation = kidAsFormAnnotation.GetWidget();
                PdfName appearanceState = annotation.GetPdfObject().GetAsName(PdfName.AS);
                if (!appearanceState.Equals(GetValue())) {
                    annotation.SetAppearanceState(new PdfName(PdfFormAnnotation.OFF_STATE_VALUE));
                }
                if (annotation.GetRectangle() == null) {
                    LOGGER.LogWarning(FormsLogMessageConstants.RADIO_HAS_NO_RECTANGLE);
                    return base.AddKid(kid);
                }
                kidAsFormAnnotation.DrawRadioButtonAndSaveAppearance(appearanceState.GetValue());
            }
            return base.AddKid(kid);
        }
    }
}

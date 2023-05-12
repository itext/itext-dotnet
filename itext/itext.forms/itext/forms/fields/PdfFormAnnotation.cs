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
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.Forms.Fields.Borders;
using iText.Forms.Fields.Properties;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Form.Renderer.Checkboximpl;
using iText.Forms.Logs;
using iText.Forms.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Fields {
    /// <summary>
    /// This class represents a single annotation in form fields hierarchy in an
    /// <see cref="iText.Forms.PdfAcroForm">AcroForm</see>.
    /// </summary>
    /// <remarks>
    /// This class represents a single annotation in form fields hierarchy in an
    /// <see cref="iText.Forms.PdfAcroForm">AcroForm</see>.
    /// <para />
    /// To be able to be wrapped with this
    /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}"/>
    /// the
    /// <see cref="iText.Kernel.Pdf.PdfObject"/>
    /// must be indirect.
    /// </remarks>
    public class PdfFormAnnotation : AbstractPdfFormField {
        public const int HIDDEN = 1;

        public const int VISIBLE_BUT_DOES_NOT_PRINT = 2;

        public const int HIDDEN_BUT_PRINTABLE = 3;

        public const int VISIBLE = 4;

        /// <summary>Value which represents "off" state of form field.</summary>
        public const String OFF_STATE_VALUE = "Off";

        /// <summary>Value which represents "on" state of form field.</summary>
        public const String ON_STATE_VALUE = "Yes";

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Forms.Fields.PdfFormAnnotation
            ));

        private const String LINE_ENDINGS_REGEXP = "\\r\\n|\\r|\\n";

        protected internal float borderWidth = 1;

        protected internal Color backgroundColor;

        protected internal Color borderColor;

        private IFormField formFieldElement;

        /// <summary>
        /// Creates a form field annotation as a wrapper of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </summary>
        /// <param name="widget">
        /// The widget which will be a kid of the
        /// <see cref="PdfFormField"/>
        /// </param>
        /// <param name="pdfDocument">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        protected internal PdfFormAnnotation(PdfWidgetAnnotation widget, PdfDocument pdfDocument)
            : this(widget.MakeIndirect(pdfDocument).GetPdfObject()) {
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
        /// <param name="pdfObject">the dictionary to be wrapped, must have an indirect reference.</param>
        protected internal PdfFormAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// Creates a
        /// <see cref="PdfFormAnnotation"/>
        /// object.
        /// </summary>
        /// <param name="pdfObject">
        /// assumed to be either a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// , or a
        /// <see cref="iText.Kernel.Pdf.PdfIndirectReference"/>
        /// to a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to create the field in.
        /// </param>
        /// <returns>
        /// a new
        /// <see cref="PdfFormAnnotation"/>
        /// , or <c>null</c> if
        /// <c>pdfObject</c> is not a widget annotation.
        /// </returns>
        public static iText.Forms.Fields.PdfFormAnnotation MakeFormAnnotation(PdfObject pdfObject, PdfDocument document
            ) {
            if (!pdfObject.IsDictionary()) {
                return null;
            }
            iText.Forms.Fields.PdfFormAnnotation field;
            PdfDictionary dictionary = (PdfDictionary)pdfObject;
            PdfName subType = dictionary.GetAsName(PdfName.Subtype);
            // If widget annotation
            if (PdfName.Widget.Equals(subType)) {
                field = PdfFormCreator.CreateFormAnnotation((PdfWidgetAnnotation)PdfAnnotation.MakeAnnotation(dictionary), 
                    document);
            }
            else {
                return null;
            }
            field.MakeIndirect(document);
            if (document != null && document.GetReader() != null && document.GetReader().GetPdfAConformanceLevel() != 
                null) {
                field.pdfAConformanceLevel = document.GetReader().GetPdfAConformanceLevel();
            }
            return field;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>
        /// that this form field refers to.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </returns>
        public virtual PdfWidgetAnnotation GetWidget() {
            PdfName subType = GetPdfObject().GetAsName(PdfName.Subtype);
            if (subType != null && subType.Equals(PdfName.Widget)) {
                return (PdfWidgetAnnotation)PdfAnnotation.MakeAnnotation(GetPdfObject());
            }
            // Should never be here
            System.Diagnostics.Debug.Assert("You are not an annotation then" == null);
            return null;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override PdfString GetDefaultAppearance() {
            return GetPdfObject().GetAsString(PdfName.DA);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override bool RegenerateField() {
            if (parent != null) {
                parent.UpdateDefaultAppearance();
            }
            return RegenerateWidget();
        }

        /// <summary>Gets the appearance state names.</summary>
        /// <returns>an array of Strings containing the names of the appearance states.</returns>
        public override String[] GetAppearanceStates() {
            ICollection<String> names = new LinkedHashSet<String>();
            PdfDictionary dic = GetPdfObject();
            dic = dic.GetAsDictionary(PdfName.AP);
            if (dic != null) {
                dic = dic.GetAsDictionary(PdfName.N);
                if (dic != null) {
                    foreach (PdfName state in dic.KeySet()) {
                        names.Add(state.GetValue());
                    }
                }
            }
            return names.ToArray(new String[names.Count]);
        }

        internal override void RetrieveStyles() {
            base.RetrieveStyles();
            PdfDictionary appearanceCharacteristics = GetPdfObject().GetAsDictionary(PdfName.MK);
            if (appearanceCharacteristics != null) {
                backgroundColor = AppearancePropToColor(appearanceCharacteristics, PdfName.BG);
                Color extractedBorderColor = AppearancePropToColor(appearanceCharacteristics, PdfName.BC);
                if (extractedBorderColor != null) {
                    borderColor = extractedBorderColor;
                }
            }
        }

        /// <summary>Basic setter for the <c>backgroundColor</c> property.</summary>
        /// <remarks>
        /// Basic setter for the <c>backgroundColor</c> property. Regenerates
        /// the field appearance after setting the new value.
        /// </remarks>
        /// <param name="backgroundColor">
        /// The new color to be set or
        /// <see langword="null"/>
        /// if no background needed.
        /// </param>
        /// <returns>
        /// The edited
        /// <see cref="PdfFormAnnotation"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormAnnotation SetBackgroundColor(Color backgroundColor) {
            this.backgroundColor = backgroundColor;
            PdfDictionary mk;
            PdfWidgetAnnotation kid = GetWidget();
            mk = kid.GetAppearanceCharacteristics();
            if (mk == null) {
                mk = new PdfDictionary();
            }
            if (backgroundColor == null) {
                mk.Remove(PdfName.BG);
            }
            else {
                mk.Put(PdfName.BG, new PdfArray(backgroundColor.GetColorValue()));
            }
            kid.SetAppearanceCharacteristics(mk);
            RegenerateField();
            return this;
        }

        /// <summary>Get rotation property specified in this form annotation.</summary>
        /// <returns>
        /// 
        /// <c>int</c>
        /// value which represents field's rotation
        /// </returns>
        public virtual int GetRotation() {
            PdfDictionary mk = GetWidget().GetAppearanceCharacteristics();
            return mk == null || mk.GetAsInt(PdfName.R) == null ? 0 : (int)mk.GetAsInt(PdfName.R);
        }

        /// <summary>Basic setter for the <c>degRotation</c> property.</summary>
        /// <remarks>
        /// Basic setter for the <c>degRotation</c> property. Regenerates
        /// the field appearance after setting the new value.
        /// </remarks>
        /// <param name="degRotation">The new degRotation to be set</param>
        /// <returns>
        /// The edited
        /// <see cref="PdfFormAnnotation"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormAnnotation SetRotation(int degRotation) {
            if (degRotation % 90 != 0) {
                throw new ArgumentException("degRotation.must.be.a.multiple.of.90");
            }
            else {
                degRotation %= 360;
                if (degRotation < 0) {
                    degRotation += 360;
                }
            }
            PdfDictionary mk = GetWidget().GetAppearanceCharacteristics();
            if (mk == null) {
                mk = new PdfDictionary();
                this.Put(PdfName.MK, mk);
            }
            mk.Put(PdfName.R, new PdfNumber(degRotation));
            RegenerateField();
            return this;
        }

        /// <summary>
        /// Sets the action on
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation">widget</see>
        /// of this annotation form field.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>
        /// The edited
        /// <see cref="PdfFormAnnotation"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormAnnotation SetAction(PdfAction action) {
            PdfWidgetAnnotation widget = GetWidget();
            if (widget != null) {
                widget.SetAction(action);
            }
            return this;
        }

        /// <summary>Set the visibility flags of the form field annotation.</summary>
        /// <remarks>
        /// Set the visibility flags of the form field annotation.
        /// Options are: HIDDEN, HIDDEN_BUT_PRINTABLE, VISIBLE, VISIBLE_BUT_DOES_NOT_PRINT.
        /// </remarks>
        /// <param name="visibility">visibility option.</param>
        /// <returns>
        /// The edited
        /// <see cref="PdfFormAnnotation"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormAnnotation SetVisibility(int visibility) {
            switch (visibility) {
                case HIDDEN: {
                    Put(PdfName.F, new PdfNumber(PdfAnnotation.PRINT | PdfAnnotation.HIDDEN));
                    break;
                }

                case VISIBLE_BUT_DOES_NOT_PRINT: {
                    break;
                }

                case HIDDEN_BUT_PRINTABLE: {
                    Put(PdfName.F, new PdfNumber(PdfAnnotation.PRINT | PdfAnnotation.NO_VIEW));
                    break;
                }

                case VISIBLE:
                default: {
                    Put(PdfName.F, new PdfNumber(PdfAnnotation.PRINT));
                    break;
                }
            }
            return this;
        }

        /// <summary>Gets the border width for the field.</summary>
        /// <returns>the current border width.</returns>
        public virtual float GetBorderWidth() {
            PdfDictionary bs = GetWidget().GetBorderStyle();
            if (bs != null) {
                PdfNumber w = bs.GetAsNumber(PdfName.W);
                if (w != null) {
                    borderWidth = w.FloatValue();
                }
            }
            return borderWidth;
        }

        /// <summary>Sets the border width for the field.</summary>
        /// <param name="borderWidth">The new border width.</param>
        /// <returns>
        /// The edited
        /// <see cref="PdfFormAnnotation"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormAnnotation SetBorderWidth(float borderWidth) {
            // Acrobat doesn't support float border width therefore we round it.
            int roundedBorderWidth = (int)MathematicUtil.Round(borderWidth);
            PdfDictionary bs = GetWidget().GetBorderStyle();
            if (bs == null) {
                bs = new PdfDictionary();
                Put(PdfName.BS, bs);
            }
            bs.Put(PdfName.W, new PdfNumber(roundedBorderWidth));
            this.borderWidth = roundedBorderWidth;
            RegenerateField();
            return this;
        }

        /// <summary>Get border object specified in the widget annotation dictionary.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Borders.Border"/>
        /// specified in the widget annotation dictionary
        /// </returns>
        public virtual Border GetBorder() {
            float borderWidth = GetBorderWidth();
            Border border = FormBorderFactory.GetBorder(this.GetWidget().GetBorderStyle(), borderWidth, borderColor, backgroundColor
                );
            if (border == null && borderWidth > 0 && borderColor != null) {
                border = new SolidBorder(borderColor, Math.Max(1, borderWidth));
            }
            return border;
        }

        /// <summary>Sets the border style for the field.</summary>
        /// <param name="style">the new border style.</param>
        /// <returns>
        /// The edited
        /// <see cref="PdfFormAnnotation"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormAnnotation SetBorderStyle(PdfDictionary style) {
            GetWidget().SetBorderStyle(style);
            RegenerateField();
            return this;
        }

        /// <summary>Sets the Border Color.</summary>
        /// <param name="color">the new value for the Border Color.</param>
        /// <returns>
        /// The edited
        /// <see cref="PdfFormAnnotation"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormAnnotation SetBorderColor(Color color) {
            borderColor = color;
            PdfDictionary mk;
            PdfWidgetAnnotation kid = GetWidget();
            mk = kid.GetAppearanceCharacteristics();
            if (mk == null) {
                mk = new PdfDictionary();
            }
            if (borderColor == null) {
                mk.Remove(PdfName.BC);
            }
            else {
                mk.Put(PdfName.BC, new PdfArray(borderColor.GetColorValue()));
            }
            kid.SetAppearanceCharacteristics(mk);
            RegenerateField();
            return this;
        }

        /// <summary>Specifies on which page the form field's widget must be shown.</summary>
        /// <param name="pageNum">the page number.</param>
        /// <returns>
        /// The edited
        /// <see cref="PdfFormAnnotation"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormAnnotation SetPage(int pageNum) {
            PdfWidgetAnnotation widget = GetWidget();
            if (widget != null) {
                widget.SetPage(GetDocument().GetPage(pageNum));
            }
            return this;
        }

        /// <summary>
        /// This method sets the model element associated with the current annotation and can be useful to take into account
        /// when drawing those properties that the annotation does not have.
        /// </summary>
        /// <remarks>
        /// This method sets the model element associated with the current annotation and can be useful to take into account
        /// when drawing those properties that the annotation does not have. Note that annotation properties will take
        /// precedence, so such properties cannot be overridden by using this method (e.g. background, text color, etc.).
        /// <para />
        /// Also note that the model element won't be used for annotations for choice form field.
        /// </remarks>
        /// <param name="element">model element to set.</param>
        /// <returns>
        /// this
        /// <see cref="PdfFormAnnotation"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormAnnotation SetFormFieldElement(IFormField element) {
            this.formFieldElement = element;
            RegenerateWidget();
            return this;
        }

        /// <summary>Sets an appearance for (the widgets related to) the form field.</summary>
        /// <param name="appearanceType">
        /// the type of appearance stream to be added
        /// <list type="bullet">
        /// <item><description> PdfName.N: normal appearance
        /// </description></item>
        /// <item><description> PdfName.R: rollover appearance
        /// </description></item>
        /// <item><description> PdfName.D: down appearance
        /// </description></item>
        /// </list>
        /// </param>
        /// <param name="appearanceState">
        /// the state of the form field that needs to be true
        /// for the appearance to be used. Differentiates between several streams
        /// of the same type.
        /// </param>
        /// <param name="appearanceStream">
        /// the appearance instructions, as a
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
        /// </param>
        /// <returns>
        /// The edited
        /// <see cref="PdfFormAnnotation"/>.
        /// </returns>
        public virtual iText.Forms.Fields.PdfFormAnnotation SetAppearance(PdfName appearanceType, String appearanceState
            , PdfStream appearanceStream) {
            PdfDictionary dic = GetPdfObject();
            PdfDictionary ap = dic.GetAsDictionary(PdfName.AP);
            if (ap != null) {
                PdfDictionary appearanceDictionary = ap.GetAsDictionary(appearanceType);
                if (appearanceDictionary == null) {
                    ap.Put(appearanceType, appearanceStream);
                }
                else {
                    appearanceDictionary.Put(new PdfName(appearanceState), appearanceStream);
                }
            }
            return this;
        }

        /// <summary>
        /// Gets a
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// that matches the current size and position of this form field.
        /// </summary>
        /// <param name="field">current form field.</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// that matches the current size and position of this form field
        /// annotation.
        /// </returns>
        protected internal virtual Rectangle GetRect(PdfDictionary field) {
            PdfArray rect = field.GetAsArray(PdfName.Rect);
            return rect == null ? null : rect.ToRectangle();
        }

        /// <summary>Draws a border using the borderWidth and borderColor of the form field.</summary>
        /// <param name="canvas">
        /// The
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// on which to draw
        /// </param>
        /// <param name="xObject">The PdfFormXObject</param>
        /// <param name="width">The width of the rectangle to draw</param>
        /// <param name="height">The height of the rectangle to draw</param>
        protected internal virtual void DrawBorder(PdfCanvas canvas, PdfFormXObject xObject, float width, float height
            ) {
            canvas.SaveState();
            float borderWidth = GetBorderWidth();
            PdfDictionary bs = GetWidget().GetBorderStyle();
            if (borderWidth < 0) {
                borderWidth = 0;
            }
            if (backgroundColor != null) {
                canvas.SetFillColor(backgroundColor).Rectangle(0, 0, width, height).Fill();
            }
            if (borderWidth > 0 && borderColor != null) {
                borderWidth = Math.Max(1, borderWidth);
                canvas.SetStrokeColor(borderColor).SetLineWidth(borderWidth);
                Border border = FormBorderFactory.GetBorder(bs, borderWidth, borderColor, backgroundColor);
                if (border != null) {
                    float borderWidthX2 = borderWidth + borderWidth;
                    border.Draw(canvas, new Rectangle(borderWidth, borderWidth, width - borderWidthX2, height - borderWidthX2)
                        );
                }
                else {
                    canvas.Rectangle(0, 0, width, height).Stroke();
                }
            }
            PdfArray matrix = GetRotationMatrix(GetRotation() % 360, height, width);
            if (matrix != null) {
                xObject.Put(PdfName.Matrix, matrix);
            }
            canvas.RestoreState();
        }

        /// <summary>Draws the appearance of a push button and saves it into an appearance stream.</summary>
        protected internal virtual void DrawPushButtonFieldAndSaveAppearance() {
            Rectangle rectangle = GetRect(this.GetPdfObject());
            if (rectangle == null) {
                return;
            }
            float width = rectangle.GetWidth();
            float height = rectangle.GetHeight();
            CreateInputButton();
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, width, height));
            PdfArray matrix = GetRotationMatrix(GetRotation() % 360, height, width);
            if (matrix != null) {
                xObject.Put(PdfName.Matrix, matrix);
            }
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(xObject, this.GetDocument());
            SetMetaInfoToCanvas(canvas);
            String caption = parent.GetDisplayValue();
            if (caption != null && !String.IsNullOrEmpty(caption)) {
                ((Button)formFieldElement).SetSingleLineValue(caption);
            }
            float imagePadding = borderColor == null ? 0 : borderWidth;
            if (parent.img != null) {
                // If we got here, the button will only contain the image that the user has set into the annotation.
                // There is no way to pass other elements with this image.
                formFieldElement.GetChildren().Clear();
                Image image = new Image(new PdfImageXObject(parent.img), imagePadding, imagePadding);
                image.SetHeight(height - 2 * imagePadding);
                image.SetWidth(width - 2 * imagePadding);
                ((Button)formFieldElement).Add(image);
            }
            else {
                if (parent.form != null) {
                    // If we got here, the button will only contain the image that the user has set as form into the annotation.
                    // There is no way to pass other elements with this image as form.
                    formFieldElement.GetChildren().Clear();
                    Image image = new Image(parent.form, imagePadding, imagePadding);
                    image.SetHeight(height - 2 * imagePadding);
                    ((Button)formFieldElement).Add(image);
                }
                else {
                    xObject.GetResources().AddFont(GetDocument(), GetFont());
                }
            }
            canvas.Add(formFieldElement);
            PdfDictionary ap = new PdfDictionary();
            PdfStream normalAppearanceStream = xObject.GetPdfObject();
            if (normalAppearanceStream != null) {
                PdfName stateName = GetPdfObject().GetAsName(PdfName.AS);
                if (stateName == null) {
                    stateName = new PdfName("push");
                }
                GetPdfObject().Put(PdfName.AS, stateName);
                PdfDictionary normalAppearance = new PdfDictionary();
                normalAppearance.Put(stateName, normalAppearanceStream);
                ap.Put(PdfName.N, normalAppearance);
                ap.SetModified();
            }
            Put(PdfName.AP, ap);
            // We need to draw waitingDrawingElements (drawn inside close method), but the close method
            // flushes TagTreePointer that will be used later, so set null to the corresponding property.
            canvas.SetProperty(Property.TAGGING_HELPER, null);
            canvas.Close();
        }

        /// <summary>Draws the appearance of a radio button with a specified value and saves it into an appearance stream.
        ///     </summary>
        /// <param name="value">the value of the radio button.</param>
        protected internal virtual void DrawRadioButtonAndSaveAppearance(String value) {
            Rectangle rectangle = GetRect(this.GetPdfObject());
            if (rectangle == null) {
                return;
            }
            if (!(formFieldElement is Radio)) {
                // Create it one time and re-set properties during each widget regeneration.
                formFieldElement = new Radio("");
            }
            SetModelElementProperties(GetRect(GetPdfObject()));
            // First draw off appearance
            ((Radio)formFieldElement).SetChecked(false);
            PdfFormXObject xObjectOff = new PdfFormXObject(new Rectangle(0, 0, rectangle.GetWidth(), rectangle.GetHeight
                ()));
            iText.Layout.Canvas canvasOff = new iText.Layout.Canvas(xObjectOff, this.GetDocument());
            SetMetaInfoToCanvas(canvasOff);
            canvasOff.Add(formFieldElement);
            PdfDictionary normalAppearance = new PdfDictionary();
            normalAppearance.Put(new PdfName(OFF_STATE_VALUE), xObjectOff.GetPdfObject());
            // Draw on appearance
            if (value != null && !String.IsNullOrEmpty(value) && !iText.Forms.Fields.PdfFormAnnotation.OFF_STATE_VALUE
                .Equals(value)) {
                ((Radio)formFieldElement).SetChecked(true);
                PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, rectangle.GetWidth(), rectangle.GetHeight(
                    )));
                iText.Layout.Canvas canvas = new iText.Layout.Canvas(xObject, this.GetDocument());
                SetMetaInfoToCanvas(canvas);
                canvas.Add(formFieldElement);
                normalAppearance.Put(new PdfName(value), xObject.GetPdfObject());
            }
            GetWidget().SetNormalAppearance(normalAppearance);
        }

        /// <summary>Draws the appearance of a list box form field and saves it into an appearance stream.</summary>
        protected internal virtual void DrawListFormFieldAndSaveAppearance() {
            Rectangle rectangle = GetRect(this.GetPdfObject());
            if (rectangle == null) {
                return;
            }
            if (!(formFieldElement is ListBoxField)) {
                // Create it once and reset properties during each widget regeneration.
                formFieldElement = new ListBoxField("", 0, parent.GetFieldFlag(PdfChoiceFormField.FF_MULTI_SELECT));
            }
            formFieldElement.SetProperty(FormProperty.FORM_FIELD_MULTIPLE, parent.GetFieldFlag(PdfChoiceFormField.FF_MULTI_SELECT
                ));
            PdfArray indices = GetParent().GetAsArray(PdfName.I);
            PdfArray options = parent.GetOptions();
            for (int index = 0; index < options.Size(); ++index) {
                PdfObject option = options.Get(index);
                String exportValue = null;
                String displayValue = null;
                if (option.IsString()) {
                    exportValue = option.ToString();
                }
                else {
                    if (option.IsArray()) {
                        PdfArray optionArray = (PdfArray)option;
                        if (optionArray.Size() > 1) {
                            exportValue = optionArray.Get(0).ToString();
                            displayValue = optionArray.Get(1).ToString();
                        }
                    }
                }
                if (exportValue == null) {
                    continue;
                }
                bool selected = indices == null ? false : indices.Contains(new PdfNumber(index));
                SelectFieldItem existingItem = ((ListBoxField)formFieldElement).GetOption(exportValue);
                if (existingItem == null) {
                    existingItem = new SelectFieldItem(exportValue, displayValue);
                    ((ListBoxField)formFieldElement).AddOption(existingItem);
                }
                existingItem.GetElement().SetProperty(Property.TEXT_ALIGNMENT, parent.GetJustification());
                existingItem.GetElement().SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
                existingItem.GetElement().SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                existingItem.GetElement().SetProperty(FormProperty.FORM_FIELD_SELECTED, selected);
            }
            formFieldElement.SetProperty(Property.FONT, GetFont());
            if (GetColor() != null) {
                formFieldElement.SetProperty(Property.FONT_COLOR, new TransparentColor(GetColor()));
            }
            SetModelElementProperties(rectangle);
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, rectangle.GetWidth(), rectangle.GetHeight(
                )));
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(xObject, this.GetDocument());
            SetMetaInfoToCanvas(canvas);
            canvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            canvas.GetPdfCanvas().BeginVariableText().SaveState().EndPath();
            canvas.Add(formFieldElement);
            canvas.GetPdfCanvas().RestoreState().EndVariableText();
            GetWidget().SetNormalAppearance(xObject.GetPdfObject());
        }

        /// <summary>Draws the appearance of a text form field and saves it into an appearance stream.</summary>
        protected internal virtual void DrawTextFormFieldAndSaveAppearance() {
            Rectangle rectangle = GetRect(this.GetPdfObject());
            if (rectangle == null) {
                return;
            }
            String value = parent.GetDisplayValue();
            if (!(parent.IsMultiline() && formFieldElement is TextArea || !parent.IsMultiline() && formFieldElement is
                 InputField)) {
                // Create it one time and re-set properties during each widget regeneration.
                formFieldElement = parent.IsMultiline() ? (IFormField)new TextArea("") : (IFormField)new InputField("");
            }
            if (parent.IsMultiline()) {
                formFieldElement.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(GetFontSize()));
            }
            else {
                formFieldElement.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(GetFontSize(new PdfArray(rectangle
                    ), parent.GetValueAsString())));
                value = iText.Commons.Utils.StringUtil.ReplaceAll(value, LINE_ENDINGS_REGEXP, " ");
            }
            formFieldElement.SetValue(value);
            formFieldElement.SetProperty(Property.FONT, GetFont());
            formFieldElement.SetProperty(Property.TEXT_ALIGNMENT, parent.GetJustification());
            formFieldElement.SetProperty(FormProperty.FORM_FIELD_PASSWORD_FLAG, GetParentField().IsPassword());
            formFieldElement.SetProperty(Property.ADD_MARKED_CONTENT_TEXT, true);
            if (GetColor() != null) {
                formFieldElement.SetProperty(Property.FONT_COLOR, new TransparentColor(GetColor()));
            }
            // Rotation
            int fieldRotation = GetRotation() % 360;
            PdfArray matrix = GetRotationMatrix(fieldRotation, rectangle.GetHeight(), rectangle.GetWidth());
            if (fieldRotation == 90 || fieldRotation == 270) {
                Rectangle invertedRectangle = rectangle.Clone();
                invertedRectangle.SetWidth(rectangle.GetHeight());
                invertedRectangle.SetHeight(rectangle.GetWidth());
                rectangle = invertedRectangle;
            }
            SetModelElementProperties(rectangle);
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, rectangle.GetWidth(), rectangle.GetHeight(
                )));
            if (matrix != null) {
                xObject.Put(PdfName.Matrix, matrix);
            }
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(xObject, this.GetDocument());
            SetMetaInfoToCanvas(canvas);
            canvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            canvas.Add(formFieldElement);
            GetWidget().SetNormalAppearance(xObject.GetPdfObject());
        }

        /// <summary>Draws the appearance of a Combo box form field and saves it into an appearance stream.</summary>
        protected internal virtual void DrawComboBoxAndSaveAppearance() {
            Rectangle rectangle = GetRect(this.GetPdfObject());
            if (rectangle == null) {
                return;
            }
            if (!(formFieldElement is ComboBoxField)) {
                formFieldElement = new ComboBoxField("");
            }
            ComboBoxField comboBoxField = (ComboBoxField)formFieldElement;
            PrepareComboBoxFieldWithCorrectOptionsAndValues(comboBoxField);
            comboBoxField.SetFont(GetFont());
            SetModelElementProperties(rectangle);
            if (GetFontSize() <= 0) {
                Rectangle r2 = rectangle.Clone();
                // because the border is drawn inside the rectangle, we need to take this into account
                float marginToApply = borderWidth;
                r2.ApplyMargins(marginToApply, marginToApply, marginToApply, marginToApply, false);
                UnitValue estimatedFontSize = UnitValue.CreatePointValue(GetFontSize(new PdfArray(r2), parent.GetValueAsString
                    ()));
                comboBoxField.SetFontSize(estimatedFontSize.GetValue());
            }
            else {
                comboBoxField.SetFontSize(GetFontSize());
            }
            if (GetColor() != null) {
                comboBoxField.SetFontColor(GetColor());
            }
            comboBoxField.SetTextAlignment(parent.GetJustification());
            Rectangle pdfXobjectRectangle = new Rectangle(0, 0, rectangle.GetWidth(), rectangle.GetHeight());
            PdfFormXObject xObject = new PdfFormXObject(pdfXobjectRectangle);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(xObject, GetDocument());
            canvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            SetMetaInfoToCanvas(canvas);
            canvas.SetFont(GetFont());
            canvas.GetPdfCanvas().BeginVariableText().SaveState().EndPath();
            canvas.Add(comboBoxField);
            canvas.GetPdfCanvas().RestoreState().EndVariableText();
            GetWidget().SetNormalAppearance(xObject.GetPdfObject());
        }

        private void PrepareComboBoxFieldWithCorrectOptionsAndValues(ComboBoxField comboBoxField) {
            foreach (PdfObject option in parent.GetOptions()) {
                SelectFieldItem item = null;
                if (option.IsString()) {
                    item = new SelectFieldItem(((PdfString)option).GetValue());
                }
                if (option.IsArray()) {
                    System.Diagnostics.Debug.Assert(option is PdfArray);
                    PdfArray array = (PdfArray)option;
                    int thereShouldBeTwoElementsInArray = 2;
                    if (array.Size() == thereShouldBeTwoElementsInArray) {
                        String exportValue = ((PdfString)array.Get(0)).GetValue();
                        String displayValue = ((PdfString)array.Get(1)).GetValue();
                        item = new SelectFieldItem(exportValue, displayValue);
                    }
                }
                if (item != null && comboBoxField.GetOption(item.GetExportValue()) == null) {
                    comboBoxField.AddOption(item);
                }
            }
            comboBoxField.SetSelected(parent.GetDisplayValue());
        }

        /// <summary>Draw a checkbox and save its appearance.</summary>
        /// <param name="onStateName">the name of the appearance state for the checked state</param>
        protected internal virtual void DrawCheckBoxAndSaveAppearance(String onStateName) {
            Rectangle rect = GetRect(this.GetPdfObject());
            if (rect == null) {
                return;
            }
            ReconstructCheckBoxType();
            CreateCheckBox();
            if (GetWidget().GetNormalAppearanceObject() == null) {
                GetWidget().SetNormalAppearance(new PdfDictionary());
            }
            PdfDictionary normalAppearance = new PdfDictionary();
            ((CheckBox)formFieldElement).SetChecked(false);
            PdfFormXObject xObjectOff = new PdfFormXObject(new Rectangle(0, 0, rect.GetWidth(), rect.GetHeight()));
            iText.Layout.Canvas canvasOff = new iText.Layout.Canvas(xObjectOff, GetDocument());
            SetMetaInfoToCanvas(canvasOff);
            canvasOff.Add(formFieldElement);
            if (GetPdfAConformanceLevel() == null) {
                xObjectOff.GetResources().AddFont(GetDocument(), GetFont());
            }
            normalAppearance.Put(new PdfName(OFF_STATE_VALUE), xObjectOff.GetPdfObject());
            String onStateNameForAp = onStateName;
            if (onStateName == null || String.IsNullOrEmpty(onStateName) || iText.Forms.Fields.PdfFormAnnotation.OFF_STATE_VALUE
                .Equals(onStateName)) {
                onStateNameForAp = ON_STATE_VALUE;
            }
            ((CheckBox)formFieldElement).SetChecked(true);
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, rect.GetWidth(), rect.GetHeight()));
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(xObject, this.GetDocument());
            SetMetaInfoToCanvas(canvas);
            canvas.Add(formFieldElement);
            normalAppearance.Put(new PdfName(onStateNameForAp), xObject.GetPdfObject());
            GetWidget().SetNormalAppearance(normalAppearance);
            PdfDictionary mk = new PdfDictionary();
            // We put the zapfDingbats code of the checkbox in the MK dictionary to make sure there is a way
            // to retrieve the checkbox type even if the appearance is not present.
            mk.Put(PdfName.CA, new PdfString(PdfCheckBoxRenderingStrategy.ZAPFDINGBATS_CHECKBOX_MAPPING.GetByKey(parent
                .checkType.GetValue())));
            GetWidget().Put(PdfName.MK, mk);
            SetCheckBoxAppearanceState(onStateName);
        }

        internal static void SetMetaInfoToCanvas(iText.Layout.Canvas canvas) {
            MetaInfoContainer metaInfo = FormsMetaInfoStaticContainer.GetMetaInfoForLayout();
            if (metaInfo != null) {
                canvas.SetProperty(Property.META_INFO, metaInfo);
            }
        }

        internal virtual bool RegenerateWidget() {
            if (parent == null) {
                return true;
            }
            PdfName type = parent.GetFormType();
            if ((PdfName.Ch.Equals(type) && parent.GetFieldFlag(PdfChoiceFormField.FF_COMBO)) || this.IsCombTextFormField
                ()) {
                if (parent.GetFieldFlag(PdfChoiceFormField.FF_COMBO) && formFieldElement != null) {
                    DrawComboBoxAndSaveAppearance();
                    return true;
                }
                return TextAndChoiceLegacyDrawer.RegenerateTextAndChoiceField(this);
            }
            else {
                if (PdfName.Ch.Equals(type) && !parent.GetFieldFlag(PdfChoiceFormField.FF_COMBO)) {
                    if (formFieldElement != null) {
                        DrawListFormFieldAndSaveAppearance();
                        return true;
                    }
                    else {
                        return TextAndChoiceLegacyDrawer.RegenerateTextAndChoiceField(this);
                    }
                }
                else {
                    if (PdfName.Tx.Equals(type)) {
                        DrawTextFormFieldAndSaveAppearance();
                        return true;
                    }
                    else {
                        if (PdfName.Btn.Equals(type)) {
                            if (parent.GetFieldFlag(PdfButtonFormField.FF_PUSH_BUTTON)) {
                                DrawPushButtonFieldAndSaveAppearance();
                            }
                            else {
                                if (parent.GetFieldFlag(PdfButtonFormField.FF_RADIO)) {
                                    DrawRadioButtonAndSaveAppearance(GetRadioButtonValue());
                                }
                                else {
                                    DrawCheckBoxAndSaveAppearance(parent.GetValueAsString());
                                }
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal virtual void CreateInputButton() {
            if (!(formFieldElement is Button)) {
                // Create it one time and re-set properties during each widget regeneration.
                formFieldElement = new Button(parent.GetFieldName().ToUnicodeString());
            }
            ((Button)formFieldElement).SetFont(GetFont());
            ((Button)formFieldElement).SetFontSize(GetFontSize(GetPdfObject().GetAsArray(PdfName.Rect), parent.GetDisplayValue
                ()));
            if (GetColor() != null) {
                ((Button)formFieldElement).SetFontColor(color);
            }
            SetModelElementProperties(GetRect(GetPdfObject()));
        }

        internal virtual float GetFontSize(PdfArray bBox, String value) {
            if (GetFontSize() == 0) {
                if (bBox == null || value == null || String.IsNullOrEmpty(value)) {
                    return DEFAULT_FONT_SIZE;
                }
                else {
                    return FontSizeUtil.ApproximateFontSizeToFitSingleLine(GetFont(), bBox.ToRectangle(), value, MIN_FONT_SIZE
                        , borderWidth);
                }
            }
            return GetFontSize();
        }

        private bool IsCombTextFormField() {
            PdfName type = parent.GetFormType();
            if (PdfName.Tx.Equals(type) && parent.GetFieldFlag(PdfTextFormField.FF_COMB)) {
                int maxLen = PdfFormCreator.CreateTextFormField(parent.GetPdfObject()).GetMaxLen();
                if (maxLen == 0 || parent.IsMultiline()) {
                    LOGGER.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.COMB_FLAG_MAY_BE_SET_ONLY_IF_MAXLEN_IS_PRESENT
                        ));
                    return false;
                }
                return true;
            }
            return false;
        }

        private String GetRadioButtonValue() {
            foreach (String state in GetAppearanceStates()) {
                if (!OFF_STATE_VALUE.Equals(state)) {
                    return state;
                }
            }
            return null;
        }

        private void SetCheckBoxAppearanceState(String onStateName) {
            PdfWidgetAnnotation widget = GetWidget();
            if (widget.GetNormalAppearanceObject() != null && widget.GetNormalAppearanceObject().ContainsKey(new PdfName
                (onStateName))) {
                widget.SetAppearanceState(new PdfName(onStateName));
            }
            else {
                widget.SetAppearanceState(new PdfName(OFF_STATE_VALUE));
            }
        }

        private void ReconstructCheckBoxType() {
            // if checkbox type is null it means we are reading from a document and we need to retrieve the type from the
            // mk dictionary in the ca
            if (parent.checkType == null) {
                PdfDictionary oldMk = GetWidget().GetAppearanceCharacteristics();
                if (oldMk != null) {
                    PdfString oldCa = oldMk.GetAsString(PdfName.CA);
                    if (oldCa != null && PdfCheckBoxRenderingStrategy.ZAPFDINGBATS_CHECKBOX_MAPPING.ContainsValue(oldCa.GetValue
                        ())) {
                        parent.checkType = new NullableContainer<CheckBoxType>(PdfCheckBoxRenderingStrategy.ZAPFDINGBATS_CHECKBOX_MAPPING
                            .GetByValue(oldCa.GetValue()));
                        // we need to set the font size to 0 to make sure the font size is recalculated
                        fontSize = 0;
                    }
                }
            }
            // if its still null default to default value
            if (parent.checkType == null) {
                parent.checkType = new NullableContainer<CheckBoxType>(CheckBoxType.CROSS);
            }
        }

        private void CreateCheckBox() {
            if (!(formFieldElement is CheckBox)) {
                // Create it one time and re-set properties during each widget regeneration.
                formFieldElement = new CheckBox("");
            }
            formFieldElement.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(GetFontSize()));
            SetModelElementProperties(GetRect(GetPdfObject()));
            ((CheckBox)formFieldElement).SetPdfAConformanceLevel(GetPdfAConformanceLevel());
            ((CheckBox)formFieldElement).SetCheckBoxType(parent.checkType.GetValue());
        }

        private void SetModelElementProperties(Rectangle rectangle) {
            if (backgroundColor != null) {
                formFieldElement.SetProperty(Property.BACKGROUND, new Background(backgroundColor));
            }
            formFieldElement.SetProperty(Property.BORDER, GetBorder());
            // Set fixed size
            BoxSizingPropertyValue? boxSizing = formFieldElement.GetProperty<BoxSizingPropertyValue?>(Property.BOX_SIZING
                );
            // Borders are already taken into account for rectangle area, but shouldn't be included into width and height
            // of the field in case of content-box value of box-sizing property.
            float extraBorderWidth = BoxSizingPropertyValue.CONTENT_BOX == boxSizing ? 2 * borderWidth : 0;
            formFieldElement.SetWidth(rectangle.GetWidth() - extraBorderWidth);
            formFieldElement.SetHeight(rectangle.GetHeight() - extraBorderWidth);
            // Always flatten
            formFieldElement.SetInteractive(false);
        }

        private static PdfArray GetRotationMatrix(int rotation, float height, float width) {
            switch (rotation) {
                case 0: {
                    return null;
                }

                case 90: {
                    return new PdfArray(new float[] { 0, 1, -1, 0, height, 0 });
                }

                case 180: {
                    return new PdfArray(new float[] { -1, 0, 0, -1, width, height });
                }

                case 270: {
                    return new PdfArray(new float[] { 0, -1, 1, 0, 0, width });
                }

                default: {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Fields.PdfFormAnnotation));
                    logger.LogError(FormsLogMessageConstants.INCORRECT_WIDGET_ROTATION);
                    return null;
                }
            }
        }

        private static Color AppearancePropToColor(PdfDictionary appearanceCharacteristics, PdfName property) {
            PdfArray colorData = appearanceCharacteristics.GetAsArray(property);
            if (colorData != null) {
                float[] backgroundFloat = new float[colorData.Size()];
                for (int i = 0; i < colorData.Size(); i++) {
                    backgroundFloat[i] = colorData.GetAsNumber(i).FloatValue();
                }
                switch (colorData.Size()) {
                    case 0: {
                        return null;
                    }

                    case 1: {
                        return new DeviceGray(backgroundFloat[0]);
                    }

                    case 3: {
                        return new DeviceRgb(backgroundFloat[0], backgroundFloat[1], backgroundFloat[2]);
                    }

                    case 4: {
                        return new DeviceCmyk(backgroundFloat[0], backgroundFloat[1], backgroundFloat[2], backgroundFloat[3]);
                    }
                }
            }
            return null;
        }
    }
}

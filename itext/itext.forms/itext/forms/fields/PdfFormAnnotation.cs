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
using iText.Commons.Utils;
using iText.Forms.Fields.Borders;
using iText.Forms.Fields.Properties;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Logs;
using iText.Forms.Util;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
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

        /// <summary>Default padding X offset.</summary>
        internal const float X_OFFSET = 2;

        protected internal float borderWidth = 1;

        protected internal Color backgroundColor;

        protected internal Color borderColor;

        protected internal int rotation = 0;

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
        internal PdfFormAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
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
                this.rotation = degRotation;
            }
            PdfDictionary mk = GetWidget().GetAppearanceCharacteristics();
            if (mk == null) {
                mk = new PdfDictionary();
                this.Put(PdfName.MK, mk);
            }
            mk.Put(PdfName.R, new PdfNumber(degRotation));
            this.rotation = degRotation;
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
            PdfDictionary bs = GetWidget().GetBorderStyle();
            if (bs == null) {
                bs = new PdfDictionary();
                Put(PdfName.BS, bs);
            }
            bs.Put(PdfName.W, new PdfNumber(borderWidth));
            this.borderWidth = borderWidth;
            RegenerateField();
            return this;
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
                field = new iText.Forms.Fields.PdfFormAnnotation((PdfWidgetAnnotation)PdfAnnotation.MakeAnnotation(dictionary
                    ), document);
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

        /// <summary>Draws the visual appearance of text in a form field.</summary>
        /// <param name="rect">The location on the page for the list field.</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </param>
        /// <param name="fontSize">The size of the font.</param>
        /// <param name="value">The initial value.</param>
        /// <param name="appearance">The appearance.</param>
        protected internal virtual void DrawTextAppearance(Rectangle rect, PdfFont font, float fontSize, String value
            , PdfFormXObject appearance) {
            PdfStream stream = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfResources resources = appearance.GetResources();
            PdfCanvas canvas = new PdfCanvas(stream, resources, GetDocument());
            float height = rect.GetHeight();
            float width = rect.GetWidth();
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, width, height));
            DrawBorder(canvas, xObject, width, height);
            if (parent.IsPassword()) {
                value = ObfuscatePassword(value);
            }
            canvas.BeginVariableText().SaveState().EndPath();
            TextAlignment? textAlignment = parent.ConvertJustificationToTextAlignment();
            float x = 0;
            if (textAlignment == TextAlignment.RIGHT) {
                x = rect.GetWidth();
            }
            else {
                if (textAlignment == TextAlignment.CENTER) {
                    x = rect.GetWidth() / 2;
                }
            }
            iText.Layout.Canvas modelCanvas = new iText.Layout.Canvas(canvas, new Rectangle(0, -height, 0, 2 * height)
                );
            modelCanvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            SetMetaInfoToCanvas(modelCanvas);
            Style paragraphStyle = new Style().SetFont(font).SetFontSize(fontSize);
            paragraphStyle.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 1));
            if (GetColor() != null) {
                paragraphStyle.SetProperty(Property.FONT_COLOR, new TransparentColor(GetColor()));
            }
            int maxLen = new PdfTextFormField(parent.GetPdfObject()).GetMaxLen();
            // check if /Comb has been set
            if (parent.GetFieldFlag(PdfTextFormField.FF_COMB) && 0 != maxLen) {
                float widthPerCharacter = width / maxLen;
                int numberOfCharacters = Math.Min(maxLen, value.Length);
                int start;
                switch (textAlignment) {
                    case TextAlignment.RIGHT: {
                        start = (maxLen - numberOfCharacters);
                        break;
                    }

                    case TextAlignment.CENTER: {
                        start = (maxLen - numberOfCharacters) / 2;
                        break;
                    }

                    default: {
                        start = 0;
                        break;
                    }
                }
                float startOffset = widthPerCharacter * (start + 0.5f);
                for (int i = 0; i < numberOfCharacters; i++) {
                    modelCanvas.ShowTextAligned(new Paragraph(value.JSubstring(i, i + 1)).AddStyle(paragraphStyle), startOffset
                         + widthPerCharacter * i, rect.GetHeight() / 2, TextAlignment.CENTER, VerticalAlignment.MIDDLE);
                }
            }
            else {
                if (parent.GetFieldFlag(PdfTextFormField.FF_COMB)) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Fields.PdfFormAnnotation));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.COMB_FLAG_MAY_BE_SET_ONLY_IF_MAXLEN_IS_PRESENT
                        ));
                }
                modelCanvas.ShowTextAligned(CreateParagraphForTextFieldValue(value).AddStyle(paragraphStyle).SetPaddings(0
                    , X_OFFSET, 0, X_OFFSET), x, rect.GetHeight() / 2, textAlignment, VerticalAlignment.MIDDLE);
            }
            canvas.RestoreState().EndVariableText();
            appearance.GetPdfObject().SetData(stream.GetBytes());
        }

        protected internal virtual void DrawMultiLineTextAppearance(Rectangle rect, PdfFont font, String value, PdfFormXObject
             appearance) {
            PdfStream stream = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfResources resources = appearance.GetResources();
            PdfCanvas canvas = new PdfCanvas(stream, resources, GetDocument());
            float width = rect.GetWidth();
            float height = rect.GetHeight();
            DrawBorder(canvas, appearance, width, height);
            canvas.BeginVariableText();
            Rectangle areaRect = new Rectangle(0, 0, width, height);
            iText.Layout.Canvas modelCanvas = new iText.Layout.Canvas(canvas, areaRect);
            modelCanvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            SetMetaInfoToCanvas(modelCanvas);
            Paragraph paragraph = CreateParagraphForTextFieldValue(value).SetFont(font).SetMargin(0).SetPadding(3).SetMultipliedLeading
                (1);
            if (GetFontSize() == 0) {
                paragraph.SetFontSize(ApproximateFontSizeToFitMultiLine(paragraph, areaRect, modelCanvas.GetRenderer()));
            }
            else {
                paragraph.SetFontSize(GetFontSize());
            }
            paragraph.SetProperty(Property.FORCED_PLACEMENT, true);
            paragraph.SetTextAlignment(parent.ConvertJustificationToTextAlignment());
            if (GetColor() != null) {
                paragraph.SetFontColor(GetColor());
            }
            // here we subtract an epsilon to make sure that element won't be split but overflown
            paragraph.SetHeight(height - 0.00001f);
            paragraph.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            paragraph.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.FIT);
            paragraph.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.HIDDEN);
            modelCanvas.Add(paragraph);
            canvas.EndVariableText();
            appearance.GetPdfObject().SetData(stream.GetBytes());
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
            ApplyRotation(xObject, height, width);
            canvas.RestoreState();
        }

        /// <summary>Draws the appearance of a checkbox with a specified state value.</summary>
        /// <param name="width">the width of the checkbox to draw</param>
        /// <param name="height">the height of the checkbox to draw</param>
        /// <param name="onStateName">the state of the form field that will be drawn</param>
        protected internal virtual void DrawCheckAppearance(float width, float height, String onStateName) {
            //TODO DEVSIX-7426 remove method
            Rectangle rect = new Rectangle(0, 0, width, height);
            PdfStream streamOn = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfCanvas canvasOn = new PdfCanvas(streamOn, new PdfResources(), GetDocument());
            PdfFormXObject xObjectOn = new PdfFormXObject(rect);
            DrawBorder(canvasOn, xObjectOn, width, height);
            DrawCheckBox(canvasOn, width, height, GetFontSize());
            xObjectOn.GetPdfObject().GetOutputStream().WriteBytes(streamOn.GetBytes());
            xObjectOn.GetResources().AddFont(GetDocument(), GetFont());
            PdfStream streamOff = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfCanvas canvasOff = new PdfCanvas(streamOff, new PdfResources(), GetDocument());
            PdfFormXObject xObjectOff = new PdfFormXObject(rect);
            DrawBorder(canvasOff, xObjectOff, width, height);
            xObjectOff.GetPdfObject().GetOutputStream().WriteBytes(streamOff.GetBytes());
            xObjectOff.GetResources().AddFont(GetDocument(), GetFont());
            PdfDictionary normalAppearance = new PdfDictionary();
            normalAppearance.Put(new PdfName(onStateName), xObjectOn.GetPdfObject());
            normalAppearance.Put(new PdfName(OFF_STATE_VALUE), xObjectOff.GetPdfObject());
            PdfDictionary mk = new PdfDictionary();
            mk.Put(PdfName.CA, new PdfString(parent.text));
            PdfWidgetAnnotation widget = GetWidget();
            widget.Put(PdfName.MK, mk);
            widget.SetNormalAppearance(normalAppearance);
        }

        /// <summary>Draws PDF/A-2 compliant check appearance.</summary>
        /// <remarks>
        /// Draws PDF/A-2 compliant check appearance.
        /// Actually it's just PdfA check appearance. According to corrigendum there is no difference between them
        /// </remarks>
        /// <param name="width">width of the checkbox</param>
        /// <param name="height">height of the checkbox</param>
        /// <param name="onStateName">name that corresponds to the "On" state of the checkbox</param>
        /// <param name="checkType">
        /// the type that determines how the checkbox will look like. Instance of
        /// <see cref="iText.Forms.Fields.Properties.CheckBoxType"/>
        /// </param>
        protected internal virtual void DrawPdfA2CheckAppearance(float width, float height, String onStateName, CheckBoxType
             checkType) {
            //TODO DEVSIX-7426 remove method
            parent.checkType = checkType;
            Rectangle rect = new Rectangle(0, 0, width, height);
            PdfStream streamOn = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfCanvas canvasOn = new PdfCanvas(streamOn, new PdfResources(), GetDocument());
            PdfFormXObject xObjectOn = new PdfFormXObject(rect);
            xObjectOn.GetResources();
            DrawBorder(canvasOn, xObjectOn, width, height);
            DrawPdfACheckBox(canvasOn, width, height, true);
            xObjectOn.GetPdfObject().GetOutputStream().WriteBytes(streamOn.GetBytes());
            PdfStream streamOff = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfCanvas canvasOff = new PdfCanvas(streamOff, new PdfResources(), GetDocument());
            PdfFormXObject xObjectOff = new PdfFormXObject(rect);
            xObjectOff.GetResources();
            DrawBorder(canvasOff, xObjectOff, width, height);
            xObjectOff.GetPdfObject().GetOutputStream().WriteBytes(streamOff.GetBytes());
            PdfDictionary normalAppearance = new PdfDictionary();
            normalAppearance.Put(new PdfName(onStateName), xObjectOn.GetPdfObject());
            normalAppearance.Put(new PdfName(OFF_STATE_VALUE), xObjectOff.GetPdfObject());
            PdfDictionary mk = new PdfDictionary();
            mk.Put(PdfName.CA, new PdfString(parent.text));
            PdfWidgetAnnotation widget = GetWidget();
            widget.Put(PdfName.MK, mk);
            widget.SetNormalAppearance(normalAppearance);
        }

        /// <summary>Draws the appearance for a push button.</summary>
        /// <param name="width">the width of the pushbutton</param>
        /// <param name="height">the width of the pushbutton</param>
        /// <param name="text">the text to display on the button</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        /// <returns>
        /// a new
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// </returns>
        protected internal virtual PdfFormXObject DrawPushButtonAppearance(float width, float height, String text, 
            PdfFont font, float fontSize) {
            PdfStream stream = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfCanvas canvas = new PdfCanvas(stream, new PdfResources(), GetDocument());
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, width, height));
            DrawBorder(canvas, xObject, width, height);
            if (parent.img != null) {
                PdfImageXObject imgXObj = new PdfImageXObject(parent.img);
                canvas.AddXObjectWithTransformationMatrix(imgXObj, width - borderWidth, 0, 0, height - borderWidth, borderWidth
                     / 2, borderWidth / 2);
                xObject.GetResources().AddImage(imgXObj);
            }
            else {
                if (parent.form != null) {
                    canvas.AddXObjectWithTransformationMatrix(parent.form, (height - borderWidth) / parent.form.GetHeight(), 0
                        , 0, (height - borderWidth) / parent.form.GetHeight(), borderWidth / 2, borderWidth / 2);
                    xObject.GetResources().AddForm(parent.form);
                }
                else {
                    DrawButton(canvas, 0, 0, width, height, text, font, fontSize);
                    xObject.GetResources().AddFont(GetDocument(), font);
                }
            }
            xObject.GetPdfObject().GetOutputStream().WriteBytes(stream.GetBytes());
            return xObject;
        }

        /// <summary>Performs the low-level drawing operations to draw a button object.</summary>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// of the page to draw on.
        /// </param>
        /// <param name="x">will be ignored, according to spec it shall be 0</param>
        /// <param name="y">will be ignored, according to spec it shall be 0</param>
        /// <param name="width">the width of the button</param>
        /// <param name="height">the width of the button</param>
        /// <param name="text">the text to display on the button</param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </param>
        /// <param name="fontSize">the size of the font</param>
        protected internal virtual void DrawButton(PdfCanvas canvas, float x, float y, float width, float height, 
            String text, PdfFont font, float fontSize) {
            if (GetColor() == null) {
                color = ColorConstants.BLACK;
            }
            if (text == null) {
                text = "";
            }
            Paragraph paragraph = new Paragraph(text).SetFont(font).SetFontSize(fontSize).SetMargin(0).SetMultipliedLeading
                (1).SetVerticalAlignment(VerticalAlignment.MIDDLE);
            iText.Layout.Canvas modelCanvas = new iText.Layout.Canvas(canvas, new Rectangle(0, -height, width, 2 * height
                ));
            modelCanvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            SetMetaInfoToCanvas(modelCanvas);
            modelCanvas.ShowTextAligned(paragraph, width / 2, height / 2, TextAlignment.CENTER, VerticalAlignment.MIDDLE
                );
        }

        /// <summary>Performs the low-level drawing operations to draw a checkbox object.</summary>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// of the page to draw on.
        /// </param>
        /// <param name="width">the width of the button</param>
        /// <param name="height">the width of the button</param>
        /// <param name="fontSize">the size of the font</param>
        protected internal virtual void DrawCheckBox(PdfCanvas canvas, float width, float height, float fontSize) {
            //TODO DEVSIX-7426 remove method
            if (parent.checkType == CheckBoxType.CROSS) {
                DrawingUtil.DrawCross(canvas, width, height, borderWidth);
                return;
            }
            PdfFont ufont = GetFont();
            if (fontSize <= 0) {
                // there is no min font size for checkbox, however we can't set 0, because it means auto size.
                fontSize = ApproximateFontSizeToFitSingleLine(ufont, new Rectangle(width, height), parent.text, 0.1f);
            }
            // PdfFont gets all width in 1000 normalized units
            canvas.BeginText().SetFontAndSize(ufont, fontSize).ResetFillColorRgb().SetTextMatrix((width - ufont.GetWidth
                (parent.text, fontSize)) / 2, (height - ufont.GetAscent(parent.text, fontSize)) / 2).ShowText(parent.text
                ).EndText();
        }

        //TODO DEVSIX-7426 remove method
        protected internal virtual void DrawPdfACheckBox(PdfCanvas canvas, float width, float height, bool on) {
            if (!on) {
                return;
            }
            switch (parent.checkType) {
                case CheckBoxType.CHECK: {
                    DrawingUtil.DrawPdfACheck(canvas, width, height);
                    break;
                }

                case CheckBoxType.CIRCLE: {
                    DrawingUtil.DrawPdfACircle(canvas, width, height);
                    break;
                }

                case CheckBoxType.CROSS: {
                    DrawingUtil.DrawPdfACross(canvas, width, height);
                    break;
                }

                case CheckBoxType.DIAMOND: {
                    DrawingUtil.DrawPdfADiamond(canvas, width, height);
                    break;
                }

                case CheckBoxType.SQUARE: {
                    DrawingUtil.DrawPdfASquare(canvas, width, height);
                    break;
                }

                case CheckBoxType.STAR: {
                    DrawingUtil.DrawPdfAStar(canvas, width, height);
                    break;
                }
            }
        }

        /// <summary>Draws the appearance of a radio button with a specified value and saves it into an appearance stream.
        ///     </summary>
        /// <param name="value">the value of the radio button.</param>
        protected internal virtual void DrawRadioButtonAndSaveAppearance(String value) {
            Rectangle rectangle = GetRect(this.GetPdfObject());
            if (rectangle == null) {
                return;
            }
            Radio formField = CreateRadio();
            // First draw off appearance
            formField.SetChecked(false);
            PdfFormXObject xObjectOff = new PdfFormXObject(new Rectangle(0, 0, rectangle.GetWidth(), rectangle.GetHeight
                ()));
            iText.Layout.Canvas canvasOff = new iText.Layout.Canvas(xObjectOff, this.GetDocument());
            canvasOff.Add(formField);
            PdfDictionary normalAppearance = new PdfDictionary();
            normalAppearance.Put(new PdfName(OFF_STATE_VALUE), xObjectOff.GetPdfObject());
            // Draw on appearance
            if (value != null && !String.IsNullOrEmpty(value) && !iText.Forms.Fields.PdfFormAnnotation.OFF_STATE_VALUE
                .Equals(value)) {
                formField.SetChecked(true);
                PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, rectangle.GetWidth(), rectangle.GetHeight(
                    )));
                iText.Layout.Canvas canvas = new iText.Layout.Canvas(xObject, this.GetDocument());
                canvas.Add(formField);
                normalAppearance.Put(new PdfName(value), xObject.GetPdfObject());
            }
            GetWidget().SetNormalAppearance(normalAppearance);
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

        /// <summary>Draws the visual appearance of Choice box in a form field.</summary>
        /// <param name="rect">The location on the page for the list field</param>
        /// <param name="value">The initial value</param>
        /// <param name="appearance">The appearance</param>
        internal virtual void DrawChoiceAppearance(Rectangle rect, float fontSize, String value, PdfFormXObject appearance
            , int topIndex) {
            PdfStream stream = (PdfStream)new PdfStream().MakeIndirect(GetDocument());
            PdfResources resources = appearance.GetResources();
            PdfCanvas canvas = new PdfCanvas(stream, resources, GetDocument());
            float width = rect.GetWidth();
            float height = rect.GetHeight();
            float widthBorder = 6.0f;
            float heightBorder = 2.0f;
            DrawBorder(canvas, appearance, width, height);
            canvas.BeginVariableText().SaveState().Rectangle(3, 3, width - widthBorder, height - heightBorder).Clip().
                EndPath();
            iText.Layout.Canvas modelCanvas = new iText.Layout.Canvas(canvas, new Rectangle(3, 0, Math.Max(0, width - 
                widthBorder), Math.Max(0, height - heightBorder)));
            modelCanvas.SetProperty(Property.APPEARANCE_STREAM_LAYOUT, true);
            SetMetaInfoToCanvas(modelCanvas);
            Div div = new Div();
            if (parent.GetFieldFlag(PdfChoiceFormField.FF_COMBO)) {
                div.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }
            div.SetHeight(Math.Max(0, height - heightBorder));
            IList<String> strings = GetFont().SplitString(value, fontSize, width - widthBorder);
            for (int index = 0; index < strings.Count; index++) {
                bool? isFull = modelCanvas.GetRenderer().GetPropertyAsBoolean(Property.FULL);
                if (true.Equals(isFull)) {
                    break;
                }
                Paragraph paragraph = new Paragraph(strings[index]).SetFont(GetFont()).SetFontSize(fontSize).SetMargins(0, 
                    0, 0, 0).SetMultipliedLeading(1);
                paragraph.SetProperty(Property.FORCED_PLACEMENT, true);
                paragraph.SetTextAlignment(parent.ConvertJustificationToTextAlignment());
                if (GetColor() != null) {
                    paragraph.SetFontColor(GetColor());
                }
                if (!parent.GetFieldFlag(PdfChoiceFormField.FF_COMBO)) {
                    PdfArray indices = GetParent().GetAsArray(PdfName.I);
                    if (indices != null && indices.Size() > 0) {
                        foreach (PdfObject ind in indices) {
                            if (!ind.IsNumber()) {
                                continue;
                            }
                            if (((PdfNumber)ind).GetValue() == index + topIndex) {
                                paragraph.SetBackgroundColor(new DeviceRgb(10, 36, 106));
                                paragraph.SetFontColor(ColorConstants.LIGHT_GRAY);
                            }
                        }
                    }
                }
                div.Add(paragraph);
            }
            modelCanvas.Add(div);
            canvas.RestoreState().EndVariableText();
            appearance.GetPdfObject().SetData(stream.GetBytes());
        }

        internal static void CreatePushButtonAppearanceState(PdfDictionary widget) {
            PdfDictionary appearances = widget.GetAsDictionary(PdfName.AP);
            PdfStream normalAppearanceStream = appearances.GetAsStream(PdfName.N);
            if (normalAppearanceStream != null) {
                PdfName stateName = widget.GetAsName(PdfName.AS);
                if (stateName == null) {
                    stateName = new PdfName("push");
                }
                widget.Put(PdfName.AS, stateName);
                PdfDictionary normalAppearance = new PdfDictionary();
                normalAppearance.Put(stateName, normalAppearanceStream);
                appearances.Put(PdfName.N, normalAppearance);
            }
        }

        internal static void SetMetaInfoToCanvas(iText.Layout.Canvas canvas) {
            MetaInfoContainer metaInfo = FormsMetaInfoStaticContainer.GetMetaInfoForLayout();
            if (metaInfo != null) {
                canvas.SetProperty(Property.META_INFO, metaInfo);
            }
        }

        internal virtual void RegeneratePushButtonField() {
            PdfDictionary widget = GetPdfObject();
            PdfFormXObject appearance;
            Rectangle rect = GetRect(widget);
            PdfDictionary apDic = widget.GetAsDictionary(PdfName.AP);
            if (apDic == null) {
                Put(PdfName.AP, apDic = new PdfDictionary());
            }
            appearance = DrawPushButtonAppearance(rect.GetWidth(), rect.GetHeight(), parent.GetDisplayValue(), GetFont
                (), GetFontSize(widget.GetAsArray(PdfName.Rect), parent.GetDisplayValue()));
            apDic.Put(PdfName.N, appearance.GetPdfObject());
            if (GetPdfAConformanceLevel() != null) {
                CreatePushButtonAppearanceState(widget);
            }
        }

        //TODO DEVSIX-7426 remove method
        internal virtual void RegenerateCheckboxField(CheckBoxType checkType) {
            parent.SetCheckType(checkType);
            String value = parent.GetValueAsString();
            Rectangle rect = GetRect(GetPdfObject());
            PdfWidgetAnnotation widget = (PdfWidgetAnnotation)PdfAnnotation.MakeAnnotation(GetPdfObject());
            if (GetPdfAConformanceLevel() == null) {
                DrawCheckAppearance(rect.GetWidth(), rect.GetHeight(), OFF_STATE_VALUE.Equals(value) ? ON_STATE_VALUE : value
                    );
            }
            else {
                DrawPdfA2CheckAppearance(rect.GetWidth(), rect.GetHeight(), OFF_STATE_VALUE.Equals(value) ? ON_STATE_VALUE
                     : value, parent.checkType);
                widget.SetFlag(PdfAnnotation.PRINT);
            }
            if (widget.GetNormalAppearanceObject() != null && widget.GetNormalAppearanceObject().ContainsKey(new PdfName
                (value))) {
                widget.SetAppearanceState(new PdfName(value));
            }
            else {
                widget.SetAppearanceState(new PdfName(OFF_STATE_VALUE));
            }
        }

        internal virtual bool RegenerateTextAndChoiceField() {
            String value = parent.GetDisplayValue();
            PdfName type = parent.GetFormType();
            PdfPage page = PdfAnnotation.MakeAnnotation(GetPdfObject()).GetPage();
            PdfArray bBox = GetPdfObject().GetAsArray(PdfName.Rect);
            //Apply Page rotation
            int pageRotation = 0;
            if (page != null) {
                pageRotation = page.GetRotation();
                //Clockwise, so negative
                pageRotation *= -1;
            }
            PdfArray matrix;
            if (pageRotation % 90 == 0) {
                //Cast angle to [-360, 360]
                double angle = pageRotation % 360;
                //Get angle in radians
                angle = DegreeToRadians(angle);
                Rectangle initialBboxRectangle = bBox.ToRectangle();
                //rotate the bounding box
                Rectangle rect = initialBboxRectangle.Clone();
                //Calculate origin offset
                double translationWidth = 0;
                double translationHeight = 0;
                if (angle >= -1 * Math.PI && angle <= -1 * Math.PI / 2) {
                    translationWidth = rect.GetWidth();
                }
                if (angle <= -1 * Math.PI) {
                    translationHeight = rect.GetHeight();
                }
                //Store rotation and translation in the matrix
                matrix = new PdfArray(new double[] { Math.Cos(angle), -Math.Sin(angle), Math.Sin(angle), Math.Cos(angle), 
                    translationWidth, translationHeight });
                // If the angle is a multiple of 90 and not a multiple of 180, height and width of the bounding box
                // need to be switched
                if (angle % (Math.PI / 2) == 0 && angle % (Math.PI) != 0) {
                    rect.SetWidth(initialBboxRectangle.GetHeight());
                    rect.SetHeight(initialBboxRectangle.GetWidth());
                }
                // Adapt origin
                rect.SetX(rect.GetX() + (float)translationWidth);
                rect.SetY(rect.GetY() + (float)translationHeight);
                //Copy Bounding box
                bBox = new PdfArray(rect);
            }
            else {
                //Avoid NPE when handling corrupt pdfs
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Fields.PdfFormAnnotation));
                logger.LogError(FormsLogMessageConstants.INCORRECT_PAGEROTATION);
                matrix = new PdfArray(new double[] { 1, 0, 0, 1, 0, 0 });
            }
            //Apply field rotation
            float fieldRotation = 0;
            if (this.GetPdfObject().GetAsDictionary(PdfName.MK) != null && this.GetPdfObject().GetAsDictionary(PdfName
                .MK).Get(PdfName.R) != null) {
                fieldRotation = (float)this.GetPdfObject().GetAsDictionary(PdfName.MK).GetAsFloat(PdfName.R);
                //Get relative field rotation
                fieldRotation += pageRotation;
            }
            if (fieldRotation % 90 == 0) {
                Rectangle initialBboxRectangle = bBox.ToRectangle();
                //Cast angle to [-360, 360]
                double angle = fieldRotation % 360;
                //Get angle in radians
                angle = DegreeToRadians(angle);
                //Calculate origin offset
                double translationWidth = CalculateTranslationWidthAfterFieldRot(initialBboxRectangle, DegreeToRadians(pageRotation
                    ), angle);
                double translationHeight = CalculateTranslationHeightAfterFieldRot(initialBboxRectangle, DegreeToRadians(pageRotation
                    ), angle);
                //Concatenate rotation and translation into the matrix
                Matrix currentMatrix = new Matrix(matrix.GetAsNumber(0).FloatValue(), matrix.GetAsNumber(1).FloatValue(), 
                    matrix.GetAsNumber(2).FloatValue(), matrix.GetAsNumber(3).FloatValue(), matrix.GetAsNumber(4).FloatValue
                    (), matrix.GetAsNumber(5).FloatValue());
                Matrix toConcatenate = new Matrix((float)Math.Cos(angle), (float)(-Math.Sin(angle)), (float)(Math.Sin(angle
                    )), (float)(Math.Cos(angle)), (float)translationWidth, (float)translationHeight);
                currentMatrix = currentMatrix.Multiply(toConcatenate);
                matrix = new PdfArray(new float[] { currentMatrix.Get(0), currentMatrix.Get(1), currentMatrix.Get(3), currentMatrix
                    .Get(4), currentMatrix.Get(6), currentMatrix.Get(7) });
                // Construct bounding box
                Rectangle rect = initialBboxRectangle.Clone();
                // If the angle is a multiple of 90 and not a multiple of 180, height and width of the bounding box
                // need to be switched
                if (angle % (Math.PI / 2) == 0 && angle % (Math.PI) != 0) {
                    rect.SetWidth(initialBboxRectangle.GetHeight());
                    rect.SetHeight(initialBboxRectangle.GetWidth());
                }
                rect.SetX(rect.GetX() + (float)translationWidth);
                rect.SetY(rect.GetY() + (float)translationHeight);
                // Copy Bounding box
                bBox = new PdfArray(rect);
            }
            // Create appearance
            Rectangle bboxRectangle = bBox.ToRectangle();
            PdfFormXObject appearance = new PdfFormXObject(new Rectangle(0, 0, bboxRectangle.GetWidth(), bboxRectangle
                .GetHeight()));
            appearance.Put(PdfName.Matrix, matrix);
            //Create text appearance
            if (PdfName.Tx.Equals(type)) {
                if (parent.IsMultiline()) {
                    DrawMultiLineTextAppearance(bboxRectangle, GetFont(), value, appearance);
                }
                else {
                    DrawTextAppearance(bboxRectangle, GetFont(), GetFontSize(bBox, value), value, appearance);
                }
            }
            else {
                int topIndex = 0;
                if (!parent.GetFieldFlag(PdfChoiceFormField.FF_COMBO)) {
                    PdfNumber topIndexNum = this.GetParent().GetAsNumber(PdfName.TI);
                    PdfArray options = parent.GetOptions();
                    if (null != options) {
                        topIndex = null != topIndexNum ? topIndexNum.IntValue() : 0;
                        PdfArray visibleOptions = topIndex > 0 ? new PdfArray(options.SubList(topIndex, options.Size())) : (PdfArray
                            )options.Clone();
                        value = PdfFormField.OptionsArrayToString(visibleOptions);
                    }
                }
                DrawChoiceAppearance(bboxRectangle, GetFontSize(bBox, value), value, appearance, topIndex);
            }
            PdfDictionary ap = new PdfDictionary();
            ap.Put(PdfName.N, appearance.GetPdfObject());
            ap.SetModified();
            Put(PdfName.AP, ap);
            return true;
        }

        internal virtual bool RegenerateWidget() {
            if (parent == null) {
                return true;
            }
            PdfName type = parent.GetFormType();
            if (PdfName.Tx.Equals(type) || PdfName.Ch.Equals(type)) {
                return RegenerateTextAndChoiceField();
            }
            else {
                if (PdfName.Btn.Equals(type)) {
                    if (parent.GetFieldFlag(PdfButtonFormField.FF_PUSH_BUTTON)) {
                        RegeneratePushButtonField();
                    }
                    else {
                        if (parent.GetFieldFlag(PdfButtonFormField.FF_RADIO)) {
                            DrawRadioButtonAndSaveAppearance(GetRadioButtonValue());
                        }
                        else {
                            //TODO DEVSIX-7426 remove flag
                            if (ExperimentalFeatures.ENABLE_EXPERIMENTAL_CHECKBOX_RENDERING) {
                                DrawCheckBoxAndSaveAppearanceExperimental(parent.GetValueAsString());
                            }
                            else {
                                //TODO DEVSIX-7426 remove method
                                RegenerateCheckboxField(parent.checkType);
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        internal virtual Radio CreateRadio() {
            Rectangle rect = GetRect(GetPdfObject());
            if (rect == null) {
                return null;
            }
            // id doesn't matter here
            Radio radio = new Radio("");
            // Border
            if (GetBorderWidth() > 0 && borderColor != null) {
                Border border = new SolidBorder(Math.Max(1, GetBorderWidth()));
                border.SetColor(borderColor);
                radio.SetBorder(border);
            }
            if (backgroundColor != null) {
                radio.SetBackgroundColor(backgroundColor);
            }
            // Set fixed size
            radio.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(rect.GetWidth()));
            radio.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(rect.GetHeight()));
            // Always flatten
            radio.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
            return radio;
        }

        private static double DegreeToRadians(double angle) {
            return Math.PI * angle / 180.0;
        }

        private static Paragraph CreateParagraphForTextFieldValue(String value) {
            Text text = new Text(value);
            text.SetNextRenderer(new FormFieldValueNonTrimmingTextRenderer(text));
            return new Paragraph(text);
        }

        private String GetRadioButtonValue() {
            foreach (String state in GetAppearanceStates()) {
                if (!OFF_STATE_VALUE.Equals(state)) {
                    return state;
                }
            }
            return null;
        }

        private float GetFontSize(PdfArray bBox, String value) {
            if (GetFontSize() == 0) {
                if (bBox == null || value == null || String.IsNullOrEmpty(value)) {
                    return DEFAULT_FONT_SIZE;
                }
                else {
                    return ApproximateFontSizeToFitSingleLine(GetFont(), bBox.ToRectangle(), value, MIN_FONT_SIZE);
                }
            }
            return GetFontSize();
        }

        private static float ApproximateFontSizeToFitMultiLine(Paragraph paragraph, Rectangle rect, IRenderer parentRenderer
            ) {
            IRenderer renderer = paragraph.CreateRendererSubTree().SetParent(parentRenderer);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(1, rect));
            float lFontSize = MIN_FONT_SIZE;
            float rFontSize = DEFAULT_FONT_SIZE;
            paragraph.SetFontSize(DEFAULT_FONT_SIZE);
            if (renderer.Layout(layoutContext).GetStatus() != LayoutResult.FULL) {
                int numberOfIterations = 6;
                for (int i = 0; i < numberOfIterations; i++) {
                    float mFontSize = (lFontSize + rFontSize) / 2;
                    paragraph.SetFontSize(mFontSize);
                    LayoutResult result = renderer.Layout(layoutContext);
                    if (result.GetStatus() == LayoutResult.FULL) {
                        lFontSize = mFontSize;
                    }
                    else {
                        rFontSize = mFontSize;
                    }
                }
            }
            else {
                lFontSize = DEFAULT_FONT_SIZE;
            }
            return lFontSize;
        }

        // For text field that value shall be min 4, for checkbox there is no min value.
        private float ApproximateFontSizeToFitSingleLine(PdfFont localFont, Rectangle bBox, String value, float minValue
            ) {
            float fs;
            float height = bBox.GetHeight() - borderWidth * 2;
            int[] fontBbox = localFont.GetFontProgram().GetFontMetrics().GetBbox();
            fs = FontProgram.ConvertGlyphSpaceToTextSpace(height / (fontBbox[2] - fontBbox[1]));
            float baseWidth = localFont.GetWidth(value, 1);
            if (baseWidth != 0) {
                float availableWidth = Math.Max(bBox.GetWidth() - borderWidth * 2, 0);
                // This constant is taken based on what was the resultant padding in previous version
                // of this algorithm in case border width was zero.
                float absMaxPadding = 4f;
                // relative value is quite big in order to preserve visible padding on small field sizes.
                // This constant is taken arbitrary, based on visual similarity to Acrobat behaviour.
                float relativePaddingForSmallSizes = 0.15f;
                // with current constants, if availableWidth is less than ~26 points, padding will be made relative
                if (availableWidth * relativePaddingForSmallSizes < absMaxPadding) {
                    availableWidth -= availableWidth * relativePaddingForSmallSizes * 2;
                }
                else {
                    availableWidth -= absMaxPadding * 2;
                }
                fs = Math.Min(fs, availableWidth / baseWidth);
            }
            return Math.Max(fs, minValue);
        }

        /// <summary>
        /// Calculate the necessary height offset after applying field rotation
        /// so that the origin of the bounding box is the lower left corner with respect to the field text.
        /// </summary>
        /// <param name="bBox">bounding box rectangle before rotation</param>
        /// <param name="pageRotation">rotation of the page</param>
        /// <param name="relFieldRotation">rotation of the field relative to the page</param>
        /// <returns>translation value for height</returns>
        private static float CalculateTranslationHeightAfterFieldRot(Rectangle bBox, double pageRotation, double relFieldRotation
            ) {
            if (relFieldRotation == 0) {
                return 0.0f;
            }
            if (pageRotation == 0) {
                if (relFieldRotation == Math.PI / 2) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI) {
                    return bBox.GetHeight();
                }
            }
            if (pageRotation == -Math.PI / 2) {
                if (relFieldRotation == -Math.PI / 2) {
                    return bBox.GetWidth() - bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI / 2) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI) {
                    return bBox.GetWidth();
                }
            }
            if (pageRotation == -Math.PI) {
                if (relFieldRotation == -1 * Math.PI) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == -1 * Math.PI / 2) {
                    return bBox.GetHeight() - bBox.GetWidth();
                }
                if (relFieldRotation == Math.PI / 2) {
                    return bBox.GetWidth();
                }
            }
            if (pageRotation == -3 * Math.PI / 2) {
                if (relFieldRotation == -3 * Math.PI / 2) {
                    return bBox.GetWidth();
                }
                if (relFieldRotation == -Math.PI) {
                    return bBox.GetWidth();
                }
            }
            return 0.0f;
        }

        /// <summary>
        /// Calculate the necessary width offset after applying field rotation
        /// so that the origin of the bounding box is the lower left corner with respect to the field text.
        /// </summary>
        /// <param name="bBox">bounding box rectangle before rotation</param>
        /// <param name="pageRotation">rotation of the page</param>
        /// <param name="relFieldRotation">rotation of the field relative to the page</param>
        /// <returns>translation value for width</returns>
        private static float CalculateTranslationWidthAfterFieldRot(Rectangle bBox, double pageRotation, double relFieldRotation
            ) {
            if (relFieldRotation == 0) {
                return 0.0f;
            }
            if (pageRotation == 0 && (relFieldRotation == Math.PI || relFieldRotation == 3 * Math.PI / 2)) {
                return bBox.GetWidth();
            }
            if (pageRotation == -Math.PI / 2) {
                if (relFieldRotation == -Math.PI / 2 || relFieldRotation == Math.PI) {
                    return bBox.GetHeight();
                }
            }
            if (pageRotation == -Math.PI) {
                if (relFieldRotation == -1 * Math.PI) {
                    return bBox.GetWidth();
                }
                if (relFieldRotation == -1 * Math.PI / 2) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == Math.PI / 2) {
                    return -1 * (bBox.GetHeight() - bBox.GetWidth());
                }
            }
            if (pageRotation == -3 * Math.PI / 2) {
                if (relFieldRotation == -3 * Math.PI / 2) {
                    return -1 * (bBox.GetWidth() - bBox.GetHeight());
                }
                if (relFieldRotation == -Math.PI) {
                    return bBox.GetHeight();
                }
                if (relFieldRotation == -Math.PI / 2) {
                    return bBox.GetWidth();
                }
            }
            return 0.0f;
        }

        private static String ObfuscatePassword(String text) {
            char[] pchar = new char[text.Length];
            for (int i = 0; i < text.Length; i++) {
                pchar[i] = '*';
            }
            return new String(pchar);
        }

        private void ApplyRotation(PdfFormXObject xObject, float height, float width) {
            switch (rotation) {
                case 90: {
                    xObject.Put(PdfName.Matrix, new PdfArray(new float[] { 0, 1, -1, 0, height, 0 }));
                    break;
                }

                case 180: {
                    xObject.Put(PdfName.Matrix, new PdfArray(new float[] { -1, 0, 0, -1, width, height }));
                    break;
                }

                case 270: {
                    xObject.Put(PdfName.Matrix, new PdfArray(new float[] { 0, -1, 1, 0, 0, width }));
                    break;
                }

                default: {
                    // Rotation 0 - do nothing
                    break;
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

        /// <summary>Experimental method to draw a checkbox and save its appearance.</summary>
        /// <param name="onStateName">the name of the appearance state for the checked state</param>
        protected internal virtual void DrawCheckBoxAndSaveAppearanceExperimental(String onStateName) {
            //TODO DEVSIX-7426 rename experimental method
            Rectangle rect = GetRect(this.GetPdfObject());
            if (rect == null) {
                return;
            }
            CheckBox formField = CreateCheckBox();
            if (formField == null) {
                return;
            }
            // First draw off appearance
            if (GetWidget().GetNormalAppearanceObject() == null) {
                GetWidget().SetNormalAppearance(new PdfDictionary());
            }
            PdfDictionary normalAppearance = GetWidget().GetNormalAppearanceObject();
            // Draw on appearance
            formField.SetChecked(false);
            PdfFormXObject xObjectOff = new PdfFormXObject(new Rectangle(0, 0, rect.GetWidth(), rect.GetHeight()));
            iText.Layout.Canvas canvasOff = new iText.Layout.Canvas(xObjectOff, GetDocument());
            canvasOff.Add(formField);
            normalAppearance.Put(new PdfName(OFF_STATE_VALUE), xObjectOff.GetPdfObject());
            if (onStateName != null && !String.IsNullOrEmpty(onStateName) && !iText.Forms.Fields.PdfFormAnnotation.OFF_STATE_VALUE
                .Equals(onStateName)) {
                formField.SetChecked(true);
                PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, rect.GetWidth(), rect.GetHeight()));
                iText.Layout.Canvas canvas = new iText.Layout.Canvas(xObject, this.GetDocument());
                canvas.Add(formField);
                normalAppearance.Put(new PdfName(onStateName), xObject.GetPdfObject());
            }
            PdfDictionary mk = new PdfDictionary();
            mk.Put(PdfName.CA, new PdfString(parent.text));
            GetWidget().Put(PdfName.MK, mk);
            PdfWidgetAnnotation widget = GetWidget();
            if (widget.GetNormalAppearanceObject() != null && widget.GetNormalAppearanceObject().ContainsKey(new PdfName
                (onStateName))) {
                widget.SetAppearanceState(new PdfName(onStateName));
            }
            else {
                widget.SetAppearanceState(new PdfName(OFF_STATE_VALUE));
            }
        }

        private CheckBox CreateCheckBox() {
            Rectangle rect = GetRect(GetPdfObject());
            if (rect == null) {
                return null;
            }
            // id doesn't matter here
            CheckBox checkBox = new CheckBox("");
            if (GetBorderWidth() > 0 && borderColor != null) {
                Border border = new SolidBorder(Math.Max(1, GetBorderWidth()));
                border.SetColor(borderColor);
                checkBox.SetBorder(border);
            }
            if (backgroundColor != null) {
                checkBox.SetBackgroundColor(backgroundColor);
            }
            // Set fixed size
            checkBox.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(rect.GetWidth()));
            checkBox.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(rect.GetHeight()));
            // Always flatten
            checkBox.SetInteractive(false);
            checkBox.SetPdfAConformanceLevel(GetPdfAConformanceLevel());
            checkBox.SetCheckBoxType(parent.checkType);
            return checkBox;
        }
    }
}

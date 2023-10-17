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
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Logs;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Tagging;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// Abstract
    /// <see cref="iText.Layout.Renderer.BlockRenderer"/>
    /// for form fields.
    /// </summary>
    public abstract class AbstractFormFieldRenderer : BlockRenderer {
        /// <summary>The flat renderer.</summary>
        protected internal IRenderer flatRenderer;

        /// <summary>
        /// Creates a new
        /// <see cref="AbstractFormFieldRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        internal AbstractFormFieldRenderer(IFormField modelElement)
            : base(modelElement) {
        }

        /// <summary>Checks if form fields need to be flattened.</summary>
        /// <returns>true, if fields need to be flattened</returns>
        public virtual bool IsFlatten() {
            if (parent != null) {
                // First check parent. This is a workaround for the case when some fields are inside other fields
                // either directly or via other elements (input text field inside div inside input button field). In this
                // case we do not want to create a form field for the inner field and just flatten it.
                IRenderer nextParent = parent;
                while (nextParent != null) {
                    if (nextParent is iText.Forms.Form.Renderer.AbstractFormFieldRenderer) {
                        return true;
                    }
                    nextParent = nextParent.GetParent();
                }
            }
            bool? flatten = GetPropertyAsBoolean(FormProperty.FORM_FIELD_FLATTEN);
            return flatten == null ? (bool)modelElement.GetDefaultProperty<bool>(FormProperty.FORM_FIELD_FLATTEN) : (bool
                )flatten;
        }

        /// <summary>Gets the default value of the form field.</summary>
        /// <returns>the default value of the form field</returns>
        public virtual String GetDefaultValue() {
            String defaultValue = this.GetProperty<String>(FormProperty.FORM_FIELD_VALUE);
            return defaultValue == null ? modelElement.GetDefaultProperty<String>(FormProperty.FORM_FIELD_VALUE) : defaultValue;
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            childRenderers.Clear();
            flatRenderer = null;
            float parentWidth = layoutContext.GetArea().GetBBox().GetWidth();
            float parentHeight = layoutContext.GetArea().GetBBox().GetHeight();
            IRenderer renderer = CreateFlatRenderer();
            if (renderer.GetOwnProperty<OverflowPropertyValue?>(Property.OVERFLOW_X) == null) {
                renderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            }
            if (renderer.GetOwnProperty<OverflowPropertyValue?>(Property.OVERFLOW_Y) == null) {
                renderer.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            }
            AddChild(renderer);
            Rectangle bBox = layoutContext.GetArea().GetBBox().Clone().MoveDown(INF - parentHeight).SetHeight(INF);
            layoutContext.GetArea().SetBBox(bBox);
            // A workaround for the issue that super.layout clears Property.FORCED_PLACEMENT,
            // but we need it later in this function
            bool isForcedPlacement = true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT));
            LayoutResult result = base.Layout(layoutContext);
            if (childRenderers.IsEmpty()) {
                ITextLogManager.GetLogger(GetType()).LogError(FormsLogMessageConstants.ERROR_WHILE_LAYOUT_OF_FORM_FIELD);
                occupiedArea.GetBBox().SetWidth(0).SetHeight(0);
            }
            else {
                flatRenderer = childRenderers[0];
                ProcessLangAttribute();
                childRenderers.Clear();
                childRenderers.Add(flatRenderer);
                AdjustFieldLayout(layoutContext);
                if (IsLayoutBasedOnFlatRenderer()) {
                    Rectangle fBox = flatRenderer.GetOccupiedArea().GetBBox();
                    occupiedArea.GetBBox().SetX(fBox.GetX()).SetY(fBox.GetY()).SetWidth(fBox.GetWidth()).SetHeight(fBox.GetHeight
                        ());
                    ApplyPaddings(occupiedArea.GetBBox(), true);
                    ApplyBorderBox(occupiedArea.GetBBox(), true);
                    ApplyMargins(occupiedArea.GetBBox(), true);
                }
                else {
                    if (isForcedPlacement) {
                        // This block of code appeared here because of
                        // TODO DEVSIX-5042 HEIGHT property is ignored when FORCED_PLACEMENT is true
                        // Height is wrong for the flat renderer and we adjust it here
                        Rectangle fBox = GetOccupiedArea().GetBBox();
                        LayoutArea childOccupiedArea = flatRenderer.GetOccupiedArea();
                        childOccupiedArea.GetBBox().SetY(fBox.GetY()).SetHeight(fBox.GetHeight());
                    }
                }
            }
            if (!isForcedPlacement && !IsRendererFit(parentWidth, parentHeight)) {
                occupiedArea.GetBBox().SetWidth(0).SetHeight(0);
                return new MinMaxWidthLayoutResult(LayoutResult.NOTHING, occupiedArea, null, this, this).SetMinMaxWidth(new 
                    MinMaxWidth());
            }
            if (result.GetStatus() != LayoutResult.FULL || !IsRendererFit(parentWidth, parentHeight)) {
                ITextLogManager.GetLogger(GetType()).LogWarning(FormsLogMessageConstants.INPUT_FIELD_DOES_NOT_FIT);
            }
            return new MinMaxWidthLayoutResult(LayoutResult.FULL, occupiedArea, this, null).SetMinMaxWidth(new MinMaxWidth
                (occupiedArea.GetBBox().GetWidth(), occupiedArea.GetBBox().GetWidth(), 0));
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(DrawContext drawContext) {
            if (flatRenderer != null) {
                if (IsFlatten()) {
                    base.Draw(drawContext);
                }
                else {
                    DrawChildren(drawContext);
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawChildren(DrawContext drawContext) {
            drawContext.GetCanvas().SaveState();
            bool flatten = IsFlatten();
            if (flatten) {
                drawContext.GetCanvas().Rectangle(ApplyBorderBox(occupiedArea.GetBBox(), false)).Clip().EndPath();
                flatRenderer.Draw(drawContext);
            }
            else {
                ApplyAcroField(drawContext);
            }
            drawContext.GetCanvas().RestoreState();
        }

        /// <summary><inheritDoc/></summary>
        public override MinMaxWidth GetMinMaxWidth() {
            childRenderers.Clear();
            flatRenderer = null;
            IRenderer renderer = CreateFlatRenderer();
            AddChild(renderer);
            MinMaxWidth minMaxWidth = base.GetMinMaxWidth();
            return minMaxWidth;
        }

        /// <summary>Adjusts the field layout.</summary>
        /// <param name="layoutContext">layout context</param>
        protected internal abstract void AdjustFieldLayout(LayoutContext layoutContext);

        /// <summary>Creates the flat renderer instance.</summary>
        /// <returns>the renderer instance</returns>
        protected internal abstract IRenderer CreateFlatRenderer();

        /// <summary>Applies the AcroField widget.</summary>
        /// <param name="drawContext">the draw context</param>
        protected internal abstract void ApplyAcroField(DrawContext drawContext);

        /// <summary>Gets the model id.</summary>
        /// <returns>the model id</returns>
        protected internal virtual String GetModelId() {
            return ((IFormField)GetModelElement()).GetId();
        }

        /// <summary>Checks if the renderer fits a certain width and height.</summary>
        /// <param name="availableWidth">the available width</param>
        /// <param name="availableHeight">the available height</param>
        /// <returns>true, if the renderer fits</returns>
        protected internal virtual bool IsRendererFit(float availableWidth, float availableHeight) {
            if (occupiedArea == null) {
                return false;
            }
            return availableHeight >= occupiedArea.GetBBox().GetHeight() && ((availableWidth >= occupiedArea.GetBBox()
                .GetWidth()) || (this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X) == OverflowPropertyValue
                .VISIBLE));
        }

        /// <summary>Gets the accessibility language.</summary>
        /// <returns>the accessibility language</returns>
        protected internal virtual String GetLang() {
            return this.GetProperty<String>(FormProperty.FORM_ACCESSIBILITY_LANGUAGE);
        }

        /// <summary>Determines, whether the layout is based in the renderer itself or flat renderer.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if layout is based on flat renderer, false otherwise
        /// </returns>
        protected internal virtual bool IsLayoutBasedOnFlatRenderer() {
            return true;
        }

        protected internal virtual void WriteAcroFormFieldLangAttribute(PdfDocument pdfDoc) {
            if (pdfDoc.IsTagged()) {
                TagTreePointer formParentPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                IList<String> kidsRoles = formParentPointer.GetKidsRoles();
                int lastFormIndex = kidsRoles.LastIndexOf(StandardRoles.FORM);
                TagTreePointer formPointer = formParentPointer.MoveToKid(lastFormIndex);
                if (GetLang() != null) {
                    formPointer.GetProperties().SetLanguage(GetLang());
                }
                formParentPointer.MoveToParent();
            }
        }

        /// <summary>Deletes all margin properties.</summary>
        /// <remarks>
        /// Deletes all margin properties. Used in
        /// <c>applyAcroField</c>
        /// to not apply margins twice as we already use area
        /// with margins applied (margins shouldn't be an interactive part of the field, i.e. included into its occupied
        /// area).
        /// </remarks>
        /// <returns>the map of deleted margins</returns>
        internal virtual IDictionary<int, Object> DeleteMargins() {
            IDictionary<int, Object> margins = new Dictionary<int, Object>();
            margins.Put(Property.MARGIN_TOP, this.modelElement.GetOwnProperty<UnitValue>(Property.MARGIN_TOP));
            margins.Put(Property.MARGIN_BOTTOM, this.modelElement.GetOwnProperty<UnitValue>(Property.MARGIN_BOTTOM));
            margins.Put(Property.MARGIN_LEFT, this.modelElement.GetOwnProperty<UnitValue>(Property.MARGIN_LEFT));
            margins.Put(Property.MARGIN_RIGHT, this.modelElement.GetOwnProperty<UnitValue>(Property.MARGIN_RIGHT));
            modelElement.DeleteOwnProperty(Property.MARGIN_RIGHT);
            modelElement.DeleteOwnProperty(Property.MARGIN_LEFT);
            modelElement.DeleteOwnProperty(Property.MARGIN_TOP);
            modelElement.DeleteOwnProperty(Property.MARGIN_BOTTOM);
            return margins;
        }

        /// <summary>Applies the properties to the model element.</summary>
        /// <param name="properties">the properties to apply</param>
        internal virtual void ApplyProperties(IDictionary<int, Object> properties) {
            foreach (KeyValuePair<int, Object> integerObjectEntry in properties) {
                if (integerObjectEntry.Value != null) {
                    modelElement.SetProperty(integerObjectEntry.Key, integerObjectEntry.Value);
                }
                else {
                    modelElement.DeleteOwnProperty(integerObjectEntry.Key);
                }
            }
        }

        /// <summary>Applies the border property.</summary>
        /// <param name="annotation">the annotation to set border characteristics to.</param>
        internal virtual void ApplyBorderProperty(PdfFormAnnotation annotation) {
            ApplyBorderProperty(this, annotation);
        }

        /// <summary>Applies the border property to the renderer.</summary>
        /// <param name="renderer">renderer to apply border properties to.</param>
        /// <param name="annotation">the annotation to set border characteristics to.</param>
        internal static void ApplyBorderProperty(IRenderer renderer, PdfFormAnnotation annotation) {
            Border border = renderer.GetProperty<Border>(Property.BORDER);
            if (border == null) {
                // For now, we set left border to an annotation, but appropriate borders for an element will be drawn.
                border = renderer.GetProperty<Border>(Property.BORDER_LEFT);
            }
            if (border != null) {
                annotation.SetBorderStyle(TransformBorderTypeToBorderStyleDictionary(border.GetBorderType()));
                annotation.SetBorderColor(border.GetColor());
                annotation.SetBorderWidth(border.GetWidth());
            }
        }

        private static PdfDictionary TransformBorderTypeToBorderStyleDictionary(int borderType) {
            PdfDictionary bs = new PdfDictionary();
            PdfName style;
            switch (borderType) {
                case 1001: {
                    style = PdfAnnotation.STYLE_UNDERLINE;
                    break;
                }

                case 1002: {
                    style = PdfAnnotation.STYLE_BEVELED;
                    break;
                }

                case 1003: {
                    style = PdfAnnotation.STYLE_INSET;
                    break;
                }

                case Border.DASHED_FIXED:
                case Border.DASHED:
                case Border.DOTTED: {
                    // Default dash array will be used.
                    style = PdfAnnotation.STYLE_DASHED;
                    break;
                }

                default: {
                    style = PdfAnnotation.STYLE_SOLID;
                    break;
                }
            }
            bs.Put(PdfName.S, style);
            return bs;
        }

        private void ProcessLangAttribute() {
            IPropertyContainer propertyContainer = flatRenderer.GetModelElement();
            String lang = GetLang();
            if (propertyContainer is IAccessibleElement && lang != null) {
                AccessibilityProperties properties = ((IAccessibleElement)propertyContainer).GetAccessibilityProperties();
                if (properties.GetLanguage() == null) {
                    properties.SetLanguage(lang);
                }
            }
        }
    }
}

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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Logs;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
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

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates a new
        /// <see cref="AbstractFormFieldRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        internal AbstractFormFieldRenderer(IFormField modelElement)
            : base(modelElement) {
        }
//\endcond

        /// <summary>Checks if form fields need to be flattened.</summary>
        /// <returns>true, if fields need to be flattened.</returns>
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
        /// <returns>the default value of the form field.</returns>
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
            LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
            if (taggingHelper != null) {
                taggingHelper.AddKidsHint(this, JavaCollectionsUtil.SingletonList(renderer));
                LayoutTaggingHelper.AddTreeHints(taggingHelper, renderer);
            }
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
        public override MinMaxWidth GetMinMaxWidth() {
            childRenderers.Clear();
            flatRenderer = null;
            IRenderer renderer = CreateFlatRenderer();
            AddChild(renderer);
            MinMaxWidth minMaxWidth = base.GetMinMaxWidth();
            return minMaxWidth;
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawChildren(DrawContext drawContext) {
            drawContext.GetCanvas().SaveState();
            bool flatten = IsFlatten();
            if (flatten) {
                PdfCanvas canvas = drawContext.GetCanvas();
                canvas.Rectangle(ApplyBorderBox(occupiedArea.GetBBox(), false)).Clip().EndPath();
                flatRenderer.Draw(drawContext);
            }
            else {
                ApplyAcroField(drawContext);
                WriteAcroFormFieldLangAttribute(drawContext.GetDocument());
            }
            drawContext.GetCanvas().RestoreState();
        }

        /// <summary>Applies the accessibility properties to the form field.</summary>
        /// <param name="formField">the form field to which the accessibility properties should be applied</param>
        /// <param name="pdfDocument">the document to which the form field belongs</param>
        protected internal virtual void ApplyAccessibilityProperties(PdfFormField formField, PdfDocument pdfDocument
            ) {
            PdfFormField.ApplyAccessibilityProperties(formField, ((IAccessibleElement)this.modelElement), pdfDocument);
        }

        /// <summary>Adjusts the field layout.</summary>
        /// <param name="layoutContext">layout context</param>
        protected internal abstract void AdjustFieldLayout(LayoutContext layoutContext);

        /// <summary>Creates the flat renderer instance.</summary>
        /// <returns>the renderer instance.</returns>
        protected internal abstract IRenderer CreateFlatRenderer();

        /// <summary>Applies the AcroField widget.</summary>
        /// <param name="drawContext">the draw context</param>
        protected internal abstract void ApplyAcroField(DrawContext drawContext);

        /// <summary>Gets the model id.</summary>
        /// <returns>the model id.</returns>
        protected internal virtual String GetModelId() {
            return ((IFormField)GetModelElement()).GetId();
        }

        /// <summary>Checks if the renderer fits a certain width and height.</summary>
        /// <param name="availableWidth">the available width</param>
        /// <param name="availableHeight">the available height</param>
        /// <returns>true, if the renderer fits.</returns>
        protected internal virtual bool IsRendererFit(float availableWidth, float availableHeight) {
            if (occupiedArea == null) {
                return false;
            }
            return availableHeight >= occupiedArea.GetBBox().GetHeight() && ((availableWidth >= occupiedArea.GetBBox()
                .GetWidth()) || (this.GetProperty<OverflowPropertyValue?>(Property.OVERFLOW_X) == OverflowPropertyValue
                .VISIBLE));
        }

        /// <summary>
        /// Gets the accessibility language using
        /// <see cref="iText.Layout.Tagging.IAccessibleElement.GetAccessibilityProperties()"/>.
        /// </summary>
        /// <returns>the accessibility language.</returns>
        protected internal virtual String GetLang() {
            String language = null;
            if (this.GetModelElement() is IAccessibleElement) {
                language = ((IAccessibleElement)this.GetModelElement()).GetAccessibilityProperties().GetLanguage();
            }
            return language;
        }

        /// <summary>Gets the conformance.</summary>
        /// <remarks>Gets the conformance. If the conformance is not set, the conformance of the document is used.</remarks>
        /// <param name="document">the document</param>
        /// <returns>the conformance or null if the conformance is not set.</returns>
        protected internal virtual PdfConformance GetConformance(PdfDocument document) {
            PdfConformance conformance = this.GetProperty<PdfConformance>(FormProperty.FORM_CONFORMANCE_LEVEL);
            if (conformance != null) {
                return conformance;
            }
            if (document == null) {
                return null;
            }
            return document.GetConformance();
        }

        /// <summary>Determines, whether the layout is based in the renderer itself or flat renderer.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if layout is based on flat renderer, false otherwise.
        /// </returns>
        protected internal virtual bool IsLayoutBasedOnFlatRenderer() {
            return true;
        }

        /// <summary>Sets the form accessibility language identifier of the form element in case the document is tagged.
        ///     </summary>
        /// <param name="pdfDoc">the document which contains form field</param>
        protected internal virtual void WriteAcroFormFieldLangAttribute(PdfDocument pdfDoc) {
            if (pdfDoc.IsTagged()) {
                TagTreePointer formParentPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                IList<String> kidsRoles = formParentPointer.GetKidsRoles();
                int lastFormIndex = kidsRoles.LastIndexOf(StandardRoles.FORM);
                TagTreePointer formPointer = formParentPointer.MoveToKid(lastFormIndex);
                String lang = GetLang();
                if (lang != null) {
                    formPointer.GetProperties().SetLanguage(lang);
                }
                formParentPointer.MoveToParent();
            }
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

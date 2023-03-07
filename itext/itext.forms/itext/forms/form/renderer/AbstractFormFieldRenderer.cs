/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Forms.Logs;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
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

        /* (non-Javadoc)
        * @see com.itextpdf.layout.renderer.BlockRenderer#layout(com.itextpdf.layout.layout.LayoutContext)
        */
        public override LayoutResult Layout(LayoutContext layoutContext) {
            childRenderers.Clear();
            flatRenderer = null;
            float parentWidth = layoutContext.GetArea().GetBBox().GetWidth();
            float parentHeight = layoutContext.GetArea().GetBBox().GetHeight();
            IRenderer renderer = CreateFlatRenderer();
            renderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            renderer.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            AddChild(renderer);
            Rectangle bBox = layoutContext.GetArea().GetBBox().Clone().MoveDown(INF - parentHeight).SetHeight(INF);
            layoutContext.GetArea().SetBBox(bBox);
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
            }
            if (!true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT)) && !IsRendererFit(parentWidth, parentHeight
                )) {
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

        /* (non-Javadoc)
        * @see com.itextpdf.layout.renderer.BlockRenderer#draw(com.itextpdf.layout.renderer.DrawContext)
        */
        public override void Draw(DrawContext drawContext) {
            if (flatRenderer != null) {
                base.Draw(drawContext);
            }
        }

        /* (non-Javadoc)
        * @see com.itextpdf.layout.renderer.AbstractRenderer#drawChildren(com.itextpdf.layout.renderer.DrawContext)
        */
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

        /* (non-Javadoc)
        * @see com.itextpdf.layout.renderer.BlockRenderer#getMinMaxWidth(float)
        */
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

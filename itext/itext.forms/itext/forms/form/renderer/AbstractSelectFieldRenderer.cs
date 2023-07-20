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
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer {
    /// <summary>
    /// Abstract
    /// <see cref="iText.Layout.Renderer.BlockRenderer"/>
    /// for select form fields.
    /// </summary>
    public abstract class AbstractSelectFieldRenderer : BlockRenderer {
        /// <summary>
        /// Creates a new
        /// <see cref="AbstractSelectFieldRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="modelElement">the model element</param>
        protected internal AbstractSelectFieldRenderer(AbstractSelectField modelElement)
            : base(modelElement) {
            AddChild(CreateFlatRenderer());
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            // Resolve width here in case it's relative, while parent width is still intact.
            // If it's inline-block context, relative width is already resolved.
            float? width = RetrieveWidth(layoutContext.GetArea().GetBBox().GetWidth());
            if (width != null) {
                UpdateWidth(UnitValue.CreatePointValue((float)width));
            }
            float childrenMaxWidth = GetMinMaxWidth().GetMaxWidth();
            LayoutArea area = layoutContext.GetArea().Clone();
            area.GetBBox().MoveDown(INF - area.GetBBox().GetHeight()).SetHeight(INF).SetWidth(childrenMaxWidth + EPS);
            // A workaround for the issue that super.layout clears Property.FORCED_PLACEMENT,
            // but we need it later in this function
            bool isForcedPlacement = true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT));
            LayoutResult layoutResult = base.Layout(new LayoutContext(area, layoutContext.GetMarginsCollapseInfo(), layoutContext
                .GetFloatRendererAreas(), layoutContext.IsClippedHeight()));
            if (layoutResult.GetStatus() != LayoutResult.FULL) {
                if (isForcedPlacement) {
                    layoutResult = MakeLayoutResultFull(layoutContext.GetArea(), layoutResult);
                }
                else {
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, this);
                }
            }
            float availableHeight = layoutContext.GetArea().GetBBox().GetHeight();
            bool isClippedHeight = layoutContext.IsClippedHeight();
            Rectangle dummy = new Rectangle(0, 0);
            ApplyMargins(dummy, true);
            ApplyBorderBox(dummy, true);
            ApplyPaddings(dummy, true);
            float additionalHeight = dummy.GetHeight();
            availableHeight -= additionalHeight;
            availableHeight = Math.Max(availableHeight, 0);
            float actualHeight = GetOccupiedArea().GetBBox().GetHeight() - additionalHeight;
            float finalSelectFieldHeight = GetFinalSelectFieldHeight(availableHeight, actualHeight, isClippedHeight);
            if (finalSelectFieldHeight < 0) {
                return new LayoutResult(LayoutResult.NOTHING, null, null, this, this);
            }
            float delta = finalSelectFieldHeight - actualHeight;
            if (Math.Abs(delta) > EPS) {
                GetOccupiedArea().GetBBox().IncreaseHeight(delta).MoveDown(delta);
            }
            return layoutResult;
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(DrawContext drawContext) {
            if (IsFlatten()) {
                base.Draw(drawContext);
            }
            else {
                DrawChildren(drawContext);
            }
        }

        public override void DrawChildren(DrawContext drawContext) {
            if (IsFlatten()) {
                base.DrawChildren(drawContext);
            }
            else {
                ApplyAcroField(drawContext);
            }
        }

        /// <summary>Gets the accessibility language.</summary>
        /// <returns>the accessibility language</returns>
        protected internal virtual String GetLang() {
            return this.GetProperty<String>(FormProperty.FORM_ACCESSIBILITY_LANGUAGE);
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

        protected internal abstract IRenderer CreateFlatRenderer();

        protected internal abstract void ApplyAcroField(DrawContext drawContext);

        /// <summary>Checks if form fields need to be flattened.</summary>
        /// <returns>true, if fields need to be flattened</returns>
        protected internal virtual bool IsFlatten() {
            return (bool)GetPropertyAsBoolean(FormProperty.FORM_FIELD_FLATTEN);
        }

        /// <summary>Gets the model id.</summary>
        /// <returns>the model id</returns>
        protected internal virtual String GetModelId() {
            return ((IFormField)GetModelElement()).GetId();
        }

        /// <summary>
        /// Retrieve the options from select field (can be combo box or list box field) and set them
        /// to the form field builder.
        /// </summary>
        /// <param name="builder">
        /// 
        /// <see cref="iText.Forms.Fields.ChoiceFormFieldBuilder"/>
        /// to set options to.
        /// </param>
        /// <param name="field">
        /// 
        /// <see cref="iText.Forms.Form.Element.AbstractSelectField"/>
        /// to retrieve the options from.
        /// </param>
        protected internal virtual void SetupBuilderValues(ChoiceFormFieldBuilder builder, AbstractSelectField field
            ) {
            IList<SelectFieldItem> options = field.GetItems();
            if (options.IsEmpty()) {
                builder.SetOptions(new String[0]);
                return;
            }
            bool supportExportValueAndDisplayValue = field.HasExportAndDisplayValues();
            // If one element has export value and display value, then all elements must have export value and display value
            if (supportExportValueAndDisplayValue) {
                String[][] exportValuesAndDisplayValues = new String[options.Count][];
                for (int i = 0; i < options.Count; i++) {
                    SelectFieldItem option = options[i];
                    String[] exportValues = new String[2];
                    exportValues[0] = option.GetExportValue();
                    exportValues[1] = option.GetDisplayValue();
                    exportValuesAndDisplayValues[i] = exportValues;
                }
                builder.SetOptions(exportValuesAndDisplayValues);
            }
            else {
                // In normal case we just use display values as this will correctly give the one value that we need
                String[] displayValues = new String[options.Count];
                for (int i = 0; i < options.Count; i++) {
                    SelectFieldItem option = options[i];
                    displayValues[i] = option.GetDisplayValue();
                }
                builder.SetOptions(displayValues);
            }
        }

        protected internal virtual float GetFinalSelectFieldHeight(float availableHeight, float actualHeight, bool
             isClippedHeight) {
            bool isForcedPlacement = true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT));
            if (!isClippedHeight && actualHeight > availableHeight) {
                if (isForcedPlacement) {
                    return availableHeight;
                }
                return -1;
            }
            return actualHeight;
        }

        protected internal virtual IList<IRenderer> GetOptionsMarkedSelected(IRenderer optionsSubTree) {
            IList<IRenderer> selectedOptions = new List<IRenderer>();
            foreach (IRenderer option in optionsSubTree.GetChildRenderers()) {
                if (IsOptionRenderer(option)) {
                    if (true.Equals(option.GetProperty<bool?>(FormProperty.FORM_FIELD_SELECTED))) {
                        selectedOptions.Add(option);
                    }
                }
                else {
                    IList<IRenderer> subSelectedOptions = GetOptionsMarkedSelected(option);
                    selectedOptions.AddAll(subSelectedOptions);
                }
            }
            return selectedOptions;
        }

        internal static bool IsOptGroupRenderer(IRenderer renderer) {
            return renderer.HasProperty(FormProperty.FORM_FIELD_LABEL) && !renderer.HasProperty(FormProperty.FORM_FIELD_SELECTED
                );
        }

        internal static bool IsOptionRenderer(IRenderer child) {
            return child.HasProperty(FormProperty.FORM_FIELD_SELECTED);
        }

        private LayoutResult MakeLayoutResultFull(LayoutArea layoutArea, LayoutResult layoutResult) {
            IRenderer splitRenderer = layoutResult.GetSplitRenderer() == null ? this : layoutResult.GetSplitRenderer();
            if (occupiedArea == null) {
                occupiedArea = new LayoutArea(layoutArea.GetPageNumber(), new Rectangle(layoutArea.GetBBox().GetLeft(), layoutArea
                    .GetBBox().GetTop(), 0, 0));
            }
            layoutResult = new LayoutResult(LayoutResult.FULL, occupiedArea, splitRenderer, null);
            return layoutResult;
        }
    }
}

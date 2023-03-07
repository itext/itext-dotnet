/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
            if (!IsFlatten()) {
                // TODO DEVSIX-1901
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Form.Renderer.AbstractSelectFieldRenderer));
                logger.LogWarning(FormsLogMessageConstants.ACROFORM_NOT_SUPPORTED_FOR_SELECT);
                SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
            }
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
            LayoutResult layoutResult = base.Layout(new LayoutContext(area, layoutContext.GetMarginsCollapseInfo(), layoutContext
                .GetFloatRendererAreas(), layoutContext.IsClippedHeight()));
            if (layoutResult.GetStatus() != LayoutResult.FULL) {
                if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
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

        private LayoutResult MakeLayoutResultFull(LayoutArea layoutArea, LayoutResult layoutResult) {
            IRenderer splitRenderer = layoutResult.GetSplitRenderer() == null ? this : layoutResult.GetSplitRenderer();
            if (occupiedArea == null) {
                occupiedArea = new LayoutArea(layoutArea.GetPageNumber(), new Rectangle(layoutArea.GetBBox().GetLeft(), layoutArea
                    .GetBBox().GetTop(), 0, 0));
            }
            layoutResult = new LayoutResult(LayoutResult.FULL, occupiedArea, splitRenderer, null);
            return layoutResult;
        }

        internal static bool IsOptGroupRenderer(IRenderer renderer) {
            return renderer.HasProperty(FormProperty.FORM_FIELD_LABEL) && !renderer.HasProperty(FormProperty.FORM_FIELD_SELECTED
                );
        }

        internal static bool IsOptionRenderer(IRenderer child) {
            return child.HasProperty(FormProperty.FORM_FIELD_SELECTED);
        }
    }
}

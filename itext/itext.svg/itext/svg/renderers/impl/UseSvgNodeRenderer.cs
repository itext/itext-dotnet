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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Css.Impl;
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>Renderer implementing the use tag.</summary>
    /// <remarks>Renderer implementing the use tag. This tag allows you to reuse previously defined elements.</remarks>
    public class UseSvgNodeRenderer : AbstractSvgNodeRenderer {
        protected internal override void DoDraw(SvgDrawContext context) {
            if (this.attributesAndStyles != null) {
                String elementToReUse = GetAttribute(SvgConstants.Attributes.HREF);
                if (elementToReUse == null) {
                    elementToReUse = GetAttribute(SvgConstants.Attributes.XLINK_HREF);
                }
                if (elementToReUse != null && !String.IsNullOrEmpty(elementToReUse) && IsValidHref(elementToReUse)) {
                    String normalizedName = SvgTextUtil.FilterReferenceValue(elementToReUse);
                    if (!context.IsIdUsedByUseTagBefore(normalizedName)) {
                        ISvgNodeRenderer template = context.GetNamedObject(normalizedName);
                        // Clone template
                        ISvgNodeRenderer clonedObject = template == null ? null : template.CreateDeepCopy();
                        // Resolve parent inheritance
                        SvgNodeRendererInheritanceResolver.ApplyInheritanceToSubTree(this, clonedObject, context.GetCssContext());
                        if (clonedObject != null) {
                            if (clonedObject is AbstractSvgNodeRenderer) {
                                ((AbstractSvgNodeRenderer)clonedObject).SetPartOfClipPath(partOfClipPath);
                            }
                            PdfCanvas currentCanvas = context.GetCurrentCanvas();
                            // If X or Y attribute is null, then default 0 value will be returned
                            float x = ParseHorizontalLength(GetAttribute(SvgConstants.Attributes.X), context);
                            float y = ParseVerticalLength(GetAttribute(SvgConstants.Attributes.Y), context);
                            AffineTransform inverseMatrix = null;
                            if (!CssUtils.CompareFloats(x, 0) || !CssUtils.CompareFloats(y, 0)) {
                                AffineTransform translation = AffineTransform.GetTranslateInstance(x, y);
                                currentCanvas.ConcatMatrix(translation);
                                if (partOfClipPath) {
                                    try {
                                        inverseMatrix = translation.CreateInverse();
                                    }
                                    catch (NoninvertibleTransformException ex) {
                                        ITextLogManager.GetLogger(typeof(UseSvgNodeRenderer)).LogWarning(ex, SvgLogMessageConstant.NONINVERTIBLE_TRANSFORMATION_MATRIX_USED_IN_CLIP_PATH
                                            );
                                    }
                                }
                            }
                            // Setting the parent of the referenced element to this instance
                            clonedObject.SetParent(this);
                            // Width, and height have no effect on use elements, unless the element referenced has a viewBox
                            // i.e. they only have an effect when use refers to a svg or symbol element.
                            if (clonedObject is SvgTagSvgNodeRenderer || clonedObject is SymbolSvgNodeRenderer) {
                                if (GetAttribute(SvgConstants.Attributes.WIDTH) != null) {
                                    float width = ParseHorizontalLength(GetAttribute(SvgConstants.Attributes.WIDTH), context);
                                    clonedObject.SetAttribute(SvgConstants.Attributes.WIDTH, Convert.ToString(width, System.Globalization.CultureInfo.InvariantCulture
                                        ) + CommonCssConstants.PT);
                                }
                                if (GetAttribute(SvgConstants.Attributes.HEIGHT) != null) {
                                    float height = ParseVerticalLength(GetAttribute(SvgConstants.Attributes.HEIGHT), context);
                                    clonedObject.SetAttribute(SvgConstants.Attributes.HEIGHT, Convert.ToString(height, System.Globalization.CultureInfo.InvariantCulture
                                        ) + CommonCssConstants.PT);
                                }
                            }
                            clonedObject.Draw(context);
                            // Unsetting the parent of the referenced element
                            clonedObject.SetParent(null);
                            if (inverseMatrix != null) {
                                currentCanvas.ConcatMatrix(inverseMatrix);
                            }
                        }
                    }
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal override void PostDraw(SvgDrawContext context) {
        }
//\endcond

        private bool IsValidHref(String name) {
            return name.StartsWith("#");
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            UseSvgNodeRenderer copy = new UseSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }
    }
}

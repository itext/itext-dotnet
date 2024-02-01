/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
                String elementToReUse = this.attributesAndStyles.Get(SvgConstants.Attributes.XLINK_HREF);
                if (elementToReUse == null) {
                    elementToReUse = this.attributesAndStyles.Get(SvgConstants.Attributes.HREF);
                }
                if (elementToReUse != null && !String.IsNullOrEmpty(elementToReUse) && IsValidHref(elementToReUse)) {
                    String normalizedName = SvgTextUtil.FilterReferenceValue(elementToReUse);
                    if (!context.IsIdUsedByUseTagBefore(normalizedName)) {
                        ISvgNodeRenderer template = context.GetNamedObject(normalizedName);
                        // Clone template
                        ISvgNodeRenderer namedObject = template == null ? null : template.CreateDeepCopy();
                        // Resolve parent inheritance
                        SvgNodeRendererInheritanceResolver.ApplyInheritanceToSubTree(this, namedObject, context.GetCssContext());
                        if (namedObject != null) {
                            if (namedObject is AbstractSvgNodeRenderer) {
                                ((AbstractSvgNodeRenderer)namedObject).SetPartOfClipPath(partOfClipPath);
                            }
                            PdfCanvas currentCanvas = context.GetCurrentCanvas();
                            float x = 0f;
                            float y = 0f;
                            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.X)) {
                                x = CssDimensionParsingUtils.ParseAbsoluteLength(this.attributesAndStyles.Get(SvgConstants.Attributes.X));
                            }
                            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.Y)) {
                                y = CssDimensionParsingUtils.ParseAbsoluteLength(this.attributesAndStyles.Get(SvgConstants.Attributes.Y));
                            }
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
                            // setting the parent of the referenced element to this instance
                            namedObject.SetParent(this);
                            namedObject.Draw(context);
                            // unsetting the parent of the referenced element
                            namedObject.SetParent(null);
                            if (inverseMatrix != null) {
                                currentCanvas.ConcatMatrix(inverseMatrix);
                            }
                        }
                    }
                }
            }
        }

        internal override void PostDraw(SvgDrawContext context) {
        }

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

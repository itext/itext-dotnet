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

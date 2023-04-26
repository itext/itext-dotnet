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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public class TextSvgTSpanBranchRenderer : TextSvgBranchRenderer {
        private const float EPS = 0.0001f;

        public TextSvgTSpanBranchRenderer() {
            this.performRootTransformations = false;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return GetParent().GetObjectBoundingBox(context);
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            TextSvgBranchRenderer copy = new iText.Svg.Renderers.Impl.TextSvgTSpanBranchRenderer();
            FillCopy(copy);
            return copy;
        }

        protected internal override void DoDraw(SvgDrawContext context) {
            if (GetChildren().Count > 0) {
                // if branch has no children, don't do anything
                PdfCanvas currentCanvas = context.GetCurrentCanvas();
                if (this.attributesAndStyles != null) {
                    foreach (ISvgTextNodeRenderer c in GetChildren()) {
                        ApplyTextRenderingMode(currentCanvas);
                        ResolveFont(context);
                        currentCanvas.SetFontAndSize(GetFont(), GetCurrentFontSize());
                        float childLength = c.GetTextContentLength(GetCurrentFontSize(), GetFont());
                        if (c.ContainsAbsolutePositionChange()) {
                            // TODO: DEVSIX-2507 support rotate and other attributes
                            float[][] absolutePositions = c.GetAbsolutePositionChanges();
                            AffineTransform newTransform = GetTextTransform(absolutePositions, context);
                            // Overwrite the last transformation stored in the context
                            context.SetLastTextTransform(newTransform);
                            // Apply transformation
                            currentCanvas.SetTextMatrix(newTransform);
                            // Absolute position changes requires resetting the current text move in the context
                            context.ResetTextMove();
                            context.SetPreviousElementTextMove(null);
                        }
                        // Handle Text-Anchor declarations
                        float textAnchorCorrection = GetTextAnchorAlignmentCorrection(childLength);
                        if (!CssUtils.CompareFloats(0f, textAnchorCorrection)) {
                            context.AddTextMove(textAnchorCorrection, 0);
                        }
                        // Move needs to happen before the saving of the state in order for it to cascade beyond
                        if (c.ContainsRelativeMove()) {
                            float[] childMove = c.GetRelativeTranslation();
                            //-y to account for the text-matrix transform we do in the text root to account
                            // for the coordinates
                            context.AddTextMove(childMove[0], -childMove[1]);
                            context.SetPreviousElementTextMove(new float[] { context.GetPreviousElementTextMove()[0] + childMove[0], context
                                .GetPreviousElementTextMove()[1] - childMove[1] });
                        }
                        CanvasGraphicsState savedState = new CanvasGraphicsState(currentCanvas.GetGraphicsState());
                        c.Draw(context);
                        ApplyGSDifference(currentCanvas, savedState);
                        context.AddTextMove(childLength, 0);
                        if (!FloatsAreEqual(childLength, 0)) {
                            context.SetPreviousElementTextMove(new float[] { childLength, 0 });
                        }
                    }
                }
            }
        }

        // This method is used to follow q/Q store/restore approach. If some graphics characteristics
        // have been updated while processing this renderer's children, they are restored.
        internal virtual void ApplyGSDifference(PdfCanvas currentCanvas, CanvasGraphicsState savedGs) {
            CanvasGraphicsState newGs = currentCanvas.GetGraphicsState();
            if (!FloatsAreEqual(savedGs.GetCharSpacing(), newGs.GetCharSpacing())) {
                currentCanvas.SetCharacterSpacing(savedGs.GetCharSpacing());
            }
            if (savedGs.GetFillColor() != newGs.GetFillColor()) {
                currentCanvas.SetFillColor(savedGs.GetFillColor());
            }
            if (savedGs.GetFont() != newGs.GetFont() || !FloatsAreEqual(savedGs.GetFontSize(), newGs.GetFontSize())) {
                currentCanvas.SetFontAndSize(savedGs.GetFont(), savedGs.GetFontSize());
            }
            if (!FloatsAreEqual(savedGs.GetLineWidth(), newGs.GetLineWidth())) {
                currentCanvas.SetLineWidth(savedGs.GetLineWidth());
            }
            if (savedGs.GetStrokeColor() != newGs.GetStrokeColor()) {
                currentCanvas.SetStrokeColor(savedGs.GetStrokeColor());
            }
            if (savedGs.GetTextRenderingMode() != newGs.GetTextRenderingMode()) {
                currentCanvas.SetTextRenderingMode(savedGs.GetTextRenderingMode());
            }
            // Only the next extended options are set in svg
            if (!FloatsAreEqual(savedGs.GetFillOpacity(), newGs.GetFillOpacity()) || !FloatsAreEqual(savedGs.GetStrokeOpacity
                (), newGs.GetStrokeOpacity())) {
                PdfExtGState extGState = new PdfExtGState();
                extGState.SetFillOpacity(savedGs.GetFillOpacity());
                extGState.SetStrokeOpacity(savedGs.GetStrokeOpacity());
                currentCanvas.SetExtGState(extGState);
            }
        }

        private static bool FloatsAreEqual(float first, float second) {
            return Math.Abs(first - second) < EPS;
        }
    }
}

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
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Font;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;text&gt; and &lt;tspan&gt; tag.
    /// </summary>
    public class TextSvgBranchRenderer : AbstractSvgNodeRenderer, ISvgTextNodeRenderer {
        /// <summary>Top level transformation to flip the y-axis results in the character glyphs being mirrored, this tf corrects for this behaviour
        ///     </summary>
        protected internal static readonly AffineTransform TEXTFLIP = new AffineTransform(1, 0, 0, -1, 0, 0);

        private readonly IList<ISvgTextNodeRenderer> children = new List<ISvgTextNodeRenderer>();

        protected internal bool performRootTransformations;

        private PdfFont font;

        private bool moveResolved;

        private float xMove;

        private float yMove;

        private bool posResolved;

        private float[] xPos;

        private float[] yPos;

        private bool whiteSpaceProcessed = false;

        public TextSvgBranchRenderer() {
            performRootTransformations = true;
            moveResolved = false;
            posResolved = false;
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            iText.Svg.Renderers.Impl.TextSvgBranchRenderer copy = new iText.Svg.Renderers.Impl.TextSvgBranchRenderer();
            FillCopy(copy);
            return copy;
        }

        internal virtual void FillCopy(iText.Svg.Renderers.Impl.TextSvgBranchRenderer copy) {
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
        }

        public void AddChild(ISvgTextNodeRenderer child) {
            // Final method, in order to disallow adding null
            if (child != null) {
                children.Add(child);
            }
        }

        public IList<ISvgTextNodeRenderer> GetChildren() {
            // Final method, in order to disallow modifying the List
            return JavaCollectionsUtil.UnmodifiableList(children);
        }

        public virtual float GetTextContentLength(float parentFontSize, PdfFont font) {
            return 0.0f;
        }

        // Branch renderers do not contain any text themselves and do not contribute to the text length
        public virtual float[] GetRelativeTranslation() {
            if (!moveResolved) {
                ResolveTextMove();
            }
            return new float[] { xMove, yMove };
        }

        public virtual bool ContainsRelativeMove() {
            if (!moveResolved) {
                ResolveTextMove();
            }
            bool isNullMove = CssUtils.CompareFloats(0f, xMove) && CssUtils.CompareFloats(0f, yMove);
            // comparision to 0
            return !isNullMove;
        }

        public virtual bool ContainsAbsolutePositionChange() {
            if (!posResolved) {
                ResolveTextPosition();
            }
            return (xPos != null && xPos.Length > 0) || (yPos != null && yPos.Length > 0);
        }

        public virtual float[][] GetAbsolutePositionChanges() {
            if (!posResolved) {
                ResolveTextPosition();
            }
            return new float[][] { xPos, yPos };
        }

        public virtual void MarkWhiteSpaceProcessed() {
            whiteSpaceProcessed = true;
        }

        public virtual TextRectangle GetTextRectangle(SvgDrawContext context, Point basePoint) {
            if (this.attributesAndStyles != null) {
                ResolveFont(context);
                double x = 0;
                double y = 0;
                if (GetAbsolutePositionChanges()[0] != null) {
                    x = GetAbsolutePositionChanges()[0][0];
                }
                else {
                    if (basePoint != null) {
                        x = basePoint.GetX();
                    }
                }
                if (GetAbsolutePositionChanges()[1] != null) {
                    y = GetAbsolutePositionChanges()[1][0];
                }
                else {
                    if (basePoint != null) {
                        y = basePoint.GetY();
                    }
                }
                basePoint = new Point(x, y);
                basePoint.Translate(GetRelativeTranslation()[0], GetRelativeTranslation()[1]);
                Rectangle commonRect = null;
                foreach (ISvgTextNodeRenderer child in GetChildren()) {
                    if (child != null) {
                        TextRectangle rectangle = child.GetTextRectangle(context, basePoint);
                        basePoint = rectangle.GetTextBaseLineRightPoint();
                        commonRect = Rectangle.GetCommonRectangle(commonRect, rectangle);
                    }
                }
                if (commonRect != null) {
                    return new TextRectangle(commonRect.GetX(), commonRect.GetY(), commonRect.GetWidth(), commonRect.GetHeight
                        (), (float)basePoint.GetY());
                }
            }
            return null;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return GetTextRectangle(context, null);
        }

        /// <summary>
        /// Method that will set properties to be inherited by this branch renderer's
        /// children and will iterate over all children in order to draw them.
        /// </summary>
        /// <param name="context">
        /// the object that knows the place to draw this element and
        /// maintains its state
        /// </param>
        protected internal override void DoDraw(SvgDrawContext context) {
            if (GetChildren().Count > 0) {
                // if branch has no children, don't do anything
                PdfCanvas currentCanvas = context.GetCurrentCanvas();
                context.ResetTextMove();
                context.SetLastTextTransform(null);
                if (this.attributesAndStyles != null) {
                    foreach (ISvgTextNodeRenderer c in children) {
                        currentCanvas.SaveState();
                        currentCanvas.BeginText();
                        PerformRootTransformations(currentCanvas, context);
                        ApplyTextRenderingMode(currentCanvas);
                        ResolveFont(context);
                        currentCanvas.SetFontAndSize(font, GetCurrentFontSize());
                        float childLength = c.GetTextContentLength(GetCurrentFontSize(), font);
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
                        }
                        else {
                            if (c is TextLeafSvgNodeRenderer && !context.GetLastTextTransform().IsIdentity()) {
                                currentCanvas.SetTextMatrix(context.GetLastTextTransform());
                            }
                        }
                        // Handle Text-Anchor declarations
                        float textAnchorCorrection = GetTextAnchorAlignmentCorrection(childLength);
                        if (!CssUtils.CompareFloats(0f, textAnchorCorrection)) {
                            context.AddTextMove(textAnchorCorrection, 0);
                        }
                        // Move needs to happen before the saving of the state in order for it to cascade beyond
                        if (c.ContainsRelativeMove()) {
                            float[] childMove = c.GetRelativeTranslation();
                            context.AddTextMove(childMove[0], -childMove[1]);
                        }
                        //-y to account for the text-matrix transform we do in the text root to account for the coordinates
                        c.Draw(context);
                        context.AddTextMove(childLength, 0);
                        context.SetPreviousElementTextMove(null);
                        currentCanvas.EndText();
                        currentCanvas.RestoreState();
                    }
                }
            }
        }

        internal virtual void PerformRootTransformations(PdfCanvas currentCanvas, SvgDrawContext context) {
            // Current transformation matrix results in the character glyphs being mirrored, correct with inverse tf
            AffineTransform rootTf;
            if (this.ContainsAbsolutePositionChange()) {
                rootTf = GetTextTransform(this.GetAbsolutePositionChanges(), context);
            }
            else {
                rootTf = new AffineTransform(TEXTFLIP);
            }
            currentCanvas.SetTextMatrix(rootTf);
            // Apply relative move
            if (this.ContainsRelativeMove()) {
                float[] rootMove = this.GetRelativeTranslation();
                //-y to account for the text-matrix transform we do in the text root to account for the coordinates
                context.AddTextMove(rootMove[0], -rootMove[1]);
            }
            // Handle white-spaces
            if (!whiteSpaceProcessed) {
                SvgTextUtil.ProcessWhiteSpace(this, true);
            }
        }

        private void ResolveTextMove() {
            if (this.attributesAndStyles != null) {
                String xRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.DX);
                String yRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.DY);
                IList<String> xValuesList = SvgCssUtils.SplitValueList(xRawValue);
                IList<String> yValuesList = SvgCssUtils.SplitValueList(yRawValue);
                xMove = 0f;
                yMove = 0f;
                if (!xValuesList.IsEmpty()) {
                    xMove = CssDimensionParsingUtils.ParseAbsoluteLength(xValuesList[0]);
                }
                if (!yValuesList.IsEmpty()) {
                    yMove = CssDimensionParsingUtils.ParseAbsoluteLength(yValuesList[0]);
                }
                moveResolved = true;
            }
        }

        private FontInfo ResolveFontName(String fontFamily, String fontWeight, String fontStyle, FontProvider provider
            , FontSet tempFonts) {
            bool isBold = SvgConstants.Attributes.BOLD.EqualsIgnoreCase(fontWeight);
            bool isItalic = SvgConstants.Attributes.ITALIC.EqualsIgnoreCase(fontStyle);
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            IList<String> stringArrayList = new List<String>();
            stringArrayList.Add(fontFamily);
            fontCharacteristics.SetBoldFlag(isBold);
            fontCharacteristics.SetItalicFlag(isItalic);
            return provider.GetFontSelector(stringArrayList, fontCharacteristics, tempFonts).BestMatch();
        }

        internal virtual void ResolveFont(SvgDrawContext context) {
            FontProvider provider = context.GetFontProvider();
            FontSet tempFonts = context.GetTempFonts();
            font = null;
            if (!provider.GetFontSet().IsEmpty() || (tempFonts != null && !tempFonts.IsEmpty())) {
                String fontFamily = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_FAMILY);
                String fontWeight = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_WEIGHT);
                String fontStyle = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_STYLE);
                fontFamily = fontFamily != null ? fontFamily.Trim() : "";
                FontInfo fontInfo = ResolveFontName(fontFamily, fontWeight, fontStyle, provider, tempFonts);
                font = provider.GetPdfFont(fontInfo, tempFonts);
            }
            if (font == null) {
                try {
                    // TODO: DEVSIX-2057 each call of createFont() create a new instance of PdfFont.
                    // FontProvider shall be used instead.
                    font = PdfFontFactory.CreateFont();
                }
                catch (System.IO.IOException e) {
                    throw new SvgProcessingException(SvgExceptionMessageConstant.FONT_NOT_FOUND, e);
                }
            }
        }

        /// <summary>Return the font used in this text element.</summary>
        /// <remarks>
        /// Return the font used in this text element.
        /// Note that font should already be resolved with
        /// <see cref="ResolveFont(iText.Svg.Renderers.SvgDrawContext)"/>.
        /// </remarks>
        /// <returns>font of the current text element</returns>
        internal virtual PdfFont GetFont() {
            return font;
        }

        private void ResolveTextPosition() {
            if (this.attributesAndStyles != null) {
                String xRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.X);
                String yRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.Y);
                xPos = GetPositionsFromString(xRawValue);
                yPos = GetPositionsFromString(yRawValue);
                posResolved = true;
            }
        }

        private static float[] GetPositionsFromString(String rawValuesString) {
            float[] result = null;
            IList<String> valuesList = SvgCssUtils.SplitValueList(rawValuesString);
            if (!valuesList.IsEmpty()) {
                result = new float[valuesList.Count];
                for (int i = 0; i < valuesList.Count; i++) {
                    result[i] = CssDimensionParsingUtils.ParseAbsoluteLength(valuesList[i]);
                }
            }
            return result;
        }

        internal static AffineTransform GetTextTransform(float[][] absolutePositions, SvgDrawContext context) {
            AffineTransform tf = new AffineTransform();
            // If x is not specified, but y is, we need to correct for preceding text.
            if (absolutePositions[0] == null && absolutePositions[1] != null) {
                absolutePositions[0] = new float[] { context.GetTextMove()[0] };
            }
            // If y is not present, we can replace it with a neutral transformation (0.0f)
            if (absolutePositions[1] == null) {
                absolutePositions[1] = new float[] { 0.0f };
            }
            tf.Concatenate(TEXTFLIP);
            tf.Concatenate(AffineTransform.GetTranslateInstance(absolutePositions[0][0], -absolutePositions[1][0]));
            return tf;
        }

        internal virtual void ApplyTextRenderingMode(PdfCanvas currentCanvas) {
            // Fill only is the default for text operation in PDF
            if (doStroke && doFill) {
                currentCanvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.FILL_STROKE);
            }
            else {
                //Default for SVG
                if (doStroke) {
                    currentCanvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
                }
                else {
                    currentCanvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.FILL);
                }
            }
        }

        private void DeepCopyChildren(iText.Svg.Renderers.Impl.TextSvgBranchRenderer deepCopy) {
            foreach (ISvgTextNodeRenderer child in children) {
                ISvgTextNodeRenderer newChild = (ISvgTextNodeRenderer)child.CreateDeepCopy();
                child.SetParent(deepCopy);
                deepCopy.AddChild(newChild);
            }
        }

        internal virtual float GetTextAnchorAlignmentCorrection(float childContentLength) {
            // Resolve text anchor
            // TODO DEVSIX-2631 properly resolve text-anchor by taking entire line into account, not only children of the current TextSvgBranchRenderer
            float textAnchorXCorrection = 0.0f;
            if (this.attributesAndStyles != null && this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.TEXT_ANCHOR
                )) {
                String textAnchorValue = this.GetAttribute(SvgConstants.Attributes.TEXT_ANCHOR);
                // Middle
                if (SvgConstants.Values.TEXT_ANCHOR_MIDDLE.Equals(textAnchorValue)) {
                    if (xPos != null && xPos.Length > 0) {
                        textAnchorXCorrection -= childContentLength / 2;
                    }
                }
                // End
                if (SvgConstants.Values.TEXT_ANCHOR_END.Equals(textAnchorValue)) {
                    if (xPos != null && xPos.Length > 0) {
                        textAnchorXCorrection -= childContentLength;
                    }
                }
            }
            return textAnchorXCorrection;
        }
    }
}

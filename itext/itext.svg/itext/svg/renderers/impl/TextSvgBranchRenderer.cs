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
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Css;
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

        [Obsolete]
        protected internal bool performRootTransformations;

        private Paragraph paragraph;

        private Rectangle objectBoundingBox;

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

//\cond DO_NOT_DOCUMENT
        internal virtual void FillCopy(iText.Svg.Renderers.Impl.TextSvgBranchRenderer copy) {
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
        }
//\endcond

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

        [Obsolete]
        public virtual float GetTextContentLength(float parentFontSize, PdfFont font) {
            return 0.0f;
        }

        // Branch renderers do not contain any text themselves and do not contribute to the text length
        [Obsolete]
        public virtual float[] GetRelativeTranslation() {
            return GetRelativeTranslation(new SvgDrawContext(null, null));
        }

        public virtual float[] GetRelativeTranslation(SvgDrawContext context) {
            if (!moveResolved) {
                ResolveTextMove(context);
            }
            return new float[] { xMove, yMove };
        }

        [Obsolete]
        public virtual bool ContainsRelativeMove() {
            return ContainsRelativeMove(new SvgDrawContext(null, null));
        }

        public virtual bool ContainsRelativeMove(SvgDrawContext context) {
            if (!moveResolved) {
                ResolveTextMove(context);
            }
            bool isNullMove = CssUtils.CompareFloats(0f, xMove) && CssUtils.CompareFloats(0f, yMove);
            // comparison to 0
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

        public virtual TextRectangle GetTextRectangle(SvgDrawContext context, Point startPoint) {
            if (this.attributesAndStyles == null) {
                return null;
            }
            startPoint = GetStartPoint(context, startPoint);
            Rectangle commonRect = null;
            Rectangle textChunkRect = null;
            IList<ISvgTextNodeRenderer> children = new List<ISvgTextNodeRenderer>();
            CollectChildren(children);
            float rootX = (float)startPoint.GetX();
            String textAnchorValue = this.GetAttribute(SvgConstants.Attributes.TEXT_ANCHOR);
            // We resolve absolutely positioned text chunks similar to doDraw method, but here we are interested only in
            // building of properly positioned rectangles without any drawing or visual properties applying.
            foreach (ISvgTextNodeRenderer child in children) {
                if (child is iText.Svg.Renderers.Impl.TextSvgBranchRenderer) {
                    startPoint = ((iText.Svg.Renderers.Impl.TextSvgBranchRenderer)child).GetStartPoint(context, startPoint);
                    if (child.ContainsAbsolutePositionChange() && textChunkRect != null) {
                        commonRect = GetCommonRectangleWithAnchor(commonRect, textChunkRect, rootX, textAnchorValue);
                        // Start new text chunk.
                        textChunkRect = null;
                        textAnchorValue = child.GetAttribute(SvgConstants.Attributes.TEXT_ANCHOR);
                        rootX = (float)startPoint.GetX();
                    }
                }
                else {
                    TextRectangle rectangle = child.GetTextRectangle(context, startPoint);
                    startPoint = rectangle.GetTextBaseLineRightPoint();
                    textChunkRect = Rectangle.GetCommonRectangle(textChunkRect, rectangle);
                }
            }
            if (textChunkRect != null) {
                commonRect = GetCommonRectangleWithAnchor(commonRect, textChunkRect, rootX, textAnchorValue);
                return new TextRectangle(commonRect.GetX(), commonRect.GetY(), commonRect.GetWidth(), commonRect.GetHeight
                    (), (float)startPoint.GetY());
            }
            return null;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            if (GetParent() is iText.Svg.Renderers.Impl.TextSvgBranchRenderer) {
                return GetParent().GetObjectBoundingBox(context);
            }
            if (objectBoundingBox == null) {
                // Handle white-spaces
                if (!whiteSpaceProcessed) {
                    SvgTextUtil.ProcessWhiteSpace(this, true);
                }
                objectBoundingBox = GetTextRectangle(context, null);
            }
            return objectBoundingBox;
        }

//\cond DO_NOT_DOCUMENT
        internal override void PreDraw(SvgDrawContext context) {
            base.PreDraw(context);
            SvgTextUtil.ApplyTextDecoration(this, doFill, doStroke, context);
        }
//\endcond

        /// <summary>
        /// Method that will set properties to be inherited by this branch renderer's
        /// children and will iterate over all children in order to draw them.
        /// </summary>
        /// <param name="context">
        /// the object that knows the place to draw this element and
        /// maintains its state
        /// </param>
        protected internal override void DoDraw(SvgDrawContext context) {
            if (GetChildren().IsEmpty() || this.attributesAndStyles == null) {
                return;
            }
            // Handle white-spaces
            if (!whiteSpaceProcessed) {
                SvgTextUtil.ProcessWhiteSpace(this, true);
            }
            this.paragraph = new Paragraph();
            this.paragraph.SetProperty(Property.FORCED_PLACEMENT, true);
            this.paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.SVG_MODE);
            this.paragraph.SetProperty(Property.NO_SOFT_WRAP_INLINE, true);
            this.paragraph.SetMargin(0);
            ApplyTextRenderingMode(paragraph);
            ApplyFontProperties(paragraph, context);
            // We resolve and draw absolutely positioned text chunks similar to getTextRectangle method. We are interested
            // not only in building of properly positioned rectangles, but also in drawing and text properties applying.
            StartNewTextChunk(context, TEXTFLIP);
            PerformDrawing(context);
            DrawLastTextChunk(context);
            context.SetSvgTextProperties(new SvgTextProperties());
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void ApplyFontProperties(IElement element, SvgDrawContext context) {
            element.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(GetCurrentFontSize(context)));
            FontProvider provider = context.GetFontProvider();
            element.SetProperty(Property.FONT_PROVIDER, provider);
            FontSet tempFonts = context.GetTempFonts();
            element.SetProperty(Property.FONT_SET, tempFonts);
            String fontFamily = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_FAMILY);
            String fontWeight = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_WEIGHT);
            String fontStyle = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_STYLE);
            element.SetProperty(Property.FONT, new String[] { fontFamily == null ? "" : fontFamily.Trim() });
            element.SetProperty(Property.FONT_WEIGHT, fontWeight);
            element.SetProperty(Property.FONT_STYLE, fontStyle);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ApplyTextRenderingMode(IElement element) {
            // Fill only is the default for text operation in PDF
            if (doStroke && doFill) {
                // Default for SVG
                element.SetProperty(Property.TEXT_RENDERING_MODE, PdfCanvasConstants.TextRenderingMode.FILL_STROKE);
            }
            else {
                if (doStroke) {
                    element.SetProperty(Property.TEXT_RENDERING_MODE, PdfCanvasConstants.TextRenderingMode.STROKE);
                }
                else {
                    element.SetProperty(Property.TEXT_RENDERING_MODE, PdfCanvasConstants.TextRenderingMode.FILL);
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddTextChild(Text text, SvgDrawContext drawContext) {
            if (GetParent() is iText.Svg.Renderers.Impl.TextSvgBranchRenderer) {
                ((iText.Svg.Renderers.Impl.TextSvgBranchRenderer)GetParent()).AddTextChild(text, drawContext);
                return;
            }
            text.SetProperty(Property.POSITION, LayoutPosition.RELATIVE);
            text.SetProperty(Property.LEFT, drawContext.GetRelativePosition()[0]);
            text.SetProperty(Property.BOTTOM, drawContext.GetRelativePosition()[1]);
            paragraph.Add(text);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void PerformDrawing(SvgDrawContext context) {
            if (this.ContainsAbsolutePositionChange()) {
                DrawLastTextChunk(context);
                // TODO: DEVSIX-2507 support rotate and other attributes
                float[][] absolutePositions = this.GetAbsolutePositionChanges();
                AffineTransform newTransform = GetTextTransform(absolutePositions, context);
                StartNewTextChunk(context, newTransform);
            }
            if (this.ContainsRelativeMove(context)) {
                float[] rootMove = this.GetRelativeTranslation(context);
                context.AddTextMove(rootMove[0], rootMove[1]);
                context.MoveRelativePosition(rootMove[0], rootMove[1]);
            }
            foreach (ISvgTextNodeRenderer child in children) {
                SvgTextProperties textProperties = new SvgTextProperties(context.GetSvgTextProperties());
                child.SetParent(this);
                child.Draw(context);
                context.SetSvgTextProperties(textProperties);
            }
        }
//\endcond

        private void StartNewTextChunk(SvgDrawContext context, AffineTransform newTransform) {
            ApplyTextAnchor();
            context.SetRootTransform(newTransform);
            context.ResetTextMove();
            context.ResetRelativePosition();
        }

        private void DrawLastTextChunk(SvgDrawContext context) {
            if (GetParent() is iText.Svg.Renderers.Impl.TextSvgBranchRenderer) {
                ((iText.Svg.Renderers.Impl.TextSvgBranchRenderer)GetParent()).DrawLastTextChunk(context);
                return;
            }
            if (paragraph.GetChildren().IsEmpty()) {
                return;
            }
            ParagraphRenderer paragraphRenderer = new ParagraphRenderer(paragraph);
            paragraph.SetNextRenderer(paragraphRenderer);
            using (iText.Layout.Canvas canvas = new iText.Layout.Canvas(context.GetCurrentCanvas(), new Rectangle((float
                )context.GetRootTransform().GetTranslateX(), (float)context.GetRootTransform().GetTranslateY(), 1e6f, 
                0))) {
                canvas.Add(paragraph);
            }
            float textLength = paragraphRenderer.GetLines()[0].GetOccupiedAreaBBox().GetWidth();
            context.AddTextMove(textLength, 0);
            paragraph.GetChildren().Clear();
        }

//\cond DO_NOT_DOCUMENT
        internal override void ApplyFillAndStrokeProperties(AbstractSvgNodeRenderer.FillProperties fillProperties, 
            AbstractSvgNodeRenderer.StrokeProperties strokeProperties, SvgDrawContext context) {
            if (fillProperties != null) {
                context.GetSvgTextProperties().SetFillColor(fillProperties.GetColor());
                if (!CssUtils.CompareFloats(fillProperties.GetOpacity(), 1f)) {
                    context.GetSvgTextProperties().SetFillOpacity(fillProperties.GetOpacity());
                }
            }
            if (strokeProperties != null) {
                if (strokeProperties.GetLineDashParameters() != null) {
                    SvgStrokeParameterConverter.PdfLineDashParameters lineDashParameters = strokeProperties.GetLineDashParameters
                        ();
                    context.GetSvgTextProperties().SetDashPattern(lineDashParameters.GetDashArray(), lineDashParameters.GetDashPhase
                        ());
                }
                if (strokeProperties.GetColor() != null) {
                    context.GetSvgTextProperties().SetStrokeColor(strokeProperties.GetColor());
                }
                context.GetSvgTextProperties().SetLineWidth(strokeProperties.GetWidth());
                if (!CssUtils.CompareFloats(strokeProperties.GetOpacity(), 1f)) {
                    context.GetSvgTextProperties().SetStrokeOpacity(strokeProperties.GetOpacity());
                }
            }
        }
//\endcond

        private void ResolveTextMove(SvgDrawContext context) {
            if (this.attributesAndStyles != null) {
                String xRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.DX);
                String yRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.DY);
                IList<String> xValuesList = SvgCssUtils.SplitValueList(xRawValue);
                IList<String> yValuesList = SvgCssUtils.SplitValueList(yRawValue);
                xMove = 0f;
                yMove = 0f;
                if (!xValuesList.IsEmpty()) {
                    xMove = ParseHorizontalLength(xValuesList[0], context);
                }
                if (!yValuesList.IsEmpty()) {
                    yMove = ParseVerticalLength(yValuesList[0], context);
                }
                moveResolved = true;
            }
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

        private static AffineTransform GetTextTransform(float[][] absolutePositions, SvgDrawContext context) {
            AffineTransform tf = new AffineTransform();
            // If x is not specified, but y is, we need to correct for preceding text.
            if (absolutePositions[0] == null && absolutePositions[1] != null) {
                absolutePositions[0] = new float[] { (float)context.GetRootTransform().GetTranslateX() + context.GetTextMove
                    ()[0] };
            }
            // If y is not present, we should take the last text y
            if (absolutePositions[1] == null) {
                absolutePositions[1] = new float[] { (float)context.GetRootTransform().GetTranslateY() + context.GetTextMove
                    ()[1] };
            }
            tf.Concatenate(TEXTFLIP);
            tf.Concatenate(AffineTransform.GetTranslateInstance(absolutePositions[0][0], -absolutePositions[1][0]));
            return tf;
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

        /// <summary>
        /// Adjust absolutely positioned text chunk (shift it to the start of view port, apply text anchor) and
        /// merge it with the common text rectangle.
        /// </summary>
        /// <param name="commonRect">rectangle for the whole text tag</param>
        /// <param name="textChunkRect">rectangle for the last absolutely positioned text chunk</param>
        /// <param name="absoluteX">last absolute x position</param>
        /// <param name="textAnchorValue">text anchor for the last text chunk</param>
        /// <returns>merged common text rectangle</returns>
        private static Rectangle GetCommonRectangleWithAnchor(Rectangle commonRect, Rectangle textChunkRect, float
             absoluteX, String textAnchorValue) {
            textChunkRect.MoveRight(absoluteX - textChunkRect.GetX());
            if (SvgConstants.Values.TEXT_ANCHOR_MIDDLE.Equals(textAnchorValue)) {
                textChunkRect.MoveRight(-textChunkRect.GetWidth() / 2);
            }
            if (SvgConstants.Values.TEXT_ANCHOR_END.Equals(textAnchorValue)) {
                textChunkRect.MoveRight(-textChunkRect.GetWidth());
            }
            return Rectangle.GetCommonRectangle(commonRect, textChunkRect);
        }

        private void DeepCopyChildren(iText.Svg.Renderers.Impl.TextSvgBranchRenderer deepCopy) {
            foreach (ISvgTextNodeRenderer child in children) {
                ISvgTextNodeRenderer newChild = (ISvgTextNodeRenderer)child.CreateDeepCopy();
                newChild.SetParent(deepCopy);
                deepCopy.AddChild(newChild);
            }
        }

        private void ApplyTextAnchor() {
            if (this.attributesAndStyles != null && this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.TEXT_ANCHOR
                )) {
                String textAnchorValue = this.GetAttribute(SvgConstants.Attributes.TEXT_ANCHOR);
                ApplyTextAnchor(textAnchorValue);
            }
        }

        private void ApplyTextAnchor(String textAnchorValue) {
            if (GetParent() is iText.Svg.Renderers.Impl.TextSvgBranchRenderer) {
                ((iText.Svg.Renderers.Impl.TextSvgBranchRenderer)GetParent()).ApplyTextAnchor(textAnchorValue);
                return;
            }
            if (SvgConstants.Values.TEXT_ANCHOR_MIDDLE.Equals(textAnchorValue)) {
                paragraph.SetProperty(Property.TEXT_ANCHOR, TextAnchor.MIDDLE);
                return;
            }
            if (SvgConstants.Values.TEXT_ANCHOR_END.Equals(textAnchorValue)) {
                paragraph.SetProperty(Property.TEXT_ANCHOR, TextAnchor.END);
                return;
            }
            paragraph.SetProperty(Property.TEXT_ANCHOR, TextAnchor.START);
        }

        private Point GetStartPoint(SvgDrawContext context, Point basePoint) {
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
            basePoint.Move(GetRelativeTranslation(context)[0], GetRelativeTranslation(context)[1]);
            return basePoint;
        }

        private void CollectChildren(IList<ISvgTextNodeRenderer> children) {
            foreach (ISvgTextNodeRenderer child in GetChildren()) {
                children.Add(child);
                if (child is iText.Svg.Renderers.Impl.TextSvgBranchRenderer) {
                    ((iText.Svg.Renderers.Impl.TextSvgBranchRenderer)child).CollectChildren(children);
                }
            }
        }
    }
}

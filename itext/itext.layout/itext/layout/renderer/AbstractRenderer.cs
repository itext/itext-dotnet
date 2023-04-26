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
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Font;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>
    /// Defines the most common properties and behavior that are shared by most
    /// <see cref="IRenderer"/>
    /// implementations.
    /// </summary>
    /// <remarks>
    /// Defines the most common properties and behavior that are shared by most
    /// <see cref="IRenderer"/>
    /// implementations. All default Renderers are subclasses of
    /// this default implementation.
    /// </remarks>
    public abstract class AbstractRenderer : IRenderer {
        public const float OVERLAP_EPSILON = 1e-4f;

        /// <summary>
        /// The maximum difference between
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// coordinates to consider rectangles equal
        /// </summary>
        protected internal const float EPS = 1e-4f;

        /// <summary>The infinity value which is used while layouting</summary>
        protected internal const float INF = 1e6f;

        /// <summary>
        /// The common ordering index of top side in arrays of four elements which define top, right, bottom,
        /// left sides values (e.g. margins, borders, paddings).
        /// </summary>
        internal const int TOP_SIDE = 0;

        /// <summary>
        /// The common ordering index of right side in arrays of four elements which define top, right, bottom,
        /// left sides values (e.g. margins, borders, paddings).
        /// </summary>
        internal const int RIGHT_SIDE = 1;

        /// <summary>
        /// The common ordering index of bottom side in arrays of four elements which define top, right, bottom,
        /// left sides values (e.g. margins, borders, paddings).
        /// </summary>
        internal const int BOTTOM_SIDE = 2;

        /// <summary>
        /// The common ordering index of left side in arrays of four elements which define top, right, bottom,
        /// left sides values (e.g. margins, borders, paddings).
        /// </summary>
        internal const int LEFT_SIDE = 3;

        private const int ARC_RIGHT_DEGREE = 0;

        private const int ARC_TOP_DEGREE = 90;

        private const int ARC_LEFT_DEGREE = 180;

        private const int ARC_BOTTOM_DEGREE = 270;

        private const int ARC_QUARTER_CLOCKWISE_EXTENT = -90;

        protected internal IList<IRenderer> childRenderers = new List<IRenderer>();

        protected internal IList<IRenderer> positionedRenderers = new List<IRenderer>();

        protected internal IPropertyContainer modelElement;

        protected internal bool flushed = false;

        protected internal LayoutArea occupiedArea;

        protected internal IRenderer parent;

        protected internal IDictionary<int, Object> properties = new Dictionary<int, Object>();

        protected internal bool isLastRendererForModelElement = true;

        /// <summary>Creates a renderer.</summary>
        protected internal AbstractRenderer() {
        }

        /// <summary>Creates a renderer for the specified layout element.</summary>
        /// <param name="modelElement">the layout element that will be drawn by this renderer</param>
        protected internal AbstractRenderer(IElement modelElement) {
            this.modelElement = modelElement;
        }

        protected internal AbstractRenderer(iText.Layout.Renderer.AbstractRenderer other) {
            this.childRenderers = other.childRenderers;
            this.positionedRenderers = other.positionedRenderers;
            this.modelElement = other.modelElement;
            this.flushed = other.flushed;
            this.occupiedArea = other.occupiedArea != null ? other.occupiedArea.Clone() : null;
            this.parent = other.parent;
            this.properties.AddAll(other.properties);
            this.isLastRendererForModelElement = other.isLastRendererForModelElement;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void AddChild(IRenderer renderer) {
            // https://www.webkit.org/blog/116/webcore-rendering-iii-layout-basics
            // "The rules can be summarized as follows:"...
            int? positioning = renderer.GetProperty<int?>(Property.POSITION);
            if (positioning == null || positioning == LayoutPosition.RELATIVE || positioning == LayoutPosition.STATIC) {
                childRenderers.Add(renderer);
            }
            else {
                if (positioning == LayoutPosition.FIXED) {
                    iText.Layout.Renderer.AbstractRenderer root = this;
                    while (root.parent is iText.Layout.Renderer.AbstractRenderer) {
                        root = (iText.Layout.Renderer.AbstractRenderer)root.parent;
                    }
                    if (root == this) {
                        positionedRenderers.Add(renderer);
                    }
                    else {
                        root.AddChild(renderer);
                    }
                }
                else {
                    if (positioning == LayoutPosition.ABSOLUTE) {
                        // For position=absolute, if none of the top, bottom, left, right properties are provided,
                        // the content should be displayed in the flow of the current content, not overlapping it.
                        // The behavior is just if it would be statically positioned except it does not affect other elements
                        iText.Layout.Renderer.AbstractRenderer positionedParent = this;
                        bool noPositionInfo = iText.Layout.Renderer.AbstractRenderer.NoAbsolutePositionInfo(renderer);
                        while (!positionedParent.IsPositioned() && !noPositionInfo) {
                            IRenderer parent = positionedParent.parent;
                            if (parent is iText.Layout.Renderer.AbstractRenderer) {
                                positionedParent = (iText.Layout.Renderer.AbstractRenderer)parent;
                            }
                            else {
                                break;
                            }
                        }
                        if (positionedParent == this) {
                            positionedRenderers.Add(renderer);
                        }
                        else {
                            positionedParent.AddChild(renderer);
                        }
                    }
                }
            }
            // Fetch positioned renderers from non-positioned child because they might be stuck there because child's parent was null previously
            if (renderer is iText.Layout.Renderer.AbstractRenderer && !((iText.Layout.Renderer.AbstractRenderer)renderer
                ).IsPositioned() && ((iText.Layout.Renderer.AbstractRenderer)renderer).positionedRenderers.Count > 0) {
                // For position=absolute, if none of the top, bottom, left, right properties are provided,
                // the content should be displayed in the flow of the current content, not overlapping it.
                // The behavior is just if it would be statically positioned except it does not affect other elements
                int pos = 0;
                IList<IRenderer> childPositionedRenderers = ((iText.Layout.Renderer.AbstractRenderer)renderer).positionedRenderers;
                while (pos < childPositionedRenderers.Count) {
                    if (iText.Layout.Renderer.AbstractRenderer.NoAbsolutePositionInfo(childPositionedRenderers[pos])) {
                        pos++;
                    }
                    else {
                        positionedRenderers.Add(childPositionedRenderers[pos]);
                        childPositionedRenderers.JRemoveAt(pos);
                    }
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IPropertyContainer GetModelElement() {
            return modelElement;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IList<IRenderer> GetChildRenderers() {
            return childRenderers;
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool HasProperty(int property) {
            return HasOwnProperty(property) || (modelElement != null && modelElement.HasProperty(property)) || (parent
                 != null && Property.IsPropertyInherited(property) && parent.HasProperty(property));
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool HasOwnProperty(int property) {
            return properties.ContainsKey(property);
        }

        /// <summary>
        /// Checks if this renderer or its model element have the specified property,
        /// i.e. if it was set to this very element or its very model element earlier.
        /// </summary>
        /// <param name="property">the property to be checked</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this instance or its model element have given own property,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool HasOwnOrModelProperty(int property) {
            return HasOwnOrModelProperty(this, property);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void DeleteOwnProperty(int property) {
            properties.JRemove(property);
        }

        /// <summary>
        /// Deletes property from this very renderer, or in case the property is specified on its model element, the
        /// property of the model element is deleted
        /// </summary>
        /// <param name="property">the property key to be deleted</param>
        public virtual void DeleteProperty(int property) {
            if (properties.ContainsKey(property)) {
                properties.JRemove(property);
            }
            else {
                if (modelElement != null) {
                    modelElement.DeleteOwnProperty(property);
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual T1 GetProperty<T1>(int key) {
            Object property;
            if ((property = properties.Get(key)) != null || properties.ContainsKey(key)) {
                return (T1)property;
            }
            if (modelElement != null && ((property = modelElement.GetProperty<T1>(key)) != null || modelElement.HasProperty
                (key))) {
                return (T1)property;
            }
            if (parent != null && Property.IsPropertyInherited(key) && (property = parent.GetProperty<T1>(key)) != null
                ) {
                return (T1)property;
            }
            property = this.GetDefaultProperty<T1>(key);
            if (property != null) {
                return (T1)property;
            }
            return modelElement != null ? modelElement.GetDefaultProperty<T1>(key) : (T1)(Object)null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual T1 GetOwnProperty<T1>(int property) {
            return (T1)properties.Get(property);
        }

        /// <summary><inheritDoc/></summary>
        public virtual T1 GetProperty<T1>(int property, T1 defaultValue) {
            T1 result = this.GetProperty<T1>(property);
            return result != null ? result : defaultValue;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void SetProperty(int property, Object value) {
            properties.Put(property, value);
        }

        /// <summary><inheritDoc/></summary>
        public virtual T1 GetDefaultProperty<T1>(int property) {
            return (T1)(Object)null;
        }

        /// <summary>Returns a property with a certain key, as a font object.</summary>
        /// <param name="property">
        /// an
        /// <see cref="iText.Layout.Properties.Property">enum value</see>
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// </returns>
        public virtual PdfFont GetPropertyAsFont(int property) {
            return this.GetProperty<PdfFont>(property);
        }

        /// <summary>Returns a property with a certain key, as a color.</summary>
        /// <param name="property">
        /// an
        /// <see cref="iText.Layout.Properties.Property">enum value</see>
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// </returns>
        public virtual Color GetPropertyAsColor(int property) {
            return this.GetProperty<Color>(property);
        }

        /// <summary>
        /// Returns a property with a certain key, as a
        /// <see cref="iText.Layout.Properties.TransparentColor"/>.
        /// </summary>
        /// <param name="property">
        /// an
        /// <see cref="iText.Layout.Properties.Property">enum value</see>
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Layout.Properties.TransparentColor"/>
        /// </returns>
        public virtual TransparentColor GetPropertyAsTransparentColor(int property) {
            return this.GetProperty<TransparentColor>(property);
        }

        /// <summary>Returns a property with a certain key, as a floating point value.</summary>
        /// <param name="property">
        /// an
        /// <see cref="iText.Layout.Properties.Property">enum value</see>
        /// </param>
        /// <returns>
        /// a
        /// <see cref="float?"/>
        /// </returns>
        public virtual float? GetPropertyAsFloat(int property) {
            return NumberUtil.AsFloat(this.GetProperty<Object>(property));
        }

        /// <summary>Returns a property with a certain key, as a floating point value.</summary>
        /// <param name="property">
        /// an
        /// <see cref="iText.Layout.Properties.Property">enum value</see>
        /// </param>
        /// <param name="defaultValue">default value to be returned if property is not found</param>
        /// <returns>
        /// a
        /// <see cref="float?"/>
        /// </returns>
        public virtual float? GetPropertyAsFloat(int property, float? defaultValue) {
            return NumberUtil.AsFloat(this.GetProperty<Object>(property, defaultValue));
        }

        /// <summary>Returns a property with a certain key, as a boolean value.</summary>
        /// <param name="property">
        /// an
        /// <see cref="iText.Layout.Properties.Property">enum value</see>
        /// </param>
        /// <returns>
        /// a
        /// <see cref="bool?"/>
        /// </returns>
        public virtual bool? GetPropertyAsBoolean(int property) {
            return this.GetProperty<bool?>(property);
        }

        /// <summary>Returns a property with a certain key, as a unit value.</summary>
        /// <param name="property">
        /// an
        /// <see cref="iText.Layout.Properties.Property">enum value</see>
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// </returns>
        public virtual UnitValue GetPropertyAsUnitValue(int property) {
            return this.GetProperty<UnitValue>(property);
        }

        /// <summary>Returns a property with a certain key, as an integer value.</summary>
        /// <param name="property">
        /// an
        /// <see cref="iText.Layout.Properties.Property">enum value</see>
        /// </param>
        /// <returns>
        /// a
        /// <see cref="int?"/>
        /// </returns>
        public virtual int? GetPropertyAsInteger(int property) {
            return NumberUtil.AsInteger(this.GetProperty<Object>(property));
        }

        /// <summary>Returns a string representation of the renderer.</summary>
        /// <returns>
        /// a
        /// <see cref="System.String"/>
        /// </returns>
        /// <seealso cref="System.Object.ToString()"/>
        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (IRenderer renderer in childRenderers) {
                sb.Append(renderer.ToString());
            }
            return sb.ToString();
        }

        /// <summary><inheritDoc/></summary>
        public virtual LayoutArea GetOccupiedArea() {
            return occupiedArea;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Draw(DrawContext drawContext) {
            ApplyDestinationsAndAnnotation(drawContext);
            bool relativePosition = IsRelativePosition();
            if (relativePosition) {
                ApplyRelativePositioningTranslation(false);
            }
            BeginElementOpacityApplying(drawContext);
            DrawBackground(drawContext);
            DrawBorder(drawContext);
            DrawChildren(drawContext);
            DrawPositionedChildren(drawContext);
            EndElementOpacityApplying(drawContext);
            if (relativePosition) {
                ApplyRelativePositioningTranslation(true);
            }
            flushed = true;
        }

        protected internal virtual void BeginElementOpacityApplying(DrawContext drawContext) {
            float? opacity = this.GetPropertyAsFloat(Property.OPACITY);
            if (opacity != null && opacity < 1f) {
                PdfExtGState extGState = new PdfExtGState();
                extGState.SetStrokeOpacity((float)opacity).SetFillOpacity((float)opacity);
                drawContext.GetCanvas().SaveState().SetExtGState(extGState);
            }
        }

        protected internal virtual void EndElementOpacityApplying(DrawContext drawContext) {
            float? opacity = this.GetPropertyAsFloat(Property.OPACITY);
            if (opacity != null && opacity < 1f) {
                drawContext.GetCanvas().RestoreState();
            }
        }

        /// <summary>
        /// Draws a background layer if it is defined by a key
        /// <see cref="iText.Layout.Properties.Property.BACKGROUND"/>
        /// in either the layout element or this
        /// <see cref="IRenderer"/>
        /// itself.
        /// </summary>
        /// <param name="drawContext">the context (canvas, document, etc) of this drawing operation.</param>
        public virtual void DrawBackground(DrawContext drawContext) {
            Background background = this.GetProperty<Background>(Property.BACKGROUND);
            IList<BackgroundImage> backgroundImagesList = this.GetProperty<IList<BackgroundImage>>(Property.BACKGROUND_IMAGE
                );
            if (background != null || backgroundImagesList != null) {
                Rectangle bBox = GetOccupiedAreaBBox();
                bool isTagged = drawContext.IsTaggingEnabled();
                if (isTagged) {
                    drawContext.GetCanvas().OpenTag(new CanvasArtifact());
                }
                Rectangle backgroundArea = GetBackgroundArea(ApplyMargins(bBox, false));
                if (backgroundArea.GetWidth() <= 0 || backgroundArea.GetHeight() <= 0) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                    logger.LogInformation(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.RECTANGLE_HAS_NEGATIVE_OR_ZERO_SIZES
                        , "background"));
                }
                else {
                    bool backgroundAreaIsClipped = false;
                    if (background != null) {
                        // TODO DEVSIX-4525 determine how background-clip affects background-radius
                        Rectangle clippedBackgroundArea = ApplyBackgroundBoxProperty(backgroundArea.Clone(), background.GetBackgroundClip
                            ());
                        backgroundAreaIsClipped = ClipBackgroundArea(drawContext, clippedBackgroundArea);
                        DrawColorBackground(background, drawContext, clippedBackgroundArea);
                    }
                    if (backgroundImagesList != null) {
                        backgroundAreaIsClipped = DrawBackgroundImagesList(backgroundImagesList, backgroundAreaIsClipped, drawContext
                            , backgroundArea);
                    }
                    if (backgroundAreaIsClipped) {
                        drawContext.GetCanvas().RestoreState();
                    }
                }
                if (isTagged) {
                    drawContext.GetCanvas().CloseTag();
                }
            }
        }

        private void DrawColorBackground(Background background, DrawContext drawContext, Rectangle colorBackgroundArea
            ) {
            TransparentColor backgroundColor = new TransparentColor(background.GetColor(), background.GetOpacity());
            drawContext.GetCanvas().SaveState().SetFillColor(backgroundColor.GetColor());
            backgroundColor.ApplyFillTransparency(drawContext.GetCanvas());
            drawContext.GetCanvas().Rectangle((double)colorBackgroundArea.GetX() - background.GetExtraLeft(), (double)
                colorBackgroundArea.GetY() - background.GetExtraBottom(), (double)colorBackgroundArea.GetWidth() + background
                .GetExtraLeft() + background.GetExtraRight(), (double)colorBackgroundArea.GetHeight() + background.GetExtraTop
                () + background.GetExtraBottom()).Fill().RestoreState();
        }

        private Rectangle ApplyBackgroundBoxProperty(Rectangle rectangle, BackgroundBox clip) {
            if (BackgroundBox.PADDING_BOX == clip) {
                ApplyBorderBox(rectangle, false);
            }
            else {
                if (BackgroundBox.CONTENT_BOX == clip) {
                    ApplyBorderBox(rectangle, false);
                    ApplyPaddings(rectangle, false);
                }
            }
            return rectangle;
        }

        private bool DrawBackgroundImagesList(IList<BackgroundImage> backgroundImagesList, bool backgroundAreaIsClipped
            , DrawContext drawContext, Rectangle backgroundArea) {
            for (int i = backgroundImagesList.Count - 1; i >= 0; i--) {
                BackgroundImage backgroundImage = backgroundImagesList[i];
                if (backgroundImage != null && backgroundImage.IsBackgroundSpecified()) {
                    // TODO DEVSIX-4525 determine how background-clip affects background-radius
                    if (!backgroundAreaIsClipped) {
                        backgroundAreaIsClipped = ClipBackgroundArea(drawContext, backgroundArea);
                    }
                    DrawBackgroundImage(backgroundImage, drawContext, backgroundArea);
                }
            }
            return backgroundAreaIsClipped;
        }

        private void DrawBackgroundImage(BackgroundImage backgroundImage, DrawContext drawContext, Rectangle backgroundArea
            ) {
            Rectangle originBackgroundArea = ApplyBackgroundBoxProperty(backgroundArea.Clone(), backgroundImage.GetBackgroundOrigin
                ());
            float[] imageWidthAndHeight = BackgroundSizeCalculationUtil.CalculateBackgroundImageSize(backgroundImage, 
                originBackgroundArea.GetWidth(), originBackgroundArea.GetHeight());
            PdfXObject backgroundXObject = backgroundImage.GetImage();
            if (backgroundXObject == null) {
                backgroundXObject = backgroundImage.GetForm();
            }
            Rectangle imageRectangle;
            UnitValue xPosition = UnitValue.CreatePointValue(0);
            UnitValue yPosition = UnitValue.CreatePointValue(0);
            if (backgroundXObject == null) {
                AbstractLinearGradientBuilder gradientBuilder = backgroundImage.GetLinearGradientBuilder();
                if (gradientBuilder == null) {
                    return;
                }
                // fullWidth and fullHeight is 0 because percentage shifts are ignored for linear-gradients
                backgroundImage.GetBackgroundPosition().CalculatePositionValues(0, 0, xPosition, yPosition);
                backgroundXObject = CreateXObject(gradientBuilder, originBackgroundArea, drawContext.GetDocument());
                imageRectangle = new Rectangle(originBackgroundArea.GetLeft() + xPosition.GetValue(), originBackgroundArea
                    .GetTop() - imageWidthAndHeight[1] - yPosition.GetValue(), imageWidthAndHeight[0], imageWidthAndHeight
                    [1]);
            }
            else {
                backgroundImage.GetBackgroundPosition().CalculatePositionValues(originBackgroundArea.GetWidth() - imageWidthAndHeight
                    [0], originBackgroundArea.GetHeight() - imageWidthAndHeight[1], xPosition, yPosition);
                imageRectangle = new Rectangle(originBackgroundArea.GetLeft() + xPosition.GetValue(), originBackgroundArea
                    .GetTop() - imageWidthAndHeight[1] - yPosition.GetValue(), imageWidthAndHeight[0], imageWidthAndHeight
                    [1]);
            }
            if (imageRectangle.GetWidth() <= 0 || imageRectangle.GetHeight() <= 0) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogInformation(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.RECTANGLE_HAS_NEGATIVE_OR_ZERO_SIZES
                    , "background-image"));
            }
            else {
                Rectangle clippedBackgroundArea = ApplyBackgroundBoxProperty(backgroundArea.Clone(), backgroundImage.GetBackgroundClip
                    ());
                drawContext.GetCanvas().SaveState().Rectangle(clippedBackgroundArea).Clip().EndPath();
                DrawPdfXObject(imageRectangle, backgroundImage, drawContext, backgroundXObject, backgroundArea, originBackgroundArea
                    );
                drawContext.GetCanvas().RestoreState();
            }
        }

        private static void DrawPdfXObject(Rectangle imageRectangle, BackgroundImage backgroundImage, DrawContext 
            drawContext, PdfXObject backgroundXObject, Rectangle backgroundArea, Rectangle originBackgroundArea) {
            BlendMode blendMode = backgroundImage.GetBlendMode();
            if (blendMode != BlendMode.NORMAL) {
                drawContext.GetCanvas().SetExtGState(new PdfExtGState().SetBlendMode(blendMode.GetPdfRepresentation()));
            }
            Point whitespace = backgroundImage.GetRepeat().PrepareRectangleToDrawingAndGetWhitespace(imageRectangle, originBackgroundArea
                , backgroundImage.GetBackgroundSize());
            float initialX = imageRectangle.GetX();
            int counterY = 1;
            bool firstDraw = true;
            bool isCurrentOverlaps;
            bool isNextOverlaps;
            do {
                DrawPdfXObjectHorizontally(imageRectangle, backgroundImage, drawContext, backgroundXObject, backgroundArea
                    , firstDraw, (float)whitespace.GetX());
                firstDraw = false;
                imageRectangle.SetX(initialX);
                isCurrentOverlaps = imageRectangle.Overlaps(backgroundArea, OVERLAP_EPSILON);
                if (counterY % 2 == 1) {
                    isNextOverlaps = imageRectangle.MoveDown((imageRectangle.GetHeight() + (float)whitespace.GetY()) * counterY
                        ).Overlaps(backgroundArea, OVERLAP_EPSILON);
                }
                else {
                    isNextOverlaps = imageRectangle.MoveUp((imageRectangle.GetHeight() + (float)whitespace.GetY()) * counterY)
                        .Overlaps(backgroundArea, OVERLAP_EPSILON);
                }
                ++counterY;
            }
            while (!backgroundImage.GetRepeat().IsNoRepeatOnYAxis() && (isCurrentOverlaps || isNextOverlaps));
        }

        private static void DrawPdfXObjectHorizontally(Rectangle imageRectangle, BackgroundImage backgroundImage, 
            DrawContext drawContext, PdfXObject backgroundXObject, Rectangle backgroundArea, bool firstDraw, float
             xWhitespace) {
            bool isItFirstDraw = firstDraw;
            int counterX = 1;
            bool isCurrentOverlaps;
            bool isNextOverlaps;
            do {
                if (imageRectangle.Overlaps(backgroundArea, OVERLAP_EPSILON) || isItFirstDraw) {
                    drawContext.GetCanvas().AddXObjectFittedIntoRectangle(backgroundXObject, imageRectangle);
                    isItFirstDraw = false;
                }
                isCurrentOverlaps = imageRectangle.Overlaps(backgroundArea, OVERLAP_EPSILON);
                if (counterX % 2 == 1) {
                    isNextOverlaps = imageRectangle.MoveRight((imageRectangle.GetWidth() + xWhitespace) * counterX).Overlaps(backgroundArea
                        , OVERLAP_EPSILON);
                }
                else {
                    isNextOverlaps = imageRectangle.MoveLeft((imageRectangle.GetWidth() + xWhitespace) * counterX).Overlaps(backgroundArea
                        , OVERLAP_EPSILON);
                }
                ++counterX;
            }
            while (!backgroundImage.GetRepeat().IsNoRepeatOnXAxis() && (isCurrentOverlaps || isNextOverlaps));
        }

        /// <summary>
        /// Create a
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// with the given area and containing a linear gradient inside.
        /// </summary>
        /// <param name="linearGradientBuilder">the linear gradient builder</param>
        /// <param name="xObjectArea">the result object area</param>
        /// <param name="document">the pdf document</param>
        /// <returns>the xObject with a specified area and a linear gradient</returns>
        public static PdfFormXObject CreateXObject(AbstractLinearGradientBuilder linearGradientBuilder, Rectangle 
            xObjectArea, PdfDocument document) {
            Rectangle formBBox = new Rectangle(0, 0, xObjectArea.GetWidth(), xObjectArea.GetHeight());
            PdfFormXObject xObject = new PdfFormXObject(formBBox);
            if (linearGradientBuilder != null) {
                Color gradientColor = linearGradientBuilder.BuildColor(formBBox, null, document);
                if (gradientColor != null) {
                    new PdfCanvas(xObject, document).SetColor(gradientColor, true).Rectangle(formBBox).Fill();
                }
            }
            return xObject;
        }

        /// <summary>Evaluate the actual background</summary>
        /// <param name="occupiedAreaWithMargins">the current occupied area with applied margins</param>
        /// <returns>the actual background area</returns>
        protected internal virtual Rectangle GetBackgroundArea(Rectangle occupiedAreaWithMargins) {
            return occupiedAreaWithMargins;
        }

        protected internal virtual bool ClipBorderArea(DrawContext drawContext, Rectangle outerBorderBox) {
            return ClipArea(drawContext, outerBorderBox, true, true, false, true);
        }

        protected internal virtual bool ClipBackgroundArea(DrawContext drawContext, Rectangle outerBorderBox) {
            return ClipArea(drawContext, outerBorderBox, true, false, false, false);
        }

        protected internal virtual bool ClipBackgroundArea(DrawContext drawContext, Rectangle outerBorderBox, bool
             considerBordersBeforeClipping) {
            return ClipArea(drawContext, outerBorderBox, true, false, considerBordersBeforeClipping, false);
        }

        private bool ClipArea(DrawContext drawContext, Rectangle outerBorderBox, bool clipOuter, bool clipInner, bool
             considerBordersBeforeOuterClipping, bool considerBordersBeforeInnerClipping) {
            // border widths should be considered only once
            System.Diagnostics.Debug.Assert(false == considerBordersBeforeOuterClipping || false == considerBordersBeforeInnerClipping
                );
            // border widths
            float[] borderWidths = new float[] { 0, 0, 0, 0 };
            // outer box
            float[] outerBox = new float[] { outerBorderBox.GetTop(), outerBorderBox.GetRight(), outerBorderBox.GetBottom
                (), outerBorderBox.GetLeft() };
            // radii
            bool hasNotNullRadius = false;
            BorderRadius[] borderRadii = GetBorderRadii();
            float[] verticalRadii = CalculateRadii(borderRadii, outerBorderBox, false);
            float[] horizontalRadii = CalculateRadii(borderRadii, outerBorderBox, true);
            for (int i = 0; i < 4; i++) {
                verticalRadii[i] = Math.Min(verticalRadii[i], outerBorderBox.GetHeight() / 2);
                horizontalRadii[i] = Math.Min(horizontalRadii[i], outerBorderBox.GetWidth() / 2);
                if (!hasNotNullRadius && (0 != verticalRadii[i] || 0 != horizontalRadii[i])) {
                    hasNotNullRadius = true;
                }
            }
            if (hasNotNullRadius) {
                // coordinates of corner centers
                float[] cornersX = new float[] { outerBox[3] + horizontalRadii[0], outerBox[1] - horizontalRadii[1], outerBox
                    [1] - horizontalRadii[2], outerBox[3] + horizontalRadii[3] };
                float[] cornersY = new float[] { outerBox[0] - verticalRadii[0], outerBox[0] - verticalRadii[1], outerBox[
                    2] + verticalRadii[2], outerBox[2] + verticalRadii[3] };
                PdfCanvas canvas = drawContext.GetCanvas();
                canvas.SaveState();
                if (considerBordersBeforeOuterClipping) {
                    borderWidths = DecreaseBorderRadiiWithBorders(horizontalRadii, verticalRadii, outerBox, cornersX, cornersY
                        );
                }
                // clip border area outside
                if (clipOuter) {
                    ClipOuterArea(canvas, horizontalRadii, verticalRadii, outerBox, cornersX, cornersY);
                }
                if (considerBordersBeforeInnerClipping) {
                    borderWidths = DecreaseBorderRadiiWithBorders(horizontalRadii, verticalRadii, outerBox, cornersX, cornersY
                        );
                }
                // clip border area inside
                if (clipInner) {
                    ClipInnerArea(canvas, horizontalRadii, verticalRadii, outerBox, cornersX, cornersY, borderWidths);
                }
            }
            return hasNotNullRadius;
        }

        private void ClipOuterArea(PdfCanvas canvas, float[] horizontalRadii, float[] verticalRadii, float[] outerBox
            , float[] cornersX, float[] cornersY) {
            double top = outerBox[TOP_SIDE];
            double right = outerBox[RIGHT_SIDE];
            double bottom = outerBox[BOTTOM_SIDE];
            double left = outerBox[LEFT_SIDE];
            // left top corner
            if (0 != horizontalRadii[0] || 0 != verticalRadii[0]) {
                double arcBottom = ((double)cornersY[TOP_SIDE]) - verticalRadii[TOP_SIDE];
                double arcRight = ((double)cornersX[TOP_SIDE]) + horizontalRadii[TOP_SIDE];
                canvas.MoveTo(left, bottom).ArcContinuous(left, arcBottom, arcRight, top, ARC_LEFT_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                    ).LineTo(right, top).LineTo(right, bottom).LineTo(left, bottom);
                canvas.Clip().EndPath();
            }
            // right top corner
            if (0 != horizontalRadii[1] || 0 != verticalRadii[1]) {
                double arcLeft = ((double)cornersX[RIGHT_SIDE]) - horizontalRadii[RIGHT_SIDE];
                double arcBottom = ((double)cornersY[RIGHT_SIDE]) - verticalRadii[RIGHT_SIDE];
                canvas.MoveTo(left, top).ArcContinuous(arcLeft, top, right, arcBottom, ARC_TOP_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                    ).LineTo(right, bottom).LineTo(left, bottom).LineTo(left, top);
                canvas.Clip().EndPath();
            }
            // right bottom corner
            if (0 != horizontalRadii[2] || 0 != verticalRadii[2]) {
                double arcTop = ((double)cornersY[BOTTOM_SIDE]) + verticalRadii[BOTTOM_SIDE];
                double arcLeft = ((double)cornersX[BOTTOM_SIDE]) - horizontalRadii[BOTTOM_SIDE];
                canvas.MoveTo(right, top).ArcContinuous(right, arcTop, arcLeft, bottom, ARC_RIGHT_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                    ).LineTo(left, bottom).LineTo(left, top).LineTo(right, top);
                canvas.Clip().EndPath();
            }
            // left bottom corner
            if (0 != horizontalRadii[3] || 0 != verticalRadii[3]) {
                double arcRight = ((double)cornersX[LEFT_SIDE]) + horizontalRadii[LEFT_SIDE];
                double arcTop = ((double)cornersY[LEFT_SIDE]) + verticalRadii[LEFT_SIDE];
                canvas.MoveTo(right, bottom).ArcContinuous(arcRight, bottom, left, arcTop, ARC_BOTTOM_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                    ).LineTo(left, top).LineTo(right, top).LineTo(right, bottom);
                canvas.Clip().EndPath();
            }
        }

        private void ClipInnerArea(PdfCanvas canvas, float[] horizontalRadii, float[] verticalRadii, float[] outerBox
            , float[] cornersX, float[] cornersY, float[] borderWidths) {
            double top = outerBox[TOP_SIDE];
            double right = outerBox[RIGHT_SIDE];
            double bottom = outerBox[BOTTOM_SIDE];
            double left = outerBox[LEFT_SIDE];
            double x1 = cornersX[TOP_SIDE];
            double y1 = cornersY[TOP_SIDE];
            double x2 = cornersX[RIGHT_SIDE];
            double y2 = cornersY[RIGHT_SIDE];
            double x3 = cornersX[BOTTOM_SIDE];
            double y3 = cornersY[BOTTOM_SIDE];
            double x4 = cornersX[LEFT_SIDE];
            double y4 = cornersY[LEFT_SIDE];
            double topBorderWidth = borderWidths[TOP_SIDE];
            double rightBorderWidth = borderWidths[RIGHT_SIDE];
            double bottomBorderWidth = borderWidths[BOTTOM_SIDE];
            double leftBorderWidth = borderWidths[LEFT_SIDE];
            // left top corner
            if (0 != horizontalRadii[0] || 0 != verticalRadii[0]) {
                canvas.Arc(left, y1 - verticalRadii[TOP_SIDE], x1 + horizontalRadii[TOP_SIDE], top, ARC_LEFT_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                    ).LineTo(x2, top).LineTo(right, y2).LineTo(right, y3).LineTo(x3, bottom).LineTo(x4, bottom).LineTo(left
                    , y4).LineTo(left, y1).LineTo(left - leftBorderWidth, y1).LineTo(left - leftBorderWidth, bottom - bottomBorderWidth
                    ).LineTo(right + rightBorderWidth, bottom - bottomBorderWidth).LineTo(right + rightBorderWidth, top + 
                    topBorderWidth).LineTo(left - leftBorderWidth, top + topBorderWidth).LineTo(left - leftBorderWidth, y1
                    );
                canvas.Clip().EndPath();
            }
            // right top corner
            if (0 != horizontalRadii[1] || 0 != verticalRadii[1]) {
                canvas.Arc(x2 - horizontalRadii[RIGHT_SIDE], top, right, y2 - verticalRadii[RIGHT_SIDE], ARC_TOP_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                    ).LineTo(right, y3).LineTo(x3, bottom).LineTo(x4, bottom).LineTo(left, y4).LineTo(left, y1).LineTo(x1, 
                    top).LineTo(x2, top).LineTo(x2, top + topBorderWidth).LineTo(left - leftBorderWidth, top + topBorderWidth
                    ).LineTo(left - leftBorderWidth, bottom - bottomBorderWidth).LineTo(right + rightBorderWidth, bottom -
                     bottomBorderWidth).LineTo(right + rightBorderWidth, top + topBorderWidth).LineTo(x2, top + topBorderWidth
                    );
                canvas.Clip().EndPath();
            }
            // right bottom corner
            if (0 != horizontalRadii[2] || 0 != verticalRadii[2]) {
                canvas.Arc(right, y3 + verticalRadii[BOTTOM_SIDE], x3 - horizontalRadii[BOTTOM_SIDE], bottom, ARC_RIGHT_DEGREE
                    , ARC_QUARTER_CLOCKWISE_EXTENT).LineTo(x4, bottom).LineTo(left, y4).LineTo(left, y1).LineTo(x1, top).LineTo
                    (x2, top).LineTo(right, y2).LineTo(right, y3).LineTo(right + rightBorderWidth, y3).LineTo(right + rightBorderWidth
                    , top + topBorderWidth).LineTo(left - leftBorderWidth, top + topBorderWidth).LineTo(left - leftBorderWidth
                    , bottom - bottomBorderWidth).LineTo(right + rightBorderWidth, bottom - bottomBorderWidth).LineTo(right
                     + rightBorderWidth, y3);
                canvas.Clip().EndPath();
            }
            // left bottom corner
            if (0 != horizontalRadii[3] || 0 != verticalRadii[3]) {
                canvas.Arc(x4 + horizontalRadii[LEFT_SIDE], bottom, left, y4 + verticalRadii[LEFT_SIDE], ARC_BOTTOM_DEGREE
                    , ARC_QUARTER_CLOCKWISE_EXTENT).LineTo(left, y1).LineTo(x1, top).LineTo(x2, top).LineTo(right, y2).LineTo
                    (right, y3).LineTo(x3, bottom).LineTo(x4, bottom).LineTo(x4, bottom - bottomBorderWidth).LineTo(right 
                    + rightBorderWidth, bottom - bottomBorderWidth).LineTo(right + rightBorderWidth, top + topBorderWidth)
                    .LineTo(left - leftBorderWidth, top + topBorderWidth).LineTo(left - leftBorderWidth, bottom - bottomBorderWidth
                    ).LineTo(x4, bottom - bottomBorderWidth);
                canvas.Clip().EndPath();
            }
        }

        private float[] DecreaseBorderRadiiWithBorders(float[] horizontalRadii, float[] verticalRadii, float[] outerBox
            , float[] cornersX, float[] cornersY) {
            Border[] borders = GetBorders();
            float[] borderWidths = new float[] { 0, 0, 0, 0 };
            if (borders[0] != null) {
                borderWidths[0] = borders[0].GetWidth();
                outerBox[0] -= borders[0].GetWidth();
                if (cornersY[1] > outerBox[0]) {
                    cornersY[1] = outerBox[0];
                }
                if (cornersY[0] > outerBox[0]) {
                    cornersY[0] = outerBox[0];
                }
                verticalRadii[0] = Math.Max(0, verticalRadii[0] - borders[0].GetWidth());
                verticalRadii[1] = Math.Max(0, verticalRadii[1] - borders[0].GetWidth());
            }
            if (borders[1] != null) {
                borderWidths[1] = borders[1].GetWidth();
                outerBox[1] -= borders[1].GetWidth();
                if (cornersX[1] > outerBox[1]) {
                    cornersX[1] = outerBox[1];
                }
                if (cornersX[2] > outerBox[1]) {
                    cornersX[2] = outerBox[1];
                }
                horizontalRadii[1] = Math.Max(0, horizontalRadii[1] - borders[1].GetWidth());
                horizontalRadii[2] = Math.Max(0, horizontalRadii[2] - borders[1].GetWidth());
            }
            if (borders[2] != null) {
                borderWidths[2] = borders[2].GetWidth();
                outerBox[2] += borders[2].GetWidth();
                if (cornersY[2] < outerBox[2]) {
                    cornersY[2] = outerBox[2];
                }
                if (cornersY[3] < outerBox[2]) {
                    cornersY[3] = outerBox[2];
                }
                verticalRadii[2] = Math.Max(0, verticalRadii[2] - borders[2].GetWidth());
                verticalRadii[3] = Math.Max(0, verticalRadii[3] - borders[2].GetWidth());
            }
            if (borders[3] != null) {
                borderWidths[3] = borders[3].GetWidth();
                outerBox[3] += borders[3].GetWidth();
                if (cornersX[3] < outerBox[3]) {
                    cornersX[3] = outerBox[3];
                }
                if (cornersX[0] < outerBox[3]) {
                    cornersX[0] = outerBox[3];
                }
                horizontalRadii[3] = Math.Max(0, horizontalRadii[3] - borders[3].GetWidth());
                horizontalRadii[0] = Math.Max(0, horizontalRadii[0] - borders[3].GetWidth());
            }
            return borderWidths;
        }

        /// <summary>
        /// Performs the drawing operation for all
        /// <see cref="IRenderer">children</see>
        /// of this renderer.
        /// </summary>
        /// <param name="drawContext">the context (canvas, document, etc) of this drawing operation.</param>
        public virtual void DrawChildren(DrawContext drawContext) {
            IList<IRenderer> waitingRenderers = new List<IRenderer>();
            foreach (IRenderer child in childRenderers) {
                Transform transformProp = child.GetProperty<Transform>(Property.TRANSFORM);
                RootRenderer rootRenderer = GetRootRenderer();
                IList<IRenderer> waiting = (rootRenderer != null && !rootRenderer.waitingDrawingElements.Contains(child)) ? 
                    rootRenderer.waitingDrawingElements : waitingRenderers;
                ProcessWaitingDrawing(child, transformProp, waiting);
                if (!FloatingHelper.IsRendererFloating(child) && transformProp == null) {
                    child.Draw(drawContext);
                }
            }
            foreach (IRenderer waitingRenderer in waitingRenderers) {
                waitingRenderer.Draw(drawContext);
            }
        }

        /// <summary>
        /// Performs the drawing operation for the border of this renderer, if
        /// defined by any of the
        /// <see cref="iText.Layout.Properties.Property.BORDER"/>
        /// values in either the layout
        /// element or this
        /// <see cref="IRenderer"/>
        /// itself.
        /// </summary>
        /// <param name="drawContext">the context (canvas, document, etc) of this drawing operation.</param>
        public virtual void DrawBorder(DrawContext drawContext) {
            Border[] borders = GetBorders();
            bool gotBorders = false;
            foreach (Border border in borders) {
                gotBorders = gotBorders || border != null;
            }
            if (gotBorders) {
                float topWidth = borders[0] != null ? borders[0].GetWidth() : 0;
                float rightWidth = borders[1] != null ? borders[1].GetWidth() : 0;
                float bottomWidth = borders[2] != null ? borders[2].GetWidth() : 0;
                float leftWidth = borders[3] != null ? borders[3].GetWidth() : 0;
                Rectangle bBox = GetBorderAreaBBox();
                if (bBox.GetWidth() < 0 || bBox.GetHeight() < 0) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.RECTANGLE_HAS_NEGATIVE_SIZE, "border"
                        ));
                    return;
                }
                float x1 = bBox.GetX();
                float y1 = bBox.GetY();
                float x2 = bBox.GetX() + bBox.GetWidth();
                float y2 = bBox.GetY() + bBox.GetHeight();
                bool isTagged = drawContext.IsTaggingEnabled();
                PdfCanvas canvas = drawContext.GetCanvas();
                if (isTagged) {
                    canvas.OpenTag(new CanvasArtifact());
                }
                Rectangle borderRect = ApplyMargins(occupiedArea.GetBBox().Clone(), GetMargins(), false);
                bool isAreaClipped = ClipBorderArea(drawContext, borderRect);
                BorderRadius[] borderRadii = GetBorderRadii();
                float[] verticalRadii = CalculateRadii(borderRadii, borderRect, false);
                float[] horizontalRadii = CalculateRadii(borderRadii, borderRect, true);
                for (int i = 0; i < 4; i++) {
                    verticalRadii[i] = Math.Min(verticalRadii[i], borderRect.GetHeight() / 2);
                    horizontalRadii[i] = Math.Min(horizontalRadii[i], borderRect.GetWidth() / 2);
                }
                if (borders[0] != null) {
                    if (0 != horizontalRadii[0] || 0 != verticalRadii[0] || 0 != horizontalRadii[1] || 0 != verticalRadii[1]) {
                        borders[0].Draw(canvas, x1, y2, x2, y2, horizontalRadii[0], verticalRadii[0], horizontalRadii[1], verticalRadii
                            [1], Border.Side.TOP, leftWidth, rightWidth);
                    }
                    else {
                        borders[0].Draw(canvas, x1, y2, x2, y2, Border.Side.TOP, leftWidth, rightWidth);
                    }
                }
                if (borders[1] != null) {
                    if (0 != horizontalRadii[1] || 0 != verticalRadii[1] || 0 != horizontalRadii[2] || 0 != verticalRadii[2]) {
                        borders[1].Draw(canvas, x2, y2, x2, y1, horizontalRadii[1], verticalRadii[1], horizontalRadii[2], verticalRadii
                            [2], Border.Side.RIGHT, topWidth, bottomWidth);
                    }
                    else {
                        borders[1].Draw(canvas, x2, y2, x2, y1, Border.Side.RIGHT, topWidth, bottomWidth);
                    }
                }
                if (borders[2] != null) {
                    if (0 != horizontalRadii[2] || 0 != verticalRadii[2] || 0 != horizontalRadii[3] || 0 != verticalRadii[3]) {
                        borders[2].Draw(canvas, x2, y1, x1, y1, horizontalRadii[2], verticalRadii[2], horizontalRadii[3], verticalRadii
                            [3], Border.Side.BOTTOM, rightWidth, leftWidth);
                    }
                    else {
                        borders[2].Draw(canvas, x2, y1, x1, y1, Border.Side.BOTTOM, rightWidth, leftWidth);
                    }
                }
                if (borders[3] != null) {
                    if (0 != horizontalRadii[3] || 0 != verticalRadii[3] || 0 != horizontalRadii[0] || 0 != verticalRadii[0]) {
                        borders[3].Draw(canvas, x1, y1, x1, y2, horizontalRadii[3], verticalRadii[3], horizontalRadii[0], verticalRadii
                            [0], Border.Side.LEFT, bottomWidth, topWidth);
                    }
                    else {
                        borders[3].Draw(canvas, x1, y1, x1, y2, Border.Side.LEFT, bottomWidth, topWidth);
                    }
                }
                if (isAreaClipped) {
                    drawContext.GetCanvas().RestoreState();
                }
                if (isTagged) {
                    canvas.CloseTag();
                }
            }
        }

        /// <summary>
        /// Indicates whether this renderer is flushed or not, i.e. if
        /// <see cref="Draw(DrawContext)"/>
        /// has already
        /// been called.
        /// </summary>
        /// <returns>whether the renderer has been flushed</returns>
        /// <seealso cref="Draw(DrawContext)"/>
        public virtual bool IsFlushed() {
            return flushed;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRenderer SetParent(IRenderer parent) {
            this.parent = parent;
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRenderer GetParent() {
            return parent;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Move(float dxRight, float dyUp) {
            ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
            if (occupiedArea == null) {
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED
                    , "Moving won't be performed."));
                return;
            }
            occupiedArea.GetBBox().MoveRight(dxRight);
            occupiedArea.GetBBox().MoveUp(dyUp);
            foreach (IRenderer childRenderer in childRenderers) {
                childRenderer.Move(dxRight, dyUp);
            }
            foreach (IRenderer childRenderer in positionedRenderers) {
                childRenderer.Move(dxRight, dyUp);
            }
        }

        /// <summary>
        /// Gets all rectangles that this
        /// <see cref="IRenderer"/>
        /// can draw upon in the given area.
        /// </summary>
        /// <param name="area">
        /// a physical area on the
        /// <see cref="DrawContext"/>
        /// </param>
        /// <returns>
        /// a list of
        /// <see cref="iText.Kernel.Geom.Rectangle">rectangles</see>
        /// </returns>
        public virtual IList<Rectangle> InitElementAreas(LayoutArea area) {
            return JavaCollectionsUtil.SingletonList(area.GetBBox());
        }

        /// <summary>
        /// Gets the bounding box that contains all content written to the
        /// <see cref="DrawContext"/>
        /// by this
        /// <see cref="IRenderer"/>.
        /// </summary>
        /// <returns>
        /// the smallest
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// that surrounds the content
        /// </returns>
        public virtual Rectangle GetOccupiedAreaBBox() {
            return occupiedArea.GetBBox().Clone();
        }

        /// <summary>Gets the border box of a renderer.</summary>
        /// <remarks>
        /// Gets the border box of a renderer.
        /// This is a box used to draw borders.
        /// </remarks>
        /// <returns>border box of a renderer</returns>
        public virtual Rectangle GetBorderAreaBBox() {
            Rectangle rect = GetOccupiedAreaBBox();
            ApplyMargins(rect, false);
            ApplyBorderBox(rect, false);
            return rect;
        }

        public virtual Rectangle GetInnerAreaBBox() {
            Rectangle rect = GetOccupiedAreaBBox();
            ApplyMargins(rect, false);
            ApplyBorderBox(rect, false);
            ApplyPaddings(rect, false);
            return rect;
        }

        /// <summary>Applies margins, borders and paddings of the renderer on the given rectangle.</summary>
        /// <param name="rect">a rectangle margins, borders and paddings will be applied on.</param>
        /// <param name="reverse">
        /// indicates whether margins, borders and paddings will be applied
        /// inside (in case of false) or outside (in case of true) the rectangle.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle">border box</see>
        /// of the renderer
        /// </returns>
        internal virtual Rectangle ApplyMarginsBordersPaddings(Rectangle rect, bool reverse) {
            ApplyMargins(rect, reverse);
            ApplyBorderBox(rect, reverse);
            ApplyPaddings(rect, reverse);
            return rect;
        }

        /// <summary>Applies margins of the renderer on the given rectangle</summary>
        /// <param name="rect">a rectangle margins will be applied on.</param>
        /// <param name="reverse">
        /// indicates whether margins will be applied
        /// inside (in case of false) or outside (in case of true) the rectangle.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle">border box</see>
        /// of the renderer
        /// </returns>
        /// <seealso cref="GetMargins()"/>
        public virtual Rectangle ApplyMargins(Rectangle rect, bool reverse) {
            return this.ApplyMargins(rect, GetMargins(), reverse);
        }

        /// <summary>
        /// Applies the border box of the renderer on the given rectangle
        /// If the border of a certain side is null, the side will remain as it was.
        /// </summary>
        /// <param name="rect">a rectangle the border box will be applied on.</param>
        /// <param name="reverse">
        /// indicates whether the border box will be applied
        /// inside (in case of false) or outside (in case of true) the rectangle.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle">border box</see>
        /// of the renderer
        /// </returns>
        /// <seealso cref="GetBorders()"/>
        public virtual Rectangle ApplyBorderBox(Rectangle rect, bool reverse) {
            Border[] borders = GetBorders();
            return ApplyBorderBox(rect, borders, reverse);
        }

        /// <summary>Applies paddings of the renderer on the given rectangle</summary>
        /// <param name="rect">a rectangle paddings will be applied on.</param>
        /// <param name="reverse">
        /// indicates whether paddings will be applied
        /// inside (in case of false) or outside (in case of true) the rectangle.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle">border box</see>
        /// of the renderer
        /// </returns>
        /// <seealso cref="GetPaddings()"/>
        public virtual Rectangle ApplyPaddings(Rectangle rect, bool reverse) {
            return ApplyPaddings(rect, GetPaddings(), reverse);
        }

        public virtual bool IsFirstOnRootArea() {
            return IsFirstOnRootArea(false);
        }

        protected internal virtual void ApplyDestinationsAndAnnotation(DrawContext drawContext) {
            ApplyDestination(drawContext.GetDocument());
            ApplyAction(drawContext.GetDocument());
            ApplyLinkAnnotation(drawContext.GetDocument());
        }

        protected internal static bool IsBorderBoxSizing(IRenderer renderer) {
            BoxSizingPropertyValue? boxSizing = renderer.GetProperty<BoxSizingPropertyValue?>(Property.BOX_SIZING);
            return boxSizing != null && boxSizing.Equals(BoxSizingPropertyValue.BORDER_BOX);
        }

        protected internal virtual bool IsOverflowProperty(OverflowPropertyValue? equalsTo, int overflowProperty) {
            return IsOverflowProperty(equalsTo, this.GetProperty<OverflowPropertyValue?>(overflowProperty));
        }

        protected internal static bool IsOverflowProperty(OverflowPropertyValue? equalsTo, IRenderer renderer, int
             overflowProperty) {
            return IsOverflowProperty(equalsTo, renderer.GetProperty<OverflowPropertyValue?>(overflowProperty));
        }

        protected internal static bool IsOverflowProperty(OverflowPropertyValue? equalsTo, OverflowPropertyValue? 
            rendererOverflowProperty) {
            return equalsTo.Equals(rendererOverflowProperty) || equalsTo.Equals(OverflowPropertyValue.FIT) && rendererOverflowProperty
                 == null;
        }

        protected internal static bool IsOverflowFit(OverflowPropertyValue? rendererOverflowProperty) {
            return rendererOverflowProperty == null || OverflowPropertyValue.FIT.Equals(rendererOverflowProperty);
        }

        /// <summary>Replaces given property own value with the given value.</summary>
        /// <param name="property">the property to be replaced</param>
        /// <param name="replacementValue">the value with which property will be replaced</param>
        /// <typeparam name="T">the type associated with the property</typeparam>
        /// <returns>previous property value</returns>
        internal virtual T ReplaceOwnProperty<T>(int property, T replacementValue) {
            T ownProperty = this.GetOwnProperty<T>(property);
            SetProperty(property, replacementValue);
            return ownProperty;
        }

        /// <summary>Returns back own value of the given property.</summary>
        /// <param name="property">the property to be returned back</param>
        /// <param name="prevValue">the value which will be returned back</param>
        /// <typeparam name="T">the type associated with the property</typeparam>
        internal virtual void ReturnBackOwnProperty<T>(int property, T prevValue) {
            if (prevValue == null) {
                DeleteOwnProperty(property);
            }
            else {
                SetProperty(property, prevValue);
            }
        }

        /// <summary>Checks if this renderer has intrinsic aspect ratio.</summary>
        /// <returns>true, if aspect ratio is defined for this renderer, false otherwise</returns>
        internal virtual bool HasAspectRatio() {
            // TODO DEVSIX-5255 This method should be changed after we support aspect-ratio property
            return false;
        }

        /// <summary>Gets intrinsic aspect ratio for this renderer.</summary>
        /// <returns>aspect ratio, if it is defined for this renderer, null otherwise</returns>
        internal virtual float? GetAspectRatio() {
            // TODO DEVSIX-5255 This method should be changed after we support aspect-ratio property
            return null;
        }

        internal static void ProcessWaitingDrawing(IRenderer child, Transform transformProp, IList<IRenderer> waitingDrawing
            ) {
            if (FloatingHelper.IsRendererFloating(child) || transformProp != null) {
                waitingDrawing.Add(child);
            }
            Border outlineProp = child.GetProperty<Border>(Property.OUTLINE);
            if (outlineProp != null && child is iText.Layout.Renderer.AbstractRenderer) {
                iText.Layout.Renderer.AbstractRenderer abstractChild = (iText.Layout.Renderer.AbstractRenderer)child;
                if (abstractChild.IsRelativePosition()) {
                    abstractChild.ApplyRelativePositioningTranslation(false);
                }
                Div outlines = new Div().SetNeutralRole();
                if (transformProp != null) {
                    outlines.SetProperty(Property.TRANSFORM, transformProp);
                }
                outlines.SetProperty(Property.BORDER, outlineProp);
                float offset = outlines.GetProperty<Border>(Property.BORDER).GetWidth();
                if (abstractChild.GetPropertyAsFloat(Property.OUTLINE_OFFSET) != null) {
                    offset += (float)abstractChild.GetPropertyAsFloat(Property.OUTLINE_OFFSET);
                }
                DivRenderer div = new DivRenderer(outlines);
                div.SetParent(abstractChild.GetParent());
                Rectangle divOccupiedArea = abstractChild.ApplyMargins(abstractChild.occupiedArea.Clone().GetBBox(), false
                    ).MoveLeft(offset).MoveDown(offset);
                divOccupiedArea.SetWidth(divOccupiedArea.GetWidth() + 2 * offset).SetHeight(divOccupiedArea.GetHeight() + 
                    2 * offset);
                div.occupiedArea = new LayoutArea(abstractChild.GetOccupiedArea().GetPageNumber(), divOccupiedArea);
                float outlineWidth = div.GetProperty<Border>(Property.BORDER).GetWidth();
                if (divOccupiedArea.GetWidth() >= outlineWidth * 2 && divOccupiedArea.GetHeight() >= outlineWidth * 2) {
                    waitingDrawing.Add(div);
                }
                if (abstractChild.IsRelativePosition()) {
                    abstractChild.ApplyRelativePositioningTranslation(true);
                }
            }
        }

        /// <summary>Retrieves element's fixed content box width, if it's set.</summary>
        /// <remarks>
        /// Retrieves element's fixed content box width, if it's set.
        /// Takes into account
        /// <see cref="iText.Layout.Properties.Property.BOX_SIZING"/>
        /// ,
        /// <see cref="iText.Layout.Properties.Property.MIN_WIDTH"/>
        /// ,
        /// and
        /// <see cref="iText.Layout.Properties.Property.MAX_WIDTH"/>
        /// properties.
        /// </remarks>
        /// <param name="parentBoxWidth">
        /// width of the parent element content box.
        /// If element has relative width, it will be
        /// calculated relatively to this parameter.
        /// </param>
        /// <returns>element's fixed content box width or null if it's not set.</returns>
        /// <seealso cref="HasAbsoluteUnitValue(int)"/>
        protected internal virtual float? RetrieveWidth(float parentBoxWidth) {
            float? minWidth = RetrieveUnitValue(parentBoxWidth, Property.MIN_WIDTH);
            float? maxWidth = RetrieveUnitValue(parentBoxWidth, Property.MAX_WIDTH);
            if (maxWidth != null && minWidth != null && minWidth > maxWidth) {
                maxWidth = minWidth;
            }
            float? width = RetrieveUnitValue(parentBoxWidth, Property.WIDTH);
            if (width != null) {
                if (maxWidth != null) {
                    width = width > maxWidth ? maxWidth : width;
                }
                if (minWidth != null) {
                    width = width < minWidth ? minWidth : width;
                }
            }
            else {
                if (maxWidth != null) {
                    width = maxWidth < parentBoxWidth ? maxWidth : null;
                }
            }
            if (width != null && IsBorderBoxSizing(this)) {
                width -= CalculatePaddingBorderWidth(this);
            }
            return width != null ? (float?)Math.Max(0, (float)width) : null;
        }

        /// <summary>Retrieves element's fixed content box max width, if it's set.</summary>
        /// <remarks>
        /// Retrieves element's fixed content box max width, if it's set.
        /// Takes into account
        /// <see cref="iText.Layout.Properties.Property.BOX_SIZING"/>
        /// and
        /// <see cref="iText.Layout.Properties.Property.MIN_WIDTH"/>
        /// properties.
        /// </remarks>
        /// <param name="parentBoxWidth">
        /// width of the parent element content box.
        /// If element has relative width, it will be
        /// calculated relatively to this parameter.
        /// </param>
        /// <returns>element's fixed content box max width or null if it's not set.</returns>
        /// <seealso cref="HasAbsoluteUnitValue(int)"/>
        protected internal virtual float? RetrieveMaxWidth(float parentBoxWidth) {
            float? maxWidth = RetrieveUnitValue(parentBoxWidth, Property.MAX_WIDTH);
            if (maxWidth != null) {
                float? minWidth = RetrieveUnitValue(parentBoxWidth, Property.MIN_WIDTH);
                if (minWidth != null && minWidth > maxWidth) {
                    maxWidth = minWidth;
                }
                if (IsBorderBoxSizing(this)) {
                    maxWidth -= CalculatePaddingBorderWidth(this);
                }
                return maxWidth > 0 ? maxWidth : 0;
            }
            else {
                return null;
            }
        }

        /// <summary>Retrieves element's fixed content box max width, if it's set.</summary>
        /// <remarks>
        /// Retrieves element's fixed content box max width, if it's set.
        /// Takes into account
        /// <see cref="iText.Layout.Properties.Property.BOX_SIZING"/>
        /// property value.
        /// </remarks>
        /// <param name="parentBoxWidth">
        /// width of the parent element content box.
        /// If element has relative width, it will be
        /// calculated relatively to this parameter.
        /// </param>
        /// <returns>element's fixed content box max width or null if it's not set.</returns>
        /// <seealso cref="HasAbsoluteUnitValue(int)"/>
        protected internal virtual float? RetrieveMinWidth(float parentBoxWidth) {
            float? minWidth = RetrieveUnitValue(parentBoxWidth, Property.MIN_WIDTH);
            if (minWidth != null) {
                if (IsBorderBoxSizing(this)) {
                    minWidth -= CalculatePaddingBorderWidth(this);
                }
                return minWidth > 0 ? minWidth : 0;
            }
            else {
                return null;
            }
        }

        /// <summary>Updates fixed content box width value for this renderer.</summary>
        /// <remarks>
        /// Updates fixed content box width value for this renderer.
        /// Takes into account
        /// <see cref="iText.Layout.Properties.Property.BOX_SIZING"/>
        /// property value.
        /// </remarks>
        /// <param name="updatedWidthValue">element's new fixed content box width.</param>
        protected internal virtual void UpdateWidth(UnitValue updatedWidthValue) {
            if (updatedWidthValue.IsPointValue() && IsBorderBoxSizing(this)) {
                updatedWidthValue.SetValue(updatedWidthValue.GetValue() + CalculatePaddingBorderWidth(this));
            }
            SetProperty(Property.WIDTH, updatedWidthValue);
        }

        /// <summary>Retrieves the element's fixed content box height, if it's set.</summary>
        /// <remarks>
        /// Retrieves the element's fixed content box height, if it's set.
        /// Takes into account
        /// <see cref="iText.Layout.Properties.Property.BOX_SIZING"/>
        /// ,
        /// <see cref="iText.Layout.Properties.Property.MIN_HEIGHT"/>
        /// ,
        /// and
        /// <see cref="iText.Layout.Properties.Property.MAX_HEIGHT"/>
        /// properties.
        /// </remarks>
        /// <returns>element's fixed content box height or null if it's not set.</returns>
        protected internal virtual float? RetrieveHeight() {
            float? height = null;
            UnitValue heightUV = GetPropertyAsUnitValue(Property.HEIGHT);
            float? parentResolvedHeight = RetrieveResolvedParentDeclaredHeight();
            float? minHeight = null;
            float? maxHeight = null;
            if (heightUV != null) {
                if (parentResolvedHeight == null) {
                    if (heightUV.IsPercentValue()) {
                        //If the height is a relative value and no parent with a resolved height can be found, treat it as null
                        height = null;
                    }
                    else {
                        //Since no parent height is resolved, only point-value min and max should be taken into account
                        UnitValue minHeightUV = GetPropertyAsUnitValue(Property.MIN_HEIGHT);
                        if (minHeightUV != null && minHeightUV.IsPointValue()) {
                            minHeight = minHeightUV.GetValue();
                        }
                        UnitValue maxHeightUV = GetPropertyAsUnitValue(Property.MAX_HEIGHT);
                        if (maxHeightUV != null && maxHeightUV.IsPointValue()) {
                            maxHeight = maxHeightUV.GetValue();
                        }
                        //If the height is stored as a point value, we do not care about the parent's resolved height
                        height = heightUV.GetValue();
                    }
                }
                else {
                    minHeight = RetrieveUnitValue((float)parentResolvedHeight, Property.MIN_HEIGHT);
                    maxHeight = RetrieveUnitValue((float)parentResolvedHeight, Property.MAX_HEIGHT);
                    height = RetrieveUnitValue((float)parentResolvedHeight, Property.HEIGHT);
                }
                if (maxHeight != null && minHeight != null && minHeight > maxHeight) {
                    maxHeight = minHeight;
                }
                if (height != null) {
                    if (maxHeight != null) {
                        height = height > maxHeight ? maxHeight : height;
                    }
                    if (minHeight != null) {
                        height = height < minHeight ? minHeight : height;
                    }
                }
                if (height != null && IsBorderBoxSizing(this)) {
                    height -= CalculatePaddingBorderHeight(this);
                }
            }
            return height != null ? (float?)Math.Max(0, (float)height) : null;
        }

        /// <summary>Calculates the element corner's border radii.</summary>
        /// <param name="radii">defines border radii of the element</param>
        /// <param name="area">defines the area of the element</param>
        /// <param name="horizontal">defines whether horizontal or vertical radii should be calculated</param>
        /// <returns>the element corner's border radii.</returns>
        private float[] CalculateRadii(BorderRadius[] radii, Rectangle area, bool horizontal) {
            float[] results = new float[4];
            UnitValue value;
            for (int i = 0; i < 4; i++) {
                if (null != radii[i]) {
                    value = horizontal ? radii[i].GetHorizontalRadius() : radii[i].GetVerticalRadius();
                    if (value != null) {
                        if (value.GetUnitType() == UnitValue.PERCENT) {
                            results[i] = value.GetValue() * (horizontal ? area.GetWidth() : area.GetHeight()) / 100;
                        }
                        else {
                            System.Diagnostics.Debug.Assert(value.GetUnitType() == UnitValue.POINT);
                            results[i] = value.GetValue();
                        }
                    }
                    else {
                        results[i] = 0;
                    }
                }
                else {
                    results[i] = 0;
                }
            }
            return results;
        }

        /// <summary>Updates fixed content box height value for this renderer.</summary>
        /// <remarks>
        /// Updates fixed content box height value for this renderer.
        /// Takes into account
        /// <see cref="iText.Layout.Properties.Property.BOX_SIZING"/>
        /// property value.
        /// </remarks>
        /// <param name="updatedHeight">element's new fixed content box height, shall be not null.</param>
        protected internal virtual void UpdateHeight(UnitValue updatedHeight) {
            if (IsBorderBoxSizing(this) && updatedHeight.IsPointValue()) {
                updatedHeight.SetValue(updatedHeight.GetValue() + CalculatePaddingBorderHeight(this));
            }
            SetProperty(Property.HEIGHT, updatedHeight);
        }

        /// <summary>Retrieve element's content box max-ehight, if it's set.</summary>
        /// <remarks>
        /// Retrieve element's content box max-ehight, if it's set.
        /// Takes into account
        /// <see cref="iText.Layout.Properties.Property.BOX_SIZING"/>
        /// property value.
        /// </remarks>
        /// <returns>element's content box max-height or null if it's not set.</returns>
        protected internal virtual float? RetrieveMaxHeight() {
            float? maxHeight = null;
            float? minHeight = null;
            float? directParentDeclaredHeight = RetrieveDirectParentDeclaredHeight();
            UnitValue maxHeightAsUV = GetPropertyAsUnitValue(Property.MAX_HEIGHT);
            if (maxHeightAsUV != null) {
                if (directParentDeclaredHeight == null) {
                    if (maxHeightAsUV.IsPercentValue()) {
                        maxHeight = null;
                    }
                    else {
                        minHeight = RetrieveMinHeight();
                        //Since no parent height is resolved, only point-value min should be taken into account
                        UnitValue minHeightUV = GetPropertyAsUnitValue(Property.MIN_HEIGHT);
                        if (minHeightUV != null && minHeightUV.IsPointValue()) {
                            minHeight = minHeightUV.GetValue();
                        }
                        //We don't care about a baseline if the max-height is explicitly defined
                        maxHeight = maxHeightAsUV.GetValue();
                    }
                }
                else {
                    maxHeight = RetrieveUnitValue((float)directParentDeclaredHeight, Property.MAX_HEIGHT);
                }
                if (maxHeight != null) {
                    if (minHeight != null && minHeight > maxHeight) {
                        maxHeight = minHeight;
                    }
                    if (IsBorderBoxSizing(this)) {
                        maxHeight -= CalculatePaddingBorderHeight(this);
                    }
                    return maxHeight > 0 ? maxHeight : 0;
                }
            }
            //Max height is not set, but height might be set
            return RetrieveHeight();
        }

        /// <summary>Updates content box max-height value for this renderer.</summary>
        /// <remarks>
        /// Updates content box max-height value for this renderer.
        /// Takes into account
        /// <see cref="iText.Layout.Properties.Property.BOX_SIZING"/>
        /// property value.
        /// </remarks>
        /// <param name="updatedMaxHeight">element's new content box max-height, shall be not null.</param>
        protected internal virtual void UpdateMaxHeight(UnitValue updatedMaxHeight) {
            if (IsBorderBoxSizing(this) && updatedMaxHeight.IsPointValue()) {
                updatedMaxHeight.SetValue(updatedMaxHeight.GetValue() + CalculatePaddingBorderHeight(this));
            }
            SetProperty(Property.MAX_HEIGHT, updatedMaxHeight);
        }

        /// <summary>Retrieves element's content box min-height, if it's set.</summary>
        /// <remarks>
        /// Retrieves element's content box min-height, if it's set.
        /// Takes into account
        /// <see cref="iText.Layout.Properties.Property.BOX_SIZING"/>
        /// property value.
        /// </remarks>
        /// <returns>element's content box min-height or null if it's not set.</returns>
        protected internal virtual float? RetrieveMinHeight() {
            float? minHeight = null;
            float? directParentDeclaredHeight = RetrieveDirectParentDeclaredHeight();
            UnitValue minHeightUV = GetPropertyAsUnitValue(this, Property.MIN_HEIGHT);
            if (minHeightUV != null) {
                if (directParentDeclaredHeight == null) {
                    if (minHeightUV.IsPercentValue()) {
                        //if there is no baseline to compare against, a relative value evaluates to null
                        minHeight = null;
                    }
                    else {
                        //If the min-height is stored as a point value, we do not care about a baseline.
                        minHeight = minHeightUV.GetValue();
                    }
                }
                else {
                    minHeight = RetrieveUnitValue((float)directParentDeclaredHeight, Property.MIN_HEIGHT);
                }
                if (minHeight != null) {
                    if (IsBorderBoxSizing(this)) {
                        minHeight -= CalculatePaddingBorderHeight(this);
                    }
                    return minHeight > 0 ? minHeight : 0;
                }
            }
            //min-height might be zero, but height might be set
            return RetrieveHeight();
        }

        /// <summary>Updates content box min-height value for this renderer.</summary>
        /// <remarks>
        /// Updates content box min-height value for this renderer.
        /// Takes into account
        /// <see cref="iText.Layout.Properties.Property.BOX_SIZING"/>
        /// property value.
        /// </remarks>
        /// <param name="updatedMinHeight">element's new content box min-height, shall be not null.</param>
        protected internal virtual void UpdateMinHeight(UnitValue updatedMinHeight) {
            if (IsBorderBoxSizing(this) && updatedMinHeight.IsPointValue()) {
                updatedMinHeight.SetValue(updatedMinHeight.GetValue() + CalculatePaddingBorderHeight(this));
            }
            SetProperty(Property.MIN_HEIGHT, updatedMinHeight);
        }

        protected internal virtual float? RetrieveUnitValue(float baseValue, int property) {
            return RetrieveUnitValue(baseValue, property, false);
        }

        protected internal virtual float? RetrieveUnitValue(float baseValue, int property, bool pointOnly) {
            UnitValue value = this.GetProperty<UnitValue>(property);
            if (pointOnly && value.GetUnitType() == UnitValue.POINT) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , property));
            }
            if (value != null) {
                if (value.GetUnitType() == UnitValue.PERCENT) {
                    // during mathematical operations the precision can be lost, so avoiding them if possible (100 / 100 == 1) is a good practice
                    return value.GetValue() != 100 ? baseValue * value.GetValue() / 100 : baseValue;
                }
                else {
                    System.Diagnostics.Debug.Assert(value.GetUnitType() == UnitValue.POINT);
                    return value.GetValue();
                }
            }
            else {
                return null;
            }
        }

        protected internal virtual IDictionary<int, Object> GetOwnProperties() {
            return properties;
        }

        protected internal virtual void AddAllProperties(IDictionary<int, Object> properties) {
            this.properties.AddAll(properties);
        }

        /// <summary>Gets the first yLine of the nested children recursively.</summary>
        /// <remarks>
        /// Gets the first yLine of the nested children recursively. E.g. for a list, this will be the yLine of the
        /// first item (if the first item is indeed a paragraph).
        /// NOTE: this method will no go further than the first child.
        /// </remarks>
        /// <returns>the first yline of the nested children, null if there is no text found</returns>
        protected internal virtual float? GetFirstYLineRecursively() {
            if (childRenderers.Count == 0) {
                return null;
            }
            return ((iText.Layout.Renderer.AbstractRenderer)childRenderers[0]).GetFirstYLineRecursively();
        }

        protected internal virtual float? GetLastYLineRecursively() {
            if (!AllowLastYLineRecursiveExtraction()) {
                return null;
            }
            for (int i = childRenderers.Count - 1; i >= 0; i--) {
                IRenderer child = childRenderers[i];
                if (child is iText.Layout.Renderer.AbstractRenderer) {
                    float? lastYLine = ((iText.Layout.Renderer.AbstractRenderer)child).GetLastYLineRecursively();
                    if (lastYLine != null) {
                        return lastYLine;
                    }
                }
            }
            return null;
        }

        protected internal virtual bool AllowLastYLineRecursiveExtraction() {
            return !IsOverflowProperty(OverflowPropertyValue.HIDDEN, Property.OVERFLOW_X) && !IsOverflowProperty(OverflowPropertyValue
                .HIDDEN, Property.OVERFLOW_Y);
        }

        /// <summary>Applies given margins on the given rectangle</summary>
        /// <param name="rect">a rectangle margins will be applied on.</param>
        /// <param name="margins">the margins to be applied on the given rectangle</param>
        /// <param name="reverse">
        /// indicates whether margins will be applied
        /// inside (in case of false) or outside (in case of true) the rectangle.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle">border box</see>
        /// of the renderer
        /// </returns>
        protected internal virtual Rectangle ApplyMargins(Rectangle rect, UnitValue[] margins, bool reverse) {
            if (!margins[TOP_SIDE].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_TOP));
            }
            if (!margins[RIGHT_SIDE].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_RIGHT));
            }
            if (!margins[BOTTOM_SIDE].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_BOTTOM));
            }
            if (!margins[LEFT_SIDE].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_LEFT));
            }
            return rect.ApplyMargins(margins[TOP_SIDE].GetValue(), margins[RIGHT_SIDE].GetValue(), margins[BOTTOM_SIDE
                ].GetValue(), margins[LEFT_SIDE].GetValue(), reverse);
        }

        /// <summary>Returns margins of the renderer</summary>
        /// <returns>
        /// a
        /// <c>float[]</c>
        /// margins of the renderer
        /// </returns>
        protected internal virtual UnitValue[] GetMargins() {
            return GetMargins(this);
        }

        /// <summary>Returns paddings of the renderer</summary>
        /// <returns>
        /// a
        /// <c>float[]</c>
        /// paddings of the renderer
        /// </returns>
        protected internal virtual UnitValue[] GetPaddings() {
            return GetPaddings(this);
        }

        /// <summary>Applies given paddings on the given rectangle</summary>
        /// <param name="rect">a rectangle paddings will be applied on.</param>
        /// <param name="paddings">the paddings to be applied on the given rectangle</param>
        /// <param name="reverse">
        /// indicates whether paddings will be applied
        /// inside (in case of false) or outside (in case of false) the rectangle.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle">border box</see>
        /// of the renderer
        /// </returns>
        protected internal virtual Rectangle ApplyPaddings(Rectangle rect, UnitValue[] paddings, bool reverse) {
            if (paddings[0] != null && !paddings[0].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_TOP));
            }
            if (paddings[1] != null && !paddings[1].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_RIGHT));
            }
            if (paddings[2] != null && !paddings[2].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_BOTTOM));
            }
            if (paddings[3] != null && !paddings[3].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_LEFT));
            }
            return rect.ApplyMargins(paddings[0] != null ? paddings[0].GetValue() : 0, paddings[1] != null ? paddings[
                1].GetValue() : 0, paddings[2] != null ? paddings[2].GetValue() : 0, paddings[3] != null ? paddings[3]
                .GetValue() : 0, reverse);
        }

        /// <summary>Applies the given border box (borders) on the given rectangle</summary>
        /// <param name="rect">a rectangle paddings will be applied on.</param>
        /// <param name="borders">
        /// the
        /// <see cref="iText.Layout.Borders.Border">borders</see>
        /// to be applied on the given rectangle
        /// </param>
        /// <param name="reverse">
        /// indicates whether the border box will be applied
        /// inside (in case of false) or outside (in case of false) the rectangle.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle">border box</see>
        /// of the renderer
        /// </returns>
        protected internal virtual Rectangle ApplyBorderBox(Rectangle rect, Border[] borders, bool reverse) {
            float topWidth = borders[0] != null ? borders[0].GetWidth() : 0;
            float rightWidth = borders[1] != null ? borders[1].GetWidth() : 0;
            float bottomWidth = borders[2] != null ? borders[2].GetWidth() : 0;
            float leftWidth = borders[3] != null ? borders[3].GetWidth() : 0;
            return rect.ApplyMargins(topWidth, rightWidth, bottomWidth, leftWidth, reverse);
        }

        protected internal virtual void ApplyAbsolutePosition(Rectangle parentRect) {
            float? top = this.GetPropertyAsFloat(Property.TOP);
            float? bottom = this.GetPropertyAsFloat(Property.BOTTOM);
            float? left = this.GetPropertyAsFloat(Property.LEFT);
            float? right = this.GetPropertyAsFloat(Property.RIGHT);
            if (left == null && right == null && BaseDirection.RIGHT_TO_LEFT.Equals(this.GetProperty<BaseDirection?>(Property
                .BASE_DIRECTION))) {
                right = 0f;
            }
            if (top == null && bottom == null) {
                top = 0f;
            }
            try {
                if (right != null) {
                    Move(parentRect.GetRight() - (float)right - occupiedArea.GetBBox().GetRight(), 0);
                }
                if (left != null) {
                    Move(parentRect.GetLeft() + (float)left - occupiedArea.GetBBox().GetLeft(), 0);
                }
                if (top != null) {
                    Move(0, parentRect.GetTop() - (float)top - occupiedArea.GetBBox().GetTop());
                }
                if (bottom != null) {
                    Move(0, parentRect.GetBottom() + (float)bottom - occupiedArea.GetBBox().GetBottom());
                }
            }
            catch (Exception) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED
                    , "Absolute positioning might be applied incorrectly."));
            }
        }

        protected internal virtual void ApplyRelativePositioningTranslation(bool reverse) {
            float top = (float)this.GetPropertyAsFloat(Property.TOP, 0f);
            float bottom = (float)this.GetPropertyAsFloat(Property.BOTTOM, 0f);
            float left = (float)this.GetPropertyAsFloat(Property.LEFT, 0f);
            float right = (float)this.GetPropertyAsFloat(Property.RIGHT, 0f);
            int reverseMultiplier = reverse ? -1 : 1;
            float dxRight = left != 0 ? left * reverseMultiplier : -right * reverseMultiplier;
            float dyUp = top != 0 ? -top * reverseMultiplier : bottom * reverseMultiplier;
            if (dxRight != 0 || dyUp != 0) {
                Move(dxRight, dyUp);
            }
        }

        protected internal virtual void ApplyDestination(PdfDocument document) {
            String destination = this.GetProperty<String>(Property.DESTINATION);
            if (destination != null) {
                int pageNumber = occupiedArea.GetPageNumber();
                if (pageNumber < 1 || pageNumber > document.GetNumberOfPages()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                    String logMessageArg = "Property.DESTINATION, which specifies this element location as destination, see ElementPropertyContainer.setDestination.";
                    logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.UNABLE_TO_APPLY_PAGE_DEPENDENT_PROP_UNKNOWN_PAGE_ON_WHICH_ELEMENT_IS_DRAWN
                        , logMessageArg));
                    return;
                }
                PdfArray array = new PdfArray();
                array.Add(document.GetPage(pageNumber).GetPdfObject());
                array.Add(PdfName.XYZ);
                array.Add(new PdfNumber(occupiedArea.GetBBox().GetX()));
                array.Add(new PdfNumber(occupiedArea.GetBBox().GetY() + occupiedArea.GetBBox().GetHeight()));
                array.Add(new PdfNumber(0));
                document.AddNamedDestination(destination, array.MakeIndirect(document));
                DeleteProperty(Property.DESTINATION);
            }
        }

        protected internal virtual void ApplyAction(PdfDocument document) {
            PdfAction action = this.GetProperty<PdfAction>(Property.ACTION);
            if (action != null) {
                PdfLinkAnnotation link = this.GetProperty<PdfLinkAnnotation>(Property.LINK_ANNOTATION);
                if (link == null) {
                    link = (PdfLinkAnnotation)new PdfLinkAnnotation(new Rectangle(0, 0, 0, 0)).SetFlags(PdfAnnotation.PRINT);
                    Border border = this.GetProperty<Border>(Property.BORDER);
                    if (border != null) {
                        link.SetBorder(new PdfArray(new float[] { 0, 0, border.GetWidth() }));
                    }
                    else {
                        link.SetBorder(new PdfArray(new float[] { 0, 0, 0 }));
                    }
                    SetProperty(Property.LINK_ANNOTATION, link);
                }
                link.SetAction(action);
            }
        }

        protected internal virtual void ApplyLinkAnnotation(PdfDocument document) {
            ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
            PdfLinkAnnotation linkAnnotation = this.GetProperty<PdfLinkAnnotation>(Property.LINK_ANNOTATION);
            if (linkAnnotation != null) {
                int pageNumber = occupiedArea.GetPageNumber();
                if (pageNumber < 1 || pageNumber > document.GetNumberOfPages()) {
                    String logMessageArg = "Property.LINK_ANNOTATION, which specifies a link associated with this element content area, see com.itextpdf.layout.element.Link.";
                    logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.UNABLE_TO_APPLY_PAGE_DEPENDENT_PROP_UNKNOWN_PAGE_ON_WHICH_ELEMENT_IS_DRAWN
                        , logMessageArg));
                    return;
                }
                // If an element with a link annotation occupies more than two pages,
                // then a NPE might occur, because of the annotation being partially flushed.
                // That's why we create and use an annotation's copy.
                PdfDictionary oldAnnotation = (PdfDictionary)linkAnnotation.GetPdfObject().Clone();
                linkAnnotation = (PdfLinkAnnotation)PdfAnnotation.MakeAnnotation(oldAnnotation);
                Rectangle pdfBBox = CalculateAbsolutePdfBBox();
                linkAnnotation.SetRectangle(new PdfArray(pdfBBox));
                PdfPage page = document.GetPage(pageNumber);
                // TODO DEVSIX-1655 This check is necessary because, in some cases, our renderer's hierarchy may contain
                //  a renderer from the different page that was already flushed
                if (page.IsFlushed()) {
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PAGE_WAS_FLUSHED_ACTION_WILL_NOT_BE_PERFORMED
                        , "link annotation applying"));
                }
                else {
                    page.AddAnnotation(linkAnnotation);
                }
            }
        }

        /// <summary>Retrieve the parent's resolved height declaration.</summary>
        /// <remarks>
        /// Retrieve the parent's resolved height declaration.
        /// If the parent has a relative height declaration, it will check it's parent recursively,
        /// </remarks>
        /// <returns>
        /// null if no height declaration is set on the parent, or if it's own height declaration cannot be resolved
        /// The float value of the resolved height otherwiser
        /// </returns>
        private float? RetrieveResolvedParentDeclaredHeight() {
            if (parent != null && parent.GetProperty<UnitValue>(Property.HEIGHT) != null) {
                UnitValue parentHeightUV = GetPropertyAsUnitValue(parent, Property.HEIGHT);
                if (parentHeightUV.IsPointValue()) {
                    return parentHeightUV.GetValue();
                }
                else {
                    return ((iText.Layout.Renderer.AbstractRenderer)parent).RetrieveHeight();
                }
            }
            else {
                return null;
            }
        }

        /// <summary>Retrieve the direct parent's absolute height property</summary>
        /// <returns>the direct parent's absolute height property value if it exists, null otherwise</returns>
        private float? RetrieveDirectParentDeclaredHeight() {
            if (parent != null && parent.GetProperty<UnitValue>(Property.HEIGHT) != null) {
                UnitValue parentHeightUV = GetPropertyAsUnitValue(parent, Property.HEIGHT);
                if (parentHeightUV.IsPointValue()) {
                    return parentHeightUV.GetValue();
                }
            }
            return null;
        }

        protected internal virtual void UpdateHeightsOnSplit(bool wasHeightClipped, iText.Layout.Renderer.AbstractRenderer
             splitRenderer, iText.Layout.Renderer.AbstractRenderer overflowRenderer) {
            UpdateHeightsOnSplit(occupiedArea.GetBBox().GetHeight(), wasHeightClipped, splitRenderer, overflowRenderer
                , true);
        }

        internal virtual void UpdateHeightsOnSplit(float usedHeight, bool wasHeightClipped, iText.Layout.Renderer.AbstractRenderer
             splitRenderer, iText.Layout.Renderer.AbstractRenderer overflowRenderer, bool enlargeOccupiedAreaOnHeightWasClipped
            ) {
            if (wasHeightClipped) {
                // if height was clipped, max height exists and can be resolved
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT);
                if (enlargeOccupiedAreaOnHeightWasClipped) {
                    float? maxHeight = RetrieveMaxHeight();
                    splitRenderer.occupiedArea.GetBBox().MoveDown((float)maxHeight - usedHeight).SetHeight((float)maxHeight);
                    usedHeight = (float)maxHeight;
                }
            }
            if (overflowRenderer == null || IsKeepTogether()) {
                return;
            }
            // Update height related properties on split or overflow
            // For relative heights, we need the parent's resolved height declaration
            float? parentResolvedHeightPropertyValue = RetrieveResolvedParentDeclaredHeight();
            UnitValue maxHeightUV = GetPropertyAsUnitValue(this, Property.MAX_HEIGHT);
            if (maxHeightUV != null) {
                if (maxHeightUV.IsPointValue()) {
                    float? maxHeight = RetrieveMaxHeight();
                    UnitValue updateMaxHeight = UnitValue.CreatePointValue((float)(maxHeight - usedHeight));
                    overflowRenderer.UpdateMaxHeight(updateMaxHeight);
                }
                else {
                    if (parentResolvedHeightPropertyValue != null) {
                        // Calculate occupied fraction and update overflow renderer
                        float currentOccupiedFraction = usedHeight / (float)parentResolvedHeightPropertyValue * 100;
                        // Fraction
                        float newFraction = maxHeightUV.GetValue() - currentOccupiedFraction;
                        // Update
                        overflowRenderer.UpdateMinHeight(UnitValue.CreatePercentValue(newFraction));
                    }
                }
            }
            // If parent has no resolved height, relative height declarations can be ignored
            UnitValue minHeightUV = GetPropertyAsUnitValue(this, Property.MIN_HEIGHT);
            if (minHeightUV != null) {
                if (minHeightUV.IsPointValue()) {
                    float? minHeight = RetrieveMinHeight();
                    UnitValue updateminHeight = UnitValue.CreatePointValue((float)(minHeight - usedHeight));
                    overflowRenderer.UpdateMinHeight(updateminHeight);
                }
                else {
                    if (parentResolvedHeightPropertyValue != null) {
                        // Calculate occupied fraction and update overflow renderer
                        float currentOccupiedFraction = usedHeight / (float)parentResolvedHeightPropertyValue * 100;
                        // Fraction
                        float newFraction = minHeightUV.GetValue() - currentOccupiedFraction;
                        // Update
                        overflowRenderer.UpdateMinHeight(UnitValue.CreatePercentValue(newFraction));
                    }
                }
            }
            // If parent has no resolved height, relative height declarations can be ignored
            UnitValue heightUV = GetPropertyAsUnitValue(this, Property.HEIGHT);
            if (heightUV != null) {
                if (heightUV.IsPointValue()) {
                    float? height = RetrieveHeight();
                    UnitValue updateHeight = UnitValue.CreatePointValue((float)(height - usedHeight));
                    overflowRenderer.UpdateHeight(updateHeight);
                }
                else {
                    if (parentResolvedHeightPropertyValue != null) {
                        // Calculate occupied fraction and update overflow renderer
                        float currentOccupiedFraction = usedHeight / (float)parentResolvedHeightPropertyValue * 100;
                        // Fraction
                        float newFraction = heightUV.GetValue() - currentOccupiedFraction;
                        // Update
                        overflowRenderer.UpdateMinHeight(UnitValue.CreatePercentValue(newFraction));
                    }
                }
            }
        }

        // If parent has no resolved height, relative height declarations can be ignored
        /// <summary>Calculates min and max width values for current renderer.</summary>
        /// <returns>
        /// instance of
        /// <see cref="iText.Layout.Minmaxwidth.MinMaxWidth"/>
        /// </returns>
        public virtual MinMaxWidth GetMinMaxWidth() {
            return MinMaxWidthUtils.CountDefaultMinMaxWidth(this);
        }

        protected internal virtual bool SetMinMaxWidthBasedOnFixedWidth(MinMaxWidth minMaxWidth) {
            // retrieve returns max width, if there is no width.
            if (HasAbsoluteUnitValue(Property.WIDTH)) {
                //Renderer may override retrieveWidth, double check is required.
                float? width = RetrieveWidth(0);
                if (width != null) {
                    minMaxWidth.SetChildrenMaxWidth((float)width);
                    minMaxWidth.SetChildrenMinWidth((float)width);
                    return true;
                }
            }
            return false;
        }

        protected internal virtual bool IsNotFittingHeight(LayoutArea layoutArea) {
            return !IsPositioned() && occupiedArea.GetBBox().GetHeight() > layoutArea.GetBBox().GetHeight();
        }

        protected internal virtual bool IsNotFittingWidth(LayoutArea layoutArea) {
            return !IsPositioned() && occupiedArea.GetBBox().GetWidth() > layoutArea.GetBBox().GetWidth();
        }

        protected internal virtual bool IsNotFittingLayoutArea(LayoutArea layoutArea) {
            return IsNotFittingHeight(layoutArea) || IsNotFittingWidth(layoutArea);
        }

        /// <summary>Indicates whether the renderer's position is fixed or not.</summary>
        /// <returns>
        /// a
        /// <c>boolean</c>
        /// </returns>
        protected internal virtual bool IsPositioned() {
            return !IsStaticLayout();
        }

        /// <summary>Indicates whether the renderer's position is fixed or not.</summary>
        /// <returns>
        /// a
        /// <c>boolean</c>
        /// </returns>
        protected internal virtual bool IsFixedLayout() {
            Object positioning = this.GetProperty<Object>(Property.POSITION);
            return Convert.ToInt32(LayoutPosition.FIXED).Equals(positioning);
        }

        protected internal virtual bool IsStaticLayout() {
            Object positioning = this.GetProperty<Object>(Property.POSITION);
            return positioning == null || Convert.ToInt32(LayoutPosition.STATIC).Equals(positioning);
        }

        protected internal virtual bool IsRelativePosition() {
            int? positioning = this.GetPropertyAsInteger(Property.POSITION);
            return Convert.ToInt32(LayoutPosition.RELATIVE).Equals(positioning);
        }

        protected internal virtual bool IsAbsolutePosition() {
            int? positioning = this.GetPropertyAsInteger(Property.POSITION);
            return Convert.ToInt32(LayoutPosition.ABSOLUTE).Equals(positioning);
        }

        protected internal virtual bool IsKeepTogether() {
            return IsKeepTogether(null);
        }

        internal virtual bool IsKeepTogether(IRenderer causeOfNothing) {
            return true.Equals(GetPropertyAsBoolean(Property.KEEP_TOGETHER)) && !(causeOfNothing is AreaBreakRenderer);
        }

        // Note! The second parameter is here on purpose. Currently occupied area is passed as a value of this parameter in
        // BlockRenderer, but actually, the block can have many areas, and occupied area will be the common area of sub-areas,
        // whereas child element alignment should be performed area-wise.
        protected internal virtual void AlignChildHorizontally(IRenderer childRenderer, Rectangle currentArea) {
            float availableWidth = currentArea.GetWidth();
            HorizontalAlignment? horizontalAlignment = childRenderer.GetProperty<HorizontalAlignment?>(Property.HORIZONTAL_ALIGNMENT
                );
            if (horizontalAlignment != null && horizontalAlignment != HorizontalAlignment.LEFT) {
                float freeSpace = availableWidth - childRenderer.GetOccupiedArea().GetBBox().GetWidth();
                if (freeSpace > 0) {
                    try {
                        switch (horizontalAlignment) {
                            case HorizontalAlignment.RIGHT: {
                                childRenderer.Move(freeSpace, 0);
                                break;
                            }

                            case HorizontalAlignment.CENTER: {
                                childRenderer.Move(freeSpace / 2, 0);
                                break;
                            }
                        }
                    }
                    catch (NullReferenceException) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.AbstractRenderer));
                        logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED
                            , "Some of the children might not end up aligned horizontally."));
                    }
                }
            }
        }

        /// <summary>Gets borders of the element in the specified order: top, right, bottom, left.</summary>
        /// <returns>
        /// an array of BorderDrawer objects.
        /// In case when certain border isn't set <c>Property.BORDER</c> is used,
        /// and if <c>Property.BORDER</c> is also not set then <c>null</c> is returned
        /// on position of this border
        /// </returns>
        protected internal virtual Border[] GetBorders() {
            return GetBorders(this);
        }

        /// <summary>Gets border radii of the element in the specified order: top-left, top-right, bottom-right, bottom-left.
        ///     </summary>
        /// <returns>
        /// an array of BorderRadius objects.
        /// In case when certain border radius isn't set <c>Property.BORDER_RADIUS</c> is used,
        /// and if <c>Property.BORDER_RADIUS</c> is also not set then <c>null</c> is returned
        /// on position of this border radius
        /// </returns>
        protected internal virtual BorderRadius[] GetBorderRadii() {
            return GetBorderRadii(this);
        }

        protected internal virtual iText.Layout.Renderer.AbstractRenderer SetBorders(Border border, int borderNumber
            ) {
            switch (borderNumber) {
                case 0: {
                    SetProperty(Property.BORDER_TOP, border);
                    break;
                }

                case 1: {
                    SetProperty(Property.BORDER_RIGHT, border);
                    break;
                }

                case 2: {
                    SetProperty(Property.BORDER_BOTTOM, border);
                    break;
                }

                case 3: {
                    SetProperty(Property.BORDER_LEFT, border);
                    break;
                }
            }
            return this;
        }

        /// <summary>
        /// Calculates the bounding box of the content in the coordinate system of the pdf entity on which content is placed,
        /// e.g. document page or form xObject.
        /// </summary>
        /// <remarks>
        /// Calculates the bounding box of the content in the coordinate system of the pdf entity on which content is placed,
        /// e.g. document page or form xObject. This is particularly useful for the cases when element is nested in the rotated
        /// element.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// which is a bbox of the content not relative to the parent's layout area but rather to
        /// the some pdf entity coordinate system.
        /// </returns>
        protected internal virtual Rectangle CalculateAbsolutePdfBBox() {
            Rectangle contentBox = GetOccupiedAreaBBox();
            IList<Point> contentBoxPoints = RectangleToPointsList(contentBox);
            iText.Layout.Renderer.AbstractRenderer renderer = this;
            while (renderer.parent != null) {
                if (renderer is BlockRenderer) {
                    float? angle = renderer.GetProperty<float?>(Property.ROTATION_ANGLE);
                    if (angle != null) {
                        BlockRenderer blockRenderer = (BlockRenderer)renderer;
                        AffineTransform rotationTransform = blockRenderer.CreateRotationTransformInsideOccupiedArea();
                        TransformPoints(contentBoxPoints, rotationTransform);
                    }
                }
                if (renderer.GetProperty<Transform>(Property.TRANSFORM) != null) {
                    if (renderer is BlockRenderer || renderer is ImageRenderer || renderer is TableRenderer) {
                        AffineTransform rotationTransform = renderer.CreateTransformationInsideOccupiedArea();
                        TransformPoints(contentBoxPoints, rotationTransform);
                    }
                }
                renderer = (iText.Layout.Renderer.AbstractRenderer)renderer.parent;
            }
            return CalculateBBox(contentBoxPoints);
        }

        /// <summary>Calculates bounding box around points.</summary>
        /// <param name="points">list of the points calculated bbox will enclose.</param>
        /// <returns>array of float values which denote left, bottom, right, top lines of bbox in this specific order</returns>
        protected internal virtual Rectangle CalculateBBox(IList<Point> points) {
            return Rectangle.CalculateBBox(points);
        }

        protected internal virtual IList<Point> RectangleToPointsList(Rectangle rect) {
            return JavaUtil.ArraysAsList(rect.ToPointsArray());
        }

        protected internal virtual IList<Point> TransformPoints(IList<Point> points, AffineTransform transform) {
            foreach (Point point in points) {
                transform.Transform(point, point);
            }
            return points;
        }

        /// <summary>
        /// This method calculates the shift needed to be applied to the points in order to position
        /// upper and left borders of their bounding box at the given lines.
        /// </summary>
        /// <param name="left">x coordinate at which points bbox left border is to be aligned</param>
        /// <param name="top">y coordinate at which points bbox upper border is to be aligned</param>
        /// <param name="points">the points, which bbox will be aligned at the given position</param>
        /// <returns>
        /// array of two floats, where first element denotes x-coordinate shift and the second
        /// element denotes y-coordinate shift which are needed to align points bbox at the given lines.
        /// </returns>
        protected internal virtual float[] CalculateShiftToPositionBBoxOfPointsAt(float left, float top, IList<Point
            > points) {
            double minX = double.MaxValue;
            double maxY = -double.MaxValue;
            foreach (Point point in points) {
                minX = Math.Min(point.GetX(), minX);
                maxY = Math.Max(point.GetY(), maxY);
            }
            float dx = (float)(left - minX);
            float dy = (float)(top - maxY);
            return new float[] { dx, dy };
        }

        /// <summary>Check if corresponding property has point value.</summary>
        /// <param name="property">
        /// 
        /// <see cref="iText.Layout.Properties.Property"/>
        /// </param>
        /// <returns>false if property value either null, or percent, otherwise true.</returns>
        protected internal virtual bool HasAbsoluteUnitValue(int property) {
            UnitValue value = this.GetProperty<UnitValue>(property);
            return value != null && value.IsPointValue();
        }

        /// <summary>Check if corresponding property has relative value.</summary>
        /// <param name="property">
        /// 
        /// <see cref="iText.Layout.Properties.Property"/>
        /// </param>
        /// <returns>false if property value either null, or point, otherwise true.</returns>
        protected internal virtual bool HasRelativeUnitValue(int property) {
            UnitValue value = this.GetProperty<UnitValue>(property);
            return value != null && value.IsPercentValue();
        }

        internal virtual bool IsFirstOnRootArea(bool checkRootAreaOnly) {
            bool isFirstOnRootArea = true;
            IRenderer ancestor = this;
            while (isFirstOnRootArea && ancestor.GetParent() != null) {
                IRenderer parent = ancestor.GetParent();
                if (parent is RootRenderer) {
                    isFirstOnRootArea = ((RootRenderer)parent).currentArea.IsEmptyArea();
                }
                else {
                    if (parent.GetOccupiedArea() == null) {
                        break;
                    }
                    else {
                        if (!checkRootAreaOnly) {
                            isFirstOnRootArea = parent.GetOccupiedArea().GetBBox().GetHeight() < EPS;
                        }
                    }
                }
                ancestor = parent;
            }
            return isFirstOnRootArea;
        }

        /// <summary>Gets pdf document from root renderers.</summary>
        /// <returns>PdfDocument, or null if there are no document</returns>
        internal virtual PdfDocument GetPdfDocument() {
            RootRenderer renderer = GetRootRenderer();
            if (renderer is DocumentRenderer) {
                Document document = ((DocumentRenderer)renderer).document;
                return document.GetPdfDocument();
            }
            else {
                if (renderer is CanvasRenderer) {
                    return ((CanvasRenderer)renderer).canvas.GetPdfDocument();
                }
                else {
                    return null;
                }
            }
        }

        internal virtual RootRenderer GetRootRenderer() {
            IRenderer currentRenderer = this;
            while (currentRenderer is iText.Layout.Renderer.AbstractRenderer) {
                if (currentRenderer is RootRenderer) {
                    return (RootRenderer)currentRenderer;
                }
                currentRenderer = ((iText.Layout.Renderer.AbstractRenderer)currentRenderer).GetParent();
            }
            return null;
        }

        internal static float CalculateAdditionalWidth(iText.Layout.Renderer.AbstractRenderer renderer) {
            Rectangle dummy = new Rectangle(0, 0);
            renderer.ApplyMargins(dummy, true);
            renderer.ApplyBorderBox(dummy, true);
            renderer.ApplyPaddings(dummy, true);
            return dummy.GetWidth();
        }

        internal static bool NoAbsolutePositionInfo(IRenderer renderer) {
            return !renderer.HasProperty(Property.TOP) && !renderer.HasProperty(Property.BOTTOM) && !renderer.HasProperty
                (Property.LEFT) && !renderer.HasProperty(Property.RIGHT);
        }

        internal static float? GetPropertyAsFloat(IRenderer renderer, int property) {
            return NumberUtil.AsFloat(renderer.GetProperty<Object>(property));
        }

        /// <summary>Returns the property of the renderer as a UnitValue if it exists and is a UnitValue, null otherwise
        ///     </summary>
        /// <param name="renderer">renderer to retrieve the property from</param>
        /// <param name="property">key for the property to retrieve</param>
        /// <returns>A UnitValue if the property is present and is a UnitValue, null otherwise</returns>
        internal static UnitValue GetPropertyAsUnitValue(IRenderer renderer, int property) {
            UnitValue result = renderer.GetProperty<UnitValue>(property);
            return result;
        }

        internal virtual void ShrinkOccupiedAreaForAbsolutePosition() {
            // In case of absolute positioning and not specified left, right, width values, the parent box is shrunk to fit
            // the children. It does not occupy all the available width if it does not need to.
            if (IsAbsolutePosition()) {
                float? left = this.GetPropertyAsFloat(Property.LEFT);
                float? right = this.GetPropertyAsFloat(Property.RIGHT);
                UnitValue width = this.GetProperty<UnitValue>(Property.WIDTH);
                if (left == null && right == null && width == null) {
                    occupiedArea.GetBBox().SetWidth(0);
                }
            }
        }

        internal virtual void DrawPositionedChildren(DrawContext drawContext) {
            foreach (IRenderer positionedChild in positionedRenderers) {
                positionedChild.Draw(drawContext);
            }
        }

        internal virtual FontCharacteristics CreateFontCharacteristics() {
            FontCharacteristics fc = new FontCharacteristics();
            if (this.HasProperty(Property.FONT_WEIGHT)) {
                fc.SetFontWeight((String)this.GetProperty<Object>(Property.FONT_WEIGHT));
            }
            if (this.HasProperty(Property.FONT_STYLE)) {
                fc.SetFontStyle((String)this.GetProperty<Object>(Property.FONT_STYLE));
            }
            return fc;
        }

        /// <summary>
        /// Gets any valid
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// for this renderer, based on
        /// <see cref="iText.Layout.Properties.Property.FONT"/>
        /// ,
        /// <see cref="iText.Layout.Properties.Property.FONT_PROVIDER"/>
        /// and
        /// <see cref="iText.Layout.Properties.Property.FONT_SET"/>
        /// properties.
        /// </summary>
        /// <remarks>
        /// Gets any valid
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// for this renderer, based on
        /// <see cref="iText.Layout.Properties.Property.FONT"/>
        /// ,
        /// <see cref="iText.Layout.Properties.Property.FONT_PROVIDER"/>
        /// and
        /// <see cref="iText.Layout.Properties.Property.FONT_SET"/>
        /// properties.
        /// This method will not change font property of renderer. Also it is not guarantied that returned font will contain
        /// all glyphs used in renderer or its children.
        /// <para />
        /// This method is usually needed for evaluating some layout characteristics like ascender or descender.
        /// </remarks>
        /// <returns>
        /// a valid
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// instance based on renderer
        /// <see cref="iText.Layout.Properties.Property.FONT"/>
        /// property.
        /// </returns>
        internal virtual PdfFont ResolveFirstPdfFont() {
            Object font = this.GetProperty<Object>(Property.FONT);
            if (font is PdfFont) {
                return (PdfFont)font;
            }
            else {
                if (font is String[]) {
                    FontProvider provider = this.GetProperty<FontProvider>(Property.FONT_PROVIDER);
                    if (provider == null) {
                        throw new InvalidOperationException(LayoutExceptionMessageConstant.FONT_PROVIDER_NOT_SET_FONT_FAMILY_NOT_RESOLVED
                            );
                    }
                    FontSet fontSet = this.GetProperty<FontSet>(Property.FONT_SET);
                    if (provider.GetFontSet().IsEmpty() && (fontSet == null || fontSet.IsEmpty())) {
                        throw new InvalidOperationException(LayoutExceptionMessageConstant.FONT_PROVIDER_NOT_SET_FONT_FAMILY_NOT_RESOLVED
                            );
                    }
                    FontCharacteristics fc = CreateFontCharacteristics();
                    return ResolveFirstPdfFont((String[])font, provider, fc, fontSet);
                }
                else {
                    throw new InvalidOperationException("String[] or PdfFont expected as value of FONT property");
                }
            }
        }

        /// <summary>
        /// Get first valid
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// for this renderer, based on given font-families, font provider and font characteristics.
        /// </summary>
        /// <remarks>
        /// Get first valid
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// for this renderer, based on given font-families, font provider and font characteristics.
        /// This method will not change font property of renderer. Also it is not guarantied that returned font will contain
        /// all glyphs used in renderer or its children.
        /// <para />
        /// This method is usually needed for evaluating some layout characteristics like ascender or descender.
        /// </remarks>
        /// <returns>
        /// a valid
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// instance based on renderer
        /// <see cref="iText.Layout.Properties.Property.FONT"/>
        /// property.
        /// </returns>
        internal virtual PdfFont ResolveFirstPdfFont(String[] font, FontProvider provider, FontCharacteristics fc, 
            FontSet additionalFonts) {
            FontSelector fontSelector = provider.GetFontSelector(JavaUtil.ArraysAsList(font), fc, additionalFonts);
            return provider.GetPdfFont(fontSelector.BestMatch(), additionalFonts);
        }

        internal static Border[] GetBorders(IRenderer renderer) {
            Border border = renderer.GetProperty<Border>(Property.BORDER);
            Border topBorder = renderer.GetProperty<Border>(Property.BORDER_TOP);
            Border rightBorder = renderer.GetProperty<Border>(Property.BORDER_RIGHT);
            Border bottomBorder = renderer.GetProperty<Border>(Property.BORDER_BOTTOM);
            Border leftBorder = renderer.GetProperty<Border>(Property.BORDER_LEFT);
            Border[] borders = new Border[] { topBorder, rightBorder, bottomBorder, leftBorder };
            if (!HasOwnOrModelProperty(renderer, Property.BORDER_TOP)) {
                borders[0] = border;
            }
            if (!HasOwnOrModelProperty(renderer, Property.BORDER_RIGHT)) {
                borders[1] = border;
            }
            if (!HasOwnOrModelProperty(renderer, Property.BORDER_BOTTOM)) {
                borders[2] = border;
            }
            if (!HasOwnOrModelProperty(renderer, Property.BORDER_LEFT)) {
                borders[3] = border;
            }
            return borders;
        }

        internal virtual void ApplyAbsolutePositionIfNeeded(LayoutContext layoutContext) {
            if (IsAbsolutePosition()) {
                ApplyAbsolutePosition(layoutContext is PositionedLayoutContext ? ((PositionedLayoutContext)layoutContext).
                    GetParentOccupiedArea().GetBBox() : layoutContext.GetArea().GetBBox());
            }
        }

        internal virtual void PreparePositionedRendererAndAreaForLayout(IRenderer childPositionedRenderer, Rectangle
             fullBbox, Rectangle parentBbox) {
            float? left = GetPropertyAsFloat(childPositionedRenderer, Property.LEFT);
            float? right = GetPropertyAsFloat(childPositionedRenderer, Property.RIGHT);
            float? top = GetPropertyAsFloat(childPositionedRenderer, Property.TOP);
            float? bottom = GetPropertyAsFloat(childPositionedRenderer, Property.BOTTOM);
            childPositionedRenderer.SetParent(this);
            AdjustPositionedRendererLayoutBoxWidth(childPositionedRenderer, fullBbox, left, right);
            if (Convert.ToInt32(LayoutPosition.ABSOLUTE).Equals(childPositionedRenderer.GetProperty<int?>(Property.POSITION
                ))) {
                UpdateMinHeightForAbsolutelyPositionedRenderer(childPositionedRenderer, parentBbox, top, bottom);
            }
        }

        private void UpdateMinHeightForAbsolutelyPositionedRenderer(IRenderer renderer, Rectangle parentRendererBox
            , float? top, float? bottom) {
            if (top != null && bottom != null && !renderer.HasProperty(Property.HEIGHT)) {
                UnitValue currentMaxHeight = GetPropertyAsUnitValue(renderer, Property.MAX_HEIGHT);
                UnitValue currentMinHeight = GetPropertyAsUnitValue(renderer, Property.MIN_HEIGHT);
                float resolvedMinHeight = Math.Max(0, parentRendererBox.GetTop() - (float)top - parentRendererBox.GetBottom
                    () - (float)bottom);
                Rectangle dummy = new Rectangle(0, 0);
                if (!IsBorderBoxSizing(renderer)) {
                    ApplyPaddings(dummy, GetPaddings(renderer), true);
                    ApplyBorderBox(dummy, GetBorders(renderer), true);
                }
                ApplyMargins(dummy, GetMargins(renderer), true);
                resolvedMinHeight -= dummy.GetHeight();
                if (currentMinHeight != null) {
                    resolvedMinHeight = Math.Max(resolvedMinHeight, currentMinHeight.GetValue());
                }
                if (currentMaxHeight != null) {
                    resolvedMinHeight = Math.Min(resolvedMinHeight, currentMaxHeight.GetValue());
                }
                renderer.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue((float)resolvedMinHeight));
            }
        }

        private void AdjustPositionedRendererLayoutBoxWidth(IRenderer renderer, Rectangle fullBbox, float? left, float?
             right) {
            if (left != null) {
                fullBbox.SetWidth(fullBbox.GetWidth() - (float)left).SetX(fullBbox.GetX() + (float)left);
            }
            if (right != null) {
                fullBbox.SetWidth(fullBbox.GetWidth() - (float)right);
            }
            if (left == null && right == null && !renderer.HasProperty(Property.WIDTH)) {
                // Other, non-block renderers won't occupy full width anyway
                MinMaxWidth minMaxWidth = renderer is BlockRenderer ? ((BlockRenderer)renderer).GetMinMaxWidth() : null;
                if (minMaxWidth != null && minMaxWidth.GetMaxWidth() < fullBbox.GetWidth()) {
                    fullBbox.SetWidth(minMaxWidth.GetMaxWidth() + iText.Layout.Renderer.AbstractRenderer.EPS);
                }
            }
        }

        internal static float CalculatePaddingBorderWidth(iText.Layout.Renderer.AbstractRenderer renderer) {
            Rectangle dummy = new Rectangle(0, 0);
            renderer.ApplyBorderBox(dummy, true);
            renderer.ApplyPaddings(dummy, true);
            return dummy.GetWidth();
        }

        internal static float CalculatePaddingBorderHeight(iText.Layout.Renderer.AbstractRenderer renderer) {
            Rectangle dummy = new Rectangle(0, 0);
            renderer.ApplyBorderBox(dummy, true);
            renderer.ApplyPaddings(dummy, true);
            return dummy.GetHeight();
        }

        /// <summary>
        /// This method creates
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// instance that could be used
        /// to transform content inside the occupied area,
        /// considering the centre of the occupiedArea as the origin of a coordinate system for transformation.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// that transforms the content and places it inside occupied area.
        /// </returns>
        private AffineTransform CreateTransformationInsideOccupiedArea() {
            Rectangle backgroundArea = ApplyMargins(occupiedArea.Clone().GetBBox(), false);
            float x = backgroundArea.GetX();
            float y = backgroundArea.GetY();
            float height = backgroundArea.GetHeight();
            float width = backgroundArea.GetWidth();
            AffineTransform transform = AffineTransform.GetTranslateInstance(-1 * (x + width / 2), -1 * (y + height / 
                2));
            transform.PreConcatenate(Transform.GetAffineTransform(this.GetProperty<Transform>(Property.TRANSFORM), width
                , height));
            transform.PreConcatenate(AffineTransform.GetTranslateInstance(x + width / 2, y + height / 2));
            return transform;
        }

        protected internal virtual void BeginTransformationIfApplied(PdfCanvas canvas) {
            if (this.GetProperty<Transform>(Property.TRANSFORM) != null) {
                AffineTransform transform = CreateTransformationInsideOccupiedArea();
                canvas.SaveState().ConcatMatrix(transform);
            }
        }

        protected internal virtual void EndTransformationIfApplied(PdfCanvas canvas) {
            if (this.GetProperty<Transform>(Property.TRANSFORM) != null) {
                canvas.RestoreState();
            }
        }

        /// <summary>
        /// Add the specified
        /// <see cref="IRenderer">renderer</see>
        /// to the end of children list and update its
        /// parent link to
        /// <c>this</c>.
        /// </summary>
        /// <param name="child">
        /// the
        /// <see cref="IRenderer">child renderer</see>
        /// to be add
        /// </param>
        internal virtual void AddChildRenderer(IRenderer child) {
            child.SetParent(this);
            this.childRenderers.Add(child);
        }

        /// <summary>
        /// Add the specified collection of
        /// <see cref="IRenderer">renderers</see>
        /// to the end of children list and
        /// update their parent links to
        /// <c>this</c>.
        /// </summary>
        /// <param name="children">
        /// the collection of
        /// <see cref="IRenderer">child renderers</see>
        /// to be add
        /// </param>
        internal virtual void AddAllChildRenderers(IList<IRenderer> children) {
            if (children == null) {
                return;
            }
            SetThisAsParent(children);
            this.childRenderers.AddAll(children);
        }

        /// <summary>
        /// Inserts the specified collection of
        /// <see cref="IRenderer">renderers</see>
        /// at the specified space of
        /// children list and update their parent links to
        /// <c>this</c>.
        /// </summary>
        /// <param name="index">index at which to insert the first element from the specified collection</param>
        /// <param name="children">
        /// the collection of
        /// <see cref="IRenderer">child renderers</see>
        /// to be add
        /// </param>
        internal virtual void AddAllChildRenderers(int index, IList<IRenderer> children) {
            SetThisAsParent(children);
            this.childRenderers.AddAll(index, children);
        }

        /// <summary>
        /// Set the specified collection of
        /// <see cref="IRenderer">renderers</see>
        /// as the children for
        /// <c>this</c>
        /// element.
        /// </summary>
        /// <remarks>
        /// Set the specified collection of
        /// <see cref="IRenderer">renderers</see>
        /// as the children for
        /// <c>this</c>
        /// element. That meant that the old collection would be cleaned, all parent links in old
        /// children to
        /// <c>this</c>
        /// would be erased (i.e. set to
        /// <see langword="null"/>
        /// ) and then the specified
        /// list of children would be added similar to
        /// <see cref="AddAllChildRenderers(System.Collections.Generic.IList{E})"/>.
        /// </remarks>
        /// <param name="children">
        /// the collection of children
        /// <see cref="IRenderer">renderers</see>
        /// to be set
        /// </param>
        internal virtual void SetChildRenderers(IList<IRenderer> children) {
            RemoveThisFromParents(this.childRenderers);
            this.childRenderers.Clear();
            AddAllChildRenderers(children);
        }

        /// <summary>
        /// Remove the child
        /// <see cref="IRenderer">renderer</see>
        /// at the specified place.
        /// </summary>
        /// <remarks>
        /// Remove the child
        /// <see cref="IRenderer">renderer</see>
        /// at the specified place. If the removed renderer has
        /// the parent link set to
        /// <c>this</c>
        /// and it would not present in the children list after
        /// removal, then the parent link of the removed renderer would be erased (i.e. set to
        /// <see langword="null"/>.
        /// </remarks>
        /// <param name="index">the index of the renderer to be removed</param>
        /// <returns>the removed renderer</returns>
        internal virtual IRenderer RemoveChildRenderer(int index) {
            IRenderer removed = this.childRenderers.JRemoveAt(index);
            RemoveThisFromParent(removed);
            return removed;
        }

        /// <summary>
        /// Remove the children
        /// <see cref="IRenderer">renderers</see>
        /// which are contains in the specified collection.
        /// </summary>
        /// <remarks>
        /// Remove the children
        /// <see cref="IRenderer">renderers</see>
        /// which are contains in the specified collection.
        /// If some of the removed renderers has the parent link set to
        /// <c>this</c>
        /// , then
        /// the parent link of the removed renderer would be erased (i.e. set to
        /// <see langword="null"/>.
        /// </remarks>
        /// <param name="children">the collections of renderers to be removed from children list</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the children list has been changed
        /// </returns>
        internal virtual bool RemoveAllChildRenderers(ICollection<IRenderer> children) {
            RemoveThisFromParents(children);
            return this.childRenderers.RemoveAll(children);
        }

        /// <summary>
        /// Update the child
        /// <see cref="IRenderer">renderer</see>
        /// at the specified place with the specified one.
        /// </summary>
        /// <remarks>
        /// Update the child
        /// <see cref="IRenderer">renderer</see>
        /// at the specified place with the specified one.
        /// If the removed renderer has the parent link set to
        /// <c>this</c>
        /// , then it would be erased
        /// (i.e. set to
        /// <see langword="null"/>
        /// ).
        /// </remarks>
        /// <param name="index">the index of the renderer to be updated</param>
        /// <param name="child">the renderer to be set</param>
        /// <returns>the removed renderer</returns>
        internal virtual IRenderer SetChildRenderer(int index, IRenderer child) {
            if (child != null) {
                child.SetParent(this);
            }
            IRenderer removedElement = this.childRenderers[index] = child;
            RemoveThisFromParent(removedElement);
            return removedElement;
        }

        /// <summary>
        /// Sets current
        /// <see cref="AbstractRenderer"/>
        /// as parent to renderers in specified collection.
        /// </summary>
        /// <param name="children">the collection of renderers to set the parent renderer on</param>
        internal virtual void SetThisAsParent(ICollection<IRenderer> children) {
            foreach (IRenderer child in children) {
                child.SetParent(this);
            }
        }

        internal virtual bool LogWarningIfGetNextRendererNotOverridden(Type baseClass, Type rendererClass) {
            if (baseClass != rendererClass) {
                ILogger logger = ITextLogManager.GetLogger(baseClass);
                logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.GET_NEXT_RENDERER_SHOULD_BE_OVERRIDDEN
                    ));
                return false;
            }
            else {
                return true;
            }
        }

        private void RemoveThisFromParent(IRenderer toRemove) {
            // we need to be sure that the removed element has no other entries in child renderers list
            if (toRemove != null && this == toRemove.GetParent() && !this.childRenderers.Contains(toRemove)) {
                toRemove.SetParent(null);
            }
        }

        private void RemoveThisFromParents(ICollection<IRenderer> children) {
            foreach (IRenderer child in children) {
                if (child != null && this == child.GetParent()) {
                    child.SetParent(null);
                }
            }
        }

        private static UnitValue[] GetMargins(IRenderer renderer) {
            return new UnitValue[] { renderer.GetProperty<UnitValue>(Property.MARGIN_TOP), renderer.GetProperty<UnitValue
                >(Property.MARGIN_RIGHT), renderer.GetProperty<UnitValue>(Property.MARGIN_BOTTOM), renderer.GetProperty
                <UnitValue>(Property.MARGIN_LEFT) };
        }

        private static BorderRadius[] GetBorderRadii(IRenderer renderer) {
            BorderRadius radius = renderer.GetProperty<BorderRadius>(Property.BORDER_RADIUS);
            BorderRadius topLeftRadius = renderer.GetProperty<BorderRadius>(Property.BORDER_TOP_LEFT_RADIUS);
            BorderRadius topRightRadius = renderer.GetProperty<BorderRadius>(Property.BORDER_TOP_RIGHT_RADIUS);
            BorderRadius bottomRightRadius = renderer.GetProperty<BorderRadius>(Property.BORDER_BOTTOM_RIGHT_RADIUS);
            BorderRadius bottomLeftRadius = renderer.GetProperty<BorderRadius>(Property.BORDER_BOTTOM_LEFT_RADIUS);
            BorderRadius[] borderRadii = new BorderRadius[] { topLeftRadius, topRightRadius, bottomRightRadius, bottomLeftRadius
                 };
            if (!HasOwnOrModelProperty(renderer, Property.BORDER_TOP_LEFT_RADIUS)) {
                borderRadii[0] = radius;
            }
            if (!HasOwnOrModelProperty(renderer, Property.BORDER_TOP_RIGHT_RADIUS)) {
                borderRadii[1] = radius;
            }
            if (!HasOwnOrModelProperty(renderer, Property.BORDER_BOTTOM_RIGHT_RADIUS)) {
                borderRadii[2] = radius;
            }
            if (!HasOwnOrModelProperty(renderer, Property.BORDER_BOTTOM_LEFT_RADIUS)) {
                borderRadii[3] = radius;
            }
            return borderRadii;
        }

        private static UnitValue[] GetPaddings(IRenderer renderer) {
            return new UnitValue[] { renderer.GetProperty<UnitValue>(Property.PADDING_TOP), renderer.GetProperty<UnitValue
                >(Property.PADDING_RIGHT), renderer.GetProperty<UnitValue>(Property.PADDING_BOTTOM), renderer.GetProperty
                <UnitValue>(Property.PADDING_LEFT) };
        }

        private static bool HasOwnOrModelProperty(IRenderer renderer, int property) {
            return renderer.HasOwnProperty(property) || (null != renderer.GetModelElement() && renderer.GetModelElement
                ().HasProperty(property));
        }

        public abstract IRenderer GetNextRenderer();

        public abstract LayoutResult Layout(LayoutContext arg1);
    }
}

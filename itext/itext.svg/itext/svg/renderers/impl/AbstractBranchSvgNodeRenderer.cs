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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// Abstract class that will be the superclass for any element that can function
    /// as a parent.
    /// </summary>
    public abstract class AbstractBranchSvgNodeRenderer : AbstractSvgNodeRenderer, IBranchSvgNodeRenderer {
        /// <summary>The number of viewBox values.</summary>
        protected internal const int VIEWBOX_VALUES_NUMBER = 4;

        private readonly IList<ISvgNodeRenderer> children = new List<ISvgNodeRenderer>();

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(AbstractBranchSvgNodeRenderer));

        /// <summary>
        /// Method that will set properties to be inherited by this branch renderer's
        /// children and will iterate over all children in order to draw them.
        /// </summary>
        /// <param name="context">
        /// the object that knows the place to draw this element and
        /// maintains its state
        /// </param>
        protected internal override void DoDraw(SvgDrawContext context) {
            // If branch has no children, don't do anything
            if (GetChildren().Count > 0) {
                PdfStream stream = new PdfStream();
                stream.Put(PdfName.Type, PdfName.XObject);
                stream.Put(PdfName.Subtype, PdfName.Form);
                PdfFormXObject xObject = (PdfFormXObject)PdfXObject.MakeXObject(stream);
                PdfCanvas newCanvas = new PdfCanvas(xObject, context.GetCurrentCanvas().GetDocument());
                ApplyViewBox(context);
                bool overflowVisible = IsOverflowVisible(this);
                // TODO (DEVSIX-3482) Currently overflow logic works only for markers.  Update this code after the ticket will be finished.
                if (this is MarkerSvgNodeRenderer && overflowVisible) {
                    WriteBBoxAccordingToVisibleOverflow(context, stream);
                }
                else {
                    Rectangle bbBox = context.GetCurrentViewPort().Clone();
                    stream.Put(PdfName.BBox, new PdfArray(bbBox));
                }
                if (this is MarkerSvgNodeRenderer) {
                    ((MarkerSvgNodeRenderer)this).ApplyMarkerAttributes(context);
                }
                context.PushCanvas(newCanvas);
                // TODO (DEVSIX-3482) Currently overflow logic works only for markers. Update this code after the ticket will be finished.
                if (!(this is MarkerSvgNodeRenderer) || !overflowVisible) {
                    ApplyViewportClip(context);
                }
                ApplyViewportTranslationCorrection(context);
                foreach (ISvgNodeRenderer child in GetChildren()) {
                    if (!(child is MarkerSvgNodeRenderer)) {
                        newCanvas.SaveState();
                        child.Draw(context);
                        newCanvas.RestoreState();
                    }
                }
                CleanUp(context);
                // Transformation already happened in AbstractSvgNodeRenderer, so no need to do a transformation here
                AddXObject(context.GetCurrentCanvas(), xObject, 0, 0);
            }
        }

//\cond DO_NOT_DOCUMENT
        //TODO: DEVSIX-5731 Replace this workaround method with PdfCanvas::addXObjectAt
        internal static void AddXObject(PdfCanvas canvas, PdfXObject xObject, float x, float y) {
            if (xObject is PdfFormXObject) {
                canvas.SaveState();
                canvas.ConcatMatrix(1, 0, 0, 1, x, y);
                PdfName name = canvas.GetResources().AddForm((PdfFormXObject)xObject);
                canvas.GetContentStream().GetOutputStream().Write(name).WriteSpace().WriteBytes(ByteUtils.GetIsoBytes("Do\n"
                    ));
                canvas.RestoreState();
            }
            else {
                canvas.AddXObjectAt(xObject, x, y);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Applies a transformation based on a viewBox for a given branch node.</summary>
        /// <param name="context">current svg draw context</param>
        internal virtual void ApplyViewBox(SvgDrawContext context) {
            float[] viewBoxValues = GetViewBoxValues();
            if (viewBoxValues.Length < VIEWBOX_VALUES_NUMBER) {
                float[] values = new float[] { 0, 0, context.GetCurrentViewPort().GetWidth(), context.GetCurrentViewPort()
                    .GetHeight() };
                Rectangle currentViewPort = context.GetCurrentViewPort();
                CalculateAndApplyViewBox(context, values, currentViewPort);
            }
            else {
                Rectangle currentViewPort = context.GetCurrentViewPort();
                CalculateAndApplyViewBox(context, viewBoxValues, currentViewPort);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual String[] RetrieveAlignAndMeet() {
            String meetOrSlice = SvgConstants.Values.MEET;
            String align = SvgConstants.Values.DEFAULT_ASPECT_RATIO;
            String preserveAspectRatioValue = this.attributesAndStyles.Get(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO
                );
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            if (preserveAspectRatioValue == null) {
                preserveAspectRatioValue = this.attributesAndStyles.Get(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO.ToLowerInvariant
                    ());
            }
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO) || this.attributesAndStyles
                .ContainsKey(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO.ToLowerInvariant())) {
                IList<String> aspectRatioValuesSplitValues = SvgCssUtils.SplitValueList(preserveAspectRatioValue);
                align = aspectRatioValuesSplitValues[0].ToLowerInvariant();
                if (aspectRatioValuesSplitValues.Count > 1) {
                    meetOrSlice = aspectRatioValuesSplitValues[1].ToLowerInvariant();
                }
            }
            if (this is MarkerSvgNodeRenderer && !SvgConstants.Values.NONE.Equals(align) && SvgConstants.Values.MEET.Equals
                (meetOrSlice)) {
                // Browsers do not correctly display markers with 'meet' option in the preserveAspectRatio attribute.
                // The Chrome, IE, and Firefox browsers set the align value to 'xMinYMin' regardless of the actual align.
                align = SvgConstants.Values.XMIN_YMIN;
            }
            return new String[] { align, meetOrSlice };
        }
//\endcond

        /// <summary>Applies a clipping operation based on the view port.</summary>
        /// <param name="context">the svg draw context</param>
        private void ApplyViewportClip(SvgDrawContext context) {
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            currentCanvas.Rectangle(context.GetCurrentViewPort());
            currentCanvas.Clip();
            currentCanvas.EndPath();
        }

        private void ApplyViewportTranslationCorrection(SvgDrawContext context) {
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            AffineTransform tf = this.CalculateViewPortTranslation(context);
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            bool preserveAspectRationNone = SvgConstants.Values.NONE.Equals(GetAttribute(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO
                )) || SvgConstants.Values.NONE.Equals(GetAttribute(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO.ToLowerInvariant
                ()));
            if (!tf.IsIdentity() && preserveAspectRationNone) {
                currentCanvas.ConcatMatrix(tf);
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>If present, process the preserveAspectRatio position.</summary>
        /// <param name="context">the svg draw context</param>
        /// <param name="viewBoxValues">the four values depicting the viewbox [min-x min-y width height]</param>
        /// <param name="align">alignment method to use</param>
        /// <param name="scaleWidth">the multiplier for scaling width</param>
        /// <param name="scaleHeight">the multiplier for scaling height</param>
        /// <returns>the transformation based on the preserveAspectRatio value</returns>
        internal virtual AffineTransform ProcessAspectRatioPosition(SvgDrawContext context, float[] viewBoxValues, 
            String align, float scaleWidth, float scaleHeight) {
            AffineTransform transform = new AffineTransform();
            Rectangle currentViewPort = context.GetCurrentViewPort();
            float midXBox = viewBoxValues[0] + (viewBoxValues[2] / 2);
            float midYBox = viewBoxValues[1] + (viewBoxValues[3] / 2);
            float midXPort = currentViewPort.GetX() + (currentViewPort.GetWidth() / 2);
            float midYPort = currentViewPort.GetY() + (currentViewPort.GetHeight() / 2);
            float x = 0f;
            float y = 0f;
            // if x attribute of svg is present, then x value of current viewport should be set according to it
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.X)) {
                x = CssDimensionParsingUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.X));
            }
            // if y attribute of svg is present, then y value of current viewport should be set according to it
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.Y)) {
                y = CssDimensionParsingUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.Y));
            }
            if (!(this is MarkerSvgNodeRenderer)) {
                x -= currentViewPort.GetX();
                y -= currentViewPort.GetY();
            }
            // need to consider previous (parent) translation before applying the current one
            switch (align.ToLowerInvariant()) {
                case SvgConstants.Values.NONE: {
                    break;
                }

                case SvgConstants.Values.XMIN_YMIN: {
                    x -= viewBoxValues[0];
                    y -= viewBoxValues[1];
                    break;
                }

                case SvgConstants.Values.XMIN_YMID: {
                    x -= viewBoxValues[0];
                    y += (midYPort - midYBox);
                    break;
                }

                case SvgConstants.Values.XMIN_YMAX: {
                    x -= viewBoxValues[0];
                    y += (currentViewPort.GetHeight() - viewBoxValues[3]);
                    break;
                }

                case SvgConstants.Values.XMID_YMIN: {
                    x += (midXPort - midXBox);
                    y -= viewBoxValues[1];
                    break;
                }

                case SvgConstants.Values.XMID_YMAX: {
                    x += (midXPort - midXBox);
                    y += (currentViewPort.GetHeight() - viewBoxValues[3]);
                    break;
                }

                case SvgConstants.Values.XMAX_YMIN: {
                    x += (currentViewPort.GetWidth() - viewBoxValues[2]);
                    y -= viewBoxValues[1];
                    break;
                }

                case SvgConstants.Values.XMAX_YMID: {
                    x += (currentViewPort.GetWidth() - viewBoxValues[2]);
                    y += (midYPort - midYBox);
                    break;
                }

                case SvgConstants.Values.XMAX_YMAX: {
                    x += (currentViewPort.GetWidth() - viewBoxValues[2]);
                    y += (currentViewPort.GetHeight() - viewBoxValues[3]);
                    break;
                }

                case SvgConstants.Values.DEFAULT_ASPECT_RATIO:
                default: {
                    x += (midXPort - midXBox);
                    y += (midYPort - midYBox);
                    break;
                }
            }
            //Rescale x and y
            x /= scaleWidth;
            y /= scaleHeight;
            transform.Translate(x, y);
            return transform;
        }
//\endcond

        /// <summary>Cleans up the SvgDrawContext by removing the current viewport and by popping the current canvas.</summary>
        /// <param name="context">context to clean</param>
        private void CleanUp(SvgDrawContext context) {
            if (GetParent() != null) {
                context.RemoveCurrentViewPort();
            }
            context.PopCanvas();
        }

        public void AddChild(ISvgNodeRenderer child) {
            // final method, in order to disallow adding null
            if (child != null) {
                children.Add(child);
            }
        }

        public IList<ISvgNodeRenderer> GetChildren() {
            // final method, in order to disallow modifying the List
            return JavaCollectionsUtil.UnmodifiableList(children);
        }

        /// <summary>
        /// Create a deep copy of every child renderer and add them to the passed
        /// <see cref="AbstractBranchSvgNodeRenderer"/>
        /// </summary>
        /// <param name="deepCopy">renderer to add copies of children to</param>
        protected internal void DeepCopyChildren(AbstractBranchSvgNodeRenderer deepCopy) {
            foreach (ISvgNodeRenderer child in children) {
                ISvgNodeRenderer newChild = child.CreateDeepCopy();
                child.SetParent(deepCopy);
                deepCopy.AddChild(newChild);
            }
        }

//\cond DO_NOT_DOCUMENT
        internal override void PostDraw(SvgDrawContext context) {
        }
//\endcond

        public abstract override ISvgNodeRenderer CreateDeepCopy();

//\cond DO_NOT_DOCUMENT
        internal override void SetPartOfClipPath(bool isPart) {
            base.SetPartOfClipPath(isPart);
            foreach (ISvgNodeRenderer child in children) {
                if (child is AbstractSvgNodeRenderer) {
                    ((AbstractSvgNodeRenderer)child).SetPartOfClipPath(isPart);
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void CalculateAndApplyViewBox(SvgDrawContext context, float[] values, Rectangle currentViewPort
            ) {
            // TODO DEVSIX-4861 change this method with using of SvgCoordinateUtils#applyViewBox
            String[] alignAndMeet = RetrieveAlignAndMeet();
            String align = alignAndMeet[0];
            String meetOrSlice = alignAndMeet[1];
            float scaleWidth = currentViewPort.GetWidth() / values[2];
            float scaleHeight = currentViewPort.GetHeight() / values[3];
            bool forceUniformScaling = !(SvgConstants.Values.NONE.Equals(align));
            if (forceUniformScaling) {
                //Scaling should preserve aspect ratio
                if (SvgConstants.Values.MEET.Equals(meetOrSlice)) {
                    scaleWidth = Math.Min(scaleWidth, scaleHeight);
                }
                else {
                    scaleWidth = Math.Max(scaleWidth, scaleHeight);
                }
                scaleHeight = scaleWidth;
            }
            AffineTransform scale = AffineTransform.GetScaleInstance(scaleWidth, scaleHeight);
            float[] scaledViewBoxValues = ScaleViewBoxValues(values, scaleWidth, scaleHeight);
            AffineTransform transform = ProcessAspectRatioPosition(context, scaledViewBoxValues, align, scaleWidth, scaleHeight
                );
            if (!scale.IsIdentity()) {
                context.GetCurrentCanvas().ConcatMatrix(scale);
                //Inverse scaling needs to be applied to viewport dimensions
                context.GetCurrentViewPort().SetWidth(currentViewPort.GetWidth() / scaleWidth).SetX(currentViewPort.GetX()
                     / scaleWidth).SetHeight(currentViewPort.GetHeight() / scaleHeight).SetY(currentViewPort.GetY() / scaleHeight
                    );
            }
            if (!transform.IsIdentity()) {
                context.GetCurrentCanvas().ConcatMatrix(transform);
                //Apply inverse translation to viewport to make it line up nicely
                context.GetCurrentViewPort().SetX(currentViewPort.GetX() + -1 * (float)transform.GetTranslateX()).SetY(currentViewPort
                    .GetY() + -1 * (float)transform.GetTranslateY());
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual float[] GetViewBoxValues() {
            if (this.attributesAndStyles == null) {
                return new float[] {  };
            }
            String viewBoxValues = attributesAndStyles.Get(SvgConstants.Attributes.VIEWBOX);
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            if (viewBoxValues == null) {
                viewBoxValues = attributesAndStyles.Get(SvgConstants.Attributes.VIEWBOX.ToLowerInvariant());
            }
            if (viewBoxValues == null) {
                return new float[] {  };
            }
            IList<String> valueStrings = SvgCssUtils.SplitValueList(viewBoxValues);
            float[] values = new float[valueStrings.Count];
            for (int i = 0; i < values.Length; i++) {
                values[i] = CssDimensionParsingUtils.ParseAbsoluteLength(valueStrings[i]);
            }
            // the value for viewBox should be 4 numbers according to the viewBox documentation
            if (values.Length != VIEWBOX_VALUES_NUMBER) {
                if (LOGGER.IsEnabled(LogLevel.Warning)) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(SvgLogMessageConstant.VIEWBOX_VALUE_MUST_BE_FOUR_NUMBERS, viewBoxValues
                        ));
                }
                return new float[] {  };
            }
            // case when viewBox width or height is negative value is an error and
            // invalidates the ‘viewBox’ attribute (according to the viewBox documentation)
            if (values[2] < 0 || values[3] < 0) {
                if (LOGGER.IsEnabled(LogLevel.Warning)) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(SvgLogMessageConstant.VIEWBOX_WIDTH_AND_HEIGHT_CANNOT_BE_NEGATIVE
                        , viewBoxValues));
                }
                return new float[] {  };
            }
            return values;
        }
//\endcond

        private static float[] ScaleViewBoxValues(float[] values, float scaleWidth, float scaleHeight) {
            float[] scaledViewBoxValues = new float[values.Length];
            scaledViewBoxValues[0] = values[0] * scaleWidth;
            scaledViewBoxValues[1] = values[1] * scaleHeight;
            scaledViewBoxValues[2] = values[2] * scaleWidth;
            scaledViewBoxValues[3] = values[3] * scaleHeight;
            return scaledViewBoxValues;
        }

        private static bool IsOverflowVisible(AbstractSvgNodeRenderer currentElement) {
            return (CommonCssConstants.VISIBLE.Equals(currentElement.attributesAndStyles.Get(CommonCssConstants.OVERFLOW
                )) || CommonCssConstants.AUTO.Equals(currentElement.attributesAndStyles.Get(CommonCssConstants.OVERFLOW
                )));
        }

        /// <summary>
        /// When in the svg element
        /// <c>overflow</c>
        /// is
        /// <c>visible</c>
        /// the corresponding formXObject
        /// should have a BBox (form XObject’s bounding box; see PDF 32000-1:2008 - 8.10.2 Form Dictionaries)
        /// that should cover the entire svg space (page in pdf) in order to be able to show parts of the element which are outside the current element viewPort.
        /// </summary>
        /// <remarks>
        /// When in the svg element
        /// <c>overflow</c>
        /// is
        /// <c>visible</c>
        /// the corresponding formXObject
        /// should have a BBox (form XObject’s bounding box; see PDF 32000-1:2008 - 8.10.2 Form Dictionaries)
        /// that should cover the entire svg space (page in pdf) in order to be able to show parts of the element which are outside the current element viewPort.
        /// To do this, we get the inverse matrix of all the current transformation matrix changes and apply it to the root viewPort.
        /// This allows you to get the root rectangle in the final coordinate system.
        /// </remarks>
        /// <param name="context">current context to get canvases and view ports</param>
        /// <param name="stream">stream to write a BBox</param>
        private static void WriteBBoxAccordingToVisibleOverflow(SvgDrawContext context, PdfStream stream) {
            IList<PdfCanvas> canvases = new List<PdfCanvas>();
            int canvasesSize = context.Size();
            for (int i = 0; i < canvasesSize; i++) {
                canvases.Add(context.PopCanvas());
            }
            AffineTransform transform = new AffineTransform();
            for (int i = canvases.Count - 1; i >= 0; i--) {
                PdfCanvas canvas = canvases[i];
                Matrix matrix = canvas.GetGraphicsState().GetCtm();
                transform.Concatenate(new AffineTransform(matrix.Get(0), matrix.Get(1), matrix.Get(3), matrix.Get(4), matrix
                    .Get(6), matrix.Get(7)));
                context.PushCanvas(canvas);
            }
            try {
                transform = transform.CreateInverse();
            }
            catch (NoninvertibleTransformException) {
                // Case with zero determiner (see PDF 32000-1:2008 - 8.3.4 Transformation Matrices - NOTE 3)
                // for example with a, b, c, d in cm equal to 0
                stream.Put(PdfName.BBox, new PdfArray(new Rectangle(0, 0, 0, 0)));
                ILogger logger = ITextLogManager.GetLogger(typeof(AbstractBranchSvgNodeRenderer));
                logger.LogWarning(SvgLogMessageConstant.UNABLE_TO_GET_INVERSE_MATRIX_DUE_TO_ZERO_DETERMINANT);
                return;
            }
            Point[] points = context.GetRootViewPort().ToPointsArray();
            transform.Transform(points, 0, points, 0, points.Length);
            Rectangle bbox = Rectangle.CalculateBBox(JavaUtil.ArraysAsList(points));
            stream.Put(PdfName.BBox, new PdfArray(bbox));
        }
    }
}

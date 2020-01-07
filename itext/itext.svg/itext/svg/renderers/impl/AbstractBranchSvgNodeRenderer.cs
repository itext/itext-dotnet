/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using Common.Logging;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// Abstract class that will be the superclass for any element that can function
    /// as a parent.
    /// </summary>
    public abstract class AbstractBranchSvgNodeRenderer : AbstractSvgNodeRenderer, IBranchSvgNodeRenderer {
        private readonly IList<ISvgNodeRenderer> children = new List<ISvgNodeRenderer>();

        /// <summary>
        /// Method that will set properties to be inherited by this branch renderer's
        /// children and will iterate over all children in order to draw them.
        /// </summary>
        /// <param name="context">
        /// the object that knows the place to draw this element and
        /// maintains its state
        /// </param>
        protected internal override void DoDraw(SvgDrawContext context) {
            // if branch has no children, don't do anything
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
                // transformation already happened in AbstractSvgNodeRenderer, so no need to do a transformation here
                context.GetCurrentCanvas().AddXObject(xObject, 0, 0);
            }
        }

        /// <summary>Applies a transformation based on a viewBox for a given branch node.</summary>
        /// <param name="context">current svg draw context</param>
        internal virtual void ApplyViewBox(SvgDrawContext context) {
            if (this.attributesAndStyles != null && this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.VIEWBOX
                )) {
                float[] values = GetViewBoxValues();
                Rectangle currentViewPort = context.GetCurrentViewPort();
                CalculateAndApplyViewBox(context, values, currentViewPort);
            }
            else {
                float[] values = new float[] { 0, 0, context.GetCurrentViewPort().GetWidth(), context.GetCurrentViewPort()
                    .GetHeight() };
                Rectangle currentViewPort = context.GetCurrentViewPort();
                CalculateAndApplyViewBox(context, values, currentViewPort);
            }
        }

        internal virtual String[] RetrieveAlignAndMeet() {
            String meetOrSlice = SvgConstants.Values.MEET;
            String align = SvgConstants.Values.DEFAULT_ASPECT_RATIO;
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO)) {
                String preserveAspectRatioValue = this.attributesAndStyles.Get(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO
                    );
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
            if (!tf.IsIdentity() && SvgConstants.Values.NONE.Equals(this.GetAttribute(SvgConstants.Attributes.PRESERVE_ASPECT_RATIO
                ))) {
                currentCanvas.ConcatMatrix(tf);
            }
        }

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
                x = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.X));
            }
            // if y attribute of svg is present, then y value of current viewport should be set according to it
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.Y)) {
                y = CssUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.Y));
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

        internal override void PostDraw(SvgDrawContext context) {
        }

        public abstract override ISvgNodeRenderer CreateDeepCopy();

        internal override void SetPartOfClipPath(bool isPart) {
            base.SetPartOfClipPath(isPart);
            foreach (ISvgNodeRenderer child in children) {
                if (child is AbstractSvgNodeRenderer) {
                    ((AbstractSvgNodeRenderer)child).SetPartOfClipPath(isPart);
                }
            }
        }

        internal virtual void CalculateAndApplyViewBox(SvgDrawContext context, float[] values, Rectangle currentViewPort
            ) {
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

        internal virtual float[] GetViewBoxValues() {
            String viewBoxValues = attributesAndStyles.Get(SvgConstants.Attributes.VIEWBOX);
            IList<String> valueStrings = SvgCssUtils.SplitValueList(viewBoxValues);
            float[] values = new float[valueStrings.Count];
            for (int i = 0; i < values.Length; i++) {
                values[i] = CssUtils.ParseAbsoluteLength(valueStrings[i]);
            }
            return values;
        }

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
                ILog logger = LogManager.GetLogger(typeof(AbstractBranchSvgNodeRenderer));
                logger.Warn(SvgLogMessageConstant.UNABLE_TO_GET_INVERSE_MATRIX_DUE_TO_ZERO_DETERMINANT);
                return;
            }
            Point[] points = context.GetRootViewPort().ToPointsArray();
            transform.Transform(points, 0, points, 0, points.Length);
            Rectangle bbox = Rectangle.CalculateBBox(JavaUtil.ArraysAsList(points));
            stream.Put(PdfName.BBox, new PdfArray(bbox));
        }
    }
}

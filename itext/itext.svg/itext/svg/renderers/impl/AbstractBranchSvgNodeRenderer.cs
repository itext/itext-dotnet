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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Css;
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
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(AbstractBranchSvgNodeRenderer));

        /// <summary>The number of viewBox values.</summary>
        /// <remarks>
        /// The number of viewBox values.
        /// Deprecate in favour of
        /// <c>SvgConstants.Values.VIEWBOX_VALUES_NUMBER</c>
        /// </remarks>
        [Obsolete]
        protected internal const int VIEWBOX_VALUES_NUMBER = 4;

        private const float EPS = 1e-6f;

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
            // If branch has no children, don't do anything
            if (!GetChildren().IsEmpty()) {
                PdfStream stream = new PdfStream();
                stream.Put(PdfName.Type, PdfName.XObject);
                stream.Put(PdfName.Subtype, PdfName.Form);
                PdfFormXObject xObject = (PdfFormXObject)PdfXObject.MakeXObject(stream);
                PdfCanvas newCanvas = new PdfCanvas(xObject, context.GetCurrentCanvas().GetDocument());
                ApplyViewBox(context);
                bool overflowVisible = IsOverflowVisible(this);
                Rectangle bbBox;
                // TODO (DEVSIX-3482) Currently overflow logic works only for markers.  Update this code after the ticket will be finished.
                if (this is MarkerSvgNodeRenderer && overflowVisible) {
                    bbBox = GetBBoxAccordingToVisibleOverflow(context);
                }
                else {
                    bbBox = context.GetCurrentViewPort().Clone();
                }
                stream.Put(PdfName.BBox, new PdfArray(bbBox));
                context.PushCanvas(newCanvas);
                // TODO (DEVSIX-3482) Currently overflow logic works only for markers. Update this code after the ticket will be finished.
                if (!(this is MarkerSvgNodeRenderer) || !overflowVisible) {
                    ApplyViewportClip(context);
                }
                foreach (ISvgNodeRenderer child in GetChildren()) {
                    if (!(child is MarkerSvgNodeRenderer)) {
                        newCanvas.SaveState();
                        child.Draw(context);
                        newCanvas.RestoreState();
                    }
                }
                CleanUp(context);
                // Transformation already happened in AbstractSvgNodeRenderer, so no need to do a transformation here
                context.GetCurrentCanvas().AddXObjectAt(xObject, bbBox.GetX(), bbBox.GetY());
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Applies a transformation based on a viewBox for a given branch node.</summary>
        /// <param name="context">current svg draw context</param>
        internal virtual void ApplyViewBox(SvgDrawContext context) {
            Rectangle currentViewPort = context.GetCurrentViewPort();
            float[] viewBoxValues = SvgCssUtils.ParseViewBox(this);
            if (viewBoxValues == null || viewBoxValues.Length < SvgConstants.Values.VIEWBOX_VALUES_NUMBER) {
                viewBoxValues = new float[] { 0, 0, currentViewPort.GetWidth(), currentViewPort.GetHeight() };
            }
            CalculateAndApplyViewBox(context, viewBoxValues, currentViewPort);
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
            // If viewBox width or height is zero we should disable rendering of the element.
            if (Math.Abs(values[2]) < EPS || Math.Abs(values[3]) < EPS) {
                if (LOGGER.IsEnabled(LogLevel.Information)) {
                    LOGGER.LogInformation(SvgLogMessageConstant.VIEWBOX_WIDTH_OR_HEIGHT_IS_ZERO);
                }
                context.GetCurrentCanvas().ConcatMatrix(AffineTransform.GetScaleInstance(0, 0));
                return;
            }
            String[] alignAndMeet = RetrieveAlignAndMeet();
            String align = alignAndMeet[0];
            String meetOrSlice = alignAndMeet[1];
            Rectangle viewBox = new Rectangle(values[0], values[1], values[2], values[3]);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(viewBox, currentViewPort, align, meetOrSlice);
            float scaleWidth = appliedViewBox.GetWidth() / viewBox.GetWidth();
            float scaleHeight = appliedViewBox.GetHeight() / viewBox.GetHeight();
            AffineTransform scale = AffineTransform.GetScaleInstance(scaleWidth, scaleHeight);
            float xOffset = appliedViewBox.GetX() / scaleWidth - viewBox.GetX();
            float yOffset = appliedViewBox.GetY() / scaleHeight - viewBox.GetY();
            AffineTransform transform = new AffineTransform();
            transform.Translate(xOffset, yOffset);
            if (!transform.IsIdentity()) {
                context.GetCurrentCanvas().ConcatMatrix(transform);
                // Apply inverse translation to viewport to make it line up nicely
                context.GetCurrentViewPort().SetX(currentViewPort.GetX() - (float)transform.GetTranslateX()).SetY(currentViewPort
                    .GetY() - (float)transform.GetTranslateY());
            }
            if (this is MarkerSvgNodeRenderer) {
                ((MarkerSvgNodeRenderer)this).ApplyMarkerAttributes(context);
            }
            if (!scale.IsIdentity()) {
                context.GetCurrentCanvas().ConcatMatrix(scale);
                // Inverse scaling needs to be applied to viewport dimensions
                context.GetCurrentViewPort().SetWidth(currentViewPort.GetWidth() / scaleWidth).SetX(currentViewPort.GetX()
                     / scaleWidth).SetHeight(currentViewPort.GetHeight() / scaleHeight).SetY(currentViewPort.GetY() / scaleHeight
                    );
            }
        }
//\endcond

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
        /// the corresponding formXObject should have a BBox
        /// (form XObject’s bounding box; see PDF 32000-1:2008 - 8.10.2 Form Dictionaries) that should cover the entire svg
        /// space (page in pdf) in order to be able to show parts of the element which are outside the current element
        /// viewPort.
        /// </summary>
        /// <remarks>
        /// When in the svg element
        /// <c>overflow</c>
        /// is
        /// <c>visible</c>
        /// the corresponding formXObject should have a BBox
        /// (form XObject’s bounding box; see PDF 32000-1:2008 - 8.10.2 Form Dictionaries) that should cover the entire svg
        /// space (page in pdf) in order to be able to show parts of the element which are outside the current element
        /// viewPort. To do this, we get the inverse matrix of all the current transformation matrix changes and apply it
        /// to the root viewPort. This allows you to get the root rectangle in the final coordinate system.
        /// </remarks>
        /// <param name="context">current context to get canvases and view ports</param>
        /// <returns>
        /// the set to
        /// <c>PdfStream</c>
        /// bbox
        /// </returns>
        private static Rectangle GetBBoxAccordingToVisibleOverflow(SvgDrawContext context) {
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
                ILogger logger = ITextLogManager.GetLogger(typeof(AbstractBranchSvgNodeRenderer));
                logger.LogWarning(SvgLogMessageConstant.UNABLE_TO_GET_INVERSE_MATRIX_DUE_TO_ZERO_DETERMINANT);
                // Case with zero determiner (see PDF 32000-1:2008 - 8.3.4 Transformation Matrices - NOTE 3)
                // for example with a, b, c, d in cm equal to 0
                return new Rectangle(0, 0, 0, 0);
            }
            Point[] points = context.GetRootViewPort().ToPointsArray();
            transform.Transform(points, 0, points, 0, points.Length);
            return Rectangle.CalculateBBox(JavaUtil.ArraysAsList(points));
        }
    }
}

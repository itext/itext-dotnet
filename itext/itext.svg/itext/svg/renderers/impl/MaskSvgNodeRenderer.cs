/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Internal.Runtime;
using iText.Commons.Logs;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// Renderer for the
    /// <c>&lt;mask&gt;</c>
    /// tag.
    /// </summary>
    public class MaskSvgNodeRenderer : AbstractBranchSvgNodeRenderer, INoDrawSvgNodeRenderer {
        private static readonly LazyLogger LOGGER = new LazyLogger(typeof(iText.Svg.Renderers.Impl.MaskSvgNodeRenderer
            ));

        private const float PX_TO_PT = 0.75F;

        // Constants as defined in SVG specification: https://www.w3.org/TR/SVG11/masking.html#MaskElementXAttribute
        private const String DEFAULT_MASK_X = "-10%";

        private const String DEFAULT_MASK_Y = "-10%";

        private const String DEFAULT_MASK_WIDTH = "120%";

        private const String DEFAULT_MASK_HEIGHT = "120%";

        private const double DEFAULT_MASK_OFFSET = -0.1;

        private const double DEFAULT_MASK_DIMENSION = 1.2;

        /// <summary>
        /// Creates a new
        /// <see cref="MaskSvgNodeRenderer"/>
        /// instance.
        /// </summary>
        public MaskSvgNodeRenderer() {
        }

//\cond DO_NOT_DOCUMENT
        // Empty constructor
        /// <summary>Draws a renderer using this mask.</summary>
        /// <remarks>Draws a renderer using this mask. The renderer's transform must already be applied to the current canvas.
        ///     </remarks>
        /// <param name="maskedRenderer">renderer to be masked</param>
        /// <param name="context">current draw context</param>
        /// <param name="maskId">normalized id/reference value of the mask definition for cycle detection</param>
        internal virtual void DrawMaskedObject(AbstractSvgNodeRenderer maskedRenderer, SvgDrawContext context, String
             maskId) {
            Rectangle maskedBoundingBox = maskedRenderer.GetObjectBoundingBox(context);
            Rectangle maskArea = CalculateMaskArea(context, maskedBoundingBox);
            if (!IsValidArea(maskArea)) {
                return;
            }
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            PdfFormXObject maskForm = CreateMaskFormWithCycleProtection(maskArea, maskedBoundingBox, context, currentCanvas
                , maskId);
            if (maskForm == null) {
                return;
            }
            PdfFormXObject maskedContentForm = CreateMaskedContentForm(maskArea, maskedRenderer, context, currentCanvas
                );
            ApplySoftMaskAndDrawMaskedContent(currentCanvas, maskedContentForm, maskForm, maskArea);
        }
//\endcond

        /// <summary><inheritDoc/></summary>
        public override ISvgNodeRenderer CreateDeepCopy() {
            iText.Svg.Renderers.Impl.MaskSvgNodeRenderer copy = new iText.Svg.Renderers.Impl.MaskSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
            return copy;
        }

        /// <summary>
        /// Mask renderers do not represent drawable graphics by themselves,
        /// so they have no own object bounding box.
        /// </summary>
        /// <param name="context">current draw context</param>
        /// <returns>
        /// 
        /// <see langword="null"/>
        /// always
        /// </returns>
        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }

        /// <summary>Mask elements are not rendered directly onto the current canvas.</summary>
        /// <remarks>
        /// Mask elements are not rendered directly onto the current canvas.
        /// They are applied through
        /// <see cref="DrawMaskedObject(AbstractSvgNodeRenderer, iText.Svg.Renderers.SvgDrawContext, System.String)"/>
        ///     .
        /// </remarks>
        /// <param name="context">current draw context</param>
        protected internal override void DoDraw(SvgDrawContext context) {
            throw new NotSupportedException(SvgExceptionMessageConstant.DRAW_NO_DRAW);
        }

        private static PdfFormXObject CreateMaskedContentForm(Rectangle maskArea, AbstractSvgNodeRenderer maskedRenderer
            , SvgDrawContext context, PdfCanvas currentCanvas) {
            PdfFormXObject maskedContentForm = new PdfFormXObject(maskArea);
            PdfTransparencyGroup group = new PdfTransparencyGroup();
            group.SetIsolated(true);
            maskedContentForm.SetGroup(group);
            PdfCanvas maskedContentCanvas = new PdfCanvas(maskedContentForm, currentCanvas.GetDocument());
            context.PushCanvas(maskedContentCanvas);
            try {
                maskedContentCanvas.Rectangle(maskArea);
                maskedContentCanvas.Clip();
                maskedContentCanvas.EndPath();
                DrawMaskedRendererCopy(maskedRenderer, context);
            }
            finally {
                context.PopCanvas();
            }
            return maskedContentForm;
        }

        // Mask caching is not implemented because it would depend on
        // target bounding box and current viewport, even with userSpaceOnUse units.
        // It would make cache hits less likely and increase implementation complexity.
        private PdfFormXObject CreateMaskForm(Rectangle maskArea, Rectangle objectBoundingBox, SvgDrawContext context
            , PdfCanvas currentCanvas) {
            bool objectBoundingBoxMaskContentUnits = IsObjectBoundingBoxMaskContentUnits();
            if (objectBoundingBoxMaskContentUnits && !IsValidArea(objectBoundingBox)) {
                return null;
            }
            PdfFormXObject maskForm = new PdfFormXObject(maskArea);
            PdfTransparencyGroup group = new PdfTransparencyGroup();
            group.SetColorSpace(IsAlphaMaskType() ? PdfName.DeviceRGB : PdfName.DeviceGray);
            maskForm.SetGroup(group);
            PdfCanvas maskCanvas = new PdfCanvas(maskForm, currentCanvas.GetDocument());
            context.PushCanvas(maskCanvas);
            context.PushMaskRenderingMode(!IsAlphaMaskType());
            try {
                maskCanvas.Rectangle(maskArea);
                maskCanvas.Clip();
                maskCanvas.EndPath();
                ApplyMaskContentUnitsTransform(maskCanvas, objectBoundingBox);
                foreach (ISvgNodeRenderer child in GetChildren()) {
                    maskCanvas.SaveState();
                    child.Draw(context);
                    maskCanvas.RestoreState();
                }
            }
            finally {
                context.PopMaskRenderingMode();
                context.PopCanvas();
            }
            return maskForm;
        }

        private PdfFormXObject CreateMaskFormWithCycleProtection(Rectangle maskArea, Rectangle objectBoundingBox, 
            SvgDrawContext context, PdfCanvas currentCanvas, String maskId) {
            if (!context.PushMaskId(maskId)) {
                return null;
            }
            try {
                return CreateMaskForm(maskArea, objectBoundingBox, context, currentCanvas);
            }
            finally {
                context.PopMaskId();
            }
        }

        private static void DrawMaskedRendererCopy(AbstractSvgNodeRenderer maskedRenderer, SvgDrawContext context) {
            AbstractSvgNodeRenderer drawableRenderer = (AbstractSvgNodeRenderer)maskedRenderer.CreateDeepCopy();
            drawableRenderer.SetParent(maskedRenderer.GetParent());
            IDictionary<String, String> drawableStyles = drawableRenderer.GetAttributeMapCopy();
            drawableStyles.JRemove(SvgConstants.Attributes.MASK);
            // The target transform has already been applied by AbstractSvgNodeRenderer.draw.
            drawableStyles.JRemove(SvgConstants.Attributes.TRANSFORM);
            drawableRenderer.SetAttributesAndStyles(drawableStyles);
            drawableRenderer.Draw(context);
        }

        /// <summary>
        /// Maps mask child coordinates to the masked object's bounding box when
        /// <c>maskContentUnits="objectBoundingBox"</c>.
        /// </summary>
        /// <remarks>
        /// Maps mask child coordinates to the masked object's bounding box when
        /// <c>maskContentUnits="objectBoundingBox"</c>
        /// . In this coordinate system, the origin is the bounding-box
        /// origin and one unit represents its full width or height, rather than a user-space length.
        /// Translation and scaling are therefore needed to align the mask content with the masked object.
        /// <para />Child renderers parse unitless lengths as pixels and convert them to points. Dividing the scale factors
        /// by
        /// <c>PX_TO_PT</c>
        /// compensates for that conversion, so a unitless length of one spans the corresponding
        /// bounding-box dimension. For
        /// <c>userSpaceOnUse</c>
        /// (the default), no transformation is needed.
        /// </remarks>
        /// <param name="maskCanvas">canvas on which the mask children will be drawn</param>
        /// <param name="objectBoundingBox">
        /// masked object's bounding box; must be non-null with positive dimensions
        /// when using
        /// <paramref name="objectBoundingBox"/>
        /// content units
        /// </param>
        private void ApplyMaskContentUnitsTransform(PdfCanvas maskCanvas, Rectangle objectBoundingBox) {
            if (!IsObjectBoundingBoxMaskContentUnits()) {
                return;
            }
            AffineTransform toObjectBoundingBox = new AffineTransform();
            toObjectBoundingBox.Translate(objectBoundingBox.GetX(), objectBoundingBox.GetY());
            toObjectBoundingBox.Scale(objectBoundingBox.GetWidth() / PX_TO_PT, objectBoundingBox.GetHeight() / PX_TO_PT
                );
            maskCanvas.ConcatMatrix(toObjectBoundingBox);
        }

        private void ApplySoftMaskAndDrawMaskedContent(PdfCanvas currentCanvas, PdfFormXObject maskedContentForm, 
            PdfFormXObject maskForm, Rectangle maskArea) {
            PdfExtGState extGState = CreateSoftMaskExtGState(maskForm);
            currentCanvas.SaveState();
            currentCanvas.SetExtGState(extGState);
            currentCanvas.AddXObjectAt(maskedContentForm, maskArea.GetX(), maskArea.GetY());
            currentCanvas.RestoreState();
        }

        private PdfExtGState CreateSoftMaskExtGState(PdfFormXObject maskForm) {
            PdfDictionary softMask = new PdfDictionary();
            softMask.Put(PdfName.S, IsAlphaMaskType() ? PdfName.Alpha : PdfName.Luminosity);
            softMask.Put(PdfName.G, maskForm.GetPdfObject());
            PdfExtGState extGState = new PdfExtGState();
            extGState.SetSoftMask(softMask);
            return extGState;
        }

        private static bool IsValidArea(Rectangle area) {
            return area != null && area.GetWidth() > 0 && area.GetHeight() > 0;
        }

        private Rectangle CalculateMaskArea(SvgDrawContext context, Rectangle objectBoundingBox) {
            if (IsObjectBoundingBoxMaskUnits()) {
                if (objectBoundingBox == null || objectBoundingBox.GetWidth() <= 0 || objectBoundingBox.GetHeight() <= 0) {
                    return null;
                }
                double xRel = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes.X)
                    , DEFAULT_MASK_OFFSET);
                double yRel = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes.Y)
                    , DEFAULT_MASK_OFFSET);
                double widthRel = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes
                    .WIDTH), DEFAULT_MASK_DIMENSION);
                double heightRel = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes
                    .HEIGHT), DEFAULT_MASK_DIMENSION);
                if (widthRel < 0 || heightRel < 0) {
                    LOGGER.Warn(() => SvgLogMessageConstant.MASK_WIDTH_OR_HEIGHT_IS_NEGATIVE);
                }
                return new Rectangle((float)(objectBoundingBox.GetX() + objectBoundingBox.GetWidth() * xRel), (float)(objectBoundingBox
                    .GetY() + objectBoundingBox.GetHeight() * yRel), (float)(objectBoundingBox.GetWidth() * widthRel), (float
                    )(objectBoundingBox.GetHeight() * heightRel));
            }
            float x = ParseHorizontalLength(GetAttributeOrDefault(SvgConstants.Attributes.X, DEFAULT_MASK_X), context);
            float y = ParseVerticalLength(GetAttributeOrDefault(SvgConstants.Attributes.Y, DEFAULT_MASK_Y), context);
            float width = ParseHorizontalLength(GetAttributeOrDefault(SvgConstants.Attributes.WIDTH, DEFAULT_MASK_WIDTH
                ), context);
            float height = ParseVerticalLength(GetAttributeOrDefault(SvgConstants.Attributes.HEIGHT, DEFAULT_MASK_HEIGHT
                ), context);
            if (width < 0 || height < 0) {
                LOGGER.Warn(() => SvgLogMessageConstant.MASK_WIDTH_OR_HEIGHT_IS_NEGATIVE);
            }
            return new Rectangle(x, y, width, height);
        }

        private bool IsAlphaMaskType() {
            String maskType = GetAttribute(SvgConstants.Attributes.MASK_TYPE);
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            if (maskType == null) {
                maskType = GetAttribute(StringNormalizer.ToLowerCase(SvgConstants.Attributes.MASK_TYPE));
            }
            return SvgConstants.Values.ALPHA.EqualsIgnoreCase(maskType);
        }

        private bool IsObjectBoundingBoxMaskUnits() {
            String maskUnits = GetAttribute(SvgConstants.Attributes.MASK_UNITS);
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            if (maskUnits == null) {
                maskUnits = GetAttribute(StringNormalizer.ToLowerCase(SvgConstants.Attributes.MASK_UNITS));
            }
            return !SvgConstants.Values.USER_SPACE_ON_USE.Equals(maskUnits);
        }

        private bool IsObjectBoundingBoxMaskContentUnits() {
            String maskContentUnits = GetAttribute(SvgConstants.Attributes.MASK_CONTENT_UNITS);
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            if (maskContentUnits == null) {
                maskContentUnits = GetAttribute(StringNormalizer.ToLowerCase(SvgConstants.Attributes.MASK_CONTENT_UNITS));
            }
            return SvgConstants.Values.OBJECT_BOUNDING_BOX.Equals(maskContentUnits);
        }
    }
}

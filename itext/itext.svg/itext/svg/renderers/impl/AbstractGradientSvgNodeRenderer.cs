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
using iText.Kernel.Colors;
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Geom;
using iText.StyledXmlParser.Css;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// abstract implementation for gradient tags
    /// (&lt;linearGradient&gt;, &lt;radialGradient&gt;).
    /// </summary>
    public abstract class AbstractGradientSvgNodeRenderer : AbstractBranchSvgNodeRenderer, ISvgPaintServer {
        protected internal const double CONVERT_COEFF = 0.75;

        /// <summary>Gradient node renderers are not directly drawable.</summary>
        /// <param name="context">
        /// the object that knows the place to draw this element and
        /// maintains its state
        /// </param>
        protected internal override void DoDraw(SvgDrawContext context) {
            throw new NotSupportedException(SvgExceptionMessageConstant.DRAW_NO_DRAW);
        }

        /// <summary><inheritDoc/></summary>
        public virtual Color CreateColor(SvgDrawContext context, Rectangle objectBoundingBox, float objectBoundingBoxMargin
            , float parentOpacity) {
            if (objectBoundingBox == null) {
                return null;
            }
            // createColor is an entry point for gradients when drawing svg, so resolving href values here
            TemplateResolveUtils.Resolve(this, context);
            IGradientBuilder builder = CreateGradientBuilderAndConfigureGeometry(context, objectBoundingBox);
            if (builder == null) {
                return null;
            }
            ConfigureGradientBuilderStopsAndSpread(builder, parentOpacity);
            return builder.BuildColor(objectBoundingBox.ApplyMargins(objectBoundingBoxMargin, objectBoundingBoxMargin, 
                objectBoundingBoxMargin, objectBoundingBoxMargin, true), context.GetCurrentCanvasTransform(), context.
                GetCurrentCanvas().GetDocument());
        }

        /// <summary>Creates and configures gradient builder specific to concrete gradient type.</summary>
        /// <param name="context">the current svg draw context</param>
        /// <param name="objectBoundingBox">target element bounding box</param>
        /// <returns>the configured builder instance for this renderer type</returns>
        [System.ObsoleteAttribute(@"deprecated in failure of making abstract")]
        protected internal virtual IGradientBuilder CreateGradientBuilderAndConfigureGeometry(SvgDrawContext context
            , Rectangle objectBoundingBox) {
            return null;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsHidden() {
            return CommonCssConstants.NONE.Equals(this.attributesAndStyles.Get(CommonCssConstants.DISPLAY));
        }

        /// <summary>Checks whether the gradient units values are on user space on use or object bounding box</summary>
        /// <returns>
        /// 
        /// <see langword="false"/>
        /// if the 'gradientUnits' value of the gradient tag equals
        /// to 'userSpaceOnUse', otherwise
        /// <see langword="true"/>
        /// </returns>
        protected internal virtual bool IsObjectBoundingBoxUnits() {
            String gradientUnits = GetAttribute(SvgConstants.Attributes.GRADIENT_UNITS);
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            if (gradientUnits == null) {
                gradientUnits = GetAttribute(StringNormalizer.ToLowerCase(SvgConstants.Attributes.GRADIENT_UNITS));
            }
            if (SvgConstants.Values.USER_SPACE_ON_USE.Equals(gradientUnits)) {
                return false;
            }
            else {
                if (gradientUnits != null && !SvgConstants.Values.OBJECT_BOUNDING_BOX.Equals(gradientUnits)) {
                    String gradientUnitsToLog = gradientUnits;
                    new LazyLogger(this.GetType()).Warn(() => MessageFormatUtil.Format(SvgLogMessageConstant.GRADIENT_INVALID_GRADIENT_UNITS_LOG
                        , gradientUnitsToLog));
                }
            }
            return true;
        }

        /// <summary>Evaluates the 'gradientTransform' transformations</summary>
        /// <returns>
        /// an
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// object representing the specified gradient transformation
        /// </returns>
        [System.ObsoleteAttribute(@"will become private")]
        protected internal virtual AffineTransform GetGradientTransform() {
            String gradientTransform = GetAttribute(SvgConstants.Attributes.GRADIENT_TRANSFORM);
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            if (gradientTransform == null) {
                gradientTransform = GetAttribute(StringNormalizer.ToLowerCase(SvgConstants.Attributes.GRADIENT_TRANSFORM));
            }
            if (gradientTransform != null && !String.IsNullOrEmpty(gradientTransform)) {
                return TransformUtils.ParseTransform(gradientTransform);
            }
            return null;
        }

        /// <summary>Creates a transformation from the gradient coordinate space to user space.</summary>
        /// <param name="objectBoundingBox">target element bounding box</param>
        /// <param name="isObjectBoundingBox">whether gradient units are object-bounding-box based</param>
        /// <returns>
        /// a composed
        /// <see cref="iText.Kernel.Geom.AffineTransform"/>
        /// to use for gradient rendering
        /// </returns>
        protected internal virtual AffineTransform GetGradientTransformToUserSpaceOnUse(Rectangle objectBoundingBox
            , bool isObjectBoundingBox) {
            AffineTransform gradientTransform = new AffineTransform();
            if (isObjectBoundingBox) {
                gradientTransform.Translate(objectBoundingBox.GetX(), objectBoundingBox.GetY());
                // We need to scale with dividing the lengths by 0.75 as further we should
                // concatenate gradient transformation matrix which has no absolute parsing.
                // For example, if gradientTransform is set to translate(1, 1) and gradientUnits
                // is set to "objectBoundingBox" then the gradient should be shifted horizontally
                // and vertically exactly by the size of the element bounding box. So, again,
                // as we parse translate(1, 1) to translation(0.75, 0.75) the bounding box in
                // the gradient vector space should be 0.75x0.75 in order for such translation
                // to shift by the complete size of bounding box.
                gradientTransform.Scale(objectBoundingBox.GetWidth() / CONVERT_COEFF, objectBoundingBox.GetHeight() / CONVERT_COEFF
                    );
            }
            AffineTransform svgGradientTransformation = GetGradientTransform();
            if (svgGradientTransformation != null) {
                gradientTransform.Concatenate(svgGradientTransformation);
            }
            return gradientTransform;
        }

        /// <summary>Construct a list of child stop renderers</summary>
        /// <returns>
        /// a list of
        /// <see cref="StopSvgNodeRenderer"/>
        /// elements that represents the child stop values
        /// </returns>
        [System.ObsoleteAttribute(@"will become private")]
        protected internal virtual IList<StopSvgNodeRenderer> GetChildStopRenderers() {
            IList<StopSvgNodeRenderer> stopRenderers = new List<StopSvgNodeRenderer>();
            foreach (ISvgNodeRenderer child in GetChildren()) {
                if (child is StopSvgNodeRenderer) {
                    stopRenderers.Add((StopSvgNodeRenderer)child);
                }
            }
            return stopRenderers;
        }

        /// <summary>Parses the gradient spread method</summary>
        /// <returns>
        /// the parsed
        /// <see cref="iText.Kernel.Colors.Gradients.GradientSpreadMethod"/>
        /// specified in the gradient
        /// </returns>
        [System.ObsoleteAttribute(@"will become private")]
        protected internal virtual GradientSpreadMethod ParseSpreadMethod() {
            String spreadMethodValue = GetAttribute(SvgConstants.Attributes.SPREAD_METHOD);
            if (spreadMethodValue == null) {
                spreadMethodValue = GetAttribute(StringNormalizer.ToLowerCase(SvgConstants.Attributes.SPREAD_METHOD));
            }
            if (spreadMethodValue == null) {
                // returning svg default spread method
                return GradientSpreadMethod.PAD;
            }
            switch (spreadMethodValue) {
                case SvgConstants.Values.SPREAD_METHOD_PAD: {
                    return GradientSpreadMethod.PAD;
                }

                case SvgConstants.Values.SPREAD_METHOD_REFLECT: {
                    return GradientSpreadMethod.REFLECT;
                }

                case SvgConstants.Values.SPREAD_METHOD_REPEAT: {
                    return GradientSpreadMethod.REPEAT;
                }

                default: {
                    String spreadMethodToLog = spreadMethodValue;
                    new LazyLogger(this.GetType()).Warn(() => MessageFormatUtil.Format(SvgLogMessageConstant.GRADIENT_INVALID_SPREAD_METHOD_LOG
                        , spreadMethodToLog));
                    return GradientSpreadMethod.PAD;
                }
            }
        }

        private void ConfigureGradientBuilderStopsAndSpread(IGradientBuilder builder, float parentOpacity) {
            foreach (GradientColorStop stopColor in ParseStops(parentOpacity)) {
                builder.AddStopColor(stopColor);
            }
            builder.SetSpread(ParseSpreadMethod());
        }

        /// <summary>Parses gradient stop children into a normalized list of color stops.</summary>
        /// <param name="parentOpacity">parent element opacity; currently reserved for future stop opacity support</param>
        /// <returns>
        /// a list of parsed and normalized
        /// <see cref="iText.Kernel.Colors.Gradients.GradientColorStop"/>
        /// instances
        /// </returns>
        private IList<GradientColorStop> ParseStops(float parentOpacity) {
            // TODO: DEVSIX-4136 opacity is not supported now.
            //  The opacity should be equal to 'parentOpacity * stopRenderer.getStopOpacity() * stopColor[3]'
            IList<GradientColorStop> stopsList = new List<GradientColorStop>();
            foreach (StopSvgNodeRenderer stopRenderer in GetChildStopRenderers()) {
                float[] stopColor = stopRenderer.GetStopColor();
                double offset = stopRenderer.GetOffset();
                stopsList.Add(new GradientColorStop(stopColor, offset, GradientColorStop.OffsetType.RELATIVE));
            }
            if (!stopsList.IsEmpty()) {
                GradientColorStop firstStop = stopsList[0];
                if (firstStop.GetOffset() > 0) {
                    stopsList.Add(0, new GradientColorStop(firstStop, 0F, GradientColorStop.OffsetType.RELATIVE));
                }
                GradientColorStop lastStop = stopsList[stopsList.Count - 1];
                if (lastStop.GetOffset() < 1) {
                    stopsList.Add(new GradientColorStop(lastStop, 1F, GradientColorStop.OffsetType.RELATIVE));
                }
            }
            return stopsList;
        }
    }
}

/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Colors;
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Geom;
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
        protected internal override void DoDraw(SvgDrawContext context) {
            throw new NotSupportedException(SvgExceptionMessageConstant.DRAW_NO_DRAW);
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
                GetAttribute(SvgConstants.Attributes.GRADIENT_UNITS.ToLowerInvariant());
            }
            if (SvgConstants.Values.USER_SPACE_ON_USE.Equals(gradientUnits)) {
                return false;
            }
            else {
                if (gradientUnits != null && !SvgConstants.Values.OBJECT_BOUNDING_BOX.Equals(gradientUnits)) {
                    ITextLogManager.GetLogger(this.GetType()).LogWarning(MessageFormatUtil.Format(SvgLogMessageConstant.GRADIENT_INVALID_GRADIENT_UNITS_LOG
                        , gradientUnits));
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
        protected internal virtual AffineTransform GetGradientTransform() {
            String gradientTransform = GetAttribute(SvgConstants.Attributes.GRADIENT_TRANSFORM);
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            if (gradientTransform == null) {
                gradientTransform = GetAttribute(SvgConstants.Attributes.GRADIENT_TRANSFORM.ToLowerInvariant());
            }
            if (gradientTransform != null && !String.IsNullOrEmpty(gradientTransform)) {
                return TransformUtils.ParseTransform(gradientTransform);
            }
            return null;
        }

        /// <summary>Construct a list of child stop renderers</summary>
        /// <returns>
        /// a list of
        /// <see cref="StopSvgNodeRenderer"/>
        /// elements that represents the child stop values
        /// </returns>
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
        protected internal virtual GradientSpreadMethod ParseSpreadMethod() {
            String spreadMethodValue = GetAttribute(SvgConstants.Attributes.SPREAD_METHOD);
            if (spreadMethodValue == null) {
                spreadMethodValue = GetAttribute(SvgConstants.Attributes.SPREAD_METHOD.ToLowerInvariant());
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
                    ITextLogManager.GetLogger(this.GetType()).LogWarning(MessageFormatUtil.Format(SvgLogMessageConstant.GRADIENT_INVALID_SPREAD_METHOD_LOG
                        , spreadMethodValue));
                    return GradientSpreadMethod.PAD;
                }
            }
        }

        public abstract Color CreateColor(SvgDrawContext arg1, Rectangle arg2, float arg3, float arg4);
    }
}

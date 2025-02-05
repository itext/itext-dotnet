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
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;circle&gt; tag.
    /// </summary>
    public class CircleSvgNodeRenderer : EllipseSvgNodeRenderer {
        protected internal override bool SetParameters(SvgDrawContext context) {
            InitCenter(context);
            String r = GetAttribute(SvgConstants.Attributes.R);
            float percentBaseValue = 0.0F;
            if (CssTypesValidationUtils.IsPercentageValue(r)) {
                if (context.GetCurrentViewPort() == null) {
                    throw new SvgProcessingException(SvgExceptionMessageConstant.ILLEGAL_RELATIVE_VALUE_NO_VIEWPORT_IS_SET);
                }
                percentBaseValue = SvgCoordinateUtils.CalculateNormalizedDiagonalLength(context);
            }
            rx = SvgCssUtils.ParseAbsoluteLength(this, r, percentBaseValue, 0.0F, context);
            ry = rx;
            return rx > 0.0F;
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            CircleSvgNodeRenderer copy = new CircleSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }
    }
}

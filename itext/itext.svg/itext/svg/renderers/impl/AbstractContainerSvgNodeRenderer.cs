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
using iText.Kernel.Geom;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public abstract class AbstractContainerSvgNodeRenderer : AbstractBranchSvgNodeRenderer {
        public override bool CanConstructViewPort() {
            return true;
        }

        protected internal override bool CanElementFill() {
            return false;
        }

        protected internal override void DoDraw(SvgDrawContext context) {
            context.AddViewPort(this.CalculateViewPort(context));
            base.DoDraw(context);
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Calculate the viewport based on the context.</summary>
        /// <param name="context">the SVG draw context</param>
        /// <returns>the viewport that applies to this renderer</returns>
        internal virtual Rectangle CalculateViewPort(SvgDrawContext context) {
            //TODO: DEVSIX-8775 the logic below should be refactored, first of all it shouldn't be applied to root svg tag
            // (though depending on implementation maybe it won't be a problem), also it need to be adjusted to support em/rem
            // which seems possible for all cases, and as for percents, I'm not sure it's possible for nested svg tags, but
            // it should be possible for symbols
            Rectangle currentViewPort = context.GetCurrentViewPort();
            // Set default values to parent viewport in the case of a nested svg tag
            float portX = currentViewPort.GetX();
            float portY = currentViewPort.GetY();
            // Default should be parent portWidth if not outermost
            float portWidth = currentViewPort.GetWidth();
            // Default should be parent height if not outermost
            float portHeight = currentViewPort.GetHeight();
            if (attributesAndStyles != null) {
                if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.X)) {
                    portX = CssDimensionParsingUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.X));
                }
                if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.Y)) {
                    portY = CssDimensionParsingUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.Y));
                }
                if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.WIDTH)) {
                    portWidth = CssDimensionParsingUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.WIDTH
                        ));
                }
                if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.HEIGHT)) {
                    portHeight = CssDimensionParsingUtils.ParseAbsoluteLength(attributesAndStyles.Get(SvgConstants.Attributes.
                        HEIGHT));
                }
            }
            return new Rectangle(portX, portY, portWidth, portHeight);
        }
//\endcond
    }
}

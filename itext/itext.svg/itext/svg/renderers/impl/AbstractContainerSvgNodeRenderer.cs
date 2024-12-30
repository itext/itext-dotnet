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
using iText.Kernel.Geom;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

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
            Rectangle percentBaseBox;
            if (GetParent() is PdfRootSvgNodeRenderer || !(GetParent() is AbstractSvgNodeRenderer)) {
                // If the current container is a top level SVG, make a copy of the current viewport.
                // It is needed to avoid double percent resolving. For absolute sized viewport we
                // will get the same viewport, so save resources and just make a copy.
                return context.GetCurrentViewPort().Clone();
            }
            else {
                // If the current container is nested container, take a view box as a percent base
                percentBaseBox = ((AbstractSvgNodeRenderer)GetParent()).GetCurrentViewBox(context);
            }
            float portX = 0;
            float portY = 0;
            float portWidth = percentBaseBox.GetWidth();
            float portHeight = percentBaseBox.GetHeight();
            if (attributesAndStyles != null) {
                portX = SvgCssUtils.ParseAbsoluteLength(this, attributesAndStyles.Get(SvgConstants.Attributes.X), percentBaseBox
                    .GetWidth(), 0, context);
                portY = SvgCssUtils.ParseAbsoluteLength(this, attributesAndStyles.Get(SvgConstants.Attributes.Y), percentBaseBox
                    .GetHeight(), 0, context);
                String widthStr = attributesAndStyles.Get(SvgConstants.Attributes.WIDTH);
                // In case widthStr==null, according to SVG spec default value is 100%, it is why default
                // value is percentBaseBox.getWidth(). See SvgConstants.Values.DEFAULT_WIDTH_AND_HEIGHT_VALUE
                portWidth = SvgCssUtils.ParseAbsoluteLength(this, widthStr, percentBaseBox.GetWidth(), percentBaseBox.GetWidth
                    (), context);
                String heightStr = attributesAndStyles.Get(SvgConstants.Attributes.HEIGHT);
                // In case heightStr==null, according to SVG spec default value is 100%, it is why default
                // value is percentBaseBox.getHeight(). See SvgConstants.Values.DEFAULT_WIDTH_AND_HEIGHT_VALUE
                portHeight = SvgCssUtils.ParseAbsoluteLength(this, heightStr, percentBaseBox.GetHeight(), percentBaseBox.GetHeight
                    (), context);
            }
            return new Rectangle(portX, portY, portWidth, portHeight);
        }
//\endcond
    }
}

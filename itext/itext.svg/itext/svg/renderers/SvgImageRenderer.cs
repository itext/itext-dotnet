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
using iText.Layout.Layout;
using iText.Layout.Renderer;
using iText.Svg;
using iText.Svg.Element;
using iText.Svg.Utils;
using iText.Svg.Xobject;

namespace iText.Svg.Renderers {
    /// <summary>
    /// Represents a renderer for the
    /// <see cref="iText.Svg.Element.SvgImage"/>
    /// layout element.
    /// </summary>
    public class SvgImageRenderer : ImageRenderer {
        /// <summary>Creates an SvgImageRenderer from its corresponding layout object.</summary>
        /// <param name="image">
        /// the
        /// <see cref="iText.Svg.Element.SvgImage"/>
        /// which this object should manage
        /// </param>
        public SvgImageRenderer(SvgImage image)
            : base(image) {
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            SvgImage svgImage = (SvgImage)modelElement;
            if (svgImage.GetSvgImageXObject().IsRelativeSized()) {
                CalculateRelativeSizedSvgSize(svgImage, layoutContext.GetArea().GetBBox());
            }
            return base.Layout(layoutContext);
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(DrawContext drawContext) {
            ((SvgImage)modelElement).GetSvgImageXObject().Generate(drawContext.GetDocument());
            base.Draw(drawContext);
        }

        private void CalculateRelativeSizedSvgSize(SvgImage svgImage, Rectangle layoutBox) {
            SvgImageXObject svgImageXObject = svgImage.GetSvgImageXObject();
            ISvgNodeRenderer svgRootRenderer = svgImageXObject.GetResult().GetRootRenderer();
            float? aspectRatio = null;
            float[] viewBoxValues = SvgCssUtils.ParseViewBox(svgRootRenderer);
            if (viewBoxValues != null && viewBoxValues.Length == SvgConstants.Values.VIEWBOX_VALUES_NUMBER) {
                // aspectRatio can also be specified by absolute height and width,
                // but in that case SVG isn't relative and processed as usual image
                aspectRatio = viewBoxValues[2] / viewBoxValues[3];
            }
            float? retrievedAreaWidth = RetrieveWidth(layoutBox.GetWidth());
            float? retrievedAreaHeight = RetrieveHeight();
            float areaWidth = retrievedAreaWidth == null ? (aspectRatio == null ? SvgConstants.Values.DEFAULT_VIEWPORT_WIDTH
                 : layoutBox.GetWidth()) : (float)retrievedAreaWidth;
            float areaHeight = retrievedAreaHeight == null ? SvgConstants.Values.DEFAULT_VIEWPORT_HEIGHT : (float)retrievedAreaHeight;
            float finalWidth;
            float finalHeight;
            if (aspectRatio != null && (retrievedAreaHeight == null || retrievedAreaWidth == null)) {
                if (retrievedAreaWidth == null && retrievedAreaHeight != null) {
                    finalHeight = areaHeight;
                    finalWidth = (float)(finalHeight * aspectRatio);
                }
                else {
                    finalWidth = areaWidth;
                    finalHeight = (float)(finalWidth / aspectRatio);
                }
            }
            else {
                finalWidth = areaWidth;
                finalHeight = areaHeight;
            }
            svgRootRenderer.SetAttribute(SvgConstants.Attributes.WIDTH, null);
            svgRootRenderer.SetAttribute(SvgConstants.Attributes.HEIGHT, null);
            svgImageXObject.UpdateBBox(finalWidth, finalHeight);
            imageWidth = svgImage.GetImageWidth();
            imageHeight = svgImage.GetImageHeight();
        }
    }
}

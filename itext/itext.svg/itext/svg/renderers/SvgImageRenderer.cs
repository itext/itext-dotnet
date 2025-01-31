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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Layout;
using iText.Layout.Properties;
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
            Rectangle layoutBox = layoutContext.GetArea().GetBBox();
            if (svgImage.GetSvgImageXObject().IsRelativeSized()) {
                CalculateRelativeSizedSvgSize(svgImage, layoutBox);
            }
            else {
                if (svgImage.GetSvgImageXObject().IsCreatedByObject() && !true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT
                    ))) {
                    SvgImageRenderer.NullableArea retrievedArea = new SvgImageRenderer.NullableArea(RetrieveWidth(layoutBox.GetWidth
                        ()), RetrieveHeight());
                    PdfArray bbox = svgImage.GetSvgImageXObject().GetBBox();
                    if (retrievedArea.width != null && retrievedArea.height != null) {
                        bbox.Set(2, new PdfNumber((double)retrievedArea.width));
                        bbox.Set(3, new PdfNumber((double)retrievedArea.height));
                        imageWidth = (float)retrievedArea.width;
                        imageHeight = (float)retrievedArea.height;
                    }
                    else {
                        if (retrievedArea.width != null) {
                            SvgImageRenderer.Area bboxArea = new SvgImageRenderer.Area(((PdfNumber)bbox.Get(2)).FloatValue(), ((PdfNumber
                                )bbox.Get(3)).FloatValue());
                            double verticalScaling = (double)retrievedArea.width / bboxArea.width;
                            bbox.Set(2, new PdfNumber((double)retrievedArea.width));
                            bbox.Set(3, new PdfNumber(bboxArea.height * verticalScaling));
                            imageWidth = (float)retrievedArea.width;
                            imageHeight = imageHeight * (float)verticalScaling;
                        }
                        else {
                            if (retrievedArea.height != null) {
                                SvgImageRenderer.Area bboxArea = new SvgImageRenderer.Area(((PdfNumber)bbox.Get(2)).FloatValue(), ((PdfNumber
                                    )bbox.Get(3)).FloatValue());
                                double horizontalScaling = (double)retrievedArea.height / bboxArea.height;
                                bbox.Set(2, new PdfNumber(bboxArea.width * horizontalScaling));
                                bbox.Set(3, new PdfNumber((double)retrievedArea.height));
                                imageWidth = imageWidth * (float)horizontalScaling;
                                imageHeight = (float)retrievedArea.height;
                            }
                        }
                    }
                }
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
            SvgImageRenderer.NullableArea retrievedArea = new SvgImageRenderer.NullableArea(RetrieveWidth(layoutBox.GetWidth
                ()), RetrieveHeight());
            bool preserveAspectRatioNone = SvgConstants.Values.NONE.Equals(svgRootRenderer.GetAttribute(SvgConstants.Attributes
                .PRESERVE_ASPECT_RATIO));
            SvgImageRenderer.Area area = new SvgImageRenderer.Area();
            area.width = retrievedArea.width == null ? (aspectRatio == null ? SvgConstants.Values.DEFAULT_VIEWPORT_WIDTH
                 : layoutBox.GetWidth()) : (float)retrievedArea.width;
            area.height = retrievedArea.height == null ? (aspectRatio == null ? SvgConstants.Values.DEFAULT_VIEWPORT_HEIGHT
                 : layoutBox.GetHeight()) : (float)retrievedArea.height;
            UnitValue elementWidth = svgImageXObject.GetElementWidth();
            UnitValue elementHeight = svgImageXObject.GetElementHeight();
            //For aspect ratio none we're using the default viewport instead of layoutBox to behave like a browser
            //But this only for <img>, for all other cases using layoutBox as a fallback
            SvgImageRenderer.Area finalArea = new SvgImageRenderer.Area();
            if (preserveAspectRatioNone && svgImageXObject.IsCreatedByImg()) {
                finalArea.width = retrievedArea.width == null ? SvgConstants.Values.DEFAULT_VIEWPORT_WIDTH : (float)retrievedArea
                    .width;
                finalArea.height = retrievedArea.height == null ? SvgConstants.Values.DEFAULT_VIEWPORT_HEIGHT : (float)retrievedArea
                    .height;
            }
            else {
                finalArea = InitMissingMetricsAndApplyAspectRatio(aspectRatio, retrievedArea, area, elementWidth, elementHeight
                    );
            }
            if (svgImageXObject.IsCreatedByImg() && viewBoxValues == null) {
                if (this.GetProperty<UnitValue>(Property.WIDTH) == null) {
                    this.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(finalArea.width));
                }
                if (RetrieveHeight() == null) {
                    this.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(finalArea.height));
                }
                svgImageXObject.UpdateBBox(finalArea.width, finalArea.height);
            }
            else {
                svgRootRenderer.SetAttribute(SvgConstants.Attributes.WIDTH, null);
                svgRootRenderer.SetAttribute(SvgConstants.Attributes.HEIGHT, null);
                svgImageXObject.UpdateBBox(finalArea.width, finalArea.height);
            }
            imageWidth = svgImage.GetImageWidth();
            imageHeight = svgImage.GetImageHeight();
        }

        private SvgImageRenderer.Area InitMissingMetricsAndApplyAspectRatio(float? aspectRatio, SvgImageRenderer.NullableArea
             retrievedArea, SvgImageRenderer.Area area, UnitValue xObjectWidth, UnitValue xObjectHeight) {
            SvgImageRenderer.Area finalArea = new SvgImageRenderer.Area();
            if (!TryToApplyAspectRatio(retrievedArea, area, finalArea, aspectRatio)) {
                if (xObjectWidth != null && xObjectWidth.IsPointValue() && retrievedArea.width == null) {
                    area.width = xObjectWidth.GetValue();
                    retrievedArea.width = area.width;
                    this.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(area.width));
                }
                if (xObjectHeight != null && xObjectHeight.IsPointValue() && retrievedArea.height == null) {
                    area.height = xObjectHeight.GetValue();
                    retrievedArea.height = area.height;
                    this.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(area.height));
                }
                bool isAspectRatioApplied = TryToApplyAspectRatio(retrievedArea, area, area, aspectRatio);
                if (!isAspectRatioApplied && aspectRatio != null && retrievedArea.height == null) {
                    // retrievedArea.width is also null here
                    area.height = (float)(area.width / aspectRatio);
                }
                finalArea.width = area.width;
                finalArea.height = area.height;
            }
            return finalArea;
        }

        private static bool TryToApplyAspectRatio(SvgImageRenderer.NullableArea retrievedArea, SvgImageRenderer.Area
             inputArea, SvgImageRenderer.Area resultArea, float? aspectRatio) {
            if (aspectRatio == null) {
                return false;
            }
            if (retrievedArea.width == null && retrievedArea.height != null) {
                resultArea.height = inputArea.height;
                resultArea.width = (float)(inputArea.height * (float)aspectRatio);
                return true;
            }
            else {
                if (retrievedArea.width != null && retrievedArea.height == null) {
                    resultArea.width = inputArea.width;
                    resultArea.height = (float)(inputArea.width / (float)aspectRatio);
                    return true;
                }
            }
            return false;
        }

        private class NullableArea {
            public float? width;

            public float? height;

            public NullableArea(float? width, float? height) {
                this.width = width;
                this.height = height;
            }
        }

        private class Area {
            public float width;

            public float height;

            public Area() {
                width = 0.0f;
                height = 0.0f;
            }

            public Area(float width, float height) {
                this.width = width;
                this.height = height;
            }
        }
    }
}

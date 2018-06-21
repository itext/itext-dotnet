using System;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>Renderer implementing the use tag.</summary>
    /// <remarks>Renderer implementing the use tag. This tag allows you to reuse previously defined elements.</remarks>
    public class UseSvgNodeRenderer : AbstractSvgNodeRenderer {
        protected internal override void DoDraw(SvgDrawContext context) {
            if (this.attributesAndStyles != null) {
                String elementToReUse = this.attributesAndStyles.Get(SvgConstants.Attributes.XLINK_HREF);
                if (elementToReUse == null) {
                    elementToReUse = this.attributesAndStyles.Get(SvgConstants.Attributes.HREF);
                }
                if (elementToReUse != null && !String.IsNullOrEmpty(elementToReUse) && IsValidHref(elementToReUse)) {
                    String normalizedName = NormalizeName(elementToReUse);
                    if (!context.IsIdUsedByUseTagBefore(normalizedName)) {
                        ISvgNodeRenderer namedObject = context.GetNamedObject(normalizedName);
                        if (namedObject != null) {
                            PdfCanvas currentCanvas = context.GetCurrentCanvas();
                            float x = 0f;
                            float y = 0f;
                            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.X)) {
                                x = CssUtils.ParseAbsoluteLength(this.attributesAndStyles.Get(SvgConstants.Attributes.X));
                            }
                            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.Y)) {
                                y = CssUtils.ParseAbsoluteLength(this.attributesAndStyles.Get(SvgConstants.Attributes.Y));
                            }
                            if (!SvgMathUtils.CompareFloats(x, 0) || !SvgMathUtils.CompareFloats(y, 0)) {
                                AffineTransform translation = AffineTransform.GetTranslateInstance(x, y);
                                currentCanvas.ConcatMatrix(translation);
                            }
                            // setting the parent of the referenced element to this instance
                            namedObject.SetParent(this);
                            namedObject.Draw(context);
                            // unsetting the parent of the referenced element
                            namedObject.SetParent(null);
                        }
                    }
                }
            }
        }

        /// <summary>The reference value will contain a hashtag character.</summary>
        /// <remarks>The reference value will contain a hashtag character. This method will filter that value.</remarks>
        /// <param name="name">value to be filtered</param>
        /// <returns>filtered value</returns>
        private String NormalizeName(String name) {
            return name.Replace("#", "").Trim();
        }

        private bool IsValidHref(String name) {
            return name.StartsWith("#");
        }
    }
}

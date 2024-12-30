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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;
using iText.Svg.Utils;

namespace iText.Svg.Xobject {
    /// <summary>A wrapper for Form XObject for SVG images.</summary>
    public class SvgImageXObject : PdfFormXObject {
        private readonly ISvgProcessorResult result;

        private readonly ResourceResolver resourceResolver;

        private bool isGenerated = false;

        private float em;

        private SvgDrawContext svgDrawContext;

        private bool isRelativeSized = false;

        /// <summary>Creates a new instance of Form XObject for the SVG image.</summary>
        /// <param name="bBox">the form XObject’s bounding box.</param>
        /// <param name="result">processor result containing the SVG information.</param>
        /// <param name="resourceResolver">
        /// 
        /// <see cref="iText.StyledXmlParser.Resolver.Resource.ResourceResolver"/>
        /// for the SVG image.
        /// </param>
        public SvgImageXObject(Rectangle bBox, ISvgProcessorResult result, ResourceResolver resourceResolver)
            : base(bBox) {
            this.result = result;
            this.resourceResolver = resourceResolver;
            this.svgDrawContext = new SvgDrawContext(resourceResolver, result.GetFontProvider());
        }

        /// <summary>Creates a new instance of Form XObject for the relative sized SVG image.</summary>
        /// <param name="result">processor result containing the SVG information</param>
        /// <param name="svgContext">the svg draw context</param>
        /// <param name="em">em value in pt</param>
        /// <param name="pdfDocument">pdf that shall contain the SVG image, can be null</param>
        public SvgImageXObject(ISvgProcessorResult result, SvgDrawContext svgContext, float em, PdfDocument pdfDocument
            )
            : this(null, result, svgContext.GetResourceResolver()) {
            if (pdfDocument != null) {
                svgContext.PushCanvas(new PdfCanvas(this, pdfDocument));
            }
            this.em = em;
            this.isRelativeSized = true;
            this.svgDrawContext = svgContext;
        }

        /// <summary>If the SVG image is relative sized.</summary>
        /// <remarks>
        /// If the SVG image is relative sized. This information
        /// is used during image layouting to resolve it's relative size.
        /// </remarks>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the SVG image is relative sized,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        /// <seealso cref="UpdateBBox(float?, float?)"/>
        /// <seealso cref="SvgImageXObject(iText.Svg.Processors.ISvgProcessorResult, iText.Svg.Renderers.SvgDrawContext, float, iText.Kernel.Pdf.PdfDocument)
        ///     "/>
        public virtual bool IsRelativeSized() {
            return isRelativeSized;
        }

        /// <summary>Sets if the SVG image is relative sized.</summary>
        /// <remarks>
        /// Sets if the SVG image is relative sized. This information
        /// is used during image layouting to resolve it's relative size.
        /// </remarks>
        /// <param name="relativeSized">
        /// 
        /// <see langword="true"/>
        /// if the SVG image is relative sized,
        /// <see langword="false"/>
        /// otherwise
        /// </param>
        /// <seealso cref="UpdateBBox(float?, float?)"/>
        /// <seealso cref="SvgImageXObject(iText.Svg.Processors.ISvgProcessorResult, iText.Svg.Renderers.SvgDrawContext, float, iText.Kernel.Pdf.PdfDocument)
        ///     "/>
        public virtual void SetRelativeSized(bool relativeSized) {
            // TODO DEVSIX-8829 remove/deprecate this method after ticket will be done
            isRelativeSized = relativeSized;
        }

        /// <summary>Returns processor result containing the SVG information.</summary>
        /// <returns>{ISvgProcessorResult} processor result.</returns>
        public virtual ISvgProcessorResult GetResult() {
            return result;
        }

        /// <summary>Returns resource resolver for the SVG image.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.StyledXmlParser.Resolver.Resource.ResourceResolver"/>
        /// instance
        /// </returns>
        [System.ObsoleteAttribute(@"not used anymore")]
        public virtual ResourceResolver GetResourceResolver() {
            return resourceResolver;
        }

        /// <summary>Processes xObject before first image generation to avoid drawing it twice or more.</summary>
        /// <remarks>
        /// Processes xObject before first image generation to avoid drawing it twice or more. It allows to reuse the same
        /// Form XObject multiple times.
        /// </remarks>
        /// <param name="document">
        /// pdf that shall contain the SVG image, can be null if constructor
        /// <see cref="SvgImageXObject(iText.Svg.Processors.ISvgProcessorResult, iText.Svg.Renderers.SvgDrawContext, float, iText.Kernel.Pdf.PdfDocument)
        ///     "/>
        /// was used
        /// </param>
        public virtual void Generate(PdfDocument document) {
            if (!isGenerated) {
                if (result is SvgProcessorResult) {
                    svgDrawContext.SetCssContext(((SvgProcessorResult)result).GetContext().GetCssContext());
                }
                svgDrawContext.SetTempFonts(result.GetTempFonts());
                svgDrawContext.AddNamedObjects(result.GetNamedObjects());
                if (svgDrawContext.Size() == 0) {
                    svgDrawContext.PushCanvas(new PdfCanvas(this, document));
                }
                ISvgNodeRenderer root = new PdfRootSvgNodeRenderer(result.GetRootRenderer());
                root.Draw(svgDrawContext);
                isGenerated = true;
            }
        }

        /// <summary>Updated XObject BBox for relative sized SVG image.</summary>
        /// <param name="areaWidth">the area width where SVG image will be drawn</param>
        /// <param name="areaHeight">the area height where SVG image will be drawn</param>
        public virtual void UpdateBBox(float? areaWidth, float? areaHeight) {
            // TODO DEVSIX-8829 change parameters to float, not Float
            if (areaWidth != null && areaHeight != null) {
                svgDrawContext.SetCustomViewport(new Rectangle((float)areaWidth, (float)areaHeight));
            }
            Rectangle bbox = SvgCssUtils.ExtractWidthAndHeight(result.GetRootRenderer(), em, svgDrawContext);
            SetBBox(new PdfArray(bbox));
        }

        /// <summary>Gets the SVG element width.</summary>
        /// <returns>the SVG element width</returns>
        public virtual UnitValue GetElementWidth() {
            String widthStr = result.GetRootRenderer().GetAttribute(SvgConstants.Attributes.WIDTH);
            return CssDimensionParsingUtils.ParseLengthValueToPt(widthStr, em, svgDrawContext.GetCssContext().GetRootFontSize
                ());
        }

        /// <summary>Gets the SVG element height.</summary>
        /// <returns>the SVG element height</returns>
        public virtual UnitValue GetElementHeight() {
            String heightStr = result.GetRootRenderer().GetAttribute(SvgConstants.Attributes.HEIGHT);
            return CssDimensionParsingUtils.ParseLengthValueToPt(heightStr, em, svgDrawContext.GetCssContext().GetRootFontSize
                ());
        }
    }
}

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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Xobject {
    /// <summary>A wrapper for Form XObject for SVG images.</summary>
    public class SvgImageXObject : PdfFormXObject {
        private readonly ISvgProcessorResult result;

        private readonly ResourceResolver resourceResolver;

        private bool isGenerated = false;

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
        public virtual ResourceResolver GetResourceResolver() {
            return resourceResolver;
        }

        /// <summary>Processes xObject before first image generation to avoid drawing it twice or more.</summary>
        /// <remarks>
        /// Processes xObject before first image generation to avoid drawing it twice or more. It allows to reuse the same
        /// Form XObject multiple times.
        /// </remarks>
        /// <param name="document">pdf that shall contain the SVG image.</param>
        public virtual void Generate(PdfDocument document) {
            if (!isGenerated) {
                PdfCanvas canvas = new PdfCanvas(this, document);
                SvgDrawContext context = new SvgDrawContext(resourceResolver, result.GetFontProvider());
                if (result is SvgProcessorResult) {
                    context.SetCssContext(((SvgProcessorResult)result).GetContext().GetCssContext());
                }
                context.AddNamedObjects(result.GetNamedObjects());
                context.PushCanvas(canvas);
                ISvgNodeRenderer root = new PdfRootSvgNodeRenderer(result.GetRootRenderer());
                root.Draw(context);
                isGenerated = true;
            }
        }
    }
}

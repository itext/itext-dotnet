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
using iText.Layout.Font;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Processors;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Processors.Impl {
    /// <summary>
    /// Default and fallback implementation of
    /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
    /// for
    /// <see cref="DefaultSvgProcessor"/>.
    /// </summary>
    public class SvgConverterProperties : ISvgConverterProperties {
        /// <summary>The media device description.</summary>
        private MediaDeviceDescription mediaDeviceDescription;

        /// <summary>The font provider.</summary>
        private FontProvider fontProvider;

        /// <summary>The base URI.</summary>
        private String baseUri = "";

        /// <summary>The resource retriever.</summary>
        private IResourceRetriever resourceRetriever;

        private ISvgNodeRendererFactory rendererFactory;

        private String charset = System.Text.Encoding.UTF8.Name();

        private CssStyleSheet cssStyleSheet = null;

        /// <summary>
        /// Creates a new
        /// <see cref="SvgConverterProperties"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="SvgConverterProperties"/>
        /// instance.
        /// Instantiates its members, IResourceRetriever and ISvgNodeRendererFactory, to its default implementations.
        /// </remarks>
        public SvgConverterProperties() {
            this.resourceRetriever = new DefaultResourceRetriever();
            this.rendererFactory = new DefaultSvgNodeRendererFactory();
        }

        public virtual iText.Svg.Processors.Impl.SvgConverterProperties SetRendererFactory(ISvgNodeRendererFactory
             rendererFactory) {
            this.rendererFactory = rendererFactory;
            return this;
        }

        public virtual iText.Svg.Processors.Impl.SvgConverterProperties SetFontProvider(FontProvider fontProvider) {
            this.fontProvider = fontProvider;
            return this;
        }

        public virtual ISvgNodeRendererFactory GetRendererFactory() {
            return this.rendererFactory;
        }

        public virtual String GetCharset() {
            // may also return null, but null will always default to UTF-8 in JSoup
            return charset;
        }

        public virtual iText.Svg.Processors.Impl.SvgConverterProperties SetCharset(String charset) {
            this.charset = charset;
            return this;
        }

        /// <summary>Gets the base URI.</summary>
        /// <returns>the base URI</returns>
        public virtual String GetBaseUri() {
            return baseUri;
        }

        /// <summary>Gets the font provider.</summary>
        /// <returns>the font provider</returns>
        public virtual FontProvider GetFontProvider() {
            return fontProvider;
        }

        /// <summary>Gets the media device description.</summary>
        /// <returns>the media device description</returns>
        public virtual MediaDeviceDescription GetMediaDeviceDescription() {
            return mediaDeviceDescription;
        }

        /// <summary>Sets the media device description.</summary>
        /// <param name="mediaDeviceDescription">the media device description</param>
        /// <returns>the ConverterProperties instance</returns>
        public virtual iText.Svg.Processors.Impl.SvgConverterProperties SetMediaDeviceDescription(MediaDeviceDescription
             mediaDeviceDescription) {
            this.mediaDeviceDescription = mediaDeviceDescription;
            return this;
        }

        /// <summary>Sets the base URI.</summary>
        /// <param name="baseUri">the base URI</param>
        /// <returns>the ConverterProperties instance</returns>
        public virtual iText.Svg.Processors.Impl.SvgConverterProperties SetBaseUri(String baseUri) {
            this.baseUri = baseUri;
            return this;
        }

        public virtual IResourceRetriever GetResourceRetriever() {
            return resourceRetriever;
        }

        /// <summary>Sets the resource retriever.</summary>
        /// <remarks>
        /// Sets the resource retriever.
        /// The resourceRetriever is used to retrieve data from resources by URL.
        /// </remarks>
        /// <param name="resourceRetriever">the resource retriever</param>
        /// <returns>
        /// the
        /// <see cref="SvgConverterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Svg.Processors.Impl.SvgConverterProperties SetResourceRetriever(IResourceRetriever resourceRetriever
            ) {
            this.resourceRetriever = resourceRetriever;
            return this;
        }

        public virtual CssStyleSheet GetCssStyleSheet() {
            return cssStyleSheet;
        }

        /// <summary>Sets the CSS style sheet.</summary>
        /// <remarks>
        /// Sets the CSS style sheet.
        /// Style sheet is used to apply CSS statements to elements.
        /// </remarks>
        /// <param name="cssStyleSheet">the CSS style sheet</param>
        /// <returns>
        /// the
        /// <see cref="SvgConverterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Svg.Processors.Impl.SvgConverterProperties SetCssStyleSheet(CssStyleSheet cssStyleSheet
            ) {
            this.cssStyleSheet = cssStyleSheet;
            return this;
        }
    }
}

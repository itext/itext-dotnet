/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.IO.Font;
using iText.Layout.Font;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Resolver.Font;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Css;
using iText.Svg.Processors;

namespace iText.Svg.Processors.Impl {
    /// <summary>Context class with accessors to properties/objects used in processing Svg documents</summary>
    public class SvgProcessorContext {
        /// <summary>The font provider.</summary>
        private FontProvider fontProvider;

        /// <summary>Temporary set of fonts used in the PDF.</summary>
        private FontSet tempFonts;

        private readonly ResourceResolver resourceResolver;

        /// <summary>The device description.</summary>
        private MediaDeviceDescription deviceDescription;

        /// <summary>The SVG CSS context.</summary>
        private readonly SvgCssContext cssContext;

        /// <summary>
        /// Instantiates a new
        /// <see cref="SvgProcessorContext"/>
        /// instance.
        /// </summary>
        /// <param name="converterProperties">
        /// a
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// instance
        /// </param>
        public SvgProcessorContext(ISvgConverterProperties converterProperties) {
            deviceDescription = converterProperties.GetMediaDeviceDescription();
            if (deviceDescription == null) {
                deviceDescription = MediaDeviceDescription.GetDefault();
            }
            fontProvider = converterProperties.GetFontProvider();
            if (fontProvider == null) {
                fontProvider = new BasicFontProvider();
            }
            resourceResolver = new ResourceResolver(converterProperties.GetBaseUri(), converterProperties.GetResourceRetriever
                ());
            cssContext = new SvgCssContext();
        }

        /// <summary>Gets the font provider.</summary>
        /// <returns>the font provider</returns>
        public virtual FontProvider GetFontProvider() {
            return fontProvider;
        }

        /// <summary>Gets the resource resolver.</summary>
        /// <returns>the resource resolver</returns>
        public virtual ResourceResolver GetResourceResolver() {
            return resourceResolver;
        }

        /// <summary>Gets the device description.</summary>
        /// <returns>the device description</returns>
        public virtual MediaDeviceDescription GetDeviceDescription() {
            return deviceDescription;
        }

        /// <summary>Gets the temporary set of fonts.</summary>
        /// <returns>the set of fonts</returns>
        public virtual FontSet GetTempFonts() {
            return tempFonts;
        }

        /// <summary>Gets the SVG CSS context.</summary>
        /// <returns>the SVG CSS context</returns>
        public virtual SvgCssContext GetCssContext() {
            return cssContext;
        }

        /// <summary>Add temporary font from @font-face.</summary>
        /// <param name="fontProgram">the font program</param>
        /// <param name="encoding">the encoding</param>
        /// <param name="alias">the alias</param>
        /// <param name="unicodeRange">the specific range of characters to be used from the font</param>
        public virtual void AddTemporaryFont(FontProgram fontProgram, String encoding, String alias, Range unicodeRange
            ) {
            if (tempFonts == null) {
                tempFonts = new FontSet();
            }
            tempFonts.AddFont(fontProgram, encoding, alias, unicodeRange);
        }

        /// <summary>Add temporary font from @font-face.</summary>
        /// <param name="fontProgram">the font program</param>
        /// <param name="encoding">the encoding</param>
        /// <param name="alias">the alias</param>
        public virtual void AddTemporaryFont(FontProgram fontProgram, String encoding, String alias) {
            if (tempFonts == null) {
                tempFonts = new FontSet();
            }
            tempFonts.AddFont(fontProgram, encoding, alias);
        }

        /// <summary>Add temporary font from @font-face.</summary>
        /// <param name="fontInfo">the font info</param>
        /// <param name="alias">the alias</param>
        public virtual void AddTemporaryFont(FontInfo fontInfo, String alias) {
            if (tempFonts == null) {
                tempFonts = new FontSet();
            }
            tempFonts.AddFont(fontInfo, alias);
        }
    }
}

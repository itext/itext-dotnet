/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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

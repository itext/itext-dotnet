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
using System;
using iText.Layout.Font;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Processors {
    /// <summary>
    /// Interface for the configuration classes used by
    /// <see cref="ISvgProcessor"/>
    /// </summary>
    public interface ISvgConverterProperties {
        /// <summary>
        /// Retrieve the factory responsible for creating
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="iText.Svg.Renderers.Factories.ISvgNodeRendererFactory"/>
        /// implementation
        /// </returns>
        ISvgNodeRendererFactory GetRendererFactory();

        /// <summary>Gets the font provider.</summary>
        /// <returns>the font provider</returns>
        FontProvider GetFontProvider();

        /// <summary>Get the name of the Charset to be used when decoding an InputStream.</summary>
        /// <remarks>
        /// Get the name of the Charset to be used when decoding an InputStream. This
        /// method is allowed to return null, in which case
        /// <c>UTF-8</c>
        /// will
        /// be used (by JSoup).
        /// <para />
        /// Please be aware that this method is NOT used when handling a
        /// <c>String</c>
        /// variable in the
        /// <see cref="iText.Svg.Converter.SvgConverter"/>.
        /// </remarks>
        /// <returns>
        /// the String name of the
        /// <see cref="System.Text.Encoding"/>
        /// used for decoding
        /// </returns>
        String GetCharset();

        /// <summary>Gets the base URI.</summary>
        /// <returns>the base URI</returns>
        String GetBaseUri();

        /// <summary>Gets the media device description.</summary>
        /// <returns>the media device description</returns>
        MediaDeviceDescription GetMediaDeviceDescription();

        /// <summary>Gets the resource retriever.</summary>
        /// <remarks>
        /// Gets the resource retriever.
        /// The resourceRetriever is used to retrieve data from resources by URL.
        /// </remarks>
        /// <returns>the resource retriever</returns>
        IResourceRetriever GetResourceRetriever();

        /// <summary>Gets the CSS style sheet.</summary>
        /// <remarks>
        /// Gets the CSS style sheet.
        /// Style sheet is used to apply CSS statements to elements.
        /// </remarks>
        /// <returns>the CSS style sheet</returns>
        CssStyleSheet GetCssStyleSheet();
    }
}

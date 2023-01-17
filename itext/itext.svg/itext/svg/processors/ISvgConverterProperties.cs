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
using iText.Layout.Font;
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
    }
}

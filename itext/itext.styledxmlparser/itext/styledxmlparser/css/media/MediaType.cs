/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using System.Collections.Generic;

namespace iText.StyledXmlParser.Css.Media {
    /// <summary>
    /// Class that bundles all the media types and allows you to registered valid media types in a
    /// <see cref="Java.Util.Set{E}"/>.
    /// </summary>
    public sealed class MediaType {
        /// <summary>The Constant registeredMediaTypes.</summary>
        private static readonly ICollection<String> registeredMediaTypes = new HashSet<String>();

        /// <summary>The Constant ALL.</summary>
        public static readonly String ALL = RegisterMediaType("all");

        /// <summary>The Constant AURAL.</summary>
        public static readonly String AURAL = RegisterMediaType("aural");

        /// <summary>The Constant BRAILLE.</summary>
        public static readonly String BRAILLE = RegisterMediaType("braille");

        /// <summary>The Constant EMBOSSED.</summary>
        public static readonly String EMBOSSED = RegisterMediaType("embossed");

        /// <summary>The Constant HANDHELD.</summary>
        public static readonly String HANDHELD = RegisterMediaType("handheld");

        /// <summary>The Constant PRINT.</summary>
        public static readonly String PRINT = RegisterMediaType("print");

        /// <summary>The Constant PROJECTION.</summary>
        public static readonly String PROJECTION = RegisterMediaType("projection");

        /// <summary>The Constant SCREEN.</summary>
        public static readonly String SCREEN = RegisterMediaType("screen");

        /// <summary>The Constant SPEECH.</summary>
        public static readonly String SPEECH = RegisterMediaType("speech");

        /// <summary>The Constant TTY.</summary>
        public static readonly String TTY = RegisterMediaType("tty");

        /// <summary>The Constant TV.</summary>
        public static readonly String TV = RegisterMediaType("tv");

        /// <summary>
        /// Creates a new
        /// <see cref="MediaType"/>
        /// instance.
        /// </summary>
        private MediaType() {
        }

        /// <summary>Checks if a media type is registered as a valid media type.</summary>
        /// <param name="mediaType">the media type</param>
        /// <returns>true, if it's a valid media type</returns>
        public static bool IsValidMediaType(String mediaType) {
            return registeredMediaTypes.Contains(mediaType);
        }

        /// <summary>Registers a media type.</summary>
        /// <param name="mediaType">the media type</param>
        /// <returns>the string</returns>
        private static String RegisterMediaType(String mediaType) {
            registeredMediaTypes.Add(mediaType);
            return mediaType;
        }
    }
}

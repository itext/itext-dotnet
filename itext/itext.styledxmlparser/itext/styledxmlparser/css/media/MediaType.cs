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

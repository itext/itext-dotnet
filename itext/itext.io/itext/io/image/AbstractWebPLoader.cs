/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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

namespace iText.IO.Image {
    /// <summary>An abstract class to control WebP image data handling.</summary>
    public abstract class AbstractWebPLoader {
        /// <summary>
        /// Creates an instance of
        /// <see cref="AbstractWebPLoader"/>.
        /// </summary>
        protected internal AbstractWebPLoader() {
        }

        // do nothing
        /// <summary>
        /// Gets
        /// <see cref="ImageData"/>
        /// from provided WebP raw image bytes.
        /// </summary>
        /// <param name="bytes">raw bytes to create WebP image data from</param>
        /// <returns>
        /// a new WebP
        /// <see cref="ImageData"/>
        /// from raw bytes
        /// </returns>
        protected internal abstract ImageData GetImageData(byte[] bytes);

        /// <summary>
        /// Gets
        /// <see cref="ImageData"/>
        /// from provided WebP URL.
        /// </summary>
        /// <param name="url">URL to create WebP image data from</param>
        /// <returns>
        /// a new WebP
        /// <see cref="ImageData"/>
        /// from URL
        /// </returns>
        protected internal abstract ImageData GetImageData(Uri url);

        /// <summary>Checks if webp-image-support module is loaded.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if webp-image-support module is loaded
        /// </returns>
        protected internal abstract bool IsWebPSupported();
    }
}

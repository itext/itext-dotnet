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

namespace iText.IO.Image {
    public class BmpImageData : RawImageData {
        private int size;

        private bool noHeader;

        /// <summary>
        /// Creates instance of
        /// <see cref="BmpImageData"/>
        /// </summary>
        /// <param name="url">url of the image</param>
        /// <param name="noHeader">indicates that the source image does not have a header</param>
        protected internal BmpImageData(Uri url, bool noHeader)
            : base(url, ImageType.BMP) {
            this.noHeader = noHeader;
        }

        /// <summary>
        /// Creates instance of
        /// <see cref="BmpImageData"/>
        /// </summary>
        /// <param name="url">url of the image</param>
        /// <param name="noHeader">indicates that the source image does not have a header</param>
        /// <param name="size">the size of the image (length of the byte array)</param>
        [System.ObsoleteAttribute(@"will be removed in 7.2")]
        protected internal BmpImageData(Uri url, bool noHeader, int size)
            : this(url, noHeader) {
            this.size = size;
        }

        /// <summary>
        /// Creates instance of
        /// <see cref="BmpImageData"/>
        /// </summary>
        /// <param name="bytes">contents of the image</param>
        /// <param name="noHeader">indicates that the source image does not have a header</param>
        protected internal BmpImageData(byte[] bytes, bool noHeader)
            : base(bytes, ImageType.BMP) {
            this.noHeader = noHeader;
        }

        /// <summary>
        /// Creates instance of
        /// <see cref="BmpImageData"/>
        /// </summary>
        /// <param name="bytes">contents of the image</param>
        /// <param name="noHeader">indicates that the source image does not have a header</param>
        /// <param name="size">the size of the image (length of the byte array)</param>
        [System.ObsoleteAttribute(@"will be removed in 7.2")]
        protected internal BmpImageData(byte[] bytes, bool noHeader, int size)
            : this(bytes, noHeader) {
            this.size = size;
        }

        /// <returns>size of the image</returns>
        [System.ObsoleteAttribute(@"will be removed in 7.2")]
        public virtual int GetSize() {
            return size;
        }

        /// <returns>True if the bitmap image does not contain a header</returns>
        public virtual bool IsNoHeader() {
            return noHeader;
        }
    }
}

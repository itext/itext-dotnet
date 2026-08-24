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
using System.Collections.Generic;
using System.IO;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Image {
    /// <summary>Holds the logical screen properties and decoded frames of a GIF image.</summary>
    public class GifImageData {
        private float logicalHeight;

        private float logicalWidth;

        private IList<ImageData> frames = new List<ImageData>();

        private byte[] data;

        private Uri url;

        /// <summary>Creates GIF data to be loaded from a URL.</summary>
        /// <param name="url">
        /// source URL, not
        /// <see langword="null"/>
        /// </param>
        protected internal GifImageData(Uri url) {
            this.url = url;
        }

        /// <summary>Creates GIF data from encoded bytes.</summary>
        /// <param name="data">encoded GIF bytes; the array is retained</param>
        protected internal GifImageData(byte[] data) {
            this.data = data;
        }

        /// <summary>Gets the logical screen height.</summary>
        /// <returns>height in pixels</returns>
        public virtual float GetLogicalHeight() {
            return logicalHeight;
        }

        /// <summary>Sets the logical screen height.</summary>
        /// <param name="logicalHeight">height in pixels</param>
        public virtual void SetLogicalHeight(float logicalHeight) {
            this.logicalHeight = logicalHeight;
        }

        /// <summary>Gets the logical screen width.</summary>
        /// <returns>width in pixels</returns>
        public virtual float GetLogicalWidth() {
            return logicalWidth;
        }

        /// <summary>Sets the logical screen width.</summary>
        /// <param name="logicalWidth">width in pixels</param>
        public virtual void SetLogicalWidth(float logicalWidth) {
            this.logicalWidth = logicalWidth;
        }

        /// <summary>Gets the decoded GIF frames.</summary>
        /// <returns>list of frames in source order</returns>
        public virtual IList<ImageData> GetFrames() {
            return frames;
        }

        /// <summary>Gets the encoded GIF bytes.</summary>
        /// <returns>
        /// retained bytes, or
        /// <see langword="null"/>
        /// until loaded
        /// </returns>
        protected internal virtual byte[] GetData() {
            return data;
        }

        /// <summary>Gets the source URL.</summary>
        /// <returns>
        /// source URL, or
        /// <see langword="null"/>
        /// when data was supplied directly
        /// </returns>
        protected internal virtual Uri GetUrl() {
            return url;
        }

        /// <summary>Appends a decoded frame.</summary>
        /// <param name="frame">decoded frame to append</param>
        protected internal virtual void AddFrame(ImageData frame) {
            frames.Add(frame);
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Load data by URL.</summary>
        /// <remarks>
        /// Load data by URL. url must be not null.
        /// Note, this method doesn't check if data or url is null.
        /// </remarks>
        internal virtual void LoadData() {
            Stream input = null;
            try {
                input = UrlUtil.OpenStream(url);
                ByteArrayOutputStream stream = new ByteArrayOutputStream();
                StreamUtil.TransferBytes(UrlUtil.OpenStream(url), stream);
                data = stream.ToArray();
            }
            finally {
                if (input != null) {
                    input.Dispose();
                }
            }
        }
//\endcond
    }
}

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
using System.Collections.Generic;
using System.IO;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Image {
    public class GifImageData {
        private float logicalHeight;

        private float logicalWidth;

        private IList<ImageData> frames = new List<ImageData>();

        private byte[] data;

        private Uri url;

        protected internal GifImageData(Uri url) {
            this.url = url;
        }

        protected internal GifImageData(byte[] data) {
            this.data = data;
        }

        public virtual float GetLogicalHeight() {
            return logicalHeight;
        }

        public virtual void SetLogicalHeight(float logicalHeight) {
            this.logicalHeight = logicalHeight;
        }

        public virtual float GetLogicalWidth() {
            return logicalWidth;
        }

        public virtual void SetLogicalWidth(float logicalWidth) {
            this.logicalWidth = logicalWidth;
        }

        public virtual IList<ImageData> GetFrames() {
            return frames;
        }

        protected internal virtual byte[] GetData() {
            return data;
        }

        protected internal virtual Uri GetUrl() {
            return url;
        }

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

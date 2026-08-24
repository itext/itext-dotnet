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
using iText.Commons.Logs;
using iText.IO.Codec;
using iText.IO.Exceptions;
using iText.IO.Source;

namespace iText.IO.Image {
    /// <summary>Image data for a selected page of a JBIG2 image.</summary>
    public class Jbig2ImageData : ImageData {
        private int page;

        /// <summary>Creates JBIG2 image data to be loaded from a URL.</summary>
        /// <param name="url">
        /// source URL, not
        /// <see langword="null"/>
        /// </param>
        /// <param name="page">one-based JBIG2 page number</param>
        protected internal Jbig2ImageData(Uri url, int page)
            : base(url, ImageType.JBIG2) {
            this.page = page;
        }

        /// <summary>Creates JBIG2 image data from encoded bytes.</summary>
        /// <param name="bytes">encoded JBIG2 bytes; the array is retained</param>
        /// <param name="page">one-based JBIG2 page number</param>
        protected internal Jbig2ImageData(byte[] bytes, int page)
            : base(bytes, ImageType.JBIG2) {
            this.page = page;
        }

        /// <summary>Gets the selected page number.</summary>
        /// <returns>one-based JBIG2 page number</returns>
        public virtual int GetPage() {
            return page;
        }

        /// <summary>Gets the number of pages in a JBIG2 image.</summary>
        /// <param name="bytes">a byte array containing a JBIG2 image</param>
        /// <returns>the number of pages</returns>
        public static int GetNumberOfPages(byte[] bytes) {
            IRandomAccessSource ras = new RandomAccessSourceFactory().CreateSource(bytes);
            return GetNumberOfPages(new RandomAccessFileOrArray(ras));
        }

        /// <summary>Gets the number of pages in a JBIG2 image.</summary>
        /// <param name="raf">
        /// a
        /// <c>RandomAccessFileOrArray</c>
        /// containing a JBIG2 image
        /// </param>
        /// <returns>the number of pages</returns>
        public static int GetNumberOfPages(RandomAccessFileOrArray raf) {
            try {
                Jbig2SegmentReader sr = new Jbig2SegmentReader(raf);
                sr.Read();
                return sr.NumberOfPages();
            }
            catch (Exception e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.JBIG2_IMAGE_EXCEPTION, e);
            }
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <see langword="false"/>
        /// , because JBIG2 images require a JBIG2Decode filter
        /// </returns>
        public override bool CanImageBeInline() {
            LazyLogger logger = new LazyLogger(typeof(ImageData));
            logger.Warn(() => iText.IO.Logs.IoLogMessageConstant.IMAGE_HAS_JBIG2DECODE_FILTER);
            return false;
        }
    }
}

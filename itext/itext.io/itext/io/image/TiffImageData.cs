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
using iText.IO.Codec;
using iText.IO.Exceptions;
using iText.IO.Source;

namespace iText.IO.Image {
    /// <summary>Image data and loading options for one page of a TIFF image.</summary>
    public class TiffImageData : RawImageData {
        private bool recoverFromImageError;

        private int page;

        private bool direct;

        /// <summary>Creates TIFF image data to be loaded from a URL.</summary>
        /// <param name="url">
        /// source URL, not
        /// <see langword="null"/>
        /// </param>
        /// <param name="recoverFromImageError">whether decoding should recover from malformed image data</param>
        /// <param name="page">one-based TIFF page number</param>
        /// <param name="direct">whether to preserve directly embeddable TIFF data</param>
        protected internal TiffImageData(Uri url, bool recoverFromImageError, int page, bool direct)
            : base(url, ImageType.TIFF) {
            this.recoverFromImageError = recoverFromImageError;
            this.page = page;
            this.direct = direct;
        }

        /// <summary>Creates TIFF image data from encoded bytes.</summary>
        /// <param name="bytes">encoded TIFF bytes; the array is retained</param>
        /// <param name="recoverFromImageError">whether decoding should recover from malformed image data</param>
        /// <param name="page">one-based TIFF page number</param>
        /// <param name="direct">whether to preserve directly embeddable TIFF data</param>
        protected internal TiffImageData(byte[] bytes, bool recoverFromImageError, int page, bool direct)
            : base(bytes, ImageType.TIFF) {
            this.recoverFromImageError = recoverFromImageError;
            this.page = page;
            this.direct = direct;
        }

        private static ImageData GetImage(Uri url, bool recoverFromImageError, int page, bool direct) {
            return new iText.IO.Image.TiffImageData(url, recoverFromImageError, page, direct);
        }

        private static ImageData GetImage(byte[] bytes, bool recoverFromImageError, int page, bool direct) {
            return new iText.IO.Image.TiffImageData(bytes, recoverFromImageError, page, direct);
        }

        /// <summary>Gets the number of pages the TIFF document has.</summary>
        /// <param name="raf">
        /// a
        /// <c>RandomAccessFileOrArray</c>
        /// containing a TIFF image.
        /// </param>
        /// <returns>the number of pages.</returns>
        public static int GetNumberOfPages(RandomAccessFileOrArray raf) {
            try {
                return TIFFDirectory.GetNumDirectories(raf);
            }
            catch (Exception e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TIFF_IMAGE_EXCEPTION, e);
            }
        }

        /// <summary>Gets the number of pages the TIFF document has.</summary>
        /// <param name="bytes">a byte array containing a TIFF image.</param>
        /// <returns>the number of pages.</returns>
        public static int GetNumberOfPages(byte[] bytes) {
            IRandomAccessSource ras = new RandomAccessSourceFactory().CreateSource(bytes);
            return GetNumberOfPages(new RandomAccessFileOrArray(ras));
        }

        /// <summary>Checks whether decoding may recover from image errors.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when recovery is enabled
        /// </returns>
        public virtual bool IsRecoverFromImageError() {
            return recoverFromImageError;
        }

        /// <summary>Gets the selected TIFF page number.</summary>
        /// <returns>one-based page number</returns>
        public virtual int GetPage() {
            return page;
        }

        /// <summary>Checks whether direct TIFF embedding was requested.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when direct embedding was requested
        /// </returns>
        public virtual bool IsDirect() {
            return direct;
        }

        /// <summary>Sets the detected original image type.</summary>
        /// <param name="originalType">original image type</param>
        public virtual void SetOriginalType(ImageType originalType) {
            this.originalType = originalType;
        }
    }
}

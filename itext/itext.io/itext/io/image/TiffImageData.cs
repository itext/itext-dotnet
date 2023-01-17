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
using iText.IO.Codec;
using iText.IO.Source;

namespace iText.IO.Image {
    public class TiffImageData : RawImageData {
        private bool recoverFromImageError;

        private int page;

        private bool direct;

        protected internal TiffImageData(Uri url, bool recoverFromImageError, int page, bool direct)
            : base(url, ImageType.TIFF) {
            this.recoverFromImageError = recoverFromImageError;
            this.page = page;
            this.direct = direct;
        }

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
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.TiffImageException, e);
            }
        }

        /// <summary>Gets the number of pages the TIFF document has.</summary>
        /// <param name="bytes">a byte array containing a TIFF image.</param>
        /// <returns>the number of pages.</returns>
        public static int GetNumberOfPages(byte[] bytes) {
            IRandomAccessSource ras = new RandomAccessSourceFactory().CreateSource(bytes);
            return GetNumberOfPages(new RandomAccessFileOrArray(ras));
        }

        public virtual bool IsRecoverFromImageError() {
            return recoverFromImageError;
        }

        public virtual int GetPage() {
            return page;
        }

        public virtual bool IsDirect() {
            return direct;
        }

        public virtual void SetOriginalType(ImageType originalType) {
            this.originalType = originalType;
        }
    }
}

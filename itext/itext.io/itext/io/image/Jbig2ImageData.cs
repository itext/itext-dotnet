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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.IO.Codec;
using iText.IO.Source;

namespace iText.IO.Image {
    public class Jbig2ImageData : ImageData {
        private int page;

        protected internal Jbig2ImageData(Uri url, int page)
            : base(url, ImageType.JBIG2) {
            this.page = page;
        }

        protected internal Jbig2ImageData(byte[] bytes, int page)
            : base(bytes, ImageType.JBIG2) {
            this.page = page;
        }

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
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.Jbig2ImageException, e);
            }
        }

        public override bool CanImageBeInline() {
            ILogger logger = ITextLogManager.GetLogger(typeof(ImageData));
            logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.IMAGE_HAS_JBIG2DECODE_FILTER);
            return false;
        }
    }
}

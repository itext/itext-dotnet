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
using iText.IO.Codec;
using iText.IO.Source;

namespace iText.IO.Image {
    internal class Jbig2ImageHelper {
        private byte[] globals;

        /// <summary>
        /// Gets a byte array that can be used as a /JBIG2Globals,
        /// or null if not applicable to the given jbig2.
        /// </summary>
        /// <param name="ra">an random access file or array</param>
        /// <returns>a byte array</returns>
        public static byte[] GetGlobalSegment(RandomAccessFileOrArray ra) {
            try {
                Jbig2SegmentReader sr = new Jbig2SegmentReader(ra);
                sr.Read();
                return sr.GetGlobal(true);
            }
            catch (Exception) {
                return null;
            }
        }

        public static void ProcessImage(ImageData jbig2) {
            if (jbig2.GetOriginalType() != ImageType.JBIG2) {
                throw new ArgumentException("JBIG2 image expected");
            }
            Jbig2ImageData image = (Jbig2ImageData)jbig2;
            try {
                IRandomAccessSource ras;
                if (image.GetData() == null) {
                    image.LoadData();
                }
                ras = new RandomAccessSourceFactory().CreateSource(image.GetData());
                RandomAccessFileOrArray raf = new RandomAccessFileOrArray(ras);
                Jbig2SegmentReader sr = new Jbig2SegmentReader(raf);
                sr.Read();
                Jbig2SegmentReader.Jbig2Page p = sr.GetPage(image.GetPage());
                raf.Close();
                image.SetHeight(p.pageBitmapHeight);
                image.SetWidth(p.pageBitmapWidth);
                image.SetBpc(1);
                image.SetColorEncodingComponentsNumber(1);
                //TODO JBIG2 globals caching
                byte[] globals = sr.GetGlobal(true);
                //TODO due to the fact, that streams now may be transformed to indirect objects only on writing,
                //pdfStream.getDocument() cannot longer be the sign of inline/indirect images
                // in case inline image pdfStream.getDocument() will be null
                if (globals != null) {
                    /*&& stream.getDocument() != null*/
                    IDictionary<String, Object> decodeParms = new Dictionary<String, Object>();
                    //                PdfStream globalsStream = new PdfStream().makeIndirect(pdfStream.getDocument());
                    //                globalsStream.getOutputStream().write(globals);
                    decodeParms.Put("JBIG2Globals", globals);
                    image.decodeParms = decodeParms;
                }
                image.SetFilter("JBIG2Decode");
                image.SetColorEncodingComponentsNumber(1);
                image.SetBpc(1);
                image.data = p.GetData(true);
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IOException.Jbig2ImageException, e);
            }
        }
    }
}

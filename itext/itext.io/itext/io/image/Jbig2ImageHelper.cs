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
using iText.IO.Codec;
using iText.IO.Exceptions;
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
                byte[] globals = sr.GetGlobal(true);
                if (globals != null) {
                    IDictionary<String, Object> decodeParms = new Dictionary<String, Object>();
                    decodeParms.Put("JBIG2Globals", globals);
                    image.decodeParms = decodeParms;
                }
                image.SetFilter("JBIG2Decode");
                image.SetColorEncodingComponentsNumber(1);
                image.SetBpc(1);
                image.data = p.GetData(true);
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.JBIG2_IMAGE_EXCEPTION, e);
            }
        }
    }
}

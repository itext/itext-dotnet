/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf.Canvas.Wmf {
    /// <summary>Image implementation for WMF, Windows Metafile.</summary>
    public class WmfImageData : ImageData {
        private static readonly byte[] wmf = new byte[] { (byte)0xD7, (byte)0xCD };

        /// <summary>Creates a WmfImage from a file.</summary>
        /// <param name="fileName">pah to the file</param>
        public WmfImageData(String fileName)
            : this(UrlUtil.ToURL(fileName)) {
        }

        /// <summary>Creates a WmfImage from a URL.</summary>
        /// <param name="url">URL to the file</param>
        public WmfImageData(Uri url)
            : base(url, ImageType.WMF) {
            byte[] imageType = ReadImageType(url);
            if (!ImageTypeIs(imageType, wmf)) {
                throw new PdfException(KernelExceptionMessageConstant.NOT_A_WMF_IMAGE);
            }
        }

        /// <summary>Creates a WmfImage from a byte[].</summary>
        /// <param name="bytes">the image bytes</param>
        public WmfImageData(byte[] bytes)
            : base(bytes, ImageType.WMF) {
            byte[] imageType = ReadImageType(bytes);
            if (!ImageTypeIs(imageType, wmf)) {
                throw new PdfException(KernelExceptionMessageConstant.NOT_A_WMF_IMAGE);
            }
        }

        private static bool ImageTypeIs(byte[] imageType, byte[] compareWith) {
            for (int i = 0; i < compareWith.Length; i++) {
                if (imageType[i] != compareWith[i]) {
                    return false;
                }
            }
            return true;
        }

        private static byte[] ReadImageType(Uri source) {
            Stream @is = null;
            try {
                @is = UrlUtil.OpenStream(source);
                byte[] bytes = new byte[8];
                @is.Read(bytes);
                return bytes;
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.IO_EXCEPTION, e);
            }
            finally {
                if (@is != null) {
                    try {
                        @is.Dispose();
                    }
                    catch (System.IO.IOException) {
                    }
                }
            }
        }

        private static byte[] ReadImageType(byte[] bytes) {
            return JavaUtil.ArraysCopyOfRange(bytes, 0, 8);
        }
    }
}

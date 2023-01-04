#if !NETSTANDARD2_0
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
using System.Drawing;
using System.IO;

namespace iText.IO.Image {
    internal class DrawingImageFactory {
        /// <summary>Gets an instance of an Image from <see cref="System.Drawing.Image"/></summary>
        /// <param name="image">the <see cref="System.Drawing.Image"/> to convert</param>
        /// <param name="color">if different from <CODE>null</CODE> the transparency pixels are replaced by this color
        /// 	</param>
        /// <returns>RawImage</returns>
        public static ImageData GetImage(System.Drawing.Image image, Color? color) {
            return GetImage(image, color, false);
        }

        /// <summary>
        /// Gets an instance of an Image from a <see cref="System.Drawing.Image"/>.
        /// </summary>
        /// <param name="image">the <see cref="System.Drawing.Image"/> to convert</param>
        /// <param name="color">
        /// if different from null the transparency
        /// pixels are replaced by this color
        /// </param>
        /// <param name="forceBW">if true the image is treated as black and white</param>
        /// <returns>an object of type ImgRaw</returns>
        public static ImageData GetImage(System.Drawing.Image image, Color? color, bool forceBW) {
            System.Drawing.Bitmap bm = (System.Drawing.Bitmap)image;
            int w = bm.Width;
            int h = bm.Height;
            int pxv = 0;
            if (forceBW) {
                int byteWidth = (w / 8) + ((w & 7) != 0 ? 1 : 0);
                byte[] pixelsByte = new byte[byteWidth * h];

                int index = 0;
                int transColor = 1;
                if (color != null) {
                    transColor = (color.Value.R + color.Value.G + color.Value.B < 384) ? 0 : 1;
                }
                int[] transparency = null;
                int cbyte = 0x80;
                int wMarker = 0;
                int currByte = 0;
                if (color != null) {
                    for (int j = 0; j < h; j++) {
                        for (int i = 0; i < w; i++) {
                            int alpha = bm.GetPixel(i, j).A;
                            if (alpha < 250) {
                                if (transColor == 1)
                                    currByte |= cbyte;
                            } else {
                                if ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0)
                                    currByte |= cbyte;
                            }
                            cbyte >>= 1;
                            if (cbyte == 0 || wMarker + 1 >= w) {
                                pixelsByte[index++] = (byte)currByte;
                                cbyte = 0x80;
                                currByte = 0;
                            }
                            ++wMarker;
                            if (wMarker >= w)
                                wMarker = 0;
                        }
                    }
                } else {
                    for (int j = 0; j < h; j++) {
                        for (int i = 0; i < w; i++) {
                            if (transparency == null) {
                                int alpha = bm.GetPixel(i, j).A;
                                if (alpha == 0) {
                                    transparency = new int[2];
                                    transparency[0] =
                                        transparency[1] = ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0) ? 1 : 0;
                                }
                            }
                            if ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0)
                                currByte |= cbyte;
                            cbyte >>= 1;
                            if (cbyte == 0 || wMarker + 1 >= w) {
                                pixelsByte[index++] = (byte)currByte;
                                cbyte = 0x80;
                                currByte = 0;
                            }
                            ++wMarker;
                            if (wMarker >= w)
                                wMarker = 0;
                        }
                    }
                }

                return ImageDataFactory.Create(w, h, 1, 1, pixelsByte, transparency);
            } else {
                byte[] pixelsByte = new byte[w * h * 3];
                byte[] smask = null;

                int index = 0;
                int red = 255;
                int green = 255;
                int blue = 255;
                if (color != null) {
                    red = color.Value.R;
                    green = color.Value.G;
                    blue = color.Value.B;
                }
                int[] transparency = null;
                if (color != null) {
                    for (int j = 0; j < h; j++) {
                        for (int i = 0; i < w; i++) {
                            int alpha = (bm.GetPixel(i, j).ToArgb() >> 24) & 0xff;
                            if (alpha < 250) {
                                pixelsByte[index++] = (byte)red;
                                pixelsByte[index++] = (byte)green;
                                pixelsByte[index++] = (byte)blue;
                            } else {
                                pxv = bm.GetPixel(i, j).ToArgb();
                                pixelsByte[index++] = (byte)((pxv >> 16) & 0xff);
                                pixelsByte[index++] = (byte)((pxv >> 8) & 0xff);
                                pixelsByte[index++] = (byte)((pxv) & 0xff);
                            }
                        }
                    }
                } else {
                    int transparentPixel = 0;
                    smask = new byte[w * h];
                    bool shades = false;
                    int smaskPtr = 0;
                    for (int j = 0; j < h; j++) {
                        for (int i = 0; i < w; i++) {
                            pxv = bm.GetPixel(i, j).ToArgb();
                            byte alpha = smask[smaskPtr++] = (byte)((pxv >> 24) & 0xff);
                            /* bugfix by Chris Nokleberg */
                            if (!shades) {
                                if (alpha != 0 && alpha != 255) {
                                    shades = true;
                                } else if (transparency == null) {
                                    if (alpha == 0) {
                                        transparentPixel = pxv & 0xffffff;
                                        transparency = new int[6];
                                        transparency[0] = transparency[1] = (transparentPixel >> 16) & 0xff;
                                        transparency[2] = transparency[3] = (transparentPixel >> 8) & 0xff;
                                        transparency[4] = transparency[5] = transparentPixel & 0xff;
                                        // Added by Michael Klink
                                        // Check whether this value for transparent pixels
                                        // has already been used for a non-transparent one
                                        // before this position
                                        for (int prevPixelI = 0; prevPixelI <= i; prevPixelI++) {
                                            for (int prevPixelJ = 0;
                                                prevPixelJ < (prevPixelI == i ? j : h);
                                                prevPixelJ++) {
                                                int prevPxV = bm.GetPixel(prevPixelI, prevPixelJ).ToArgb();
                                                if ((prevPxV & 0xffffff) == transparentPixel) {
                                                    // found a prior use of the transparentPixel color
                                                    // and, therefore, cannot make use of this color
                                                    // for transparency; we could still use an image
                                                    // mask but for simplicity let's use a soft mask
                                                    // which already is implemented here
                                                    shades = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                } else if (((pxv & 0xffffff) != transparentPixel) && (alpha == 0)) {
                                    shades = true;
                                } else if (((pxv & 0xffffff) == transparentPixel) && (alpha != 0)) {
                                    shades = true;
                                }
                            }
                            pixelsByte[index++] = (byte)((pxv >> 16) & 0xff);
                            pixelsByte[index++] = (byte)((pxv >> 8) & 0xff);
                            pixelsByte[index++] = (byte)(pxv & 0xff);
                        }
                    }
                    if (shades)
                        transparency = null;
                    else
                        smask = null;
                }
                ImageData img = ImageDataFactory.Create(w, h, 3, 8, pixelsByte, transparency);
                if (smask != null) {
                    ImageData sm = ImageDataFactory.Create(w, h, 1, 8, smask, null);
                    sm.MakeMask();
                    img.imageMask = sm;
                }
                return img;
            }
        }

        /// <summary>
        /// Converts a .NET image to a Native(PNG, JPG, GIF, WMF) image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static ImageData GetImage(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format) {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, format);
            return ImageDataFactory.Create(ms.ToArray());
        }
    }
}
#endif

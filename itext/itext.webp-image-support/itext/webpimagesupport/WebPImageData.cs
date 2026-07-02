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
using iText.IO.Colors;
using iText.IO.Exceptions;
using iText.IO.Image;
using iText.IO.Util;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace iText.Webpimagesupport {

    /// <summary>
    /// This class is a wrapper around WebP image format.
    /// </summary>
    public class WebPImageData : ImageData
    {
        /// <summary>Creates an <see cref="iText.IO.Image.ImageData">image data</see> instance from a WebP image raw bytes.</summary>
        /// <param name="bytes">raw bytes to create WebP image data from</param>
        public WebPImageData(byte[] bytes): base(bytes, ImageType.WEBP) {
            ProcessImage();
        }

        /// <summary>Creates an <see cref="iText.IO.Image.ImageData">image data</see> instance from a WebP image raw bytes.</summary>
        /// <param name="url">URL to create WebP image data from</param>
        public WebPImageData(Uri url) : base(url, ImageType.WEBP) {
            ProcessImage();
        }

        private void ProcessImage() {

            if (this.data == null) {
                try {
                    this.LoadData();
                }
                catch (System.Net.WebException e) {
                    throw new IOException(IoExceptionMessageConstant.WEBP_IMAGE_EXCEPTION);
                }
            }

            using (var skData = SKData.CreateCopy(this.data)) {
                using (var codec = SKCodec.Create(skData)) {
                    if (codec == null) {
                        throw new IOException(IoExceptionMessageConstant.WEBP_IMAGE_EXCEPTION);
                    }

                    var info = codec.Info;

                    // Convert to a known format RGBA8888
                    var imageInfo = new SKImageInfo(
                        info.Width,
                        info.Height,
                        SKColorType.Rgba8888,
                        SKAlphaType.Unpremul,
                        SKColorSpace.CreateSrgb());

                    using (var bitmap = new SKBitmap(imageInfo)) {
                        var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());

                        if (result != SKCodecResult.Success && result != SKCodecResult.IncompleteInput) {
                            throw new IOException(IoExceptionMessageConstant.WEBP_IMAGE_EXCEPTION);
                        }

                        // Extract raw pixel bytes
                        byte[] rasterData = new byte[bitmap.ByteCount];

                        System.Runtime.InteropServices.Marshal.Copy(
                            bitmap.GetPixels(),
                            rasterData,
                            0,
                            bitmap.ByteCount);

                        this.data = new byte[bitmap.ByteCount / 4 * 3];
                        byte[] alpha = new byte[bitmap.ByteCount / 4];
                        for (int i = 0, j = 0, t = 0; i < bitmap.ByteCount; i += 4) {
                            this.data[j] = rasterData[i];
                            ++j;
                            this.data[j] = rasterData[i + 1];
                            ++j;
                            this.data[j] = rasterData[i + 2];
                            ++j;

                            alpha[t] = rasterData[i + 3];
                            ++t;
                        }

                        this.imageSize = bitmap.ByteCount;
                        this.SetHeight(bitmap.Height);
                        this.SetWidth(bitmap.Width);
                        this.SetBpc(8);
                        this.SetColorEncodingComponentsNumber(3);

                        if (codec.Info.AlphaType != SKAlphaType.Opaque) {
                            ImageData softMask = ImageDataFactory.Create(bitmap.Width, bitmap.Height, 1, 8, alpha, null);
                            softMask.MakeMask();
                            this.SetImageMask(softMask);
                        }
                    }
                }
            }
        }
    }
}

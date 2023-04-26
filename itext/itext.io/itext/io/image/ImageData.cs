/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.IO.Colors;
using iText.IO.Exceptions;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Image {
    public abstract class ImageData {
        /// <summary>a static that is used for attributing a unique id to each image.</summary>
        private static long serialId = 0;

        private static readonly Object staticLock = new Object();

        protected internal Uri url;

        protected internal int[] transparency;

        protected internal ImageType originalType;

        protected internal float width;

        protected internal float height;

        protected internal byte[] data;

        protected internal int imageSize;

        protected internal int bpc = 1;

        /// <summary>Is the number of components used to encode colorspace.</summary>
        protected internal int colorEncodingComponentsNumber = -1;

        protected internal float[] decode;

        protected internal IDictionary<String, Object> decodeParms;

        protected internal bool inverted = false;

        protected internal float rotation;

        protected internal IccProfile profile;

        protected internal int dpiX = 0;

        protected internal int dpiY = 0;

        protected internal int colorTransform = 1;

        protected internal bool deflated;

        protected internal bool mask = false;

        protected internal iText.IO.Image.ImageData imageMask;

        protected internal bool interpolation;

        protected internal float XYRatio = 0;

        protected internal String filter;

        protected internal IDictionary<String, Object> imageAttributes;

        protected internal long? mySerialId = GetSerialId();

        protected internal ImageData(Uri url, ImageType type) {
            this.url = url;
            this.originalType = type;
        }

        protected internal ImageData(byte[] bytes, ImageType type) {
            this.data = bytes;
            this.originalType = type;
        }

        public virtual bool IsRawImage() {
            return false;
        }

        public virtual Uri GetUrl() {
            return url;
        }

        public virtual void SetUrl(Uri url) {
            this.url = url;
        }

        public virtual int[] GetTransparency() {
            return transparency;
        }

        public virtual void SetTransparency(int[] transparency) {
            this.transparency = transparency;
        }

        public virtual bool IsInverted() {
            return inverted;
        }

        public virtual void SetInverted(bool inverted) {
            this.inverted = inverted;
        }

        public virtual float GetRotation() {
            return rotation;
        }

        public virtual void SetRotation(float rotation) {
            this.rotation = rotation;
        }

        public virtual IccProfile GetProfile() {
            return profile;
        }

        public virtual void SetProfile(IccProfile profile) {
            this.profile = profile;
        }

        public virtual int GetDpiX() {
            return dpiX;
        }

        public virtual int GetDpiY() {
            return dpiY;
        }

        public virtual void SetDpi(int dpiX, int dpiY) {
            this.dpiX = dpiX;
            this.dpiY = dpiY;
        }

        public virtual int GetColorTransform() {
            return colorTransform;
        }

        public virtual void SetColorTransform(int colorTransform) {
            this.colorTransform = colorTransform;
        }

        public virtual bool IsDeflated() {
            return deflated;
        }

        public virtual void SetDeflated(bool deflated) {
            this.deflated = deflated;
        }

        public virtual ImageType GetOriginalType() {
            return originalType;
        }

        /// <summary>Gets the number of components used to encode colorspace.</summary>
        /// <returns>the number of components used to encode colorspace</returns>
        public virtual int GetColorEncodingComponentsNumber() {
            return colorEncodingComponentsNumber;
        }

        /// <summary>Sets the number of components used to encode colorspace.</summary>
        /// <param name="colorEncodingComponentsNumber">the number of components used to encode colorspace</param>
        public virtual void SetColorEncodingComponentsNumber(int colorEncodingComponentsNumber) {
            this.colorEncodingComponentsNumber = colorEncodingComponentsNumber;
        }

        public virtual byte[] GetData() {
            return data;
        }

        public virtual bool CanBeMask() {
            if (IsRawImage()) {
                if (bpc > 0xff) {
                    return true;
                }
            }
            return colorEncodingComponentsNumber == 1;
        }

        public virtual bool IsMask() {
            return mask;
        }

        public virtual iText.IO.Image.ImageData GetImageMask() {
            return imageMask;
        }

        public virtual void SetImageMask(iText.IO.Image.ImageData imageMask) {
            if (this.mask) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.IMAGE_MASK_CANNOT_CONTAIN_ANOTHER_IMAGE_MASK
                    );
            }
            if (!imageMask.mask) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.IMAGE_IS_NOT_A_MASK_YOU_MUST_CALL_IMAGE_DATA_MAKE_MASK
                    );
            }
            this.imageMask = imageMask;
        }

        public virtual bool IsSoftMask() {
            return mask && bpc > 1 && bpc <= 8;
        }

        public virtual void MakeMask() {
            if (!CanBeMask()) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.THIS_IMAGE_CAN_NOT_BE_AN_IMAGE_MASK);
            }
            mask = true;
        }

        public virtual float GetWidth() {
            return width;
        }

        public virtual void SetWidth(float width) {
            this.width = width;
        }

        public virtual float GetHeight() {
            return height;
        }

        public virtual void SetHeight(float height) {
            this.height = height;
        }

        public virtual int GetBpc() {
            return bpc;
        }

        public virtual void SetBpc(int bpc) {
            this.bpc = bpc;
        }

        public virtual bool IsInterpolation() {
            return interpolation;
        }

        public virtual void SetInterpolation(bool interpolation) {
            this.interpolation = interpolation;
        }

        public virtual float GetXYRatio() {
            return XYRatio;
        }

        public virtual void SetXYRatio(float XYRatio) {
            this.XYRatio = XYRatio;
        }

        public virtual IDictionary<String, Object> GetImageAttributes() {
            return imageAttributes;
        }

        public virtual void SetImageAttributes(IDictionary<String, Object> imageAttributes) {
            this.imageAttributes = imageAttributes;
        }

        public virtual String GetFilter() {
            return filter;
        }

        public virtual void SetFilter(String filter) {
            this.filter = filter;
        }

        public virtual IDictionary<String, Object> GetDecodeParms() {
            return decodeParms;
        }

        public virtual float[] GetDecode() {
            return decode;
        }

        public virtual void SetDecode(float[] decode) {
            this.decode = decode;
        }

        /// <summary>Checks if image can be inline</summary>
        /// <returns>if the image can be inline</returns>
        public virtual bool CanImageBeInline() {
            ILogger logger = ITextLogManager.GetLogger(typeof(iText.IO.Image.ImageData));
            if (imageSize > 4096) {
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.IMAGE_SIZE_CANNOT_BE_MORE_4KB);
                return false;
            }
            if (imageMask != null) {
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.IMAGE_HAS_MASK);
                return false;
            }
            return true;
        }

        /// <summary>Load data from URL.</summary>
        /// <remarks>
        /// Load data from URL. url must be not null.
        /// Note, this method doesn't check if data or url is null.
        /// </remarks>
        internal virtual void LoadData() {
            RandomAccessFileOrArray raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(url
                ));
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            StreamUtil.TransferBytes(raf, stream);
            raf.Close();
            data = stream.ToArray();
        }

        /// <summary>Creates a new serial id.</summary>
        /// <returns>the new serialId</returns>
        private static long? GetSerialId() {
            lock (staticLock) {
                return ++serialId;
            }
        }
    }
}

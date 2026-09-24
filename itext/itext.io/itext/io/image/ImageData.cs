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
using System.Collections.Generic;
using iText.Commons.Logs;
using iText.IO.Colors;
using iText.IO.Exceptions;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Image {
    /// <summary>Describes encoded image data and the attributes needed to embed it in a document.</summary>
    public abstract class ImageData {
        private static readonly LazyLogger LOGGER = new LazyLogger(typeof(iText.IO.Image.ImageData));

        /// <summary>a static that is used for attributing a unique id to each image.</summary>
        private static long serialId = 0;

        private static readonly Object staticLock = new Object();

        /// <summary>
        /// Source URL from which the image bytes can be loaded, or
        /// <see langword="null"/>
        /// when bytes were supplied directly.
        /// </summary>
        protected internal Uri url;

        /// <summary>
        /// Component-value pairs defining transparent ranges, or
        /// <see langword="null"/>
        /// when no range is specified.
        /// </summary>
        protected internal int[] transparency;

        /// <summary>Detected or declared format of the source image.</summary>
        protected internal ImageType originalType;

        /// <summary>Image width in pixels.</summary>
        protected internal float width;

        /// <summary>Image height in pixels.</summary>
        protected internal float height;

        /// <summary>
        /// Encoded image bytes, or
        /// <see langword="null"/>
        /// until data is loaded from
        /// <see cref="url"/>.
        /// </summary>
        protected internal byte[] data;

        /// <summary>Size of the image data in bytes when known.</summary>
        protected internal int imageSize;

        /// <summary>Bits per color component.</summary>
        protected internal int bpc = 1;

        /// <summary>Is the number of components used to encode colorspace.</summary>
        protected internal int colorEncodingComponentsNumber = -1;

        /// <summary>
        /// Decode array applied to image samples, or
        /// <see langword="null"/>
        /// when no decode array is specified.
        /// </summary>
        protected internal float[] decode;

        /// <summary>
        /// Parameters associated with the image decoder, or
        /// <see langword="null"/>.
        /// </summary>
        protected internal IDictionary<String, Object> decodeParms;

        /// <summary>Whether the image samples are inverted.</summary>
        protected internal bool inverted = false;

        /// <summary>Clockwise rotation to apply to the image, in degrees.</summary>
        protected internal float rotation;

        /// <summary>
        /// ICC color profile associated with the image, or
        /// <see langword="null"/>.
        /// </summary>
        protected internal IccProfile profile;

        /// <summary>
        /// Horizontal resolution in dots per inch, or
        /// <c>0</c>
        /// when unspecified.
        /// </summary>
        protected internal int dpiX = 0;

        /// <summary>
        /// Vertical resolution in dots per inch, or
        /// <c>0</c>
        /// when unspecified.
        /// </summary>
        protected internal int dpiY = 0;

        /// <summary>Color-transform selector used by formats that support it.</summary>
        protected internal int colorTransform = 1;

        /// <summary>
        /// Whether
        /// <see cref="data"/>
        /// is already deflate-compressed.
        /// </summary>
        protected internal bool deflated;

        /// <summary>Whether this image is used as an image mask.</summary>
        protected internal bool mask = false;

        /// <summary>
        /// Mask image associated with this image, or
        /// <see langword="null"/>.
        /// </summary>
        protected internal iText.IO.Image.ImageData imageMask;

        /// <summary>Whether interpolation should be requested when rendering the image.</summary>
        protected internal bool interpolation;

        /// <summary>
        /// Pixel aspect ratio, or
        /// <c>0</c>
        /// when unspecified.
        /// </summary>
        protected internal float XYRatio = 0;

        /// <summary>
        /// PDF filter name used to decode the image data, or
        /// <see langword="null"/>.
        /// </summary>
        protected internal String filter;

        /// <summary>
        /// Additional image attributes, or
        /// <see langword="null"/>.
        /// </summary>
        protected internal IDictionary<String, Object> imageAttributes;

        [Obsolete]
        protected internal long? mySerialId = GetSerialId();

        /// <summary>Creates image data whose bytes are available from a URL.</summary>
        /// <param name="url">
        /// source URL, not
        /// <see langword="null"/>
        /// </param>
        /// <param name="type">source image format</param>
        protected internal ImageData(Uri url, ImageType type) {
            this.url = url;
            this.originalType = type;
        }

        /// <summary>Creates image data from encoded bytes.</summary>
        /// <param name="bytes">encoded image bytes; the array is retained</param>
        /// <param name="type">source image format</param>
        protected internal ImageData(byte[] bytes, ImageType type) {
            this.data = bytes;
            this.originalType = type;
        }

        /// <summary>Indicates whether this instance represents raw sample data.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// for raw image data;
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsRawImage() {
            return false;
        }

        /// <summary>Gets the source URL.</summary>
        /// <returns>
        /// source URL, or
        /// <see langword="null"/>
        /// when the image was created from bytes
        /// </returns>
        public virtual Uri GetUrl() {
            return url;
        }

        /// <summary>Sets the source URL used to load image bytes.</summary>
        /// <param name="url">
        /// source URL, or
        /// <see langword="null"/>
        /// </param>
        public virtual void SetUrl(Uri url) {
            this.url = url;
        }

        /// <summary>Gets the component-value pairs that define transparent ranges.</summary>
        /// <returns>
        /// retained transparency array, or
        /// <see langword="null"/>
        /// </returns>
        public virtual int[] GetTransparency() {
            return transparency;
        }

        /// <summary>Sets component-value pairs that define transparent ranges.</summary>
        /// <param name="transparency">
        /// transparency array to retain, or
        /// <see langword="null"/>
        /// </param>
        public virtual void SetTransparency(int[] transparency) {
            this.transparency = transparency;
        }

        /// <summary>Checks whether image samples are inverted.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if samples are inverted
        /// </returns>
        public virtual bool IsInverted() {
            return inverted;
        }

        /// <summary>Sets whether image samples are inverted.</summary>
        /// <param name="inverted">
        /// 
        /// <see langword="true"/>
        /// to invert samples
        /// </param>
        public virtual void SetInverted(bool inverted) {
            this.inverted = inverted;
        }

        /// <summary>Gets the clockwise rotation applied to the image.</summary>
        /// <returns>rotation in degrees</returns>
        public virtual float GetRotation() {
            return rotation;
        }

        /// <summary>Sets the clockwise rotation applied to the image.</summary>
        /// <param name="rotation">rotation in degrees</param>
        public virtual void SetRotation(float rotation) {
            this.rotation = rotation;
        }

        /// <summary>Gets the associated ICC color profile.</summary>
        /// <returns>
        /// ICC profile, or
        /// <see langword="null"/>
        /// </returns>
        public virtual IccProfile GetProfile() {
            return profile;
        }

        /// <summary>Sets the associated ICC color profile.</summary>
        /// <param name="profile">
        /// ICC profile to associate, or
        /// <see langword="null"/>
        /// </param>
        public virtual void SetProfile(IccProfile profile) {
            this.profile = profile;
        }

        /// <summary>Gets the horizontal image resolution.</summary>
        /// <returns>
        /// dots per inch, or
        /// <c>0</c>
        /// when unspecified
        /// </returns>
        public virtual int GetDpiX() {
            return dpiX;
        }

        /// <summary>Gets the vertical image resolution.</summary>
        /// <returns>
        /// dots per inch, or
        /// <c>0</c>
        /// when unspecified
        /// </returns>
        public virtual int GetDpiY() {
            return dpiY;
        }

        /// <summary>Sets the image resolution.</summary>
        /// <param name="dpiX">horizontal resolution in dots per inch</param>
        /// <param name="dpiY">vertical resolution in dots per inch</param>
        public virtual void SetDpi(int dpiX, int dpiY) {
            this.dpiX = dpiX;
            this.dpiY = dpiY;
        }

        /// <summary>Gets the color-transform selector.</summary>
        /// <returns>color-transform selector</returns>
        public virtual int GetColorTransform() {
            return colorTransform;
        }

        /// <summary>Sets the color-transform selector.</summary>
        /// <param name="colorTransform">color-transform selector</param>
        public virtual void SetColorTransform(int colorTransform) {
            this.colorTransform = colorTransform;
        }

        /// <summary>Checks whether image bytes are already deflate-compressed.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when the image bytes are deflated
        /// </returns>
        public virtual bool IsDeflated() {
            return deflated;
        }

        /// <summary>Sets whether image bytes are already deflate-compressed.</summary>
        /// <param name="deflated">
        /// 
        /// <see langword="true"/>
        /// when the image bytes are deflated
        /// </param>
        public virtual void SetDeflated(bool deflated) {
            this.deflated = deflated;
        }

        /// <summary>Gets the source image format.</summary>
        /// <returns>source image format</returns>
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

        /// <summary>Gets the encoded image bytes.</summary>
        /// <returns>
        /// retained encoded bytes, or
        /// <see langword="null"/>
        /// until loaded
        /// </returns>
        public virtual byte[] GetData() {
            return data;
        }

        /// <summary>Checks whether this image can be converted to an image mask.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the color encoding permits masking
        /// </returns>
        public virtual bool CanBeMask() {
            if (IsRawImage()) {
                if (bpc > 0xff) {
                    return true;
                }
            }
            return colorEncodingComponentsNumber == 1;
        }

        /// <summary>Checks whether this image is an image mask.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this image is a mask
        /// </returns>
        public virtual bool IsMask() {
            return mask;
        }

        /// <summary>Gets the mask image associated with this image.</summary>
        /// <returns>
        /// mask image, or
        /// <see langword="null"/>
        /// </returns>
        public virtual iText.IO.Image.ImageData GetImageMask() {
            return imageMask;
        }

        /// <summary>Associates a mask image with this image.</summary>
        /// <param name="imageMask">image that has been made a mask</param>
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

        /// <summary>Checks whether this image is a soft mask.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this image is a soft mask
        /// </returns>
        public virtual bool IsSoftMask() {
            return mask && bpc > 1 && bpc <= 8;
        }

        /// <summary>Converts this image to an image mask.</summary>
        public virtual void MakeMask() {
            if (!CanBeMask()) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.THIS_IMAGE_CAN_NOT_BE_AN_IMAGE_MASK);
            }
            mask = true;
        }

        /// <summary>Gets the image width.</summary>
        /// <returns>width in pixels</returns>
        public virtual float GetWidth() {
            return width;
        }

        /// <summary>Sets the image width.</summary>
        /// <param name="width">width in pixels</param>
        public virtual void SetWidth(float width) {
            this.width = width;
        }

        /// <summary>Gets the image height.</summary>
        /// <returns>height in pixels</returns>
        public virtual float GetHeight() {
            return height;
        }

        /// <summary>Sets the image height.</summary>
        /// <param name="height">height in pixels</param>
        public virtual void SetHeight(float height) {
            this.height = height;
        }

        /// <summary>Gets the number of bits used for each color component.</summary>
        /// <returns>bits per component</returns>
        public virtual int GetBpc() {
            return bpc;
        }

        /// <summary>Sets the number of bits used for each color component.</summary>
        /// <param name="bpc">bits per component</param>
        public virtual void SetBpc(int bpc) {
            this.bpc = bpc;
        }

        /// <summary>Checks whether interpolation should be requested while rendering.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// to request interpolation
        /// </returns>
        public virtual bool IsInterpolation() {
            return interpolation;
        }

        /// <summary>Sets whether interpolation should be requested while rendering.</summary>
        /// <param name="interpolation">
        /// 
        /// <see langword="true"/>
        /// to request interpolation
        /// </param>
        public virtual void SetInterpolation(bool interpolation) {
            this.interpolation = interpolation;
        }

        /// <summary>Gets the pixel aspect ratio.</summary>
        /// <returns>
        /// pixel aspect ratio, or
        /// <c>0</c>
        /// when unspecified
        /// </returns>
        public virtual float GetXYRatio() {
            return XYRatio;
        }

        /// <summary>Sets the pixel aspect ratio.</summary>
        /// <param name="XYRatio">pixel aspect ratio</param>
        public virtual void SetXYRatio(float XYRatio) {
            this.XYRatio = XYRatio;
        }

        /// <summary>Gets additional image attributes.</summary>
        /// <returns>
        /// retained attribute map, or
        /// <see langword="null"/>
        /// </returns>
        public virtual IDictionary<String, Object> GetImageAttributes() {
            return imageAttributes;
        }

        /// <summary>Sets additional image attributes.</summary>
        /// <param name="imageAttributes">
        /// attribute map to retain, or
        /// <see langword="null"/>
        /// </param>
        public virtual void SetImageAttributes(IDictionary<String, Object> imageAttributes) {
            this.imageAttributes = imageAttributes;
        }

        /// <summary>Gets the PDF filter name used for the image data.</summary>
        /// <returns>
        /// filter name, or
        /// <see langword="null"/>
        /// </returns>
        public virtual String GetFilter() {
            return filter;
        }

        /// <summary>Sets the PDF filter name used for the image data.</summary>
        /// <param name="filter">
        /// PDF filter name, or
        /// <see langword="null"/>
        /// </param>
        public virtual void SetFilter(String filter) {
            this.filter = filter;
        }

        /// <summary>Gets image decoder parameters.</summary>
        /// <returns>
        /// retained decoder-parameter map, or
        /// <see langword="null"/>
        /// </returns>
        public virtual IDictionary<String, Object> GetDecodeParms() {
            return decodeParms;
        }

        /// <summary>Gets the image decode array.</summary>
        /// <returns>
        /// retained decode array, or
        /// <see langword="null"/>
        /// </returns>
        public virtual float[] GetDecode() {
            return decode;
        }

        /// <summary>Sets the image decode array.</summary>
        /// <param name="decode">
        /// decode array to retain, or
        /// <see langword="null"/>
        /// </param>
        public virtual void SetDecode(float[] decode) {
            this.decode = decode;
        }

        /// <summary>Checks if image can be inline</summary>
        /// <returns>if the image can be inline</returns>
        public virtual bool CanImageBeInline() {
            if (imageSize > 4096) {
                LOGGER.Warn(() => iText.IO.Logs.IoLogMessageConstant.IMAGE_SIZE_CANNOT_BE_MORE_4KB);
                return false;
            }
            if (imageMask != null) {
                LOGGER.Warn(() => iText.IO.Logs.IoLogMessageConstant.IMAGE_HAS_MASK);
                return false;
            }
            return true;
        }

        /// <summary>Load data from URL.</summary>
        /// <remarks>
        /// Load data from URL. url must be not null.
        /// Note, this method doesn't check if data or url is null.
        /// </remarks>
        protected internal virtual void LoadData() {
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

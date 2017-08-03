/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.IO.Image;
using iText.IO.Log;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Wmf;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>A layout element that represents an image for inclusion in the document model.</summary>
    public class Image : AbstractElement<iText.Layout.Element.Image>, ILeafElement, IAccessibleElement {
        protected internal PdfXObject xObject;

        protected internal PdfName role = PdfName.Figure;

        protected internal AccessibilityProperties tagProperties;

        /// <summary>
        /// Creates an
        /// <see cref="Image"/>
        /// from an image XObject, the representation of an
        /// image in PDF syntax.
        /// </summary>
        /// <param name="xObject">
        /// an internal
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// </param>
        public Image(PdfImageXObject xObject) {
            this.xObject = xObject;
        }

        /// <summary>
        /// Creates an
        /// <see cref="Image"/>
        /// from a form XObject, the representation of a
        /// form in PDF syntax.
        /// </summary>
        /// <param name="xObject">
        /// an internal
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// </param>
        public Image(PdfFormXObject xObject) {
            this.xObject = xObject;
        }

        /// <summary>
        /// Creates an
        /// <see cref="Image"/>
        /// from an image XObject, the representation of an
        /// image in PDF syntax, with a custom width.
        /// </summary>
        /// <param name="xObject">
        /// an internal
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// </param>
        /// <param name="width">a float value</param>
        public Image(PdfImageXObject xObject, float width) {
            this.xObject = xObject;
            SetWidth(width);
        }

        /// <summary>
        /// Creates an
        /// <see cref="Image"/>
        /// from an image XObject, the representation of an
        /// image in PDF syntax, with a custom width and on a fixed position.
        /// </summary>
        /// <param name="xObject">
        /// an internal
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// </param>
        /// <param name="x">a float value representing the horizontal offset of the lower left corner of the image</param>
        /// <param name="y">a float value representing the vertical offset of the lower left corner of the image</param>
        /// <param name="width">a float value</param>
        public Image(PdfImageXObject xObject, float x, float y, float width) {
            this.xObject = xObject;
            SetProperty(Property.X, x);
            SetProperty(Property.Y, y);
            SetWidth(width);
            SetProperty(Property.POSITION, LayoutPosition.FIXED);
        }

        /// <summary>
        /// Creates an
        /// <see cref="Image"/>
        /// from an image XObject, the representation of an
        /// image in PDF syntax, on a fixed position.
        /// </summary>
        /// <param name="xObject">
        /// an internal
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// </param>
        /// <param name="x">a float value representing the horizontal offset of the lower left corner of the image</param>
        /// <param name="y">a float value representing the vertical offset of the lower left corner of the image</param>
        public Image(PdfImageXObject xObject, float x, float y) {
            this.xObject = xObject;
            SetProperty(Property.X, x);
            SetProperty(Property.Y, y);
            SetProperty(Property.POSITION, LayoutPosition.FIXED);
        }

        /// <summary>
        /// Creates an
        /// <see cref="Image"/>
        /// from a form XObject, the representation of a
        /// form in PDF syntax.
        /// </summary>
        /// <param name="xObject">
        /// an internal
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// </param>
        /// <param name="x">a float value representing the horizontal offset of the lower left corner of the form</param>
        /// <param name="y">a float value representing the vertical offset of the lower left corner of the form</param>
        public Image(PdfFormXObject xObject, float x, float y) {
            this.xObject = xObject;
            SetProperty(Property.X, x);
            SetProperty(Property.Y, y);
            SetProperty(Property.POSITION, LayoutPosition.FIXED);
        }

        /// <summary>
        /// Creates an
        /// <see cref="Image"/>
        /// from an image resource, read in from a file
        /// with the iText I/O module.
        /// </summary>
        /// <param name="img">
        /// an internal representation of the
        /// <see cref="iText.IO.Image.ImageData">image resource</see>
        /// </param>
        public Image(ImageData img)
            : this(new PdfImageXObject(CheckImageType(img))) {
            SetProperty(Property.FLUSH_ON_DRAW, true);
        }

        /// <summary>
        /// Creates an
        /// <see cref="Image"/>
        /// from an image resource, read in from a file
        /// with the iText I/O module, on a fixed position.
        /// </summary>
        /// <param name="img">
        /// an internal representation of the
        /// <see cref="iText.IO.Image.ImageData">image resource</see>
        /// </param>
        /// <param name="x">a float value representing the horizontal offset of the lower left corner of the image</param>
        /// <param name="y">a float value representing the vertical offset of the lower left corner of the image</param>
        public Image(ImageData img, float x, float y)
            : this(new PdfImageXObject(CheckImageType(img)), x, y) {
            SetProperty(Property.FLUSH_ON_DRAW, true);
        }

        /// <summary>
        /// Creates an
        /// <see cref="Image"/>
        /// from an image resource, read in from a file
        /// with the iText I/O module, with a custom width and on a fixed position.
        /// </summary>
        /// <param name="img">
        /// an internal representation of the
        /// <see cref="iText.IO.Image.ImageData">image resource</see>
        /// </param>
        /// <param name="x">a float value representing the horizontal offset of the lower left corner of the image</param>
        /// <param name="y">a float value representing the vertical offset of the lower left corner of the image</param>
        /// <param name="width">a float value</param>
        public Image(ImageData img, float x, float y, float width)
            : this(new PdfImageXObject(CheckImageType(img)), x, y, width) {
            SetProperty(Property.FLUSH_ON_DRAW, true);
        }

        /// <summary>Gets the XObject contained in this image object</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// </returns>
        public virtual PdfXObject GetXObject() {
            return xObject;
        }

        /// <summary>Sets the rotation radAngle.</summary>
        /// <param name="radAngle">a value in radians</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Image SetRotationAngle(double radAngle) {
            SetProperty(Property.ROTATION_ANGLE, radAngle);
            return this;
        }

        /// <summary>Gets the current left margin width of the element.</summary>
        /// <returns>the left margin width, as a <code>float</code></returns>
        public virtual float? GetMarginLeft() {
            return this.GetProperty<float?>(Property.MARGIN_LEFT);
        }

        /// <summary>Sets the left margin width of the element.</summary>
        /// <param name="value">the new left margin width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Image SetMarginLeft(float value) {
            SetProperty(Property.MARGIN_LEFT, value);
            return this;
        }

        /// <summary>Gets the current right margin width of the element.</summary>
        /// <returns>the right margin width, as a <code>float</code></returns>
        public virtual float? GetMarginRight() {
            return this.GetProperty<float?>(Property.MARGIN_RIGHT);
        }

        /// <summary>Sets the right margin width of the element.</summary>
        /// <param name="value">the new right margin width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Image SetMarginRight(float value) {
            SetProperty(Property.MARGIN_RIGHT, value);
            return this;
        }

        /// <summary>Gets the current top margin width of the element.</summary>
        /// <returns>the top margin width, as a <code>float</code></returns>
        public virtual float? GetMarginTop() {
            return this.GetProperty<float?>(Property.MARGIN_TOP);
        }

        /// <summary>Sets the top margin width of the element.</summary>
        /// <param name="value">the new top margin width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Image SetMarginTop(float value) {
            SetProperty(Property.MARGIN_TOP, value);
            return this;
        }

        /// <summary>Gets the current bottom margin width of the element.</summary>
        /// <returns>the bottom margin width, as a <code>float</code></returns>
        public virtual float? GetMarginBottom() {
            return this.GetProperty<float?>(Property.MARGIN_BOTTOM);
        }

        /// <summary>Sets the bottom margin width of the element.</summary>
        /// <param name="value">the new bottom margin width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Image SetMarginBottom(float value) {
            SetProperty(Property.MARGIN_BOTTOM, value);
            return this;
        }

        /// <summary>Sets the margins around the element to a series of new widths.</summary>
        /// <param name="marginTop">the new margin top width</param>
        /// <param name="marginRight">the new margin right width</param>
        /// <param name="marginBottom">the new margin bottom width</param>
        /// <param name="marginLeft">the new margin left width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Image SetMargins(float marginTop, float marginRight, float marginBottom
            , float marginLeft) {
            return SetMarginTop(marginTop).SetMarginRight(marginRight).SetMarginBottom(marginBottom).SetMarginLeft(marginLeft
                );
        }

        /// <summary>Scale the image relative to its default size.</summary>
        /// <param name="horizontalScaling">the horizontal scaling coefficient. default value 1 = 100%</param>
        /// <param name="verticalScaling">the vertical scaling coefficient. default value 1 = 100%</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Image Scale(float horizontalScaling, float verticalScaling) {
            SetProperty(Property.HORIZONTAL_SCALING, horizontalScaling);
            SetProperty(Property.VERTICAL_SCALING, verticalScaling);
            return this;
        }

        /// <summary>Scale the image to an absolute size.</summary>
        /// <remarks>
        /// Scale the image to an absolute size. This method will preserve the
        /// width-height ratio of the image.
        /// </remarks>
        /// <param name="fitWidth">the new maximum width of the image</param>
        /// <param name="fitHeight">the new maximum height of the image</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Image ScaleToFit(float fitWidth, float fitHeight) {
            float horizontalScaling = fitWidth / xObject.GetWidth();
            float verticalScaling = fitHeight / xObject.GetHeight();
            return Scale(Math.Min(horizontalScaling, verticalScaling), Math.Min(horizontalScaling, verticalScaling));
        }

        /// <summary>Scale the image to an absolute size.</summary>
        /// <remarks>
        /// Scale the image to an absolute size. This method will <em>not</em>
        /// preserve the width-height ratio of the image.
        /// </remarks>
        /// <param name="fitWidth">the new absolute width of the image</param>
        /// <param name="fitHeight">the new absolute height of the image</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Image ScaleAbsolute(float fitWidth, float fitHeight) {
            float horizontalScaling = fitWidth / xObject.GetWidth();
            float verticalScaling = fitHeight / xObject.GetHeight();
            return Scale(horizontalScaling, verticalScaling);
        }

        /// <summary>Sets the autoscale property for both width and height.</summary>
        /// <param name="autoScale">whether or not to let the image resize automatically</param>
        /// <returns>this image</returns>
        public virtual iText.Layout.Element.Image SetAutoScale(bool autoScale) {
            if (HasProperty(Property.AUTO_SCALE_WIDTH) && HasProperty(Property.AUTO_SCALE_HEIGHT) && autoScale && ((bool
                )this.GetProperty<bool?>(Property.AUTO_SCALE_WIDTH) || (bool)this.GetProperty<bool?>(Property.AUTO_SCALE_HEIGHT
                ))) {
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Element.Image));
                logger.Warn(iText.IO.LogMessageConstant.IMAGE_HAS_AMBIGUOUS_SCALE);
            }
            SetProperty(Property.AUTO_SCALE, autoScale);
            return this;
        }

        /// <summary>Sets the autoscale property for the height of the image.</summary>
        /// <param name="autoScale">whether or not to let the image height resize automatically</param>
        /// <returns>this image</returns>
        public virtual iText.Layout.Element.Image SetAutoScaleHeight(bool autoScale) {
            if (HasProperty(Property.AUTO_SCALE_WIDTH) && autoScale && (bool)this.GetProperty<bool?>(Property.AUTO_SCALE_WIDTH
                )) {
                SetProperty(Property.AUTO_SCALE_WIDTH, false);
                SetProperty(Property.AUTO_SCALE_HEIGHT, false);
                SetProperty(Property.AUTO_SCALE, true);
            }
            else {
                SetProperty(Property.AUTO_SCALE_WIDTH, autoScale);
            }
            return this;
        }

        /// <summary>Sets the autoscale property for the width of the image.</summary>
        /// <param name="autoScale">whether or not to let the image width resize automatically</param>
        /// <returns>this image</returns>
        public virtual iText.Layout.Element.Image SetAutoScaleWidth(bool autoScale) {
            if (HasProperty(Property.AUTO_SCALE_HEIGHT) && autoScale && (bool)this.GetProperty<bool?>(Property.AUTO_SCALE_HEIGHT
                )) {
                SetProperty(Property.AUTO_SCALE_WIDTH, false);
                SetProperty(Property.AUTO_SCALE_HEIGHT, false);
                SetProperty(Property.AUTO_SCALE, true);
            }
            else {
                SetProperty(Property.AUTO_SCALE_WIDTH, autoScale);
            }
            return this;
        }

        /// <summary>Sets values for a absolute repositioning of the Element.</summary>
        /// <remarks>
        /// Sets values for a absolute repositioning of the Element. Also has as a
        /// side effect that the Element's
        /// <see cref="iText.Layout.Properties.Property.POSITION"/>
        /// is changed to
        /// <see cref="iText.Layout.Layout.LayoutPosition.FIXED">fixed</see>
        /// .
        /// </remarks>
        /// <param name="x">horizontal position on the page</param>
        /// <param name="y">vertical position on the page</param>
        /// <returns>this image.</returns>
        public virtual iText.Layout.Element.Image SetFixedPosition(float x, float y) {
            SetFixedPosition(x, y, GetWidth());
            return this;
        }

        /// <summary>
        /// Sets values for a absolute repositioning of the Element, on a specific
        /// page.
        /// </summary>
        /// <remarks>
        /// Sets values for a absolute repositioning of the Element, on a specific
        /// page. Also has as a side effect that the Element's
        /// <see cref="iText.Layout.Properties.Property.POSITION"/>
        /// is changed to
        /// <see cref="iText.Layout.Layout.LayoutPosition.FIXED">fixed</see>
        /// .
        /// </remarks>
        /// <param name="pageNumber">the page where the element must be positioned</param>
        /// <param name="x">horizontal position on the page</param>
        /// <param name="y">vertical position on the page</param>
        /// <returns>this Element.</returns>
        public virtual iText.Layout.Element.Image SetFixedPosition(int pageNumber, float x, float y) {
            SetFixedPosition(pageNumber, x, y, GetWidth());
            return this;
        }

        /// <summary>Gets width of the image.</summary>
        /// <remarks>
        /// Gets width of the image. It returns width of image or form XObject,
        /// not the width set by one of the #setWidth methods
        /// </remarks>
        /// <returns>the original width of the image</returns>
        public virtual float GetImageWidth() {
            return xObject.GetWidth();
        }

        /// <summary>Gets height of the image.</summary>
        /// <remarks>
        /// Gets height of the image. It returns height of image or form XObject,
        /// not the height set by one of the #setHeight methods
        /// </remarks>
        /// <returns>the original height of the image</returns>
        public virtual float GetImageHeight() {
            return xObject.GetHeight();
        }

        public virtual iText.Layout.Element.Image SetMaxHeight(float maxHeight) {
            SetProperty(Property.MAX_HEIGHT, maxHeight);
            return (iText.Layout.Element.Image)(Object)this;
        }

        public virtual iText.Layout.Element.Image SetMinHeight(float minHeight) {
            SetProperty(Property.MIN_HEIGHT, minHeight);
            return (iText.Layout.Element.Image)(Object)this;
        }

        public virtual iText.Layout.Element.Image SetMaxWidth(float maxWidth) {
            SetProperty(Property.MAX_WIDTH, maxWidth);
            return (iText.Layout.Element.Image)(Object)this;
        }

        public virtual iText.Layout.Element.Image SetMinWidth(float minWidth) {
            SetProperty(Property.MIN_WIDTH, minWidth);
            return (iText.Layout.Element.Image)(Object)this;
        }

        /// <summary>Gets scaled width of the image.</summary>
        /// <returns>the current scaled width</returns>
        public virtual float GetImageScaledWidth() {
            return null == this.GetProperty<float?>(Property.HORIZONTAL_SCALING) ? xObject.GetWidth() : xObject.GetWidth
                () * (float)this.GetProperty<float?>(Property.HORIZONTAL_SCALING);
        }

        /// <summary>Gets scaled height of the image.</summary>
        /// <returns>the current scaled height</returns>
        public virtual float GetImageScaledHeight() {
            return null == this.GetProperty<float?>(Property.VERTICAL_SCALING) ? xObject.GetHeight() : xObject.GetHeight
                () * (float)this.GetProperty<float?>(Property.VERTICAL_SCALING);
        }

        public virtual PdfName GetRole() {
            return role;
        }

        public virtual void SetRole(PdfName role) {
            this.role = role;
        }

        public virtual AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new AccessibilityProperties();
            }
            return tagProperties;
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new ImageRenderer(this);
        }

        private static ImageData CheckImageType(ImageData image) {
            if (image is WmfImageData) {
                throw new PdfException(PdfException.CannotCreateLayoutImageByWmfImage);
            }
            return image;
        }
    }
}

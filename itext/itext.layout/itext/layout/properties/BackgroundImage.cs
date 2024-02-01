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
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Pdf.Xobject;

namespace iText.Layout.Properties {
    /// <summary>Class to hold background-image property.</summary>
    public class BackgroundImage {
        private static readonly BlendMode DEFAULT_BLEND_MODE = BlendMode.NORMAL;

        protected internal PdfXObject image;

        protected internal AbstractLinearGradientBuilder linearGradientBuilder;

        private BlendMode blendMode = DEFAULT_BLEND_MODE;

        private readonly BackgroundRepeat repeat;

        private readonly BackgroundPosition position;

        private readonly BackgroundSize backgroundSize;

        private readonly BackgroundBox backgroundClip;

        private readonly BackgroundBox backgroundOrigin;

        /// <summary>
        /// Creates a copy of passed
        /// <see cref="BackgroundImage"/>
        /// instance.
        /// </summary>
        /// <param name="backgroundImage">
        /// 
        /// <see cref="BackgroundImage"/>
        /// for cloning
        /// </param>
        public BackgroundImage(iText.Layout.Properties.BackgroundImage backgroundImage)
            : this(backgroundImage.GetImage() == null ? (PdfXObject)backgroundImage.GetForm() : backgroundImage.GetImage
                (), backgroundImage.GetRepeat(), backgroundImage.GetBackgroundPosition(), backgroundImage.GetBackgroundSize
                (), backgroundImage.GetLinearGradientBuilder(), backgroundImage.GetBlendMode(), backgroundImage.GetBackgroundClip
                (), backgroundImage.GetBackgroundOrigin()) {
        }

        /// <summary>
        /// Gets initial image if it is instanceof
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// , otherwise returns null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// </returns>
        public virtual PdfImageXObject GetImage() {
            return image is PdfImageXObject ? (PdfImageXObject)image : null;
        }

        /// <summary>
        /// Gets initial image if it is instanceof
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// , otherwise returns null.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// </returns>
        public virtual PdfFormXObject GetForm() {
            return image is PdfFormXObject ? (PdfFormXObject)image : null;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="BackgroundImage"/>
        /// instance.
        /// </summary>
        /// <param name="image">
        /// background-image property.
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// instance.
        /// </param>
        /// <param name="repeat">
        /// background-repeat property.
        /// <see cref="BackgroundRepeat"/>
        /// instance.
        /// </param>
        /// <param name="position">
        /// background-position property.
        /// <see cref="BackgroundPosition"/>
        /// instance.
        /// </param>
        /// <param name="backgroundSize">
        /// background-size property.
        /// <see cref="BackgroundSize"/>
        /// instance.
        /// </param>
        /// <param name="linearGradientBuilder">
        /// background-image property.
        /// <see cref="iText.Kernel.Colors.Gradients.AbstractLinearGradientBuilder"/>
        /// instance.
        /// </param>
        /// <param name="blendMode">
        /// the image's blend mode.
        /// <see cref="BlendMode"/>
        /// instance.
        /// </param>
        /// <param name="clip">
        /// background-clip property.
        /// <see cref="BackgroundBox"/>
        /// instance.
        /// </param>
        /// <param name="origin">
        /// background-origin property.
        /// <see cref="BackgroundBox"/>
        /// instance.
        /// </param>
        private BackgroundImage(PdfXObject image, BackgroundRepeat repeat, BackgroundPosition position, BackgroundSize
             backgroundSize, AbstractLinearGradientBuilder linearGradientBuilder, BlendMode blendMode, BackgroundBox
             clip, BackgroundBox origin) {
            this.image = image;
            this.repeat = repeat;
            this.position = position;
            this.backgroundSize = backgroundSize;
            this.linearGradientBuilder = linearGradientBuilder;
            if (blendMode != null) {
                this.blendMode = blendMode;
            }
            this.backgroundClip = clip;
            this.backgroundOrigin = origin;
        }

        /// <summary>Gets background-position.</summary>
        /// <returns>
        /// 
        /// <see cref="BackgroundPosition"/>
        /// </returns>
        public virtual BackgroundPosition GetBackgroundPosition() {
            return position;
        }

        /// <summary>Gets linearGradientBuilder.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Colors.Gradients.AbstractLinearGradientBuilder"/>
        /// </returns>
        public virtual AbstractLinearGradientBuilder GetLinearGradientBuilder() {
            return this.linearGradientBuilder;
        }

        /// <summary>Returns is background specified.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if background is specified, otherwise false
        /// </returns>
        public virtual bool IsBackgroundSpecified() {
            return image is PdfFormXObject || image is PdfImageXObject || linearGradientBuilder != null;
        }

        /// <summary>Gets the background size property.</summary>
        /// <returns>
        /// 
        /// <see cref="BackgroundSize"/>
        /// instance
        /// </returns>
        public virtual BackgroundSize GetBackgroundSize() {
            return backgroundSize;
        }

        /// <summary>Gets initial image width.</summary>
        /// <returns>the initial image width</returns>
        public virtual float GetImageWidth() {
            return (float)image.GetWidth();
        }

        /// <summary>Gets initial image height.</summary>
        /// <returns>the initial image height</returns>
        public virtual float GetImageHeight() {
            return (float)image.GetHeight();
        }

        /// <summary>
        /// Gets image
        /// <see cref="BackgroundRepeat"/>
        /// instance.
        /// </summary>
        /// <returns>the image background repeat</returns>
        public virtual BackgroundRepeat GetRepeat() {
            return repeat;
        }

        /// <summary>Get the image's blend mode.</summary>
        /// <returns>
        /// the
        /// <see cref="BlendMode"/>
        /// representation of the image's blend mode
        /// </returns>
        public virtual BlendMode GetBlendMode() {
            return blendMode;
        }

        /// <summary>Gets background-clip.</summary>
        /// <returns>
        /// 
        /// <see cref="BackgroundBox"/>
        /// </returns>
        public virtual BackgroundBox GetBackgroundClip() {
            return backgroundClip;
        }

        /// <summary>Gets background-origin.</summary>
        /// <returns>
        /// 
        /// <see cref="BackgroundBox"/>
        /// </returns>
        public virtual BackgroundBox GetBackgroundOrigin() {
            return backgroundOrigin;
        }

        /// <summary>
        /// <see cref="BackgroundImage"/>
        /// builder class.
        /// </summary>
        public class Builder {
            private PdfXObject image;

            private AbstractLinearGradientBuilder linearGradientBuilder;

            private BackgroundPosition position = new BackgroundPosition();

            private BackgroundRepeat repeat = new BackgroundRepeat();

            private BlendMode blendMode = DEFAULT_BLEND_MODE;

            private BackgroundSize backgroundSize = new BackgroundSize();

            private BackgroundBox clip = BackgroundBox.BORDER_BOX;

            private BackgroundBox origin = BackgroundBox.PADDING_BOX;

            /// <summary>
            /// Creates a new
            /// <see cref="Builder"/>
            /// instance.
            /// </summary>
            public Builder() {
            }

            /// <summary>Sets image.</summary>
            /// <remarks>
            /// Sets image.
            /// <para />
            /// Makes linearGradientBuilder null as far as we can't have them both.
            /// </remarks>
            /// <param name="image">
            /// 
            /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
            /// to be set.
            /// </param>
            /// <returns>
            /// this
            /// <see cref="Builder"/>.
            /// </returns>
            public virtual BackgroundImage.Builder SetImage(PdfXObject image) {
                this.image = image;
                this.linearGradientBuilder = null;
                return this;
            }

            /// <summary>Sets linearGradientBuilder.</summary>
            /// <remarks>
            /// Sets linearGradientBuilder.
            /// <para />
            /// Makes image null as far as we can't have them both. It also makes background-repeat: no-repeat.
            /// </remarks>
            /// <param name="linearGradientBuilder">
            /// 
            /// <see cref="iText.Kernel.Colors.Gradients.AbstractLinearGradientBuilder"/>
            /// to be set.
            /// </param>
            /// <returns>
            /// this
            /// <see cref="Builder"/>.
            /// </returns>
            public virtual BackgroundImage.Builder SetLinearGradientBuilder(AbstractLinearGradientBuilder linearGradientBuilder
                ) {
                this.linearGradientBuilder = linearGradientBuilder;
                this.repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT);
                this.image = null;
                return this;
            }

            /// <summary>Sets background-repeat.</summary>
            /// <param name="repeat">
            /// 
            /// <see cref="BackgroundRepeat"/>
            /// to be set.
            /// </param>
            /// <returns>
            /// this
            /// <see cref="Builder"/>.
            /// </returns>
            public virtual BackgroundImage.Builder SetBackgroundRepeat(BackgroundRepeat repeat) {
                this.repeat = repeat;
                return this;
            }

            /// <summary>Sets background-position.</summary>
            /// <param name="position">
            /// 
            /// <see cref="BackgroundPosition"/>
            /// to be set.
            /// </param>
            /// <returns>
            /// this
            /// <see cref="Builder"/>.
            /// </returns>
            public virtual BackgroundImage.Builder SetBackgroundPosition(BackgroundPosition position) {
                this.position = position;
                return this;
            }

            /// <summary>Set the image's blend mode.</summary>
            /// <param name="blendMode">
            /// 
            /// <see cref="BlendMode"/>
            /// to be set.
            /// </param>
            /// <returns>
            /// this
            /// <see cref="Builder"/>.
            /// </returns>
            public virtual BackgroundImage.Builder SetBackgroundBlendMode(BlendMode blendMode) {
                if (blendMode != null) {
                    this.blendMode = blendMode;
                }
                return this;
            }

            /// <summary>Set the image's backgroundSize.</summary>
            /// <param name="backgroundSize">
            /// 
            /// <see cref="BackgroundSize"/>
            /// to be set.
            /// </param>
            /// <returns>
            /// this
            /// <see cref="Builder"/>.
            /// </returns>
            public virtual BackgroundImage.Builder SetBackgroundSize(BackgroundSize backgroundSize) {
                if (backgroundSize != null) {
                    this.backgroundSize = backgroundSize;
                }
                return this;
            }

            /// <summary>Sets background-clip.</summary>
            /// <param name="clip">
            /// 
            /// <see cref="BackgroundBox"/>
            /// to be set.
            /// </param>
            /// <returns>
            /// this
            /// <see cref="Builder"/>.
            /// </returns>
            public virtual BackgroundImage.Builder SetBackgroundClip(BackgroundBox clip) {
                this.clip = clip;
                return this;
            }

            /// <summary>Sets background-origin.</summary>
            /// <param name="origin">
            /// 
            /// <see cref="BackgroundBox"/>
            /// to be set.
            /// </param>
            /// <returns>
            /// this
            /// <see cref="Builder"/>.
            /// </returns>
            public virtual BackgroundImage.Builder SetBackgroundOrigin(BackgroundBox origin) {
                this.origin = origin;
                return this;
            }

            /// <summary>
            /// Builds new
            /// <see cref="BackgroundImage"/>
            /// using set fields.
            /// </summary>
            /// <returns>
            /// new
            /// <see cref="BackgroundImage"/>.
            /// </returns>
            public virtual BackgroundImage Build() {
                return new BackgroundImage(image, repeat, position, backgroundSize, linearGradientBuilder, blendMode, clip
                    , origin);
            }
        }
    }
}

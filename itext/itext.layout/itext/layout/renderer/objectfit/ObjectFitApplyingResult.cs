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
namespace iText.Layout.Renderer.Objectfit {
    /// <summary>
    /// The class represents results of calculating of rendered image size
    /// after applying of the
    /// <see cref="iText.Layout.Properties.ObjectFit"/>
    /// property.
    /// </summary>
    public class ObjectFitApplyingResult {
        private double renderedImageWidth;

        private double renderedImageHeight;

        private bool imageCuttingRequired;

        /// <summary>Creates a new instance of the class with default values.</summary>
        public ObjectFitApplyingResult() {
        }

        /// <summary>Creates a new instance of the class.</summary>
        /// <param name="renderedImageWidth">is a width of the image to render</param>
        /// <param name="renderedImageHeight">is a height of the image to render</param>
        /// <param name="imageCuttingRequired">
        /// is a flag showing if rendered image should be clipped
        /// as its size is greater than size of the image container
        /// </param>
        public ObjectFitApplyingResult(double renderedImageWidth, double renderedImageHeight, bool imageCuttingRequired
            ) {
            this.renderedImageWidth = renderedImageWidth;
            this.renderedImageHeight = renderedImageHeight;
            this.imageCuttingRequired = imageCuttingRequired;
        }

        /// <summary>Getter for width of rendered image.</summary>
        /// <returns>width of rendered image</returns>
        public virtual double GetRenderedImageWidth() {
            return renderedImageWidth;
        }

        /// <summary>Setter for width of rendered image.</summary>
        /// <param name="renderedImageWidth">is a new width of rendered image</param>
        public virtual void SetRenderedImageWidth(double renderedImageWidth) {
            this.renderedImageWidth = renderedImageWidth;
        }

        /// <summary>Getter for height of rendered image.</summary>
        /// <returns>height of rendered image</returns>
        public virtual double GetRenderedImageHeight() {
            return renderedImageHeight;
        }

        /// <summary>Setter for height of rendered image.</summary>
        /// <param name="renderedImageHeight">is a new height of rendered image</param>
        public virtual void SetRenderedImageHeight(double renderedImageHeight) {
            this.renderedImageHeight = renderedImageHeight;
        }

        /// <summary>
        /// Getter for a boolean value showing if at least one dimension of rendered image
        /// is greater than expected image size.
        /// </summary>
        /// <remarks>
        /// Getter for a boolean value showing if at least one dimension of rendered image
        /// is greater than expected image size. If true then image will be shown partially
        /// </remarks>
        /// <returns>true if the image need to be cutting during rendering and false otherwise</returns>
        public virtual bool IsImageCuttingRequired() {
            return imageCuttingRequired;
        }

        /// <summary>
        /// Setter for a boolean value showing if at least one dimension of rendered image
        /// is greater than expected image size.
        /// </summary>
        /// <remarks>
        /// Setter for a boolean value showing if at least one dimension of rendered image
        /// is greater than expected image size. If true then image will be shown partially
        /// </remarks>
        /// <param name="imageCuttingRequired">is a new value of the cutting-required flag</param>
        public virtual void SetImageCuttingRequired(bool imageCuttingRequired) {
            this.imageCuttingRequired = imageCuttingRequired;
        }
    }
}

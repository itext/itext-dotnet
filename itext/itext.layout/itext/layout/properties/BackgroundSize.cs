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
namespace iText.Layout.Properties {
    /// <summary>Class to hold background-size property.</summary>
    public class BackgroundSize {
        /// <summary>Width size for this image.</summary>
        /// <remarks>
        /// Width size for this image. If
        /// <see cref="UnitValue"/>
        /// is in percent, then width depends on the area of the element.
        /// </remarks>
        private UnitValue backgroundWidthSize;

        /// <summary>Height size for this image.</summary>
        /// <remarks>
        /// Height size for this image. If
        /// <see cref="UnitValue"/>
        /// is in percent, then height depends on the area of the element.
        /// </remarks>
        private UnitValue backgroundHeightSize;

        /// <summary>Image covers the entire area and its size may be more than the area.</summary>
        private bool cover;

        /// <summary>Image hsd a maximum size but not larger than the area.</summary>
        private bool contain;

        /// <summary>
        /// Creates a new
        /// <see cref="BackgroundSize"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates a new
        /// <see cref="BackgroundSize"/>
        /// instance.
        /// The "cover" and "contain" properties are not set.
        /// </remarks>
        public BackgroundSize() {
            cover = false;
            contain = false;
        }

        /// <summary>Clears all current properties and sets new width and height values.</summary>
        /// <remarks>
        /// Clears all current properties and sets new width and height values. One of the parameters
        /// can be null. Note that in this case null property will be scaled so that it becomes
        /// proportionally equal with the non-null value. If both parameters are set to null, then
        /// the default image size will be used.
        /// </remarks>
        /// <param name="width">
        /// a
        /// <see cref="UnitValue"/>
        /// object
        /// </param>
        /// <param name="height">
        /// a
        /// <see cref="UnitValue"/>
        /// object
        /// </param>
        public virtual void SetBackgroundSizeToValues(UnitValue width, UnitValue height) {
            // See also BackgroundSizeCalculationUtil#calculateBackgroundImageSize
            Clear();
            this.backgroundWidthSize = width;
            this.backgroundHeightSize = height;
        }

        /// <summary>
        /// Clears all size values and sets the "contain" property
        /// <see langword="true"/>.
        /// </summary>
        /// <seealso cref="contain"/>
        public virtual void SetBackgroundSizeToContain() {
            Clear();
            contain = true;
        }

        /// <summary>
        /// Clears all size values and sets the "cover" property
        /// <see langword="true"/>.
        /// </summary>
        /// <seealso cref="cover"/>
        public virtual void SetBackgroundSizeToCover() {
            Clear();
            cover = true;
        }

        /// <summary>Gets the background width property of the image.</summary>
        /// <returns>
        /// the
        /// <see cref="UnitValue"/>
        /// width for this image.
        /// </returns>
        /// <seealso cref="backgroundWidthSize"/>
        public virtual UnitValue GetBackgroundWidthSize() {
            return backgroundWidthSize;
        }

        /// <summary>Gets the background height property of the image.</summary>
        /// <returns>
        /// the
        /// <see cref="UnitValue"/>
        /// height for this image.
        /// </returns>
        /// <seealso cref="backgroundHeightSize"/>
        public virtual UnitValue GetBackgroundHeightSize() {
            return backgroundHeightSize;
        }

        /// <summary>Returns is size has specific property.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if size set to "contain" or "cover", otherwise false.
        /// </returns>
        public virtual bool IsSpecificSize() {
            return contain || cover;
        }

        /// <summary>Returns value of the "contain" property.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if property "contain" is set to the size, otherwise false.
        /// </returns>
        /// <seealso cref="contain"/>
        public virtual bool IsContain() {
            return contain;
        }

        /// <summary>Returns value of the "cover" property.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if property "cover" is set to the size, otherwise false.
        /// </returns>
        /// <seealso cref="cover"/>
        public virtual bool IsCover() {
            return cover;
        }

        private void Clear() {
            contain = false;
            cover = false;
            backgroundWidthSize = null;
            backgroundHeightSize = null;
        }
    }
}

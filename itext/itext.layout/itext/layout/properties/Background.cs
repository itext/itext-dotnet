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
using iText.Kernel.Colors;

namespace iText.Layout.Properties {
    /// <summary>
    /// A specialized class holding configurable properties related to an
    /// <see cref="iText.Layout.Element.IElement"/>
    /// 's background.
    /// </summary>
    /// <remarks>
    /// A specialized class holding configurable properties related to an
    /// <see cref="iText.Layout.Element.IElement"/>
    /// 's background. This class is meant to be used as the value for the
    /// <see cref="Property.BACKGROUND"/>
    /// key in an
    /// <see cref="iText.Layout.IPropertyContainer"/>
    /// . Allows
    /// to define a background color, and positive or negative changes to the
    /// location of the edges of the background coloring.
    /// </remarks>
    public class Background {
        protected internal TransparentColor transparentColor;

        protected internal float extraLeft;

        protected internal float extraRight;

        protected internal float extraTop;

        protected internal float extraBottom;

        private BackgroundBox backgroundClip = BackgroundBox.BORDER_BOX;

        /// <summary>Creates a background with a specified color.</summary>
        /// <param name="color">the background color</param>
        public Background(Color color)
            : this(color, 1f, 0, 0, 0, 0) {
        }

        /// <summary>Creates a background with a specified color and opacity.</summary>
        /// <param name="color">the background color</param>
        /// <param name="opacity">the opacity of the background color; a float between 0 and 1, where 1 stands for fully opaque color and 0 - for fully transparent
        ///     </param>
        public Background(Color color, float opacity)
            : this(color, opacity, 0, 0, 0, 0) {
        }

        /// <summary>
        /// Creates a background with a specified color, and extra space that
        /// must be counted as part of the background and therefore colored.
        /// </summary>
        /// <remarks>
        /// Creates a background with a specified color, and extra space that
        /// must be counted as part of the background and therefore colored.
        /// These values are allowed to be negative.
        /// </remarks>
        /// <param name="color">the background color</param>
        /// <param name="extraLeft">extra coloring to the left side</param>
        /// <param name="extraTop">extra coloring at the top</param>
        /// <param name="extraRight">extra coloring to the right side</param>
        /// <param name="extraBottom">extra coloring at the bottom</param>
        public Background(Color color, float extraLeft, float extraTop, float extraRight, float extraBottom)
            : this(color, 1f, extraLeft, extraTop, extraRight, extraBottom) {
        }

        /// <summary>
        /// Creates a background with a specified color, and extra space that
        /// must be counted as part of the background and therefore colored.
        /// </summary>
        /// <remarks>
        /// Creates a background with a specified color, and extra space that
        /// must be counted as part of the background and therefore colored.
        /// These values are allowed to be negative.
        /// </remarks>
        /// <param name="color">the background color</param>
        /// <param name="opacity">the opacity of the background color; a float between 0 and 1, where 1 stands for fully opaque color and 0 - for fully transparent
        ///     </param>
        /// <param name="extraLeft">extra coloring to the left side</param>
        /// <param name="extraTop">extra coloring at the top</param>
        /// <param name="extraRight">extra coloring to the right side</param>
        /// <param name="extraBottom">extra coloring at the bottom</param>
        public Background(Color color, float opacity, float extraLeft, float extraTop, float extraRight, float extraBottom
            ) {
            this.transparentColor = new TransparentColor(color, opacity);
            this.extraLeft = extraLeft;
            this.extraRight = extraRight;
            this.extraTop = extraTop;
            this.extraBottom = extraBottom;
        }

        /// <summary>Creates a background with a specified color, opacity and clip value.</summary>
        /// <param name="color">the background color</param>
        /// <param name="opacity">
        /// the opacity of the background color; a float between 0 and 1, where 1 stands for fully opaque
        /// color and 0 - for fully transparent
        /// </param>
        /// <param name="clip">the value to clip the background color</param>
        public Background(Color color, float opacity, BackgroundBox clip)
            : this(color, opacity) {
            this.backgroundClip = clip;
        }

        /// <summary>Gets the background's color.</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// of any supported kind
        /// </returns>
        public virtual Color GetColor() {
            return transparentColor.GetColor();
        }

        /// <summary>Gets the opacity of the background.</summary>
        /// <returns>a float between 0 and 1, where 1 stands for fully opaque color and 0 - for fully transparent</returns>
        public virtual float GetOpacity() {
            return transparentColor.GetOpacity();
        }

        /// <summary>Gets the extra space that must be filled to the left of the Element.</summary>
        /// <returns>a float value</returns>
        public virtual float GetExtraLeft() {
            return extraLeft;
        }

        /// <summary>Gets the extra space that must be filled to the right of the Element.</summary>
        /// <returns>a float value</returns>
        public virtual float GetExtraRight() {
            return extraRight;
        }

        /// <summary>Gets the extra space that must be filled at the top of the Element.</summary>
        /// <returns>a float value</returns>
        public virtual float GetExtraTop() {
            return extraTop;
        }

        /// <summary>Gets the extra space that must be filled at the bottom of the Element.</summary>
        /// <returns>a float value</returns>
        public virtual float GetExtraBottom() {
            return extraBottom;
        }

        /// <summary>Gets background clip value.</summary>
        /// <returns>background clip value</returns>
        public virtual BackgroundBox GetBackgroundClip() {
            return backgroundClip;
        }
    }
}

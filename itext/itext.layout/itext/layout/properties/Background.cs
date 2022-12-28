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
